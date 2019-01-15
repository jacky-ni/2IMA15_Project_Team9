using System;
using System.Collections.Generic;
using System.Linq;

namespace _2IMA15_Project_Team9.DataGenerator
{
    class DataGenerator
    {
        private int _canvasHeight = 0;
        private int _canvasWidth = 0;

        public DataGenerator(int height, int width)
        {
            _canvasHeight = height;
            _canvasWidth = width;
        }

        public List<DataPoint> GenerateGeneralCaseDataset(int n)
        {
            var dic = new Dictionary<string, DataPoint>();
            
            List<int> xs = new List<int>();
            List<int> ys = new List<int>();

            for (int i = 0; i < _canvasWidth; i++)
            {
                xs.Add(i + 1);
            }
            for (int i = 0; i < _canvasHeight; i++)
            {
                ys.Add(i + 1);
            }

            Random r = new Random();
            for (int i = 0; i < n; i++)
            {
                var j = r.Next(0, xs.Count);
                var k = r.Next(0, ys.Count);
                DataPoint outvalue = null;
                dic.TryGetValue(j + "," + k, out outvalue);
                while (outvalue != null)
                {
                    j = r.Next(0, xs.Count);
                    k = r.Next(0, ys.Count);
                    dic.TryGetValue(j + "," + k, out outvalue);
                }
                dic.Add(j + "," + k, new DataPoint(xs[j], ys[k]));
            }
            var res = dic.Values.ToList();
            ColorVertices(res);
            for (int i = 0; i < res.Count; i++)
            {
                res[i].ID = i + 1;
            }
            return res;
        }
        
        private void ColorVertices(List<DataPoint> vertices)
        {
            if (vertices.Count < 6) throw new Exception("Datagenerator - ColorVertices: Insifficient data!");

            vertices[0].Color = 1;
            vertices[1].Color = 1;
            vertices[2].Color = 2;
            vertices[3].Color = 2;
            vertices[4].Color = 3;
            vertices[5].Color = 3;

            int counter = 6;
            Random r = new Random();
            while (counter < vertices.Count)
            {
                vertices[counter].Color = r.Next(3) + 1;
                counter += 1;
            }
        }

        public List<DataPoint> GenerateGeneralBadCaseDataset(int n, int numberOfClusters)
        {
            var data = GenerateGeneralCaseDataset(n);
            KMeansAlg km = new KMeansAlg(numberOfClusters, data);
            km.InitializeCentroids(new List<DataPoint>
                    { new DataPoint(0, 0), new DataPoint(_canvasWidth/2, _canvasHeight/2), new DataPoint(_canvasWidth , _canvasHeight) });

            km.Cluster();
            data.ForEach(x => x.Color = x.Color + 1);
            for (int i = 0; i < data.Count; i++)
            {
                data[i].ID = i + 1;
            }
            return data;
        }
        
