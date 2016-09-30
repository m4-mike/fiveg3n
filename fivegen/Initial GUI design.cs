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
using fivegen;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private const int MinEdgeRadiusIndex = 1;
        private const int MaxVolumeIndex = 2;
        private const int CoplanarTestToleranceIndex = 8;
        private const int NumArguments = 13;

        private Thread RenderThread;
        private CheckBox[] ArgumentCheckboxes = new CheckBox[NumArguments];

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tableLayoutPanel1.ColumnCount = 1;
            renderPanel2.Visible = false;
            ToolTip toolTip1 = new ToolTip();

            // Set up the delays for the ToolTip.
            toolTip1.AutoPopDelay = 10000;
            toolTip1.InitialDelay = 1000;
            toolTip1.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip1.ShowAlways = true;

            // Set up the ToolTip text for the Button and Checkbox.
            // Add checkboxes to the argument array for easier traversal later
            toolTip1.SetToolTip(this.checkBox1, "Tetrahedralizes a picecwise linear complex.");
            ArgumentCheckboxes[0] = this.checkBox1;
            toolTip1.SetToolTip(this.checkBox2, "Quality mesh generation. A minimum radius-edge ratio may be specifyed.");
            ArgumentCheckboxes[1] = this.checkBox2;
            toolTip1.SetToolTip(this.checkBox3, "Applies a maximum tetrahedron volume constraint.");
            ArgumentCheckboxes[2] = this.checkBox3;
            toolTip1.SetToolTip(this.checkBox4, "Assigns attributes to identify tetrahedra in certain regions.");
            ArgumentCheckboxes[3] = this.checkBox4;
            toolTip1.SetToolTip(this.checkBox5, "Reconstructs/Refines a previously generated mesh.");
            ArgumentCheckboxes[4] = this.checkBox5;
            toolTip1.SetToolTip(this.checkBox6, "Suppresses boundary facets/segments splitting.");
            ArgumentCheckboxes[5] = this.checkBox6;
            toolTip1.SetToolTip(this.checkBox7, "Inserts a list of additional points into mesh.");
            ArgumentCheckboxes[6] = this.checkBox7;
            toolTip1.SetToolTip(this.checkBox8, "Does not merge coplanar facets.");
            ArgumentCheckboxes[7] = this.checkBox8;
            toolTip1.SetToolTip(this.checkBox9, "Set a tolerance for coplanar test.");
            ArgumentCheckboxes[8] = this.checkBox9;
            toolTip1.SetToolTip(this.checkBox10, "Detect intersections of PLC facets.");
            ArgumentCheckboxes[9] = this.checkBox10;
            toolTip1.SetToolTip(this.checkBox11, "Numbers all output items starting from zero.");
            ArgumentCheckboxes[10] = this.checkBox11;
            toolTip1.SetToolTip(this.checkBox12, "Jettison unused vertices from output .node file.");
            ArgumentCheckboxes[11] = this.checkBox12;
            toolTip1.SetToolTip(this.checkBox13, "Generates second-order subparametric elements.");
            ArgumentCheckboxes[12] = this.checkBox13;

            RenderThread = new Thread(new ThreadStart(() => {
                while (true)
                {
                    renderPanel1.Draw();
                    renderPanel2.Draw();
                }
            }));
            RenderThread.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            saveDialog();
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
            RenderThread.Abort();
        }

        private void exitToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Are you sure you want to exit?",
                      "Exit?", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes) Application.Exit();
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            saveDialog();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            MinEdgeRadiusRatioInput.Visible = ArgumentCheckboxes[MinEdgeRadiusIndex].Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            MaxVolumeInput.Visible = ArgumentCheckboxes[MaxVolumeIndex].Checked;
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            CoplanarTestToleranceInput.Visible = ArgumentCheckboxes[CoplanarTestToleranceIndex].Checked;
        }

        private void checkBox14_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox14.Checked)
            {
                tableLayoutPanel1.ColumnCount = 2;
                renderPanel2.Visible = true;
            } else
            {
                tableLayoutPanel1.ColumnCount = 1;
                renderPanel2.Visible = false;
            }
        }

        private void renderPanel1_MouseMove(object sender, MouseEventArgs e)
        {
            renderPanel1.mouseDrag(sender, e);
        }

        private void renderPanel2_MouseMove(object sender, MouseEventArgs e)
        {
            renderPanel2.mouseDrag(sender, e);
        }

        private void renderPanel2_MouseDown(object sender, MouseEventArgs e)
        {
            renderPanel2.setMouseStart(e);
        }

        private void renderPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            renderPanel1.setMouseStart(e);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //create argument string  
            textBox4.Text = ArgString();
            //send arguemnt and file to tetgen
        }

        private String ArgString()
        {
            //Takes the results from the checkboxes and creates a argument string to be sent to the tetgen .exe
            var argumentStr = new StringBuilder();
            for (var i = 0; i < ArgumentCheckboxes.Length; i++)
            {
                if (ArgumentCheckboxes[i].Checked)
                {
                    argumentStr.Append(ArgumentCheckboxes[i].Text + " ");
                    if (i == MinEdgeRadiusIndex)
                    {
                        argumentStr.Append(MinEdgeRadiusRatioInput.Text + " ");
                    }
                    if (i == MaxVolumeIndex)
                    {
                        argumentStr.Append(MaxVolumeInput.Text + " ");
                    }
                    if (i == CoplanarTestToleranceIndex)
                    {
                        argumentStr.Append(CoplanarTestToleranceInput.Text + " ");
                    }
                }
            }
            return argumentStr.ToString();
        }
    }
}
