using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SESTAR_GUI
{
    class EditableListView
    {
        private ListView listView;
        private TextBox inputBox;
        private int column;
        private int row;
        private List<bool> editables = new List<bool>();
        private List<TextJudge> judges = new List<TextJudge>();

        public EditableListView(ListView listView)
        {
            this.listView = listView;
            this.listView.View = View.Details;
            this.listView.GridLines = true;
            this.listView.Items.Add(new ListViewItem());
            this.listView.FullRowSelect = true;
            this.listView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            this.listView.MouseDoubleClick += listView_MouseDoubleClick;
            this.listView.MouseClick += listView1_MouseClick;
            this.listView.ColumnWidthChanged += listView_ColumnWidthChanged;
            this.listView.Parent.MouseClick += form_MouseClick;
        }

        public delegate bool TextJudge(string text);

        public void AddColumn(string header, bool editable, TextJudge judge)
        {
            listView.Columns.Add(header);
            editables.Add(editable);
            judges.Add(judge);
        }

        public void AddRow(string[] labels)
        {
            listView.Items.Insert(listView.Items.Count - 1, new ListViewItem(labels));
        }

        public void Clear()
        {
            listView.Items.Clear();
            this.listView.Items.Add(new ListViewItem());
        }

        public bool Judge(int row, int column)
        {
            return judges[column](listView.Items[row].SubItems[column].Text);
        }

        [DllImport("user32")]
        public static extern int GetScrollPos(int hwnd, int nBar);

        private void listView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = listView.GetItemAt(e.X, e.Y);

            Rectangle r;
            int scroll = GetScrollPos(listView.Handle.ToInt32(), 0);
            Rectangle bound = item.GetBounds(ItemBoundsPortion.Entire);
            int cellX = bound.X;

            for (int i = 0; i < listView.Columns.Count; i++)
            {
                if (scroll + e.X >= cellX && scroll + e.X < cellX + listView.Columns[i].Width)
                {
                    if ((item.Index==listView.Items.Count-1|| editables[i]) && inputBox == null)
                    {
                        try
                        {
                            
                            r = new Rectangle(listView.Location.X+ cellX+2, listView.Location.Y+ bound.Top+1, listView.Columns[i].Width, bound.Height);
                            column = i;
                            row = item.Index;
                            inputBox = new TextBox();
                            inputBox.AutoSize = false;
                            inputBox.Text = item.SubItems[i].Text;
                            inputBox.Parent = listView.Parent;
                            inputBox.BringToFront();
                            inputBox.Bounds = r;
                            inputBox.Focus();
                            inputBox.KeyPress += inputBox_KeyPress;
                            listView.Enabled = false;
                        }
                        catch
                        {
                            if (inputBox != null)
                            {
                                inputBox.Dispose();
                                inputBox = null;
                            }
                        }
                    }
                    
                    break;
                }
                cellX += listView.Columns[i].Width;
            }

            //Console.WriteLine(r);
            
        }

        private void listView_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (inputBox != null)
            {
                if (e.ColumnIndex == column)
                    inputBox.Width = listView.Columns[e.ColumnIndex].Width;
                int x = listView.Items[0].GetBounds(ItemBoundsPortion.Entire).Left;
                for (int i = 0; i < column; i++)
                {
                    x += listView.Columns[i].Width;
                }
                int y = listView.Items[0].GetBounds(ItemBoundsPortion.Entire).Top;
                for (int i = 0; i < row; i++)
                {
                    y += listView.Items[i].GetBounds(ItemBoundsPortion.Entire).Height;
                }
                inputBox.Location = new Point(x, y);
            }
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            ChangeText();
        }

        private void inputBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == 13)
                ChangeText();
        }

        private void form_MouseClick(object sender, MouseEventArgs e)
        {
            ChangeText();
        }

        private void ChangeText()
        {
            if (inputBox == null)
                return;
            
            if (judges[column](inputBox.Text))
            {
                if (row == listView.Items.Count - 1)
                {
                    string[] s = new string[listView.Columns.Count];
                    s[0] = inputBox.Text;

                    AddRow(s);
                }
                listView.Items[row].SubItems[column].Text = inputBox.Text;
                inputBox.Dispose();
                inputBox = null;
            }
            else
            {
                MessageBox.Show("Invalid parameters");
                inputBox.Dispose();
                inputBox = null;
            }
            listView.Enabled = true;
        }

        private void Relocate()
        {
            
        }
    }
}
