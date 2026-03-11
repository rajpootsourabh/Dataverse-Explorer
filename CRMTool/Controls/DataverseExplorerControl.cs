using CRMTool.Services;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using XrmToolBox.Extensibility;

namespace CRMTool.Controls
{
    public partial class DataverseExplorerControl : UserControl
    {
        private IOrganizationService _service;
        private MyPluginControl _mainPlugin;
        private DataverseExplorerService _explorerService;
        private List<DataverseExplorerService.EntityInfo> _entities;
        private List<DataverseExplorerService.EntityInfo> _filteredEntities;
        private List<DataverseExplorerService.FieldInfo> _entityFields;
        private List<string> _selectedFields;
        private string _selectedEntity;
        private Timer _searchTimer;
        private Timer _entitySearchTimer;
        private string _currentJsonData; // Store current data for export

        // Dictionary to map display text to field info
        private Dictionary<string, DataverseExplorerService.FieldInfo> _displayToFieldMap;

        private readonly int[] RecordCounts = { 10, 25, 50, 100, 250, 500, 1000 };

        public DataverseExplorerControl(IOrganizationService service, MyPluginControl mainPlugin)
        {
            // Set EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            InitializeComponent();
            _service = service;
            _mainPlugin = mainPlugin;
            _explorerService = new DataverseExplorerService(service, mainPlugin);
            _selectedFields = new List<string>();
            _entityFields = new List<DataverseExplorerService.FieldInfo>();
            _displayToFieldMap = new Dictionary<string, DataverseExplorerService.FieldInfo>();

            InitializeCustomComponents();
            LoadEntities();
        }

        private void InitializeCustomComponents()
        {
            // Setup search timers
            _searchTimer = new Timer { Interval = 300 };
            _searchTimer.Tick += SearchTimer_Tick;

            _entitySearchTimer = new Timer { Interval = 300 };
            _entitySearchTimer.Tick += EntitySearchTimer_Tick;

            // Initialize record count combo
            cmbRecordCount.Items.Clear();
            foreach (var count in RecordCounts)
            {
                cmbRecordCount.Items.Add(count.ToString());
            }
            cmbRecordCount.SelectedIndex = 0;

            // Setup checked list box
            clbFields.ItemCheck += ClbFields_ItemCheck;
            clbFields.CheckOnClick = true;

            // Initialize labels
            lblSelectedCount.Text = "Selected: 0 fields";

            // Initially show combo box, hide search
            cmbEntity.Visible = true;
            txtEntitySearch.Visible = false;
            label1.Visible = true;
            label4.Visible = false;

            // Add export options to dropdown
            InitializeExportMenu();
        }

