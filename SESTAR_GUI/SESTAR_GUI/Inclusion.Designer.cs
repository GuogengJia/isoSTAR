namespace SESTAR_GUI
{
    partial class Inclusion
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.IncluList = new System.Windows.Forms.CheckBox();
            this.RetTime = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Ok = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // IncluList
            // 
            this.IncluList.AutoSize = true;
            this.IncluList.Location = new System.Drawing.Point(71, 65);
            this.IncluList.Name = "IncluList";
            this.IncluList.Size = new System.Drawing.Size(108, 16);
            this.IncluList.TabIndex = 0;
            this.IncluList.Text = "Inclusion List";
            this.IncluList.UseVisualStyleBackColor = true;
            this.IncluList.CheckedChanged += new System.EventHandler(this.IncluList_CheckedChanged);
            // 
            // RetTime
            // 
            this.RetTime.Location = new System.Drawing.Point(71, 141);
            this.RetTime.Name = "RetTime";
            this.RetTime.Size = new System.Drawing.Size(100, 21);
            this.RetTime.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(34, 117);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(179, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "Retention Time Tolerence(min)";
            // 
            // Ok
            // 
            this.Ok.Location = new System.Drawing.Point(36, 199);
            this.Ok.Name = "Ok";
            this.Ok.Size = new System.Drawing.Size(75, 23);
            this.Ok.TabIndex = 4;
            this.Ok.Text = "OK";
            this.Ok.UseVisualStyleBackColor = true;
            this.Ok.Click += new System.EventHandler(this.Ok_Click);
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(138, 199);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 5;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // Inclusion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(255, 275);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.Ok);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.RetTime);
            this.Controls.Add(this.IncluList);
            this.Name = "Inclusion";
            this.Text = "Inclusion";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Inclusion_FormClosing);
            this.Load += new System.EventHandler(this.Inclusion_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox IncluList;
        private System.Windows.Forms.TextBox RetTime;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button Ok;
        private System.Windows.Forms.Button Cancel;
    }
}