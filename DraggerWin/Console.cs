using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Dragger
{
    public partial class Console : Form
    {
        public Console()
        {
            InitializeComponent();
        }

        private void Console_Load(object sender, EventArgs e)
        {
            try
            {
                string[] args = Environment.GetCommandLineArgs();
                string file = String.Empty;
                if (args.Length > 1)
                    file = String.Join(" ", args, 1, args.Length - 1);
                else if (File.Exists("maps.txt"))
                    file = "maps.txt";
                GameEngine.Initialize(this, file);
                GameEngine.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Sorry an error has occured.\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }        
    }
}
