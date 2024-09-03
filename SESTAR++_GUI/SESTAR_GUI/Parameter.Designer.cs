
namespace SESTAR_GUI
{
    partial class Parameter
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.minMassBox = new System.Windows.Forms.TextBox();
            this.maxMassBox = new System.Windows.Forms.TextBox();
            this.minLenBox = new System.Windows.Forms.TextBox();
            this.chargeBox = new System.Windows.Forms.TextBox();
            this.ok = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Min Mass";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 97);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Max Mass";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(31, 136);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "Min Length";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(31, 182);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "Charge";
            // 
            // minMassBox
            // 
            this.minMassBox.Location = new System.Drawing.Point(124, 55);
            this.minMassBox.Name = "minMassBox";
            this.minMassBox.Size = new System.Drawing.Size(100, 21);
            this.minMassBox.TabIndex = 4;
            // 
            // maxMassBox
            // 
            this.maxMassBox.Location = new System.Drawing.Point(124, 87);
            this.maxMassBox.Name = "maxMassBox";
            this.maxMassBox.Size = new System.Drawing.Size(100, 21);
            this.maxMassBox.TabIndex = 5;
            // 
            // minLenBox
            // 
            this.minLenBox.Location = new System.Drawing.Point(124, 136);
            this.minLenBox.Name = "minLenBox";
            this.minLenBox.Size = new System.Drawing.Size(100, 21);
            this.minLenBox.TabIndex = 6;
            // 
            // chargeBox
            // 
            this.chargeBox.Location = new System.Drawing.Point(124, 179);
            this.chargeBox.Name = "chargeBox";
            this.chargeBox.Size = new System.Drawing.Size(100, 21);
            this.chargeBox.TabIndex = 7;
            // 
            // ok
            // 
            this.ok.Location = new System.Drawing.Point(101, 258);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(75, 23);
            this.ok.TabIndex = 8;
            this.ok.Text = "OK";
            this.ok.UseVisualStyleBackColor = true;
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // Parameter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(308, 335);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.chargeBox);
            this.Controls.Add(this.minLenBox);
            this.Controls.Add(this.maxMassBox);
            this.Controls.Add(this.minMassBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Parameter";
            this.Text = "Parameter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox minMassBox;
        private System.Windows.Forms.TextBox maxMassBox;
        private System.Windows.Forms.TextBox minLenBox;
        private System.Windows.Forms.TextBox chargeBox;
        private System.Windows.Forms.Button ok;
    }
}