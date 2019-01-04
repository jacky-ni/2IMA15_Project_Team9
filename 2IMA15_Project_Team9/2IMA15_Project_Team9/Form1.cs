using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace _2IMA15_Project_Team9
{
    public partial class Form1 : Form
    {
        // For better display.
        private int _margin = 50;

        private Form _form = null;
        private List<DataGenerator.DataPoint> _rawdata = null;

        // y = _cutD * x + _cutT
        private List<float> _cutDs = new List<float>();
        private List<float> _cutTs = new List<float>();
        private DataReaderWriter _dataReaderWriter = new DataReaderWriter();

        private string _directoryPath = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string fileName = @"\GeneralCaseDataset.txt";
            if (!Initialize(sender, e)) return;

            DataGenerator.DataGenerator dg = new DataGenerator.DataGenerator(Convert.ToInt32(textBox1.Text), Convert.ToInt32(textBox2.Text));
            _rawdata = dg.GenerateGeneralCaseDataset(Convert.ToInt32(textBox3.Text));

            _dataReaderWriter.WriteRawData(_rawdata, _directoryPath + fileName);
            openNewForm(fileName);
        }


        private void button2_Click(object sender, EventArgs e)
        {
            string fileName = @"\GeneralBadCaseDataset.txt";
            if (!Initialize(sender, e)) return;

            DataGenerator.DataGenerator dg = new DataGenerator.DataGenerator(Convert.ToInt32(textBox1.Text), Convert.ToInt32(textBox2.Text));
            _rawdata = dg.GenerateGeneralBadCaseDataset(Convert.ToInt32(textBox3.Text), 3);

            _dataReaderWriter.WriteRawData(_rawdata, _directoryPath + fileName);
            openNewForm(fileName);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string fileName = @"\BestCaseDataset.txt";
            if (!Initialize(sender, e)) return;

            DataGenerator.DataGenerator dg = new DataGenerator.DataGenerator(Convert.ToInt32(textBox1.Text), Convert.ToInt32(textBox2.Text));
            _rawdata = dg.GenerateBestCaseDataset(Convert.ToInt32(textBox3.Text));

            _dataReaderWriter.WriteRawData(_rawdata, _directoryPath + fileName);
            openNewForm(fileName);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string fileName = @"\WorstCaseDataset.txt";
            if (!Initialize(sender, e)) return;

            DataGenerator.DataGenerator dg = new DataGenerator.DataGenerator(Convert.ToInt32(textBox1.Text), Convert.ToInt32(textBox2.Text));
            _rawdata = dg.GenerateWorstCaseDataset(Convert.ToInt32(textBox3.Text));

            _dataReaderWriter.WriteRawData(_rawdata, _directoryPath + fileName);
            openNewForm(fileName);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _rawdata = _dataReaderWriter.ReadRawData(ofd.FileName);
                openNewForm(ofd.FileName);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //Test();

            if (_rawdata == null)
            {
                MessageBox.Show("Error, No data generated or loaded.");
                return;
            }

            // TODO:
            var message = MakeRawDataOddAmount();
            if (message != "") MessageBox.Show(message);
            var set1 = _rawdata.Where(x => x.Color == 1).ToList();
            var set2 = _rawdata.Where(x => x.Color == 2).ToList();
            TwoColorCut tc = new TwoColorCut(set1, set2);
            foreach (var t in tc.Intersections)
            {
                _cutDs.Add(t.IntersectionPoint.X);
                _cutTs.Add(-t.IntersectionPoint.Y);
            }
            if (_form != null) _form.Refresh();
        }

        // Assume odd number of points, if not remove one.
        private string MakeRawDataOddAmount()
        {
            string msg = "";
            int r = 0, g = 0, b = 0;
            foreach (var p in _rawdata)
            {
                if (p.Color == 1) r++;
                if (p.Color == 2) g++;
                if (p.Color == 3) b++;
            }

            if (r % 2 == 0)
            {
                int t = _rawdata.Count;
                for (int i = 0; i < t; i++)
                {
                    if (_rawdata[i].Color == 1)
                    {
                        _rawdata.RemoveAt(i);
                        msg += _rawdata[i].ToString() + " is removed. \r\n";
                        break;
                    }
                }
            }
            if (g % 2 == 0)
            {
                int t = _rawdata.Count;
                for (int i = 0; i < t; i++)
                {
                    if (_rawdata[i].Color == 2)
                    {
                        _rawdata.RemoveAt(i);
                        msg += _rawdata[i].ToString() + " is removed. \r\n";
                        break;
                    }
                }
            }
            if (b % 2 == 0)
            {
                int t = _rawdata.Count;
                for (int i = 0; i < t; i++)
                {
                    if (_rawdata[i].Color == 3)
                    {
                        _rawdata.RemoveAt(i);
                        msg += _rawdata[i].ToString() + " is removed. \r\n";
                        break;
                    }
                }
            }

            return msg;
        }

        private bool Initialize(object sender, EventArgs e)
        {
            if (!Directory.Exists(_directoryPath))
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    textBox4.Text = fbd.SelectedPath;
                    toolStripStatusLabel1.Text = "Dataset is stored in directory '" + fbd.SelectedPath + "'";
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private void openNewForm(string fileName)
        {
            _cutTs.Clear();
            _cutDs.Clear();

            // For better display.
            _form = new Form();
            _form.Height = Convert.ToInt32(textBox1.Text) + _margin;
            _form.Width = Convert.ToInt32(textBox2.Text) + _margin;
            _form.Show();

            _form.Paint += _form_Paint;
            _form.FormClosing += _form_FormClosing;

            toolStripStatusLabel1.Text = "Dataset is stored in file '" + _directoryPath + fileName;

            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
        }

        private void _form_FormClosing(object sender, FormClosingEventArgs e)
        {
            _form = null;
            _rawdata = null;
            
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
        }

        private void _form_Paint(object sender, PaintEventArgs e)
        {
            var radius = 5;
            foreach (var p in _rawdata)
            {
                if (p.Color == 1)
                    e.Graphics.DrawEllipse(Pens.Blue, (float)p.X, (float)p.Y, radius, radius);
                else if (p.Color == 2)
                    e.Graphics.DrawEllipse(Pens.Red, (float)p.X, (float)p.Y, radius, radius);
                else if (p.Color == 3)
                    e.Graphics.DrawEllipse(Pens.Green, (float)p.X, (float)p.Y, radius, radius);
                else
                    e.Graphics.DrawRectangle(Pens.Black, (float)p.X, (float)p.Y, radius, radius);
            }

            if (_cutDs.Count != 0 && _cutTs.Count != 0 && _cutDs.Count==_cutTs.Count)
            {
                for (int i = 0; i < _cutDs.Count; i++)
                {
                    e.Graphics.DrawLine(Pens.Black, new Point(0, (int)_cutTs[i]), new Point(_form.Width, (int)(_form.Width * _cutDs[i] + _cutTs[i])));
                }
            }
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
            if (Convert.ToDouble(textBox3.Text) > Convert.ToDouble(textBox1.Text) * Convert.ToDouble(textBox2.Text))
            {
                MessageBox.Show("The amount of points should be less than or equal to canvas width * canvas height!");
                textBox3.Text = "";
            }
        }

        private void textBox4_DoubleClick(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                textBox4.Text = fbd.SelectedPath;
                toolStripStatusLabel1.Text = "Dataset is stored in directory '" + fbd.SelectedPath + "'";
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            _directoryPath = textBox4.Text;
        }

        /// <summary>
        /// TEST CODE
        /// </summary>
        private void Test()
        {
            // Test code.
            _rawdata = new List<DataGenerator.DataPoint>();
            var d = new DataGenerator.DataPoint(200, 100);
            d.Color = 1;
            _rawdata.Add(d);
            d = new DataGenerator.DataPoint(300, 200);
            d.Color = 1;
            _rawdata.Add(d);
            d = new DataGenerator.DataPoint(400, 100);
            d.Color = 1;
            _rawdata.Add(d);
            d = new DataGenerator.DataPoint(500, 100);
            d.Color = 1;
            _rawdata.Add(d);
            d = new DataGenerator.DataPoint(600, 100);
            d.Color = 1;
            _rawdata.Add(d);

            d = new DataGenerator.DataPoint(100, 300);
            d.Color = 2;
            _rawdata.Add(d);
            d = new DataGenerator.DataPoint(200, 400);
            d.Color = 2;
            _rawdata.Add(d);
            d = new DataGenerator.DataPoint(300, 600);
            d.Color = 2;
            _rawdata.Add(d);
            d = new DataGenerator.DataPoint(200, 300);
            d.Color = 2;
            _rawdata.Add(d);
            d = new DataGenerator.DataPoint(300, 400);
            d.Color = 2;
            _rawdata.Add(d);
            d = new DataGenerator.DataPoint(400, 600);
            d.Color = 2;
            _rawdata.Add(d);
            d = new DataGenerator.DataPoint(300, 300);
            d.Color = 2;
            _rawdata.Add(d);
            d = new DataGenerator.DataPoint(400, 400);
            d.Color = 2;
            _rawdata.Add(d);
            d = new DataGenerator.DataPoint(500, 600);
            d.Color = 2;
            _rawdata.Add(d);

            openNewForm(@"\testData.txt");
        }
    }
}
