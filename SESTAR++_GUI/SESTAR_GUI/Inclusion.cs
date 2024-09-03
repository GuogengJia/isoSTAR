using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SESTAR_GUI
{
    public partial class Inclusion : Form
    {
        public Inclusion()
        {
            InitializeComponent();
        }

        public bool InclusionList { get; private set; }
        public double RetentionTime
        {
            get
            {
                return double.Parse(retTimeText);
            }
        }
        private string retTimeText = "2";

        private void Inclusion_Load(object sender, EventArgs e)
        {
            RetTime.Text = retTimeText;
            IncluList.Checked = InclusionList;
            if (IncluList.Checked)
                RetTime.Enabled = true;
            else
                RetTime.Enabled = false;
        }

        private void Ok_Click(object sender, EventArgs e)
        {
            if (double.TryParse(RetTime.Text, out _))
            {
                InclusionList = IncluList.Checked;
                retTimeText = RetTime.Text;
                this.Close();
            }
            else
            {
                MessageBox.Show("Retention time is not numeric");
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void IncluList_CheckedChanged(object sender, EventArgs e)
        {
            if (IncluList.Checked)
                RetTime.Enabled = true;
            else
                RetTime.Enabled = false;
        }

        private void Inclusion_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
