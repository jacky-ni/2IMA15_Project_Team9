using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace _2IMA15_Project_Team9
{
    class DataReaderWriter
    {
        public List<DataGenerator.DataPoint> ReadRawData(string filepath)
        {
            FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamReader sr = new StreamReader(fs);

            var data = new List<DataGenerator.DataPoint>();
            var lines = new List<string>();
            string c = sr.ReadLine();
            while (c != null)
            {
                lines.Add(c);
                c = sr.ReadLine();
            }

            for (int i = 1; i < lines.Count; i++)
            {
                data.Add(new DataGenerator.DataPoint(Convert.ToDouble(lines[i].Split(',')[0]), Convert.ToDouble(lines[i].Split(',')[1])));
            }

            return data;
        }

        public void WriteRawData(List<DataGenerator.DataPoint> data, string filepath)
        {
            FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(fs);
            
            int r = 0, g = 0, b = 0;
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].Color == 1) r++;
                else if (data[i].Color == 2) b++;
                else g++;
            }
            sw.WriteLine(r + "," + b + "," + g);

            foreach (DataGenerator.DataPoint p in data)
            {
                sw.WriteLine(p.X + "," + p.Y);
            }

            sw.Close();
            sw.Dispose();
            fs.Close();
            fs.Dispose();
        }
    }
}