        private void InitializeExportMenu()
        {
            // Create context menu strip for export options
            var exportMenu = new ContextMenuStrip();

            var exportJsonItem = new ToolStripMenuItem("Export as JSON", null, (s, e) => ExportJson());
            var exportExcelItem = new ToolStripMenuItem("Export as Excel", null, (s, e) => ExportExcel());

            exportMenu.Items.Add(exportJsonItem);
            exportMenu.Items.Add(exportExcelItem);

            // Attach menu to export button
            btnExport.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    exportMenu.Show(btnExport, new Point(0, btnExport.Height));
                }
            };
        }

        private void LoadEntities()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                _entities = _explorerService.GetAllEntities();
                _filteredEntities = new List<DataverseExplorerService.EntityInfo>(_entities);

                UpdateEntityComboBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading entities: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _mainPlugin?.UpdateLog($"Error loading entities: {ex.Message}");
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void UpdateEntityComboBox()
        {
            cmbEntity.Items.Clear();
            cmbEntity.Items.Add("-- Select Entity --");

            foreach (var entity in _filteredEntities)
            {
                cmbEntity.Items.Add($"{entity.DisplayName} ({entity.LogicalName})");
            }

            if (cmbEntity.Items.Count > 0)
                cmbEntity.SelectedIndex = 0;
        }

        private void txtEntitySearch_TextChanged(object sender, EventArgs e)
        {
            _entitySearchTimer.Stop();
            _entitySearchTimer.Start();
        }

        private void EntitySearchTimer_Tick(object sender, EventArgs e)
        {
            _entitySearchTimer.Stop();
            FilterEntities();
        }

        private void FilterEntities()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtEntitySearch.Text))
                {
                    _filteredEntities = new List<DataverseExplorerService.EntityInfo>(_entities);
                }
                else
                {
                    var searchText = txtEntitySearch.Text.ToLower();
                    _filteredEntities = _entities
                        .Where(e => e.DisplayName.ToLower().Contains(searchText) ||
                                   e.LogicalName.ToLower().Contains(searchText))
                        .ToList();
                }

                UpdateEntityComboBox();

                if (_filteredEntities.Count == 0)
                {
                    _mainPlugin?.UpdateLog($"No entities found matching '{txtEntitySearch.Text}'");
                }
            }
            catch (Exception ex)
            {
                _mainPlugin?.UpdateLog($"Error filtering entities: {ex.Message}");
            }
        }

        private void cmbEntity_SelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            if (cmbEntity.SelectedIndex <= 0)
            {
                _selectedEntity = null;
                _entityFields.Clear();
                _selectedFields.Clear();
                _displayToFieldMap.Clear();
                UpdateFieldsList();
                return;
            }

            var selectedItem = cmbEntity.SelectedItem.ToString();
            var logicalName = ExtractLogicalName(selectedItem);
            _selectedEntity = _filteredEntities.FirstOrDefault(entity => entity.LogicalName == logicalName)?.LogicalName;

            if (!string.IsNullOrEmpty(_selectedEntity))
            {
                LoadEntityFields();
            }
        }

        private string ExtractLogicalName(string itemText)
        {
            try
            {
                var start = itemText.LastIndexOf('(') + 1;
                var end = itemText.LastIndexOf(')');
                if (start > 0 && end > start)
                {
                    return itemText.Substring(start, end - start);
                }
            }
            catch { }
            return itemText;
        }

        private void LoadEntityFields()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                _entityFields = _explorerService.GetEntityFields(_selectedEntity);
                BuildDisplayToFieldMap();
                UpdateFieldsList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading fields: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _mainPlugin?.UpdateLog($"Error loading fields: {ex.Message}");
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void BuildDisplayToFieldMap()
        {
            _displayToFieldMap.Clear();
            foreach (var field in _entityFields)
            {
                string displayText = $"{field.DisplayName} ({field.LogicalName}) - {field.Type}";
                _displayToFieldMap[displayText] = field;
            }
        }

        private void UpdateFieldsList()
        {
            clbFields.Items.Clear();

            var fieldsToShow = _entityFields;

            if (!string.IsNullOrEmpty(txtFieldSearch.Text))
            {
                var searchText = txtFieldSearch.Text.ToLower();
                fieldsToShow = _entityFields
                    .Where(field => field.DisplayName.ToLower().Contains(searchText) ||
                                field.LogicalName.ToLower().Contains(searchText) ||
                                field.Type.ToLower().Contains(searchText))
                    .ToList();
            }

            foreach (var field in fieldsToShow)
            {
                string displayText = $"{field.DisplayName} ({field.LogicalName}) - {field.Type}";
                clbFields.Items.Add(displayText, _selectedFields.Contains(field.LogicalName));
            }

            UpdateSelectedFieldsCount();
        }

        private void ClbFields_ItemCheck(object sender, ItemCheckEventArgs itemCheckEvent)
        {
            var displayText = clbFields.Items[itemCheckEvent.Index].ToString();

            // Get the field info from the map
            if (_displayToFieldMap.TryGetValue(displayText, out var fieldInfo))
            {
                if (itemCheckEvent.NewValue == CheckState.Checked)
                {
                    if (!_selectedFields.Contains(fieldInfo.LogicalName))
                    {
                        _selectedFields.Add(fieldInfo.LogicalName);
                        _mainPlugin?.UpdateLog($"Selected field: {fieldInfo.LogicalName} ({fieldInfo.DisplayName})");
                    }
                }
                else
                {
                    _selectedFields.Remove(fieldInfo.LogicalName);
                    _mainPlugin?.UpdateLog($"Deselected field: {fieldInfo.LogicalName}");
                }
            }

            UpdateSelectedFieldsCount();
        }

        private void UpdateSelectedFieldsCount()
        {
            if (lblSelectedCount.InvokeRequired)
            {
                lblSelectedCount.Invoke(new Action(() =>
                    lblSelectedCount.Text = $"Selected: {_selectedFields.Count} field(s)"));
            }
            else
            {
                lblSelectedCount.Text = $"Selected: {_selectedFields.Count} field(s)";
            }
        }

        private void btnFetchData_Click(object sender, EventArgs eventArgs)
        {
            if (string.IsNullOrEmpty(_selectedEntity))
            {
                MessageBox.Show("Please select an entity first.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Show which fields are selected
            if (_selectedFields.Any())
            {
                var fieldList = string.Join(", ", _selectedFields.Take(5));
                if (_selectedFields.Count > 5)
                    fieldList += $" and {_selectedFields.Count - 5} more...";

                var result = MessageBox.Show(
                    $"Fetching data with {_selectedFields.Count} selected fields:\n{fieldList}\n\nContinue?",
                    "Confirm Field Selection",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.No)
                    return;
            }

            FetchEntityData();
        }

        private void FetchEntityData()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                int recordCount = int.Parse(cmbRecordCount.Text);

                // Log what we're fetching
                _mainPlugin?.UpdateLog($"Fetching {recordCount} records from {_selectedEntity}");
                if (_selectedFields.Any())
                {
                    _mainPlugin?.UpdateLog($"Selected fields: {string.Join(", ", _selectedFields)}");
                }

                var data = _explorerService.GetEntityData(_selectedEntity, recordCount, _selectedFields);

                if (data != null)
                {
                    _currentJsonData = data;
                    txtJsonView.Text = data;

                    // Parse and display in grid
                    DisplayDataInGrid(data);

                    // Switch to JSON view tab
                    if (tabControl1.TabPages.Contains(tabJson))
                        tabControl1.SelectedTab = tabJson;

                    _mainPlugin?.UpdateLog($"Successfully fetched {recordCount} records from {_selectedEntity}");
                }
                else
                {
                    MessageBox.Show("No data returned or an error occurred.", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _mainPlugin?.UpdateLog($"Error fetching data: {ex.Message}");
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void DisplayDataInGrid(string jsonData)
        {
            try
            {
                dataGridView1.SuspendLayout();
                dataGridView1.Columns.Clear();
                dataGridView1.Rows.Clear();

                var parsedData = JObject.Parse(jsonData);
                var records = parsedData["value"] as JArray;

                if (records == null || records.Count == 0)
                {
                    dataGridView1.Columns.Add("Message", "Message");
                    dataGridView1.Rows.Add("No records found for this entity");
                    return;
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

                // Add columns
                foreach (var column in columns.OrderBy(columnName => columnName))
                {
                    dataGridView1.Columns.Add(column, column);
                }

                // Add rows
                foreach (var record in records)
                {
                    var row = new List<object>();
                    foreach (DataGridViewColumn gridColumn in dataGridView1.Columns)
                    {
                        var value = record[gridColumn.Name];
                        if (value != null)
                        {
                            if (value.Type == JTokenType.Object)
                            {
                                row.Add("[Complex Type]");
                            }
                            else
                            {
                                row.Add(value.ToString());
                            }
                        }
                        else
                        {
                            row.Add("");
                        }
                    }
                    dataGridView1.Rows.Add(row.ToArray());
                }

                dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

                // Limit column width
                foreach (DataGridViewColumn gridColumn in dataGridView1.Columns)
                {
                    if (gridColumn.Width > 300)
                        gridColumn.Width = 300;
                }
            }
            catch (Exception ex)
            {
                _mainPlugin?.UpdateLog($"Error displaying data in grid: {ex.Message}");
            }
            finally
            {
                dataGridView1.ResumeLayout();
            }
        }

        private void btnClearFields_Click(object sender, EventArgs eventArgs)
        {
            _selectedFields.Clear();
            for (int i = 0; i < clbFields.Items.Count; i++)
            {
                clbFields.SetItemChecked(i, false);
            }
            UpdateSelectedFieldsCount();
            _mainPlugin?.UpdateLog("Cleared all field selections");
        }

        private void txtFieldSearch_TextChanged(object sender, EventArgs eventArgs)
        {
            _searchTimer.Stop();
            _searchTimer.Start();
        }

        private void SearchTimer_Tick(object sender, EventArgs eventArgs)
        {
            _searchTimer.Stop();
            UpdateFieldsList();
        }

        private void btnRefreshEntities_Click(object sender, EventArgs eventArgs)
        {
            LoadEntities();
        }

        private void ExportJson()
        {
            if (string.IsNullOrEmpty(_currentJsonData))
            {
                MessageBox.Show("No data to export. Please fetch data first.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "JSON Files (*.json)|*.json";
                saveDialog.FileName = $"{_selectedEntity}_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                saveDialog.Title = "Export JSON Data";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        System.IO.File.WriteAllText(saveDialog.FileName, _currentJsonData);
                        MessageBox.Show($"Data exported successfully to:\n{saveDialog.FileName}",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _mainPlugin?.UpdateLog($"Exported JSON to: {saveDialog.FileName}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error exporting data: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ExportExcel()
        {
            if (string.IsNullOrEmpty(_currentJsonData))
            {
                MessageBox.Show("No data to export. Please fetch data first.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
                saveDialog.FileName = $"{_selectedEntity}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                saveDialog.Title = "Export Excel Data";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        CreateExcelFile(saveDialog.FileName);
                        MessageBox.Show($"Data exported successfully to:\n{saveDialog.FileName}",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _mainPlugin?.UpdateLog($"Exported Excel to: {saveDialog.FileName}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error exporting data: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void CreateExcelFile(string filePath)
        {
            using (var package = new ExcelPackage())
            {
                var parsedData = JObject.Parse(_currentJsonData);
                var records = parsedData["value"] as JArray;

                if (records == null || records.Count == 0)
                {
                    var emptySheet = package.Workbook.Worksheets.Add("Data");
                    emptySheet.Cells[1, 1].Value = "No records found";
                    package.SaveAs(new FileInfo(filePath));
                    return;
                }

                // Create main data worksheet
                var worksheet = package.Workbook.Worksheets.Add(_selectedEntity ?? "Data");

                // Get all unique column names and sort them
                var columns = new HashSet<string>();
                foreach (var record in records)
                {
                    foreach (var prop in ((JObject)record).Properties())
                    {
                        columns.Add(prop.Name);
                    }
                }
                var sortedColumns = columns.OrderBy(c => c).ToList();

                // Add headers with logical names
                for (int i = 0; i < sortedColumns.Count; i++)
                {
                    var cell = worksheet.Cells[1, i + 1];
                    cell.Value = sortedColumns[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                    cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                // Add data rows
                for (int row = 0; row < records.Count; row++)
                {
                    var record = records[row] as JObject;
                    for (int col = 0; col < sortedColumns.Count; col++)
                    {
                        var columnName = sortedColumns[col];
                        var value = record[columnName];

                        var cell = worksheet.Cells[row + 2, col + 1];

                        if (value == null || value.Type == JTokenType.Null)
                        {
                            cell.Value = null;
                        }
                        else if (value.Type == JTokenType.Object)
                        {
                            cell.Value = "[Complex Type]";
                        }
                        else if (value.Type == JTokenType.Boolean)
                        {
                            cell.Value = value.Value<bool>();
                        }
                        else if (value.Type == JTokenType.Integer)
                        {
                            cell.Value = value.Value<long>();
                        }
                        else if (value.Type == JTokenType.Float)
                        {
                            cell.Value = value.Value<double>();
                        }
                        else
                        {
                            cell.Value = value.ToString();
                        }

                        cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }
                }

                // Create metadata worksheet
                var metadataSheet = package.Workbook.Worksheets.Add("Metadata");
                metadataSheet.Cells[1, 1].Value = "Property";
                metadataSheet.Cells[1, 2].Value = "Value";
                metadataSheet.Cells[1, 1, 1, 2].Style.Font.Bold = true;
                metadataSheet.Cells[1, 1, 1, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                metadataSheet.Cells[1, 1, 1, 2].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                metadataSheet.Cells[2, 1].Value = "Entity";
                metadataSheet.Cells[2, 2].Value = _selectedEntity;
                metadataSheet.Cells[3, 1].Value = "Record Count";
                metadataSheet.Cells[3, 2].Value = records.Count;
                metadataSheet.Cells[4, 1].Value = "Export Date";
                metadataSheet.Cells[4, 2].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                metadataSheet.Cells[5, 1].Value = "Selected Fields";
                metadataSheet.Cells[5, 2].Value = _selectedFields.Any() ? string.Join(", ", _selectedFields) : "All Fields";

                metadataSheet.Columns[1].AutoFit();
                metadataSheet.Columns[2].AutoFit();

                // Create field information worksheet
                if (_entityFields.Any())
                {
                    var fieldsSheet = package.Workbook.Worksheets.Add("Field Info");
                    fieldsSheet.Cells[1, 1].Value = "Logical Name";
                    fieldsSheet.Cells[1, 2].Value = "Display Name";
                    fieldsSheet.Cells[1, 3].Value = "Type";
                    fieldsSheet.Cells[1, 4].Value = "Selected";

                    fieldsSheet.Cells[1, 1, 1, 4].Style.Font.Bold = true;
                    fieldsSheet.Cells[1, 1, 1, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    fieldsSheet.Cells[1, 1, 1, 4].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                    for (int i = 0; i < _entityFields.Count; i++)
                    {
                        var field = _entityFields[i];
                        fieldsSheet.Cells[i + 2, 1].Value = field.LogicalName;
                        fieldsSheet.Cells[i + 2, 2].Value = field.DisplayName;
                        fieldsSheet.Cells[i + 2, 3].Value = field.Type;
                        fieldsSheet.Cells[i + 2, 4].Value = _selectedFields.Contains(field.LogicalName) ? "Yes" : "No";

                        if (_selectedFields.Contains(field.LogicalName))
                        {
                            fieldsSheet.Cells[i + 2, 1, i + 2, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            fieldsSheet.Cells[i + 2, 1, i + 2, 4].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                        }
                    }

                    fieldsSheet.Columns[1].AutoFit();
                    fieldsSheet.Columns[2].AutoFit();
                    fieldsSheet.Columns[3].AutoFit();
                    fieldsSheet.Columns[4].AutoFit();
                }

                // Auto-fit columns in main worksheet
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Save the file
                package.SaveAs(new FileInfo(filePath));
            }
        }
    }
}