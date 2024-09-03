using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using static SESTAR_GUI.Function;
using SESTARhelper;

namespace SESTAR_GUI
{
    public partial class Form1 : Form
    {
        private Inclusion inclusionForm=new Inclusion();
        private Match matchForm=new Match();
        private Parameter parameterForm = new Parameter();
        public Form1()
        {
            InitializeComponent();
            SESTAR.ParamSetting.LoadParams("SESTARparams.xml");
            SESTAR.ParamSetting.Initialize();

        }

        private string savePath;

        private float ssCutoff = 10.0f;
        private float sdCutoff = 6.0f;
        private int lenMin = 6;
        private int lenMax = 14;

        private void Open_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "ms1(*.ms1)|*.ms1|ms2(*.ms2)|*.ms2";

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                file.Rows.Clear();
                string[] openFilePaths = openFileDialog1.FileNames;
                savePath = Path.GetDirectoryName(openFilePaths[0]);
                int i;
                foreach (string path in openFilePaths)
                {
                    i = file.Rows.Add();
                    file.Rows[i].Cells[2].Value = Path.GetFileName(path);
                    file.Rows[i].Cells[3].Value = Path.GetFileName(path) + "_result.txt";
                    file.Rows[i].Cells[1].Value = searchMethod.Items[0].ToString();
                    file.Rows[i].Cells[0].Value = "Unstarted";
                }        
            }
        }

        private Thread calcThread = null;

        private void Calc_Click(object sender, EventArgs e)
        {
            if (!Check())
                return;
            if (file.Rows.Count == 0)
            {
                MessageBox.Show("Error:\nNo input files!!!");
                return;
            }
                
            ssCutoff = float.Parse(ss.Text);
            sdCutoff = float.Parse(sd.Text);
            lenMin = int.Parse(minPeak.Text);
            lenMax = int.Parse(maxPeak.Text);

            calcThread = new Thread(Calc) { IsBackground = true };
            calcThread.Start();
        }

        private string[] acceptExtension=new string[] { ".ms1",".ms2"};

        private async void Calc()
        {
            ObjectEnable(false);
            int fileCount = 0;

            foreach (DataGridViewRow row in file.Rows)
            {
                fileCount += 1;
                ChangeStatus(fileCount - 1, "Unstarted");
            }

            fileCount = 0;
            foreach (DataGridViewRow row in file.Rows)
            {
                if ((string)row.Cells[0].Value == "Canceled")
                    break;

                fileCount += 1;
                if (!acceptExtension.Contains(Path.GetExtension((string)row.Cells[2].Value)))
                    continue;

                SetProgress(1);
                ChangeStatus(fileCount - 1, "Reading File");
                MSDataParser parser = new MSDataParser(Path.Combine(savePath, (string)row.Cells[2].Value));
                StreamWriter sw = new StreamWriter(Path.Combine(savePath, (string)row.Cells[3].Value));
                sw.WriteLine(string.Join("\t", Function.Headline));



                ChangeStatus(fileCount - 1, "Calculating");

                Stopwatch watch = new Stopwatch();
                long totaltime = 0;
                long maxtime = 0;
                Task<Scan> r = parser.Read();

                while ((string)row.Cells[0].Value != "Canceled")
                {
                    SetProgress(1 + 98 * parser.Progress / 100);
                    int cnt = 0;
                    Scan scan = await r;
                    if (scan.Scanno == 0)
                        break;
                    r = parser.Read();
                    watch.Start();
                    IntPtr intPtr;
                    switch ((string)row.Cells[1].Value)
                    {
                        case "Pattern Se":
                            intPtr = SESTAR.Search.SeStar(ref cnt, scan.Mz, scan.Intensity, scan.Mz.Length, new SearchParameters(sdCutoff, ssCutoff, lenMin, lenMax, 0, 0));
                            break;
                        case "Pattern 2":
                            intPtr = SESTAR.Search.Any(ref cnt, scan.Mz, scan.Intensity, scan.Mz.Length, 1, new SearchParameters(sdCutoff, ssCutoff, lenMin, lenMax, 0, 0));
                            break;
                        case "Pattern 3":
                            intPtr = SESTAR.Search.Any(ref cnt, scan.Mz, scan.Intensity, scan.Mz.Length, 2, new SearchParameters(sdCutoff, ssCutoff, lenMin, lenMax, 0, 0));
                            break;
                        case "4SU (MS2)":
                            intPtr = SESTAR.Search.MS2RBPDiag(ref cnt, scan.Mz, scan.Intensity, scan.Mz.Length, new SearchParameters(sdCutoff, ssCutoff, 4, 15, 0, 0));
                            break;
                        case "GGtag (MS2)":
                            intPtr = SESTAR.Search.MS2Diag(ref cnt, scan.Mz, scan.Intensity, scan.Mz.Length, new SearchParameters(sdCutoff, ssCutoff, lenMin, lenMax, 0, 0));
                            break;
                        default:
                            intPtr = SESTAR.Search.SeStar(ref cnt, scan.Mz, scan.Intensity, scan.Mz.Length, new SearchParameters(sdCutoff, ssCutoff, lenMin, lenMax, 0, 0));
                            break;
                    }
                    watch.Stop();
                    if (watch.ElapsedMilliseconds > maxtime)
                        maxtime = watch.ElapsedMilliseconds;
                    totaltime += watch.ElapsedMilliseconds;
                    watch.Reset();

                    if (cnt != 0)
                    {
                        SESTAR.Interpretation.ReadByte(intPtr, cnt, out SearchResult[] results);
                        string s = Function.WriteReport(scan.RetentionTime, scan.Mz, scan.Intensity, scan.Scanno, results);
                        sw.Write(s);
                    }



                }
                sw.Close();
                if ((string)row.Cells[0].Value == "Canceled")
                    break;
                if (inclusionForm.InclusionList)
                {
                    ChangeStatus(fileCount - 1, "Generating Inclusion List");
                    GenIncluList(Path.Combine(savePath, (string)row.Cells[3].Value), inclusionForm.RetentionTime);
                }

                ChangeStatus(fileCount - 1, "Finished");
            }

            GC.Collect();
            SetProgress(0);
            ObjectEnable(true);
            calcThread = null;

        }

        private void SetProgress(int i)
        {
            this.Invoke(new Action(() => progressBar1.Value = i));
        }

        private void ObjectEnable(bool f)
        {
            this.Invoke(new Action(()=>{
                open.Enabled = f;
                calc.Enabled = f;
                ss.Enabled = f;
                sd.Enabled = f;
                minPeak.Enabled = f;
                maxPeak.Enabled = f;
                toolsToolStripMenuItem.Enabled = f;
                file.Columns[2].ReadOnly = !f;
                file.Columns[3].ReadOnly = !f;
            }));
        }

        private void ChangeStatus(int index, string status)
        {
            this.Invoke(new Action(() => {
                file.Rows[index].Cells[0].Value = status;
            }));
        }

        private bool Check()
        {
            StringBuilder error = new StringBuilder();
            double tmpd;
            int min = -1, max = -1;
            if (double.TryParse(ss.Text,out tmpd))
            {
                if (tmpd <= 0)
                    error.AppendLine("\"SS\" should be more than 0");
            }
            else
                error.AppendLine("\"SS\" is not numeral");
            if (double.TryParse(sd.Text, out tmpd))
            {
                if (tmpd < 0)
                    error.AppendLine("\"1/SD\" should be more than/equal to 0");
            }
            else
                error.AppendLine("\"1/SD\" is not numeral");
            if (int.TryParse(minPeak.Text, out min))
            {
                if (min <= 0)
                {
                    min = -1;
                    error.AppendLine("\"minimal peaks\" should be more than 0");
                }                    
            }
            else
            {
                min = -1;
                error.AppendLine("\"minimal peaks\" is not integer");
            }        
            if (int.TryParse(maxPeak.Text, out max))
            {               
                if (max > 15)
                {
                    max = -1;
                    error.AppendLine("\"maximal peaks\" should be less than 15");
                }                
            }
            else
            {
                max = -1;
                error.AppendLine("\"maximal peaks\" is not integer");
            }      
            if (min != -1 && max != -1 && min >= max)
            {
                error.AppendLine("\"maximal peaks\" should be more than \"minimal peaks\"");
            }

            if (error.Length == 0)
                return true;
            else
            {
                MessageBox.Show("Error:\n" + error.ToString());
                return false;
            }
        }


        private void inclusionListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inclusionForm.Show();
        }

        private void peptideMatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            matchForm.Show();
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            try
            {
                GC.Collect();
                SetProgress(0);
                ObjectEnable(true);
                calcThread = null;
                int fileCount = 0;

                foreach (DataGridViewRow row in file.Rows)
                {
                    if ((string)file.Rows[fileCount].Cells[0].Value != "Finished")
                        ChangeStatus(fileCount, "Canceled");
                    fileCount += 1;
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }

        private void acceptedParametersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            parameterForm.Show();
        }

    }
}
