using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Dragger
{
    public partial class ObjectsBar : Form
    {
        Bitmap disabledMan;

        public ObjectsBar()
        {
            InitializeComponent();
            disabledMan = new Bitmap(Properties.Resources.man);
            ControlPaint.DrawImageDisabled(Graphics.FromImage(disabledMan), disabledMan, 0, 0, picMan.BackColor);
        }

        private void pic_MouseMove(object sender, MouseEventArgs e)
        {
            PictureBox pBox = (PictureBox) sender;
            if (e.Button == MouseButtons.Left)
                pBox.DoDragDrop(pBox.Tag.ToString(), DragDropEffects.Copy);
        }

        private void picMan_EnabledChanged(object sender, EventArgs e)
        {
            picMan.Image = picMan.Enabled ? Properties.Resources.man : disabledMan;

        }
    }
}