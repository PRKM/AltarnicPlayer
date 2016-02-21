using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AltanicPlayer
{
    public partial class Addlist : Form
    {
        public string toadd;
        private string[] lists;
        public Addlist()
        {
            InitializeComponent();
            lists = System.IO.Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\AlPlayer\\Lists");
        }

        private void Submit_Click(object sender, EventArgs e)
        {
            toadd = listname.Text;
            string temp = toadd.Insert(toadd.Length, ".alp");
            foreach (string comp in lists)
            {
                if (temp.Equals(comp))
                {
                    MessageBox.Show("동일한 이름의 파일이 있습니다");
                    break;
                }
                else if (temp.Equals("") || (temp == null))
                {
                    MessageBox.Show("목록의 이름을 작성하여 주세요");
                    break;
                }
                else if (comp.Equals(lists[lists.Length - 1]))
                {
                    this.Close();
                }
            }
        }

        private void listname_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                toadd = listname.Text;
                string temp = toadd.Insert(toadd.Length, ".alp");
                foreach (string comp in lists)
                {
                    if (temp.Equals(comp))
                    {
                        MessageBox.Show("동일한 이름의 파일이 있습니다");
                        break;
                    }
                    else if(temp.Equals("") || (temp == null))
                    {
                        MessageBox.Show("목록의 이름을 작성하여 주세요");
                    }
                    else if (comp.Equals(lists[lists.Length - 1]))
                    {
                        this.Close();
                    }
                }
            }
        }

        private void Addlist_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (toadd == null)
                toadd = "";
        }
    }
}
