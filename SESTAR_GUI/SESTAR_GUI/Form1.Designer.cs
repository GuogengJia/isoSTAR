namespace SESTAR_GUI
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.open = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.file = new System.Windows.Forms.DataGridView();
            this.ss = new System.Windows.Forms.TextBox();
            this.sd = new System.Windows.Forms.TextBox();
            this.minPeak = new System.Windows.Forms.TextBox();
            this.maxPeak = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.parameters = new System.Windows.Forms.GroupBox();
            this.calc = new System.Windows.Forms.Button();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inclusionListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.peptideMatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.acceptedParametersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cancel = new System.Windows.Forms.Button();
            this.status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.searchMethod = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.files = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.save = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.file)).BeginInit();
            this.parameters.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // open
            // 
            this.open.Location = new System.Drawing.Point(22, 26);
            this.open.Margin = new System.Windows.Forms.Padding(2);
            this.open.Name = "open";
            this.open.Size = new System.Drawing.Size(82, 22);
            this.open.TabIndex = 0;
            this.open.Text = "Open Files";
            this.open.UseVisualStyleBackColor = true;
            this.open.Click += new System.EventHandler(this.Open_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Multiselect = true;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(36, 317);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(2);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(748, 21);
            this.progressBar1.TabIndex = 1;
            // 
            // file
            // 
            this.file.AllowUserToAddRows = false;
            this.file.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.file.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.status,
            this.searchMethod,
            this.files,
            this.save});
            this.file.Location = new System.Drawing.Point(200, 42);
            this.file.Margin = new System.Windows.Forms.Padding(2);
            this.file.Name = "file";
            this.file.RowHeadersWidth = 82;
            this.file.RowTemplate.Height = 37;
            this.file.Size = new System.Drawing.Size(597, 256);
            this.file.TabIndex = 2;
            // 
            // ss
            // 
            this.ss.Location = new System.Drawing.Point(89, 42);
            this.ss.Margin = new System.Windows.Forms.Padding(2);
            this.ss.Name = "ss";
            this.ss.Size = new System.Drawing.Size(52, 21);
            this.ss.TabIndex = 3;
            this.ss.Text = "10";
            // 
            // sd
            // 
            this.sd.Location = new System.Drawing.Point(89, 79);
            this.sd.Margin = new System.Windows.Forms.Padding(2);
            this.sd.Name = "sd";
            this.sd.Size = new System.Drawing.Size(52, 21);
            this.sd.TabIndex = 4;
            this.sd.Text = "6";
            // 
            // minPeak
            // 
            this.minPeak.Location = new System.Drawing.Point(89, 116);
            this.minPeak.Margin = new System.Windows.Forms.Padding(2);
            this.minPeak.Name = "minPeak";
            this.minPeak.Size = new System.Drawing.Size(52, 21);
            this.minPeak.TabIndex = 5;
            this.minPeak.Text = "6";
            // 
            // maxPeak
            // 
            this.maxPeak.Location = new System.Drawing.Point(89, 154);
            this.maxPeak.Margin = new System.Windows.Forms.Padding(2);
            this.maxPeak.Name = "maxPeak";
            this.maxPeak.Size = new System.Drawing.Size(52, 21);
            this.maxPeak.TabIndex = 6;
            this.maxPeak.Text = "14";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 43);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "SS";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 80);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "1/SD";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 118);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "min peaks";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 156);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 10;
            this.label4.Text = "max peaks";
            // 
            // parameters
            // 
            this.parameters.Controls.Add(this.ss);
            this.parameters.Controls.Add(this.label4);
            this.parameters.Controls.Add(this.sd);
            this.parameters.Controls.Add(this.label3);
            this.parameters.Controls.Add(this.minPeak);
            this.parameters.Controls.Add(this.label2);
            this.parameters.Controls.Add(this.maxPeak);
            this.parameters.Controls.Add(this.label1);
            this.parameters.Location = new System.Drawing.Point(22, 100);
            this.parameters.Margin = new System.Windows.Forms.Padding(2);
            this.parameters.Name = "parameters";
            this.parameters.Padding = new System.Windows.Forms.Padding(2);
            this.parameters.Size = new System.Drawing.Size(154, 198);
            this.parameters.TabIndex = 11;
            this.parameters.TabStop = false;
            this.parameters.Text = "Parameters";
            // 
            // calc
            // 
            this.calc.Location = new System.Drawing.Point(22, 53);
            this.calc.Margin = new System.Windows.Forms.Padding(2);
            this.calc.Name = "calc";
            this.calc.Size = new System.Drawing.Size(82, 19);
            this.calc.TabIndex = 12;
            this.calc.Text = "Start";
            this.calc.UseVisualStyleBackColor = true;
            this.calc.Click += new System.EventHandler(this.Calc_Click);
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.FileName = "openFileDialog2";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(849, 25);
            this.menuStrip1.TabIndex = 14;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.inclusionListToolStripMenuItem,
            this.peptideMatchToolStripMenuItem,
            this.acceptedParametersToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(52, 21);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // inclusionListToolStripMenuItem
            // 
            this.inclusionListToolStripMenuItem.Name = "inclusionListToolStripMenuItem";
            this.inclusionListToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.inclusionListToolStripMenuItem.Text = "Inclusion List";
            this.inclusionListToolStripMenuItem.Click += new System.EventHandler(this.inclusionListToolStripMenuItem_Click);
            // 
            // peptideMatchToolStripMenuItem
            // 
            this.peptideMatchToolStripMenuItem.Name = "peptideMatchToolStripMenuItem";
            this.peptideMatchToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.peptideMatchToolStripMenuItem.Text = "Peptide Match";
            this.peptideMatchToolStripMenuItem.Click += new System.EventHandler(this.peptideMatchToolStripMenuItem_Click);
            // 
            // acceptedParametersToolStripMenuItem
            // 
            this.acceptedParametersToolStripMenuItem.Name = "acceptedParametersToolStripMenuItem";
            this.acceptedParametersToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.acceptedParametersToolStripMenuItem.Text = "Accepted Parameters";
            this.acceptedParametersToolStripMenuItem.Click += new System.EventHandler(this.acceptedParametersToolStripMenuItem_Click);
            // 
            // cancel
            // 
            this.cancel.Location = new System.Drawing.Point(22, 76);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(82, 19);
            this.cancel.TabIndex = 15;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // status
            // 
            this.status.HeaderText = "Status";
            this.status.MinimumWidth = 10;
            this.status.Name = "status";
            this.status.ReadOnly = true;
            this.status.Width = 200;
            // 
            // searchMethod
            // 
            this.searchMethod.HeaderText = "Search Method";
            this.searchMethod.Items.AddRange(new object[] {
            "Se (MS1)",
            "4SU (MS1)",
            "GGtag (MS1)",
            "4SU (MS2)",
            "GGtag (MS2)"});
            this.searchMethod.Name = "searchMethod";
            // 
            // files
            // 
            this.files.HeaderText = "Files";
            this.files.MinimumWidth = 10;
            this.files.Name = "files";
            this.files.Width = 200;
            // 
            // save
            // 
            this.save.HeaderText = "Output Name";
            this.save.MinimumWidth = 10;
            this.save.Name = "save";
            this.save.Width = 200;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(849, 375);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.calc);
            this.Controls.Add(this.parameters);
            this.Controls.Add(this.file);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.open);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "SESTAR++";
            ((System.ComponentModel.ISupportInitialize)(this.file)).EndInit();
            this.parameters.ResumeLayout(false);
            this.parameters.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button open;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.DataGridView file;
        private System.Windows.Forms.TextBox ss;
        private System.Windows.Forms.TextBox sd;
        private System.Windows.Forms.TextBox minPeak;
        private System.Windows.Forms.TextBox maxPeak;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox parameters;
        private System.Windows.Forms.Button calc;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem inclusionListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem peptideMatchToolStripMenuItem;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.ToolStripMenuItem acceptedParametersToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn status;
        private System.Windows.Forms.DataGridViewComboBoxColumn searchMethod;
        private System.Windows.Forms.DataGridViewTextBoxColumn files;
        private System.Windows.Forms.DataGridViewTextBoxColumn save;
    }
}

