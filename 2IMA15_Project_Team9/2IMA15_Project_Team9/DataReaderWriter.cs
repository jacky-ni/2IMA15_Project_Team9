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
        public List<List<double>> ReadData(string filepath)
        {
            var data = new List<List<double>>();
            
            string[] lines = File.ReadAllLines(filepath);
            for (int i = 1; i < lines.Length; i++)
            {
                var t = new List<double>();
                if (lines[i].Contains(','))
                {
                    var ls = lines[i].Split(',');
                    for (int j = 0; j < ls.Length; j++)
                    {
                        t.Add(Convert.ToDouble(ls[j]));
                    }
                }
                else
                {
                    t.Add(Convert.ToDouble(lines[i]));
                }
                data.Add(t);
            }

            return data;
        }

        public void WriteData(List<List<double>> data, string filepath)
        {
            StreamWriter fileWriter = new StreamWriter(filepath);
            foreach (List<double> line in data)
            {
                string s = "";
                foreach (var d in line)
                {
                    s += d + ",";
                }
                //Remove the last comma.
                s.Remove(s.Length - 1, 1);
                fileWriter.WriteLine(s);
            }
        }
    }
}
