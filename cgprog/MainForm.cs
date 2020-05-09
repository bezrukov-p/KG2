using lib;
using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cgprog
{
    public partial class MainForm : Form
    {
        GLControl glControl;
        Drawer drawer;

        Settings SetForm; 

        int layer = 0;
        int frames = 0;

        public MainForm()
        {
            InitializeComponent();

            glControl = new GLControl();
            glControl.Paint += GlControl_Paint;
            glControl.Size = new Size(this.Width, this.Height);
            glControl.Location = new Point(0, 0);

            this.Load += MainForm_Load;
            this.Resize += MainForm_Resize;

            this.Controls.Add(glControl);
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            glControl.Size = new Size(this.Width, this.Height);
            if (drawer != null)
                drawer.Init(glControl.Width, glControl.Height);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle;
        }

        DateTime timeStamp = DateTime.Now.AddSeconds(1);
        private void Application_Idle(object sender, EventArgs e)
        {
            while(glControl.IsIdle)
            {
                if(DateTime.Now >= timeStamp)
                {
                    label1.Text = frames + "";
                    frames = 0;

                    timeStamp = DateTime.Now.AddSeconds(1);
                }

                glControl.Invalidate();
            }
        }

        private void GlControl_Paint(object sender, PaintEventArgs e)
        {
            if(drawer != null)
            {
                drawer.Draw(layer);
                glControl.SwapBuffers();

                ++frames;
            }
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();


            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Data d = new Data();
                d.Load(openFileDialog.FileName);

                drawer = new Drawer(d);
                drawer.Init(glControl.Width, glControl.Height);
            }
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (drawer != null)
            {
                if (SetForm != null)
                {
                    SetForm.Close();
                }               

                SetForm = new Settings();
                SetForm.LayerBounds(0, drawer.Data.SizeZ - 1);
                SetForm.Show();

                SetForm.LayerTrackBar.ValueChanged += (s, o) => { layer = SetForm.LayerTrackBar.Value; };
                SetForm.ModeChaged += (s, o) => { drawer.Mode = SetForm.GetMode(); };
                SetForm.BoundsChanged += (s, o) =>
                {
                    var a = SetForm.GetBounds();
                    drawer.VMin = a[0];
                    drawer.VMax = a[1];
                };


            }
            
        }
    }
}
