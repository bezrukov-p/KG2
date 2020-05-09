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
    public partial class Settings : Form
    {
        public TrackBar LayerTrackBar => layerTrakBar;
        public event EventHandler ModeChaged;
        public event EventHandler BoundsChanged;

        public Settings()
        {
            InitializeComponent();

            this.radioButton1.CheckedChanged += (s, e) => ModeChaged?.Invoke(s, e);
            this.radioButton2.CheckedChanged += (s, e) => ModeChaged?.Invoke(s, e);
            this.radioButton3.CheckedChanged += (s, e) => ModeChaged?.Invoke(s, e);

            this.trackBar1.ValueChanged += (s, e) => BoundsChanged?.Invoke(s, e);
            this.trackBar2.ValueChanged += (s, e) => BoundsChanged?.Invoke(s, e);
        }

        public void LayerBounds(int min, int max)
        {
            layerTrakBar.Minimum = min;
            layerTrakBar.Maximum = max;
        }

        public int GetMode()
        {
            if (radioButton1.Checked)
                return 0;

            if (radioButton2.Checked)
                return 1;

            if (radioButton3.Checked)
                return 2;

            return 0;
        }

        public int[] GetBounds()
        {
            int[] b = new int[2];

            b[0] = trackBar1.Value;
            b[1] = b[0] + trackBar2.Value;

            return b;
        }

        
    }
}
