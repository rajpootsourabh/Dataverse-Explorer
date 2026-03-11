using CRMTool.Services;
using Microsoft.Xrm.Sdk;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using CRMTool.Controls;

namespace CRMTool
{
    public partial class MyPluginControl : PluginControlBase
    {
        private IOrganizationService _service;
        private string excelFilePath;
        private TabControl mainTabControl;
        private TabPage tabEntityCreator;
        private TabPage tabExplorer;
        private DataverseExplorerControl explorerControl;
        private Panel topPanel;

        private EntityCreationService _entityCreationService;
        private EntityValidationService _entityValidationService;

        public MyPluginControl()
        {
            InitializeComponent();
            _entityValidationService = new EntityValidationService();

            // Initialize the tab control and tabs after components are initialized
            this.Load += MyPluginControl_Load;
        }

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
            _service = Service;
            _entityCreationService = new EntityCreationService(_service, this);

            // Create the tab interface
            CreateTabInterface();
        }

        private void CreateTabInterface()
        {
            this.SuspendLayout();

            // Clear existing controls from designer
            this.Controls.Clear();

            // Create top panel for toolbar
            topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(45, 45, 48)
            };

            // Add toolbar buttons to top panel
            var closeButton = new Button
            {
                Text = "Close",
                Location = new Point(10, 10),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(54, 57, 63),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            closeButton.Click += tsbClose_Click;
            topPanel.Controls.Add(closeButton);

            var downloadButton = new Button
            {
                Text = "Download Sample",
                Location = new Point(100, 10),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(54, 57, 63),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            downloadButton.Click += tsbDownload_Click;
            topPanel.Controls.Add(downloadButton);

            // Create TabControl
            mainTabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F)
            };

            // Create Entity Creator Tab
            tabEntityCreator = new TabPage("Entity Creator");
            CreateEntityCreatorTab();

            // Create Explorer Tab
            tabExplorer = new TabPage("Dataverse Explorer");

            // Initialize explorer control
            explorerControl = new DataverseExplorerControl(_service, this);
            explorerControl.Dock = DockStyle.Fill;
            tabExplorer.Controls.Add(explorerControl);

            // Add tabs to TabControl
            mainTabControl.TabPages.Add(tabEntityCreator);
            mainTabControl.TabPages.Add(tabExplorer);

            // Add controls to form
            this.Controls.Add(mainTabControl);
            this.Controls.Add(topPanel);

            this.ResumeLayout(true);
        }

        private void CreateEntityCreatorTab()
        {
            tabEntityCreator.SuspendLayout();

            // Choose File Button
            var btnChooseFile = new Button
            {
                Text = "Choose Excel File",
                Location = new Point(20, 20),
                Size = new Size(150, 35),
                BackColor = Color.FromArgb(54, 57, 63),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnChooseFile.Click += btnChooseFile_Click;
            tabEntityCreator.Controls.Add(btnChooseFile);

            // Submit Button
            var btnSubmit = new Button
            {
                Text = "Create Entity",
                Location = new Point(180, 20),
                Size = new Size(150, 35),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSubmit.Click += btnSubmit_Click;
            tabEntityCreator.Controls.Add(btnSubmit);

            // File Path TextBox
            var txtFilePath = new TextBox
            {
                Location = new Point(20, 70),
                Size = new Size(800, 27),
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtFilePath.TextChanged += txtFilePath_TextChanged;
            tabEntityCreator.Controls.Add(txtFilePath);
            this.txtFilePath = txtFilePath;

            // Status TextBox
            var txtStatus = new TextBox
            {
                Location = new Point(20, 110),
                Size = new Size(800, 27),
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle
            };
            tabEntityCreator.Controls.Add(txtStatus);
            this.txtStatus = txtStatus;

            // Log TextBox
            var txtEntityLog = new TextBox
            {
                Location = new Point(20, 150),
                Size = new Size(950, 450),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 9F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            tabEntityCreator.Controls.Add(txtEntityLog);
            this.txtEntityLog = txtEntityLog;

            tabEntityCreator.ResumeLayout(false);
            tabEntityCreator.PerformLayout();
        }

        private async void btnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(excelFilePath))
            {
                MessageBox.Show("Please select an Excel file before submitting.");
                return;
            }

            txtStatus.Text = "Processing...";
            txtStatus.ForeColor = Color.Blue;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var entities = ExcelDataLoader.LoadExcelEntities(excelFilePath);

            foreach (var entity in entities)
            {
                List<string> validationErrors;

                UpdateLog($"Validating entity: {entity["schema name"]}...");
                await Task.Delay(500);

                // Validate the entity
                if (_entityValidationService.ValidateEntityProperties(entity, out validationErrors))
                {
                    UpdateLog($"Creating entity '{entity["schema name"]}'...");
                    await _entityCreationService.CreateCustomEntityAsync(entity);
                }
                else
                {
                    MessageBox.Show(string.Join(Environment.NewLine, validationErrors));
                }
            }

            stopwatch.Stop();

            txtStatus.Text = $"Task finished in {stopwatch.Elapsed.TotalSeconds:F2} seconds.";
            txtStatus.ForeColor = Color.Green;
        }

        private void btnChooseFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
                openFileDialog.Title = "Select an Excel File";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    excelFilePath = openFileDialog.FileName;
                    txtFilePath.Text = "Path: " + excelFilePath;
                }
            }
        }

        private void tsbDownload_Click(object sender, EventArgs e)
        {
            string projectDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));
            string resourceDir = Path.Combine(projectDir, "Resources");
            string sampleFileName = "CRMEntitySample.xlsx";
            string sampleFilePath = Path.Combine(resourceDir, sampleFileName);

            if (!File.Exists(sampleFilePath))
            {
                MessageBox.Show($"Sample file not found at: {sampleFilePath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
                saveFileDialog.Title = "Save Sample Excel File";
                saveFileDialog.FileName = sampleFileName;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.Copy(sampleFilePath, saveFileDialog.FileName, true);
                        MessageBox.Show("Sample file downloaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void tsbClose_Click(object sender, EventArgs e)
        {
            this.CloseTool();
        }

        private void txtFilePath_TextChanged(object sender, EventArgs e)
        {
            // Optional
        }

        public void UpdateLog(string message)
        {
            if (txtEntityLog.InvokeRequired)
            {
                txtEntityLog.Invoke(new Action(() => UpdateLog(message)));
            }
            else
            {
                txtEntityLog.AppendText($"{DateTime.Now}: {message}\r\n\r\n");
                txtEntityLog.ScrollToCaret();
            }
        }
    }
}