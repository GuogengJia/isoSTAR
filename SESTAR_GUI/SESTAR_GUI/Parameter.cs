using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//using static SESTARhelper.SESTARHelper;

namespace SESTAR_GUI
{
    public partial class Parameter : Form
    {
        private int minMass;
        private int maxMass;
        private int minLen;
        private ushort[] charge;
        public Parameter()
        {
            InitializeComponent();
        }

        private void ok_Click(object sender, EventArgs e)
        {
            //minMass = int.Parse(minMassBox.Text);
            //maxMass = int.Parse(maxMassBox.Text);
            //minLen = int.Parse(minLenBox.Text);
            //string[] tmp = chargeBox.Text.Split(',');
            //charge = new ushort[tmp.Length];
            //for (int i = 0; i < tmp.Length; i++)
            //{
            //    charge[i] = ushort.Parse(tmp[i]);
            //}
            //ChangeAcceptCharge(charge, charge.Length);
            //ReInitialize(minMass, maxMass, minLen);
            //SaveParams();
        }
    }
}
