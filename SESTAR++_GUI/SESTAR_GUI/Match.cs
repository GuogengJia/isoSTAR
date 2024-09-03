using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using static SESTAR_GUI.Function;

namespace SESTAR_GUI
{
    public partial class Match : Form
    {
        private TextBox inputBox;
        private EditableListView aminoList;
        private EditableListView peptideList;

        public Match()
        {
            InitializeComponent();
            
            //listView1.Items.Add(new ListViewItem(new string[] { "A", "123" }));
            //listView1.Items.Add(new ListViewItem(new string[] { "B", "456" }));
            aminoList = new EditableListView(listView1);
            aminoList.AddColumn("Amino Acid", true, (string a) => 
            {
                Regex r = new Regex("([BJOXZ]|[^A-Z])");
                if (a.Length != 1)
                {
                    if (a == "NTERM" || a == "CTERM")
                        return true;
                    else
                        return false;
                }
                if (r.IsMatch(a))
                    return false;
                else
                    return true;
            });
            aminoList.AddColumn("Static Modification", true, (string a) => double.TryParse(a, out _));
            //aminoList.AddRow(new string[] { "A", "123" });
            //aminoList.AddRow(new string[] { "B", "456" });
            peptideList = new EditableListView(listView2);
            peptideList.AddColumn("Protein", true, (string a) => true);
            peptideList.AddColumn("Peptide", true, (string a) =>
              {
                  Regex r = new Regex("[BJOXZ]|[^A-Z]");
                  if (r.IsMatch(a))
                      return false;
                  else
                      return true;
              });
        }

        private void load_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "txt(*.txt)|*.txt";
            string path = "";
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                aminoList.Clear();
                peptideList.Clear();
                path = openFileDialog1.FileName;
                StreamReader sr = new StreamReader(path);
                string line;
                string error = "";
                int cnt = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    cnt++;
                    string[] words = line.Trim().Split('\t');
                    try
                    {
                        if (words[0] == "A")
                        {
                            if (double.Parse(words[2]) != 0)
                                aminoList.AddRow(new string[] { words[1].Trim(), words[2].Trim() });
                            //AminoModification.Update(words[1], double.Parse(words[2]));
                        }
                        else if (words[0] == "P")
                        {
                            peptideList.AddRow(new string[] { words[1].Trim(), words[2].Trim() });
                            //AddProteins(words[1], words[2]);
                        }
                    }
                    catch (Exception er)
                    {
                        error += string.Format("line {0}: {1}\n", cnt, er.Message);
                    }


                }
                if (error != "")
                    MessageBox.Show(error);

            }
        }

        private void ok_Click(object sender, EventArgs e)
        {
            if (Check())
            {
                AminoModification.Clear();
                ClearProtein();
                for (int i = 0; i < listView1.Items.Count - 1; i++)
                {
                    AminoModification.Update(listView1.Items[i].SubItems[0].Text, double.Parse(listView1.Items[i].SubItems[1].Text));
                }
                for (int i = 0; i < listView2.Items.Count - 1; i++)
                {
                    AddProteins(listView2.Items[i].SubItems[0].Text, listView2.Items[i].SubItems[1].Text);
                }
                this.Close();
            }
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void save_Click(object sender, EventArgs e)
        {
            if (Check())
            {
                saveFileDialog1.Filter = "txt(*.txt)|*.txt";
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    StreamWriter sw = new StreamWriter(saveFileDialog1.FileName);
                    for (int i = 0; i < listView1.Items.Count - 1; i++)
                    {
                        sw.WriteLine(string.Format("A\t{0}\t{1}", listView1.Items[i].SubItems[0].Text, listView1.Items[i].SubItems[1].Text));
                    }
                    for (int i = 0; i < listView2.Items.Count - 1; i++)
                    {
                        sw.WriteLine(string.Format("P\t{0}\t{1}", listView2.Items[i].SubItems[0].Text, listView2.Items[i].SubItems[1].Text));
                    }
                    sw.Close();
                }
                    
            }
            
        }

        private bool Check()
        {
            string error = "";
            for (int i = 0; i < listView1.Items.Count - 1; i++)
            {
                for (int j = 0; j < listView1.Columns.Count; j++)
                {
                    if (!aminoList.Judge(i, j))
                        error += string.Format("row {0} column {1}: invalid parameter\n", i + 1, j + 1);
                }
            }
            for (int i = 0; i < listView2.Items.Count - 1; i++)
            {
                for (int j = 0; j < listView2.Columns.Count; j++)
                {
                    if (!peptideList.Judge(i, j))
                        error += string.Format("row {0} column {1}: invalid parameter\n", i + 1, j + 1);
                }
            }

            if (error != "")
            {
                MessageBox.Show(error);
                return false;
            }
            else
                return true;
        }

        private void clear_Click(object sender, EventArgs e)
        {
            aminoList.Clear();
            peptideList.Clear();
        }

        private void Match_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }


        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                ListView listView = (ListView)sender;
                if (listView.SelectedItems.Count > 0)
                {
                    listView.Items[listView.SelectedItems[0].Index].Remove();
                }
            }
        }

        private void listView2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                ListView listView = (ListView)sender;
                if (listView.SelectedItems.Count > 0)
                {
                    listView.Items[listView.SelectedItems[0].Index].Remove();
                }
            }
        }
    }
}
