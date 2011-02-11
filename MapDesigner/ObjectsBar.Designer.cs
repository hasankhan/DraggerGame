namespace Dragger
{
    partial class ObjectsBar
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.picWall = new System.Windows.Forms.PictureBox();
            this.picBall = new System.Windows.Forms.PictureBox();
            this.picMan = new System.Windows.Forms.PictureBox();
            this.picBasket = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picWall)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBall)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBasket)).BeginInit();
            this.SuspendLayout();
            // 
            // picWall
            // 
            this.picWall.Image = global::Dragger.Properties.Resources.wall;
            this.picWall.Location = new System.Drawing.Point(12, 175);
            this.picWall.Name = "picWall";
            this.picWall.Size = new System.Drawing.Size(50, 50);
            this.picWall.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picWall.TabIndex = 3;
            this.picWall.TabStop = false;
            this.picWall.Tag = "wall";
            this.picWall.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pic_MouseMove);
            // 
            // picBall
            // 
            this.picBall.Image = global::Dragger.Properties.Resources.ball;
            this.picBall.Location = new System.Drawing.Point(12, 7);
            this.picBall.Name = "picBall";
            this.picBall.Size = new System.Drawing.Size(50, 50);
            this.picBall.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picBall.TabIndex = 2;
            this.picBall.TabStop = false;
            this.picBall.Tag = "ball";
            this.picBall.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pic_MouseMove);
            // 
            // picMan
            // 
            this.picMan.Image = global::Dragger.Properties.Resources.man;
            this.picMan.Location = new System.Drawing.Point(12, 119);
            this.picMan.Name = "picMan";
            this.picMan.Size = new System.Drawing.Size(50, 50);
            this.picMan.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picMan.TabIndex = 1;
            this.picMan.TabStop = false;
            this.picMan.Tag = "man";
            this.picMan.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pic_MouseMove);
            this.picMan.EnabledChanged += new System.EventHandler(this.picMan_EnabledChanged);
            // 
            // picBasket
            // 
            this.picBasket.Image = global::Dragger.Properties.Resources.basket;
            this.picBasket.Location = new System.Drawing.Point(12, 63);
            this.picBasket.Name = "picBasket";
            this.picBasket.Size = new System.Drawing.Size(50, 50);
            this.picBasket.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picBasket.TabIndex = 0;
            this.picBasket.TabStop = false;
            this.picBasket.Tag = "basket";
            this.picBasket.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pic_MouseMove);
            // 
            // ObjectsBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(74, 258);
            this.ControlBox = false;
            this.Controls.Add(this.picWall);
            this.Controls.Add(this.picBall);
            this.Controls.Add(this.picMan);
            this.Controls.Add(this.picBasket);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ObjectsBar";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Toolbox";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.picWall)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBall)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBasket)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picBasket;
        private System.Windows.Forms.PictureBox picBall;
        public System.Windows.Forms.PictureBox picWall;
        public System.Windows.Forms.PictureBox picMan;
    }
}