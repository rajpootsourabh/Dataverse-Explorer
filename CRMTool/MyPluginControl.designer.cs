namespace CRMTool
{
    partial class MyPluginControl
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ToolStrip toolStripMenu;
        private System.Windows.Forms.ToolStripButton tsbClose;
        private System.Windows.Forms.ToolStripButton tsbDownload;
        private System.Windows.Forms.ToolStripSeparator tssSeparator1;
        private System.Windows.Forms.Button btnChooseFile;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.TextBox txtEntityLog;
        private System.Windows.Forms.TextBox txtStatus;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MyPluginControl));
            this.toolStripMenu = new System.Windows.Forms.ToolStrip();
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.tssSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbDownload = new System.Windows.Forms.ToolStripButton();
            this.btnChooseFile = new System.Windows.Forms.Button();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.txtEntityLog = new System.Windows.Forms.TextBox();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.toolStripMenu.SuspendLayout();
            this.SuspendLayout();

            // ToolStripMenu
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.tsbClose,
                this.tssSeparator1,
                this.tsbDownload
            });

            // Modern toolbar styling
            this.toolStripMenu.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            this.toolStripMenu.ForeColor = System.Drawing.Color.White;
            this.toolStripMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Padding = new System.Windows.Forms.Padding(5, 2, 1, 2);
            this.toolStripMenu.Size = new System.Drawing.Size(1200, 44);
            this.toolStripMenu.TabIndex = 0;
            this.toolStripMenu.Text = "toolStrip1";

            // tsbClose
            this.tsbClose.Image = ((System.Drawing.Image)(resources.GetObject("tsbClose.Image")));
            this.tsbClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbClose.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Size = new System.Drawing.Size(77, 40);
            this.tsbClose.Text = "Close";
            this.tsbClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.tsbClose.Click += new System.EventHandler(this.tsbClose_Click);

            // tssSeparator1
            this.tssSeparator1.Name = "tssSeparator1";
            this.tssSeparator1.Size = new System.Drawing.Size(6, 44);

            // tsbDownload
            this.tsbDownload.Image = ((System.Drawing.Image)(resources.GetObject("tsbDownload.Image")));
            this.tsbDownload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDownload.Margin = new System.Windows.Forms.Padding(5, 1, 10, 2);
            this.tsbDownload.Name = "tsbDownload";
            this.tsbDownload.Size = new System.Drawing.Size(142, 40);
            this.tsbDownload.Text = "Download Sample";
            this.tsbDownload.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.tsbDownload.Click += new System.EventHandler(this.tsbDownload_Click);

            // btnChooseFile
            this.btnChooseFile.BackColor = System.Drawing.Color.FromArgb(54, 57, 63);
            this.btnChooseFile.FlatAppearance.BorderSize = 0;
            this.btnChooseFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChooseFile.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnChooseFile.ForeColor = System.Drawing.Color.White;
            this.btnChooseFile.Location = new System.Drawing.Point(20, 20);
            this.btnChooseFile.Margin = new System.Windows.Forms.Padding(4);
            this.btnChooseFile.Name = "btnChooseFile";
            this.btnChooseFile.Size = new System.Drawing.Size(150, 35);
            this.btnChooseFile.TabIndex = 1;
            this.btnChooseFile.Text = "Choose Excel File";
            this.btnChooseFile.UseVisualStyleBackColor = false;
            this.btnChooseFile.Click += new System.EventHandler(this.btnChooseFile_Click);

            // btnSubmit
            this.btnSubmit.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.btnSubmit.FlatAppearance.BorderSize = 0;
            this.btnSubmit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSubmit.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnSubmit.ForeColor = System.Drawing.Color.White;
            this.btnSubmit.Location = new System.Drawing.Point(180, 20);
            this.btnSubmit.Margin = new System.Windows.Forms.Padding(4);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(150, 35);
            this.btnSubmit.TabIndex = 2;
            this.btnSubmit.Text = "Create Entity";
            this.btnSubmit.UseVisualStyleBackColor = false;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);

            // txtFilePath
            this.txtFilePath.BackColor = System.Drawing.Color.White;
            this.txtFilePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFilePath.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtFilePath.Location = new System.Drawing.Point(20, 70);
            this.txtFilePath.Margin = new System.Windows.Forms.Padding(4);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.ReadOnly = true;
            this.txtFilePath.Size = new System.Drawing.Size(800, 27);
            this.txtFilePath.TabIndex = 3;
            this.txtFilePath.TextChanged += new System.EventHandler(this.txtFilePath_TextChanged);

            // txtStatus
            this.txtStatus.BackColor = System.Drawing.Color.White;
            this.txtStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtStatus.Location = new System.Drawing.Point(20, 110);
            this.txtStatus.Margin = new System.Windows.Forms.Padding(4);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Size = new System.Drawing.Size(800, 27);
            this.txtStatus.TabIndex = 5;

            // txtEntityLog
            this.txtEntityLog.BackColor = System.Drawing.Color.White;
            this.txtEntityLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEntityLog.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtEntityLog.Location = new System.Drawing.Point(20, 150);
            this.txtEntityLog.Margin = new System.Windows.Forms.Padding(4);
            this.txtEntityLog.Multiline = true;
            this.txtEntityLog.Name = "txtEntityLog";
            this.txtEntityLog.ReadOnly = true;
            this.txtEntityLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtEntityLog.Size = new System.Drawing.Size(950, 450);
            this.txtEntityLog.TabIndex = 4;

            // MyPluginControl
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            this.Controls.Add(this.toolStripMenu);
            this.Controls.Add(this.btnChooseFile);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.txtFilePath);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.txtEntityLog);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.Name = "MyPluginControl";
            this.Size = new System.Drawing.Size(1200, 700);
            this.Load += new System.EventHandler(this.MyPluginControl_Load);
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}