using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _2IMA15_Project_Team9
{
    public partial class Form1 : Form
    {
        // For better display.
        private int _margin = 60;
        private Form _form = null;
        private List<DataGenerator.DataPoint> _rawdata = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataGenerator.DataGenerator dg = new DataGenerator.DataGenerator(Convert.ToInt32(textBox1.Text), Convert.ToInt32(textBox2.Text));
            _rawdata = dg.GenerateRamdonVertices(Convert.ToInt32(textBox3.Text) * 6);
            dg.ColorVertices(_rawdata);

            _form = new Form();
            _form.Height = Convert.ToInt32(textBox1.Text) + _margin;
            _form.Width = Convert.ToInt32(textBox2.Text) + _margin;
            _form.Show();

            // For better display.
            foreach (var p in _rawdata)
            {
                p.X += _margin / 6;
                p.Y += _margin / 6;
            }

            _form.Paint += _form_Paint;
            _form.FormClosing += _form_FormClosing;
        }

        private void _form_FormClosing(object sender, FormClosingEventArgs e)
        {
            _form = null;
            _rawdata = null;
        }

        private void _form_Paint(object sender, PaintEventArgs e)
        {
            foreach (var p in _rawdata)
            {
                if (p.Color == 1)
                    e.Graphics.DrawRectangle(Pens.Blue, (float)p.X, (float)p.Y, 1, 1);
                else if (p.Color == 2)
                    e.Graphics.DrawRectangle(Pens.Red, (float)p.X, (float)p.Y, 1, 1);
                else if (p.Color == 3)
                    e.Graphics.DrawRectangle(Pens.Green, (float)p.X, (float)p.Y, 1, 1);
                else
                    e.Graphics.DrawRectangle(Pens.Black, (float)p.X, (float)p.Y, 1, 1);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            DataGenerator.DataGenerator dg = new DataGenerator.DataGenerator(Convert.ToInt32(textBox1.Text), Convert.ToInt32(textBox2.Text));
            _rawdata = dg.GenerateBestCaseDataset(Convert.ToInt32(textBox3.Text) * 6);

            _form = new Form();
            _form.Height = Convert.ToInt32(textBox1.Text) + _margin;
            _form.Width = Convert.ToInt32(textBox2.Text) + _margin;
            _form.Show();

            // For better display.
            foreach (var p in _rawdata)
            {
                p.X += _margin / 6;
                p.Y += _margin / 6;
            }

            _form.Paint += _form_Paint;
            _form.FormClosing += _form_FormClosing;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DataGenerator.DataGenerator dg = new DataGenerator.DataGenerator(Convert.ToInt32(textBox1.Text), Convert.ToInt32(textBox2.Text));
            _rawdata = dg.GenerateWorstCaseDataset(Convert.ToInt32(textBox3.Text) * 6);

            _form = new Form();
            _form.Height = Convert.ToInt32(textBox1.Text) + _margin;
            _form.Width = Convert.ToInt32(textBox2.Text) + _margin;
            _form.Show();

            // For better display.
            foreach (var p in _rawdata)
            {
                p.X += _margin / 6;
                p.Y += _margin / 6;
            }

            _form.Paint += _form_Paint;
            _form.FormClosing += _form_FormClosing;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox3.Text == "") return;
            if (Convert.ToDouble(textBox3.Text) / 6 > Convert.ToDouble(textBox1.Text) * Convert.ToDouble(textBox2.Text))
            {
                MessageBox.Show("The amount of points should be less than or equal to canvas width * canvas height!");
                textBox3.Text = "";
            }
        }
    }
}
