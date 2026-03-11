namespace CRMTool.Controls
{
    partial class DataverseExplorerControl
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ComboBox cmbEntity;
        private System.Windows.Forms.ComboBox cmbRecordCount;
        private System.Windows.Forms.Button btnFetchData;
        private System.Windows.Forms.Button btnRefreshEntities;
        private System.Windows.Forms.Button btnClearFields;
        private System.Windows.Forms.Button btnExport; // Changed from btnExportJson to btnExport
        private System.Windows.Forms.CheckedListBox clbFields;
        private System.Windows.Forms.TextBox txtFieldSearch;
        private System.Windows.Forms.TextBox txtEntitySearch;
        private System.Windows.Forms.Label lblSelectedCount;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabGrid;
        private System.Windows.Forms.TabPage tabJson;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox txtJsonView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox grpEntity;
        private System.Windows.Forms.GroupBox grpFields;
        private System.Windows.Forms.GroupBox grpResults;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelEntity;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.cmbEntity = new System.Windows.Forms.ComboBox();
            this.cmbRecordCount = new System.Windows.Forms.ComboBox();
            this.btnFetchData = new System.Windows.Forms.Button();
            this.btnRefreshEntities = new System.Windows.Forms.Button();
            this.btnClearFields = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button(); // Changed from btnExportJson to btnExport
            this.clbFields = new System.Windows.Forms.CheckedListBox();
            this.txtFieldSearch = new System.Windows.Forms.TextBox();
            this.txtEntitySearch = new System.Windows.Forms.TextBox();
            this.lblSelectedCount = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabGrid = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.tabJson = new System.Windows.Forms.TabPage();
            this.txtJsonView = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.grpEntity = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanelEntity = new System.Windows.Forms.FlowLayoutPanel();
            this.grpFields = new System.Windows.Forms.GroupBox();
            this.grpResults = new System.Windows.Forms.GroupBox();
            this.tabControl1.SuspendLayout();
            this.tabGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tabJson.SuspendLayout();
            this.grpEntity.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanelEntity.SuspendLayout();
            this.grpFields.SuspendLayout();
            this.grpResults.SuspendLayout();
            this.SuspendLayout();

            // Entity Search TextBox
            this.txtEntitySearch = new System.Windows.Forms.TextBox();
            this.txtEntitySearch.Location = new System.Drawing.Point(145, 8);
            this.txtEntitySearch.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.txtEntitySearch.Name = "txtEntitySearch";
            this.txtEntitySearch.Size = new System.Drawing.Size(350, 27);
            this.txtEntitySearch.TabIndex = 7;
            this.txtEntitySearch.TextChanged += new System.EventHandler(this.txtEntitySearch_TextChanged);
            this.txtEntitySearch.Visible = false;

            // cmbEntity
            this.cmbEntity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEntity.FormattingEnabled = true;
            this.cmbEntity.Location = new System.Drawing.Point(145, 8);
            this.cmbEntity.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.cmbEntity.Name = "cmbEntity";
            this.cmbEntity.Size = new System.Drawing.Size(350, 28);
            this.cmbEntity.TabIndex = 1;
            this.cmbEntity.SelectedIndexChanged += new System.EventHandler(this.cmbEntity_SelectedIndexChanged);
            this.cmbEntity.Visible = true;

            // cmbRecordCount
            this.cmbRecordCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRecordCount.FormattingEnabled = true;
            this.cmbRecordCount.Location = new System.Drawing.Point(806, 8);
            this.cmbRecordCount.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.cmbRecordCount.Name = "cmbRecordCount";
            this.cmbRecordCount.Size = new System.Drawing.Size(80, 28);
            this.cmbRecordCount.TabIndex = 3;

            // btnFetchData
            this.btnFetchData.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.btnFetchData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFetchData.ForeColor = System.Drawing.Color.White;
            this.btnFetchData.Location = new System.Drawing.Point(899, 8);
            this.btnFetchData.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.btnFetchData.Name = "btnFetchData";
            this.btnFetchData.Size = new System.Drawing.Size(120, 30);
            this.btnFetchData.TabIndex = 4;
            this.btnFetchData.Text = "Fetch Data";
            this.btnFetchData.UseVisualStyleBackColor = false;
            this.btnFetchData.Click += new System.EventHandler(this.btnFetchData_Click);

            // btnRefreshEntities
            this.btnRefreshEntities.BackColor = System.Drawing.Color.FromArgb(54, 57, 63);
            this.btnRefreshEntities.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefreshEntities.ForeColor = System.Drawing.Color.White;
            this.btnRefreshEntities.Location = new System.Drawing.Point(508, 8);
            this.btnRefreshEntities.Margin = new System.Windows.Forms.Padding(3, 3, 20, 3);
            this.btnRefreshEntities.Name = "btnRefreshEntities";
            this.btnRefreshEntities.Size = new System.Drawing.Size(100, 30);
            this.btnRefreshEntities.TabIndex = 6;
            this.btnRefreshEntities.Text = "Refresh";
            this.btnRefreshEntities.UseVisualStyleBackColor = false;
            this.btnRefreshEntities.Click += new System.EventHandler(this.btnRefreshEntities_Click);

            // btnClearFields
            this.btnClearFields.BackColor = System.Drawing.Color.FromArgb(54, 57, 63);
            this.btnClearFields.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearFields.ForeColor = System.Drawing.Color.White;
            this.btnClearFields.Location = new System.Drawing.Point(930, 60);
            this.btnClearFields.Name = "btnClearFields";
            this.btnClearFields.Size = new System.Drawing.Size(100, 30);
            this.btnClearFields.TabIndex = 6;
            this.btnClearFields.Text = "Clear All";
            this.btnClearFields.UseVisualStyleBackColor = false;
            this.btnClearFields.Click += new System.EventHandler(this.btnClearFields_Click);

            // btnExport
            this.btnExport.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.btnExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExport.ForeColor = System.Drawing.Color.White;
            this.btnExport.Location = new System.Drawing.Point(1090, 55);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(70, 30);
            this.btnExport.TabIndex = 2;
            this.btnExport.Text = "Export ▼";
            this.btnExport.UseVisualStyleBackColor = false;
            // No Click event - we use MouseDown for context menu

            // clbFields
            this.clbFields.FormattingEnabled = true;
            this.clbFields.Location = new System.Drawing.Point(17, 60);
            this.clbFields.Name = "clbFields";
            this.clbFields.Size = new System.Drawing.Size(900, 106);
            this.clbFields.TabIndex = 5;

            // txtFieldSearch
            this.txtFieldSearch.Location = new System.Drawing.Point(112, 27);
            this.txtFieldSearch.Name = "txtFieldSearch";
            this.txtFieldSearch.Size = new System.Drawing.Size(300, 27);
            this.txtFieldSearch.TabIndex = 4;
            this.txtFieldSearch.TextChanged += new System.EventHandler(this.txtFieldSearch_TextChanged);

            // lblSelectedCount
            this.lblSelectedCount.AutoSize = true;
            this.lblSelectedCount.Location = new System.Drawing.Point(930, 100);
            this.lblSelectedCount.Name = "lblSelectedCount";
            this.lblSelectedCount.Size = new System.Drawing.Size(114, 20);
            this.lblSelectedCount.TabIndex = 7;
            this.lblSelectedCount.Text = "Selected: 0 fields";

            // tabControl1
            this.tabControl1.Controls.Add(this.tabGrid);
            this.tabControl1.Controls.Add(this.tabJson);
            this.tabControl1.Location = new System.Drawing.Point(17, 55);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1060, 290);
            this.tabControl1.TabIndex = 1;

            // tabGrid
            this.tabGrid.Controls.Add(this.dataGridView1);
            this.tabGrid.Location = new System.Drawing.Point(4, 29);
            this.tabGrid.Name = "tabGrid";
            this.tabGrid.Padding = new System.Windows.Forms.Padding(3);
            this.tabGrid.Size = new System.Drawing.Size(1052, 257);
            this.tabGrid.TabIndex = 0;
            this.tabGrid.Text = "Grid View";
            this.tabGrid.UseVisualStyleBackColor = true;

            // dataGridView1
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(1046, 251);
            this.dataGridView1.TabIndex = 0;

            // tabJson
            this.tabJson.Controls.Add(this.txtJsonView);
            this.tabJson.Location = new System.Drawing.Point(4, 29);
            this.tabJson.Name = "tabJson";
            this.tabJson.Padding = new System.Windows.Forms.Padding(3);
            this.tabJson.Size = new System.Drawing.Size(1052, 257);
            this.tabJson.TabIndex = 1;
            this.tabJson.Text = "JSON View";
            this.tabJson.UseVisualStyleBackColor = true;

            // txtJsonView
            this.txtJsonView.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.txtJsonView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtJsonView.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtJsonView.ForeColor = System.Drawing.Color.FromArgb(241, 196, 15);
            this.txtJsonView.Location = new System.Drawing.Point(3, 3);
            this.txtJsonView.Multiline = true;
            this.txtJsonView.Name = "txtJsonView";
            this.txtJsonView.ReadOnly = true;
            this.txtJsonView.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtJsonView.Size = new System.Drawing.Size(1046, 251);
            this.txtJsonView.TabIndex = 0;
            this.txtJsonView.WordWrap = false;

            // label1
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 10, 3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select Entity:";

            // label4
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 10);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 10, 3, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 20);
            this.label4.TabIndex = 8;
            this.label4.Text = "Search Entity:";
            this.label4.Visible = false;

            // label2
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(631, 15);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 10, 3, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Records to fetch:";

            // label3
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 20);
            this.label3.TabIndex = 3;
            this.label3.Text = "Search fields:";

            // flowLayoutPanelEntity
            this.flowLayoutPanelEntity.Controls.Add(this.label4);
            this.flowLayoutPanelEntity.Controls.Add(this.txtEntitySearch);
            this.flowLayoutPanelEntity.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanelEntity.Name = "flowLayoutPanelEntity";
            this.flowLayoutPanelEntity.Size = new System.Drawing.Size(500, 50);
            this.flowLayoutPanelEntity.TabIndex = 9;
            this.flowLayoutPanelEntity.Visible = false;

            // grpEntity
            this.grpEntity.BackColor = System.Drawing.Color.White;
            this.grpEntity.Controls.Add(this.flowLayoutPanel1);
            this.grpEntity.Controls.Add(this.flowLayoutPanelEntity);
            this.grpEntity.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpEntity.Location = new System.Drawing.Point(5, 5);
            this.grpEntity.Margin = new System.Windows.Forms.Padding(5);
            this.grpEntity.Name = "grpEntity";
            this.grpEntity.Padding = new System.Windows.Forms.Padding(10);
            this.grpEntity.Size = new System.Drawing.Size(1170, 90);
            this.grpEntity.TabIndex = 0;
            this.grpEntity.TabStop = false;
            this.grpEntity.Text = "Entity Selection";

            // flowLayoutPanel1
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.cmbEntity);
            this.flowLayoutPanel1.Controls.Add(this.btnRefreshEntities);
            this.flowLayoutPanel1.Controls.Add(this.label2);
            this.flowLayoutPanel1.Controls.Add(this.cmbRecordCount);
            this.flowLayoutPanel1.Controls.Add(this.btnFetchData);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(10, 38);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(5);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1150, 42);
            this.flowLayoutPanel1.TabIndex = 0;

            // grpFields
            this.grpFields.BackColor = System.Drawing.Color.White;
            this.grpFields.Controls.Add(this.label3);
            this.grpFields.Controls.Add(this.txtFieldSearch);
            this.grpFields.Controls.Add(this.clbFields);
            this.grpFields.Controls.Add(this.btnClearFields);
            this.grpFields.Controls.Add(this.lblSelectedCount);
            this.grpFields.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpFields.Location = new System.Drawing.Point(5, 95);
            this.grpFields.Margin = new System.Windows.Forms.Padding(5);
            this.grpFields.Name = "grpFields";
            this.grpFields.Padding = new System.Windows.Forms.Padding(10);
            this.grpFields.Size = new System.Drawing.Size(1170, 180);
            this.grpFields.TabIndex = 1;
            this.grpFields.TabStop = false;
            this.grpFields.Text = "Field Selection";

            // grpResults
            this.grpResults.BackColor = System.Drawing.Color.White;
            this.grpResults.Controls.Add(this.tabControl1);
            this.grpResults.Controls.Add(this.btnExport);
            this.grpResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpResults.Location = new System.Drawing.Point(5, 275);
            this.grpResults.Margin = new System.Windows.Forms.Padding(5);
            this.grpResults.Name = "grpResults";
            this.grpResults.Padding = new System.Windows.Forms.Padding(10);
            this.grpResults.Size = new System.Drawing.Size(1170, 370);
            this.grpResults.TabIndex = 2;
            this.grpResults.TabStop = false;
            this.grpResults.Text = "Results";

            // DataverseExplorerControl
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            this.Controls.Add(this.grpResults);
            this.Controls.Add(this.grpFields);
            this.Controls.Add(this.grpEntity);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "DataverseExplorerControl";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(1180, 650);
            this.tabControl1.ResumeLayout(false);
            this.tabGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tabJson.ResumeLayout(false);
            this.tabJson.PerformLayout();
            this.grpEntity.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanelEntity.ResumeLayout(false);
            this.flowLayoutPanelEntity.PerformLayout();
            this.grpFields.ResumeLayout(false);
            this.grpFields.PerformLayout();
            this.grpResults.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}