        public List<DataPoint> GenerateWorstCaseDataset(int n)
        {
            var left_top = new Dictionary<string, DataPoint>();
            var right_top = new Dictionary<string, DataPoint>();
            var bot_middle = new Dictionary<string, DataPoint>();

            var y_t = new List<int>();
            var y_b = new List<int>();
            var x_l = new List<int>();
            var x_m = new List<int>();
            var x_r = new List<int>();

            for (int i = 0; i < _canvasHeight / 2; i++)
            {
                y_b.Add(i);
            }
            for (int i = _canvasHeight / 2; i < _canvasHeight; i++)
            {
                y_t.Add(i);
            }
            for (int i = 0; i < _canvasWidth / 3; i++)
            {
                x_l.Add(i);
            }
            for (int i = _canvasWidth / 3; i < _canvasWidth * 2 / 3; i++)
            {
                x_m.Add(i);
            }
            for (int i = _canvasWidth * 2 / 3; i < _canvasWidth; i++)
            {
                x_r.Add(i);
            }

            Random r = new Random();
            for (int i = 0; i < n / 3; i++)
            {
                var j = r.Next(0, x_l.Count);
                var k = r.Next(0, y_t.Count);
                DataPoint outvalue = null;
                left_top.TryGetValue(j + "," + k, out outvalue);
                while (outvalue != null)
                {
                    j = r.Next(0, x_l.Count);
                    k = r.Next(0, y_t.Count);
                    left_top.TryGetValue(j + "," + k, out outvalue);
                }
                left_top.Add(j + "," + k, new DataPoint(x_l[j], y_t[k]));
            }
            for (int i = 0; i < n / 3; i++)
            {
                var j = r.Next(0, x_l.Count);
                var k = r.Next(0, y_t.Count);
                DataPoint outvalue = null;
                right_top.TryGetValue(j + "," + k, out outvalue);
                while (outvalue != null)
                {
                    j = r.Next(0, x_r.Count);
                    k = r.Next(0, y_t.Count);
                    right_top.TryGetValue(j + "," + k, out outvalue);
                }
                right_top.Add(j + "," + k, new DataPoint(x_r[j], y_t[k]));
            }
            for (int i = 0; i < n / 3; i++)
            {
                var j = r.Next(0, x_l.Count);
                var k = r.Next(0, y_t.Count);
                DataPoint outvalue = null;
                bot_middle.TryGetValue(j + "," + k, out outvalue);
                while (outvalue != null)
                {
                    j = r.Next(0, x_m.Count);
                    k = r.Next(0, y_b.Count);
                    bot_middle.TryGetValue(j + "," + k, out outvalue);
                }
                bot_middle.Add(j + "," + k, new DataPoint(x_m[j], y_b[k]));
            }

            foreach (var p in left_top.Values)
            {
                p.Color = 1;
            }
            foreach (var p in right_top.Values)
            {
                p.Color = 2;
            }
            foreach (var p in bot_middle.Values)
            {
                p.Color = 3;
            }
            var data = left_top.Values.Union(right_top.Values.Union(bot_middle.Values)).ToList();
            for (int i = 0; i < data.Count; i++)
            {
                data[i].ID = i + 1;
            }
            return data;
        }

        public List<DataPoint> GenerateBestCaseDataset(int n)
        {
            // The clear gap (a horizontal gap).
            int margin = 20;

            var top = new Dictionary<string, DataPoint>();
            var bot = new Dictionary<string, DataPoint>();

            var y_t = new List<int>();
            var y_b = new List<int>();
            var xs = new List<int>();

            for (int i = 0; i < _canvasHeight / 2 - margin; i++)
            {
                y_b.Add(i);
            }
            for (int i = _canvasHeight / 2 + margin; i < _canvasHeight; i++)
            {
                y_t.Add(i);
            }
            for (int i = 0; i < _canvasWidth; i++)
            {
                xs.Add(i);
            }

            Random r = new Random();
            for (int i = 0; i < n / 2; i++)
            {
                var j = r.Next(0, xs.Count);
                var k = r.Next(0, y_t.Count);
                DataPoint outvalue = null;
                top.TryGetValue(j + "," + k, out outvalue);
                while (outvalue != null)
                {
                    j = r.Next(0, xs.Count);
                    k = r.Next(0, y_t.Count);
                    top.TryGetValue(j + "," + k, out outvalue);
                }
                top.Add(j + "," + k, new DataPoint(xs[j], y_t[k]));
            }
            for (int i = 0; i < n / 2; i++)
            {
                var j = r.Next(0, xs.Count);
                var k = r.Next(0, y_t.Count);
                DataPoint outvalue = null;
                bot.TryGetValue(j + "," + k, out outvalue);
                while (outvalue != null)
                {
                    j = r.Next(0, xs.Count);
                    k = r.Next(0, y_b.Count);
                    bot.TryGetValue(j + "," + k, out outvalue);
                }
                bot.Add(j + "," + k, new DataPoint(xs[j], y_b[k]));
            }

            for (int i = 0; i < n / 2; i++)
            {
                top.Values.ToList()[i].Color = i % 3 + 1;
                bot.Values.ToList()[i].Color = i % 3 + 1;
            }
            var data = top.Values.Union(bot.Values).ToList();
            for (int i = 0; i < data.Count; i++)
            {
                data[i].ID = i + 1;
            }
            return data;
        }

    }
}
