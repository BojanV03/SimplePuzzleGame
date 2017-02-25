using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimplePuzzleGame
{
    public partial class fmSize : Form
    {
        public fmSize()
        {
            InitializeComponent();
            tbHeight.Text = gridHeight.ToString();
            tbWidth.Text = gridWidth.ToString();
        }

        private int gridWidth=5, gridHeight=5, fractureIterations = 5;

        public Size getSize()
        {
            Size s = new Size(gridWidth, gridHeight);
            return s;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void tbIterations_ValueChanged(object sender, EventArgs e)
        {
            fractureIterations = tbIterations.Value;
            label3.Text = "Fracture iterations: " + fractureIterations;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                int tmpWidth = Convert.ToInt32(tbWidth.Text);
                int tmpHeight = Convert.ToInt32(tbHeight.Text);
                if (tmpWidth < 5 || tmpHeight < 5 || tmpWidth > 20 || tmpHeight > 20)
                    throw new Exception();
                gridWidth = tmpWidth > tmpHeight? tmpWidth : tmpHeight;
                gridHeight = tmpWidth < tmpHeight? tmpWidth : tmpHeight;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch(Exception)
            {
                MessageBox.Show("Width and Height must be integers >= 5 and <= 20");
            }
        }

        public int getWidth()
        {
            return gridWidth;
        }

        public int getHeight()
        {
            return gridHeight;
        }
        public int getFractureIterations()
        {
            return fractureIterations;
        }
    }
}
