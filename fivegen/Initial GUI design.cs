using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Thread renderThread;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ToolTip toolTip1 = new ToolTip();

            // Set up the delays for the ToolTip.
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 1000;
            toolTip1.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip1.ShowAlways = true;

            // Set up the ToolTip text for the Button and Checkbox.
            toolTip1.SetToolTip(this.checkBox1, "Tetrahedralizes a picecwise linear complex.");
            toolTip1.SetToolTip(this.checkBox2, "Quality mesh generation. A minimum radius-edge ratio may be specifyed.");
            toolTip1.SetToolTip(this.checkBox3, "Applies a maximum tetrahedron volume constraint.");
            toolTip1.SetToolTip(this.checkBox4, "Assigns attributes to identify tetrahedra in certain regions.");
            toolTip1.SetToolTip(this.checkBox5, "Reconstructs/Refines a previously generated mesh.");
            toolTip1.SetToolTip(this.checkBox6, "Suppresses boundary facets/segments splitting.");
            toolTip1.SetToolTip(this.checkBox7, "Inserts a list of additional points into mesh.");
            toolTip1.SetToolTip(this.checkBox8, "Does not merge coplanar facets.");
            toolTip1.SetToolTip(this.checkBox9, "Set a tolerance for coplanar test.");
            toolTip1.SetToolTip(this.checkBox10, "Detect intersections of PLC facets.");
            toolTip1.SetToolTip(this.checkBox11, "Numbers all output items starting from zero.");
            toolTip1.SetToolTip(this.checkBox12, "Jettison unused vertices from output .node file.");
            toolTip1.SetToolTip(this.checkBox13, "Generates second-order subparametric elements.");

            renderThread = new Thread(new ThreadStart(() => {
                while (true)
                {
                    renderPanel1.Draw();
                    renderPanel1.Present();
                }
            }));
            renderThread.Start();
            while (!renderThread.IsAlive) ;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            saveDialog();
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void panel3_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void tabPage1_Click_1(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            openDialog();
        }

        private void openDialog()
        {
            // Create an instance of the open file dialog box.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog1.Filter = "STereoLithography Files (.stl)|*.stl|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = false;
            openFileDialog1.ShowDialog();

        }

        private void saveDialog()
        {

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.ShowDialog();

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("TetGen is a powerful command-line tool with many options for generating tetrahedral lattices. These lattices are used for many applications from mechanical engineering, biomedical research, fluid simulations, to computer games and interactive VR simulations. Their exact structure is very important for different applications, and TetGen is one of very few open-source tools available that have these features. http://wiasberlin.de/software/tetgen/ \n \nThis project will develop a graphical user interface that simplifies the use of this tool(using presets and thumbnails etc) to open up the tool to a far bigger audience.It will include 3D computer graphics to visualize the generated 3D meshes (both triangle-shell meshes, and tetrahedral lattices).",
            "About");
        }

        private void Form1_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            renderThread.Abort();
        }

        private void exitToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Are you sure you want to exit?",
                      "Exit?", MessageBoxButtons.YesNo);
            switch (dr)
            {
                case DialogResult.Yes: Application.Exit(); break;
                case DialogResult.No: break;
            }
            
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            saveDialog();
        }

        private void contentsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                textBox1.Visible = true;
            }else
            {
                textBox1.Visible = false;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                textBox2.Visible = true;
            }
            else
            {
                textBox2.Visible = false;
            }
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox9.Checked)
            {
                textBox3.Visible = true;
            }
            else
            {
                textBox3.Visible = false;
            }
        }
    }
}
