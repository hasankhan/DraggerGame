using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Dragger
{
    public partial class MapCanvas : Form
    {
        const int cellWidth = 50;
        const int cellHeight = 50;
        ObjectsBar objectsBar;
        TableLayoutPanelCellPosition spawn;

        public MapCanvas()
        {
            InitializeComponent();
            objectsBar = new ObjectsBar();
            ResizeGrid(2, 2);
        }

        private void btnResize_Click(object sender, EventArgs e)
        {
            ResizeGrid(Int32.Parse(txtRows.Text), Int32.Parse(txtColumns.Text));
        }

        private void ResizeGrid(int rows, int cols)
        {
            this.Visible = false;
            this.SuspendLayout();

            ClearGrid();            
            btnGenCode.Enabled = false;            
            CreateCells(cols, rows);

            for (int row = 0; row < tableLayoutPanel1.RowCount; row++)
                for (int col = 0; col < tableLayoutPanel1.ColumnCount; col++)
                    tableLayoutPanel1.Controls.Add(CreatePicBox(row.ToString() + "_" + col.ToString(), cellWidth, cellHeight), col, row);

            this.ResumeLayout();
            this.Visible = true;
        }

        private void CreateCells(int cols, int rows)
        {            
            tableLayoutPanel1.ColumnCount = cols;
            tableLayoutPanel1.RowCount = rows;
            for (int x = 0; x < tableLayoutPanel1.ColumnCount; x++)
                tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, cellWidth));
            for (int y = 0; y < tableLayoutPanel1.RowCount; y++)
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, cellHeight));
        }

        private void ClearGrid()
        {
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.ColumnStyles.Clear();
        }

        private Control CreatePicBox(string name, int width, int height)
        {
            PictureBox pBox = new PictureBox();
            pBox.AllowDrop = true;
            pBox.Name = name;
            pBox.DragEnter += new DragEventHandler(pBox_DragEnter);
            pBox.DragDrop += new DragEventHandler(pBox_DragDrop);
            pBox.MouseDown += new MouseEventHandler(pBox_MouseDown);
            pBox.MouseMove += new MouseEventHandler(pBox_MouseMove);
            pBox.Width = width;
            pBox.Height = height;
            return pBox;
        }

        void pBox_MouseMove(object sender, MouseEventArgs e)
        {
            PictureBox pBox = (PictureBox) sender;
            if (e.Button == MouseButtons.Left && pBox.Tag != null)
            {
                pBox.AllowDrop = false;
                if (pBox.DoDragDrop(pBox.Tag.ToString(), DragDropEffects.Move) == DragDropEffects.Move)
                {
                    pBox.Tag = null;
                    pBox.Image = null;
                }
                pBox.AllowDrop = true;
            }

        }

        void pBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                PictureBox pBox = (PictureBox)sender;
                ClearCell(pBox);
            }
        }

        private void ClearCell(PictureBox pBox)
        {
            pBox.Image = null;
            if (pBox.Tag.ToString() == "man")
                btnGenCode.Enabled = false;
            pBox.Tag = null;
            pBox.AllowDrop = true;
        }

        void pBox_DragDrop(object sender, DragEventArgs e)
        {
            PictureBox pBox = (PictureBox) sender;
            if (e.Effect == DragDropEffects.Copy || e.Effect == DragDropEffects.Move)
            {
                string obj = (string)e.Data.GetData(typeof(String));
                FillCell(pBox, obj);
            }
        }

        private void FillCell(int col, int row, string obj) { FillCell((PictureBox)tableLayoutPanel1.GetControlFromPosition(col, row), obj);}
        private void FillCell(PictureBox pBox, string obj)
        {
            pBox.Image = (Image)Properties.Resources.ResourceManager.GetObject(obj);
            pBox.Tag = obj;
            pBox.AllowDrop = false;
            if (obj == "man")
            {
                btnGenCode.Enabled = true;
                spawn = tableLayoutPanel1.GetPositionFromControl(pBox);
            }
        }

        void pBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(String)))
                if (e.AllowedEffect == DragDropEffects.Copy)
                    e.Effect = DragDropEffects.Copy;
                else if (e.AllowedEffect == DragDropEffects.Move)
                    e.Effect = DragDropEffects.Move;
        }

        private void MapCanvas_Load(object sender, EventArgs e)
        {            
            objectsBar.Show(this);
            SnapObjectsBar();          
        }

        private void MapCanvas_Move(object sender, EventArgs e)
        {
            SnapObjectsBar();
        }

        private void SnapObjectsBar()
        {
            if (this.WindowState == FormWindowState.Maximized)
                objectsBar.Left = this.Left + this.Width - objectsBar.Width;
            else
                objectsBar.Left = this.Left + this.Width;            
            objectsBar.Top = this.Top;
        }

        private void btnGenCode_Click(object sender, EventArgs e)
        {
            GenerateCode();
        }

        private void GenerateCode()
        {
            MapCode mapCode = new MapCode();
            mapCode.textBox1.AppendText("grid " + tableLayoutPanel1.RowCount + "x" + tableLayoutPanel1.ColumnCount + "\r\n");
            mapCode.textBox1.AppendText("spawn " + spawn.Row + "," + spawn.Column + "\r\n");
            for (int row = 0; row < tableLayoutPanel1.RowCount; row++)
                PrintRow(mapCode, row);
            mapCode.ShowDialog(this);
        }

        private void PrintRow(MapCode map, int row)
        {
            string temp = GetObjCode(tableLayoutPanel1.GetControlFromPosition(0, row).Tag as String).ToString();
            map.textBox1.AppendText(temp);
            for (int col = 1; col < tableLayoutPanel1.ColumnCount; col++)
            {
                string obj = tableLayoutPanel1.GetControlFromPosition(col, row).Tag as String;
                map.textBox1.AppendText(" " + GetObjCode(obj).ToString());
            }
            map.textBox1.AppendText("\r\n");
        }

        private static int GetObjCode(string obj)
        {
            switch (obj)
            {
                case "wall": return 4;
                case "basket": return 1;
                case "ball": return 2;
                default: return 0;
            }
        }
        
        private static string GetObjCode(int obj)
        {
            switch (obj)
            {
                case 4: return "wall";
                case 1: return "basket";
                case 2: return "ball";
                default: return null;
            }
        }

        private void btnGenCode_EnabledChanged(object sender, EventArgs e)
        {
            objectsBar.picMan.Enabled = !btnGenCode.Enabled;
        }

        private void MapCanvas_Resize(object sender, EventArgs e)
        {
            SnapObjectsBar();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                LoadMap(openFileDialog1.FileName);
            }
        }

        private void LoadMap(string file)
        {
            this.Visible = false;
            this.SuspendLayout();

            Map map = Map.Load(System.IO.File.OpenText(file));
            txtRows.Text = map.Rows.ToString();
            txtColumns.Text = map.Columns.ToString();
            ResizeGrid(map.Rows, map.Columns);
            spawn = new TableLayoutPanelCellPosition(map.Player.Location.X, map.Player.Location.Y);
            FillCell(spawn.Column, spawn.Row, "man");
            for (int row = 0; row < map.Rows; row++)
                for (int col = 0; col < map.Columns; col++)
                {
                    string code = GetObjCode(map[col, row]);
                    if (code != null)
                        FillCell(col, row, code);
                }

            this.ResumeLayout();
            this.Visible = true;
        }
    }
}