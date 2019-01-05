using System;
using System.Collections.Generic;
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
                var p = new DataGenerator.DataPoint(Convert.ToDouble(lines[i].Split(',')[0]), Convert.ToDouble(lines[i].Split(',')[1]));
                p.ID = i;
                data.Add(p);
            }

            var splits = lines[0].Split(',');
            int r = Convert.ToInt32(splits[0]), g = Convert.ToInt32(splits[1]), b = Convert.ToInt32(splits[2]);

            for (int i = 0; i < r; i++)
            {
                data[i].Color = 1;
            }
            for (int i = r; i < r + g; i++)
            {
                data[i].Color = 2;
            }
            for (int i = r + g; i < r + g + b; i++)
            {
                data[i].Color = 3;
            }
            
            sr.Close();
            sr.Dispose();
            fs.Close();
            fs.Dispose();

            return data;
        }

        public void WriteRawData(List<DataGenerator.DataPoint> data, string filepath)
        {
            FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            // Clear previous content.
            fs.SetLength(0);

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
                if (p.Color == 1)
                {
                    sw.WriteLine(p.X + "," + p.Y);
                }
            }
            foreach (DataGenerator.DataPoint p in data)
            {
                if (p.Color == 2)
                {
                    sw.WriteLine(p.X + "," + p.Y);
                }
            }
            foreach (DataGenerator.DataPoint p in data)
            {
                if (p.Color == 3)
                {
                    sw.WriteLine(p.X + "," + p.Y);
                }
            }

            sw.Close();
            sw.Dispose();
            fs.Close();
            fs.Dispose();
        }

        public void WriteCut(int id1, int id2, List<Swap> swaps, string filepath)
        {
            FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            // Clear previous content.
            fs.SetLength(0);

            sw.WriteLine(id1 + "," + id2);
            sw.WriteLine(swaps.Count);
            foreach (var s in swaps)
            {
                sw.WriteLine(s.PointA.ID + "," + s.PointB.ID);
            }

            sw.Close();
            sw.Dispose();
            fs.Close();
            fs.Dispose();
        }
    }
}
