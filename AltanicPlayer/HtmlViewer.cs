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
    public partial class HtmlViewer : Form
    {
        public HtmlViewer(string str)
        {
            InitializeComponent();
            textBox1.Text = str;
        }
    }
}
