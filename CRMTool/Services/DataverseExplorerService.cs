using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace CRMTool.Services
{
    public class DataverseExplorerService
    {
        private readonly IOrganizationService _service;
        private readonly MyPluginControl _pluginControl;

        public class EntityInfo
        {
            public string LogicalName { get; set; }
            public string DisplayName { get; set; }
            public string EntitySetName { get; set; }
            public string PrimaryIdAttribute { get; set; }
            public string PrimaryNameAttribute { get; set; }
        }

        public class FieldInfo
        {
            public string LogicalName { get; set; }
            public string DisplayName { get; set; }
            public string Type { get; set; }
        }

        public class ExportData
        {
            public List<Dictionary<string, object>> Records { get; set; }
            public List<string> Columns { get; set; }
            public string EntityName { get; set; }
            public DateTime ExportTime { get; set; }
        }

        public DataverseExplorerService(IOrganizationService service, MyPluginControl pluginControl)
        {
            _service = service;
            _pluginControl = pluginControl;
            // Set EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public List<EntityInfo> GetAllEntities()
        {
            try
            {
                var request = new RetrieveAllEntitiesRequest
                {
                    EntityFilters = EntityFilters.Entity,
                    RetrieveAsIfPublished = true
                };

                var response = (RetrieveAllEntitiesResponse)_service.Execute(request);

                var entities = response.EntityMetadata
                    .Where(e => !e.IsPrivate.HasValue || !e.IsPrivate.Value)
                    .Where(e => !e.IsLogicalEntity.HasValue || !e.IsLogicalEntity.Value)
                    .Select(e => new EntityInfo
                    {
                        LogicalName = e.LogicalName,
                        DisplayName = e.DisplayName?.UserLocalizedLabel?.Label ?? e.LogicalName,
                        EntitySetName = e.LogicalName,
                        PrimaryIdAttribute = e.PrimaryIdAttribute,
                        PrimaryNameAttribute = e.PrimaryNameAttribute
                    })
                    .OrderBy(e => e.DisplayName)
                    .ToList();

                return entities;
            }
            catch (Exception ex)
            {
                _pluginControl.UpdateLog($"Error fetching entities: {ex.Message}");
                return new List<EntityInfo>();
            }
        }

        public List<FieldInfo> GetEntityFields(string entityLogicalName)
        {
            try
            {
                var request = new RetrieveEntityRequest
                {
                    LogicalName = entityLogicalName,
                    EntityFilters = EntityFilters.Attributes,
                    RetrieveAsIfPublished = true
                };

                var response = (RetrieveEntityResponse)_service.Execute(request);

                var fields = response.EntityMetadata.Attributes
                    .Where(a => !a.IsLogical.HasValue || !a.IsLogical.Value)
                    .Where(a => a.AttributeOf == null)
                    .Where(a => a.DeprecatedVersion == null)
                    .Where(a => a.IsValidForRead.HasValue && a.IsValidForRead.Value)
                    .Where(a => a.AttributeType != AttributeTypeCode.PartyList)
                    .Where(a => a.AttributeType != AttributeTypeCode.ManagedProperty)
                    .Where(a => a.AttributeType != AttributeTypeCode.EntityName)
                    .Select(a => new FieldInfo
                    {
                        LogicalName = a.LogicalName,
                        DisplayName = a.DisplayName?.UserLocalizedLabel?.Label ?? a.LogicalName,
                        Type = a.AttributeType?.ToString() ?? "Unknown"
                    })
                    .OrderBy(a => a.DisplayName)
                    .ToList();

                _pluginControl.UpdateLog($"Loaded {fields.Count} fields for {entityLogicalName}");
                return fields;
            }
            catch (Exception ex)
            {
                _pluginControl.UpdateLog($"Error fetching fields for {entityLogicalName}: {ex.Message}");
                return new List<FieldInfo>();
            }
        }

        public string GetEntityData(string entityLogicalName, int topCount, List<string> selectedFields = null)
        {
            try
            {
                var query = new QueryExpression(entityLogicalName)
                {
                    TopCount = topCount,
                    PageInfo = null
                };

                var validFields = GetValidFieldsForEntity(entityLogicalName);

                if (selectedFields != null && selectedFields.Any())
                {
                    var validSelectedFields = selectedFields
                        .Where(f => validFields.Contains(f, StringComparer.OrdinalIgnoreCase))
                        .ToList();

                    if (validSelectedFields.Count != selectedFields.Count)
                    {
                        var invalidFields = selectedFields.Except(validSelectedFields, StringComparer.OrdinalIgnoreCase);
                        _pluginControl.UpdateLog($"Warning: Some fields were filtered out: {string.Join(", ", invalidFields)}");
                    }

                    if (validSelectedFields.Any())
                    {
                        query.ColumnSet = new ColumnSet(validSelectedFields.ToArray());
                    }
                    else
                    {
                        query.ColumnSet = new ColumnSet(true);
                        _pluginControl.UpdateLog("All selected fields were invalid. Falling back to all columns.");
                    }
                }
                else
                {
                    query.ColumnSet = new ColumnSet(true);
                }

                var primaryIdField = GetPrimaryIdField(entityLogicalName);
                if (!string.IsNullOrEmpty(primaryIdField))
                {
                    query.Orders.Add(new OrderExpression(primaryIdField, OrderType.Ascending));
                }

                var results = _service.RetrieveMultiple(query);

                var jsonResult = new JObject
                {
                    ["value"] = JArray.FromObject(results.Entities.Select(e => FormatEntityToJson(e, selectedFields)))
                };

                jsonResult["@odata.count"] = results.Entities.Count;
                jsonResult["@odata.totalCount"] = results.TotalRecordCount;

                return jsonResult.ToString(Formatting.Indented);
            }
            catch (Exception ex)
            {
                _pluginControl.UpdateLog($"Error fetching data for {entityLogicalName}: {ex.Message}");
                return null;
            }
        }

        public ExportData PrepareExportData(string jsonData, string entityName)
        {
            try
            {
                var exportData = new ExportData
                {
                    Records = new List<Dictionary<string, object>>(),
                    Columns = new List<string>(),
                    EntityName = entityName,
                    ExportTime = DateTime.Now
                };

                var parsedData = JObject.Parse(jsonData);
                var records = parsedData["value"] as JArray;

                if (records == null || records.Count == 0)
                {
                    return exportData;
                }

                // Get all unique column names
                var columns = new HashSet<string>();
                foreach (var record in records)
                {
                    foreach (var prop in ((JObject)record).Properties())
                    {
                        columns.Add(prop.Name);
                    }
                }
                exportData.Columns = columns.OrderBy(c => c).ToList();

                // Process each record
                foreach (var record in records)
                {
                    var row = new Dictionary<string, object>();
                    foreach (var column in exportData.Columns)
                    {
                        var value = record[column];
                        if (value != null)
                        {
                            if (value.Type == JTokenType.Object)
                            {
                                // Handle complex types
                                row[column] = value.ToString(Formatting.None);
                            }
                            else if (value.Type == JTokenType.Array)
                            {
                                row[column] = value.ToString(Formatting.None);
                            }
                            else
                            {
                                row[column] = value.ToString();
                            }
                        }
                        else
                        {
                            row[column] = "";
                        }
                    }
                    exportData.Records.Add(row);
                }

                return exportData;
            }
            catch (Exception ex)
            {
                _pluginControl.UpdateLog($"Error preparing export data: {ex.Message}");
                return null;
            }
        }

        public byte[] GenerateExcel(ExportData data)
        {
            try
            {
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add(data.EntityName);

                    // Add title and metadata
                    worksheet.Cells[1, 1].Value = $"Entity: {data.EntityName}";
                    worksheet.Cells[1, 1].Style.Font.Bold = true;
                    worksheet.Cells[1, 1].Style.Font.Size = 14;

                    worksheet.Cells[2, 1].Value = $"Export Date: {data.ExportTime:yyyy-MM-dd HH:mm:ss}";
                    worksheet.Cells[2, 1].Style.Font.Italic = true;

                    worksheet.Cells[3, 1].Value = $"Total Records: {data.Records.Count}";
                    worksheet.Cells[3, 1].Style.Font.Italic = true;

                    // Add headers (row 5)
                    for (int i = 0; i < data.Columns.Count; i++)
                    {
                        var cell = worksheet.Cells[5, i + 1];
                        cell.Value = data.Columns[i];
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(54, 57, 63));
                        cell.Style.Font.Color.SetColor(Color.White);
                        cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }

                    // Add data
                    for (int row = 0; row < data.Records.Count; row++)
                    {
                        for (int col = 0; col < data.Columns.Count; col++)
                        {
                            var cell = worksheet.Cells[row + 6, col + 1];
                            var columnName = data.Columns[col];

                            if (data.Records[row].ContainsKey(columnName))
                            {
                                cell.Value = data.Records[row][columnName];
                            }

                            // Alternate row colors
                            if (row % 2 == 0)
                            {
                                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(249, 249, 249));
                            }

                            cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        }
                    }

                    // Auto-fit columns
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    // Freeze header row
                    worksheet.View.FreezePanes(6, 1);

                    // Add filter
                    if (data.Records.Count > 0)
                    {
                        var filterRange = worksheet.Cells[5, 1, 5 + data.Records.Count, data.Columns.Count];
                        filterRange.AutoFilter = true;
                    }

                    return package.GetAsByteArray();
                }
            }
            catch (Exception ex)
            {
                _pluginControl.UpdateLog($"Error generating Excel: {ex.Message}");
                return null;
            }
        }

        private HashSet<string> GetValidFieldsForEntity(string entityLogicalName)
        {
            try
            {
                var request = new RetrieveEntityRequest
                {
                    LogicalName = entityLogicalName,
                    EntityFilters = EntityFilters.Attributes,
                    RetrieveAsIfPublished = true
                };

                var response = (RetrieveEntityResponse)_service.Execute(request);

                var validFields = response.EntityMetadata.Attributes
                    .Where(a => a.IsValidForRead.HasValue && a.IsValidForRead.Value)
                    .Where(a => a.DeprecatedVersion == null)
                    .Where(a => a.AttributeType != AttributeTypeCode.PartyList)
                    .Where(a => a.AttributeType != AttributeTypeCode.ManagedProperty)
                    .Select(a => a.LogicalName)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                return validFields;
            }
            catch
            {
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }
        }

        public string GetEntityDataWithPaging(string entityLogicalName, int pageSize, int pageNumber, List<string> selectedFields = null)
        {
            try
            {
                var query = new QueryExpression(entityLogicalName)
                {
                    PageInfo = new PagingInfo
                    {
                        Count = pageSize,
                        PageNumber = pageNumber,
                        PagingCookie = null
                    }
                };

                if (selectedFields != null && selectedFields.Any())
                {
                    query.ColumnSet = new ColumnSet(selectedFields.ToArray());
                }
                else
                {
                    query.ColumnSet = new ColumnSet(true);
                }

                var primaryIdField = GetPrimaryIdField(entityLogicalName);
                if (!string.IsNullOrEmpty(primaryIdField))
                {
                    query.Orders.Add(new OrderExpression(primaryIdField, OrderType.Ascending));
                }

                var results = _service.RetrieveMultiple(query);

                var jsonResult = new JObject
                {
                    ["value"] = JArray.FromObject(results.Entities.Select(e => FormatEntityToJson(e))),
                    ["@odata.count"] = results.Entities.Count,
                    ["@odata.nextLink"] = results.MoreRecords ? "More records available" : null
                };

                return jsonResult.ToString(Formatting.Indented);
            }
            catch (Exception ex)
            {
                _pluginControl.UpdateLog($"Error fetching data for {entityLogicalName}: {ex.Message}");
                return null;
            }
        }

        private string GetPrimaryIdField(string entityLogicalName)
        {
            try
            {
                var request = new RetrieveEntityRequest
                {
                    LogicalName = entityLogicalName,
                    EntityFilters = EntityFilters.Entity,
                    RetrieveAsIfPublished = true
                };

                var response = (RetrieveEntityResponse)_service.Execute(request);
                return response.EntityMetadata.PrimaryIdAttribute;
            }
            catch
            {
                return entityLogicalName + "id";
            }
        }

        private JObject FormatEntityToJson(Entity entity, List<string> selectedFields = null)
        {
            var jsonObject = new JObject();

            if (selectedFields != null && selectedFields.Any())
            {
                foreach (var field in selectedFields)
                {
                    if (entity.Attributes.ContainsKey(field))
                    {
                        var value = entity[field];
                        if (value == null)
                        {
                            jsonObject[field] = null;
                        }
                        else if (value is EntityReference er)
                        {
                            jsonObject[field] = new JObject
                            {
                                ["Id"] = er.Id.ToString(),
                                ["LogicalName"] = er.LogicalName,
                                ["Name"] = er.Name
                            };
                        }
                        else if (value is OptionSetValue osv)
                        {
                            jsonObject[field] = osv.Value;
                        }
                        else if (value is Money money)
                        {
                            jsonObject[field] = money.Value;
                        }
                        else if (value is AliasedValue aliased)
                        {
                            jsonObject[field] = aliased.Value?.ToString();
                        }
                        else if (value is DateTime dt)
                        {
                            jsonObject[field] = dt.ToString("yyyy-MM-ddTHH:mm:ssZ");
                        }
                        else if (value is Guid guid)
                        {
                            jsonObject[field] = guid.ToString();
                        }
                        else
                        {
                            jsonObject[field] = value.ToString();
                        }
                    }
                    else
                    {
                        jsonObject[field] = null;
                    }
                }
            }
            else
            {
                foreach (var attribute in entity.Attributes)
                {
                    if (attribute.Value == null)
                    {
                        jsonObject[attribute.Key] = null;
                    }
                    else if (attribute.Value is EntityReference er)
                    {
                        jsonObject[attribute.Key] = new JObject
                        {
                            ["Id"] = er.Id.ToString(),
                            ["LogicalName"] = er.LogicalName,
                            ["Name"] = er.Name
                        };
                    }
                    else if (attribute.Value is OptionSetValue osv)
                    {
                        jsonObject[attribute.Key] = osv.Value;
                    }
                    else if (attribute.Value is Money money)
                    {
                        jsonObject[attribute.Key] = money.Value;
                    }
                    else if (attribute.Value is AliasedValue aliased)
                    {
                        jsonObject[attribute.Key] = aliased.Value?.ToString();
                    }
                    else if (attribute.Value is DateTime dt)
                    {
                        jsonObject[attribute.Key] = dt.ToString("yyyy-MM-ddTHH:mm:ssZ");
                    }
                    else if (attribute.Value is Guid guid)
                    {
                        jsonObject[attribute.Key] = guid.ToString();
                    }
                    else
                    {
                        jsonObject[attribute.Key] = attribute.Value.ToString();
                    }
                }
            }

            return jsonObject;
        }
    }
}