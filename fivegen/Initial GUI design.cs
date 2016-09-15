using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            openDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {

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
            openFileDialog1.Filter = "Text Files (.txt)|*.txt|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = true;
            openFileDialog1.ShowDialog();


        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("TetGen is a powerful command-line tool with many options for generating tetrahedral lattices. These lattices are used for many applications from mechanical engineering, biomedical research, fluid simulations, to computer games and interactive VR simulations. Their exact structure is very important for different applications, and TetGen is one of very few open-source tools available that have these features. http://wiasberlin.de/software/tetgen/ \n \nThis project will develop a graphical user interface that simplifies the use of this tool(using presets and thumbnails etc) to open up the tool to a far bigger audience.It will include 3D computer graphics to visualize the generated 3D meshes (both triangle-shell meshes, and tetrahedral lattices).",
            "About");
        }
    }
}
