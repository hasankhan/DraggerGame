using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;
using Dragger;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StringReader reader = new StringReader(textBox1.Text);
            Map map = Map.Load(reader);
            Map inverse = map.Invert();
            PrintMap(inverse);
        }        

        private void PrintMap(Map map)
        {
            StringWriter writer = new StringWriter();
            map.Save(writer);
            textBox1.Text = writer.ToString();
        }
    }
}
