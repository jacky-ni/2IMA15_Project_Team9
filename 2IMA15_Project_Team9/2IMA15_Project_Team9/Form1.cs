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
        private string _fileName = "";
        private Form _form = null;
        private List<DataGenerator.DataPoint> _rawdata = null;

        // y = _cutD * x + _cutT
        private double _cutD = 0;
        private double _cutT = 0;
        private DataReaderWriter _dataReaderWriter = new DataReaderWriter();

        private string _directoryPath = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _fileName = @"\GeneralCaseDataset";
            if (!Initialize(sender, e)) return;

            DataGenerator.DataGenerator dg = new DataGenerator.DataGenerator(Convert.ToInt32(textBox1.Text), Convert.ToInt32(textBox2.Text));
            _rawdata = dg.GenerateGeneralCaseDataset(Convert.ToInt32(textBox3.Text));

            _dataReaderWriter.WriteRawData(_rawdata, _directoryPath + _fileName + ".txt");
            openNewForm(_fileName);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _fileName = @"\GeneralBadCaseDataset";
            if (!Initialize(sender, e)) return;

            DataGenerator.DataGenerator dg = new DataGenerator.DataGenerator(Convert.ToInt32(textBox1.Text), Convert.ToInt32(textBox2.Text));
            _rawdata = dg.GenerateGeneralBadCaseDataset(Convert.ToInt32(textBox3.Text), 3);

            _dataReaderWriter.WriteRawData(_rawdata, _directoryPath + _fileName + ".txt");
            openNewForm(_fileName);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _fileName = @"\BestCaseDataset";
            if (!Initialize(sender, e)) return;

            DataGenerator.DataGenerator dg = new DataGenerator.DataGenerator(Convert.ToInt32(textBox1.Text), Convert.ToInt32(textBox2.Text));
            _rawdata = dg.GenerateBestCaseDataset(Convert.ToInt32(textBox3.Text));

            _dataReaderWriter.WriteRawData(_rawdata, _directoryPath + _fileName + ".txt");
            openNewForm(_fileName);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            _fileName = @"\WorstCaseDataset";
            if (!Initialize(sender, e)) return;

            DataGenerator.DataGenerator dg = new DataGenerator.DataGenerator(Convert.ToInt32(textBox1.Text), Convert.ToInt32(textBox2.Text));
            _rawdata = dg.GenerateWorstCaseDataset(Convert.ToInt32(textBox3.Text));

            _dataReaderWriter.WriteRawData(_rawdata, _directoryPath + _fileName + ".txt");
            openNewForm(_fileName);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _rawdata = _dataReaderWriter.ReadRawData(ofd.FileName);
                    openNewForm(ofd.FileName);
                    textBox4.Text = Path.GetDirectoryName((ofd.FileName));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error, selected file is invalid.");
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
            if (!Directory.Exists(_directoryPath))
            {
                MessageBox.Show("Error, selected directory does not exist.");
                return;
            }

            ThreeColorCut tcc = new ThreeColorCut(_rawdata);
            _rawdata = tcc.RawData;

            if (tcc.Message != null && tcc.Message != "")
            {
                lbinfo.Text = "Information: \r\n" + tcc.Message;
            }

            _cutD = tcc.CutD;
            _cutT = tcc.CutT;
            _form.Refresh();

            if (tcc.OnLines.First().X != tcc.RawDataBackUp.Find(x => x.ID == tcc.OnLines.First().ID).X ||
                tcc.OnLines.First().Y != tcc.RawDataBackUp.Find(x => x.ID == tcc.OnLines.First().ID).Y ||
                tcc.OnLines.Last().X != tcc.RawDataBackUp.Find(x => x.ID == tcc.OnLines.Last().ID).X ||
                tcc.OnLines.Last().Y != tcc.RawDataBackUp.Find(x => x.ID == tcc.OnLines.Last().ID).Y)
            {
                bool stop = true;
            }

            _dataReaderWriter.WriteCut(tcc.OnLines.First().ID, tcc.OnLines.Last().ID, tcc.Swaps, _directoryPath + _fileName + "Cut.txt");

            button6.Enabled = false;
        }

        private double OnTopOfLine(DataGenerator.DataPoint data, Line line)
        {
            return data.Y - (line.D * data.X + line.T);
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
            _cutT = 0;
            _cutD = 0;

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
            lbinfo.Text = "";
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
        }

        private void _form_Paint(object sender, PaintEventArgs e)
        {
            var radius = 4;
            foreach (var p in _rawdata)
            {
                if (p.Swapped) radius = 10;
                else radius = 4;
                if (p.Color == 1)
                    e.Graphics.DrawEllipse(Pens.Blue, (float)p.X-radius/2, (float)p.Y - radius / 2, radius, radius);
                else if (p.Color == 2)
                    e.Graphics.DrawEllipse(Pens.Red, (float)p.X - radius / 2, (float)p.Y - radius / 2, radius, radius);
                else if (p.Color == 3)
                    e.Graphics.DrawEllipse(Pens.Green, (float)p.X - radius / 2, (float)p.Y - radius / 2, radius, radius);
                else
                    e.Graphics.DrawRectangle(Pens.Black, (float)p.X - radius / 2, (float)p.Y - radius / 2, radius, radius);
            }
            if (_cutT != 0 && _cutD != 0)
            {
                e.Graphics.DrawLine(Pens.Black, new PointF(0, (float)_cutT), new PointF(_form.Width, (float)(_form.Width * _cutD + _cutT)));
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
        /// TEST DATA
        /// </summary>
        private void Test()
        {
            // Test data.
            _rawdata = new List<DataGenerator.DataPoint>();
            var d = new DataGenerator.DataPoint(200, 100);
            d.Color = 1;
            _rawdata.Add(d);
            d = new DataGenerator.DataPoint(300, 200);
            d.Color = 1;
            _rawdata.Add(d);
            d = new DataGenerator.DataPoint(400, 200);
            d.Color = 1;
            _rawdata.Add(d);
            d = new DataGenerator.DataPoint(500, 300);
            d.Color = 1;
            _rawdata.Add(d);
            d = new DataGenerator.DataPoint(600, 100);
            d.Color = 1;
            _rawdata.Add(d);
            d = new DataGenerator.DataPoint(700, 300);
            d.Color = 1;
            _rawdata.Add(d);
            d = new DataGenerator.DataPoint(800, 400);
            d.Color = 1;
            _rawdata.Add(d);
            d = new DataGenerator.DataPoint(900, 500);
            d.Color = 1;
            _rawdata.Add(d);
            d = new DataGenerator.DataPoint(1000, 600);
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
