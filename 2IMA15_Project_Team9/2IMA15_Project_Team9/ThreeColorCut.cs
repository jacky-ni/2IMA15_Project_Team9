using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2IMA15_Project_Team9
{
    class ThreeColorCut
    {
        private List<DataGenerator.DataPoint> _rawdata = null;

        // y = CutD * x + CutT
        public double CutD { get; private set; }
        public double CutT { get; private set; }
        public List<Swap> Swaps { get; private set; }
        public string Message { get; private set; }

        public ThreeColorCut(List<DataGenerator.DataPoint> rawdata)
        {
            Swaps = new List<Swap>();
            _rawdata = rawdata;
            Message += ModifyColor();
            CalculateCut();
        }

        private void CalculateCut()
        {
            var set1 = _rawdata.Where(x => x.ModifiedColor == 1).ToList();
            var set2 = new List<DataGenerator.DataPoint>();
            if (set1.Count == 0)
            {
                set1 = _rawdata.Where(x => x.ModifiedColor == 2).ToList();
                set2 = _rawdata.Where(x => x.ModifiedColor == 3).ToList();
            }
            else
            {
                set2 = _rawdata.Where(x => x.ModifiedColor == 2).ToList();
            }

            TwoColorCut tc = new TwoColorCut(set1, set2);
            // y = _cutD * x + _cutT
            List<double> cutDs = new List<double>();
            List<double> cutTs = new List<double>();
            foreach (var t in tc.Intersections)
            {
                cutDs.Add(t.IntersectionPointX);
                cutTs.Add(-t.IntersectionPointY);
            }
            
            for (int i = 0; i < cutDs.Count; i++)
            {
                var ups = new List<DataGenerator.DataPoint>();
                var bots = new List<DataGenerator.DataPoint>();
                //var ols = new List<DataGenerator.DataPoint>();

                int up1 = 0, ol1 = 0, bot1 = 0;
                int up2 = 0, ol2 = 0, bot2 = 0;

                var line = new Line(cutDs[i], cutTs[i], 0);
                foreach (var p in _rawdata)
                {
                    double r = OnTopOfLine(p, line);

                    if (Math.Abs(r) > 0 && Math.Abs(r) < 0.1)
                    {
                        // This is where the error happens.
                    }

                    if (p.Color == 1)
                    {
                        if (r > 0)
                        {
                            ups.Add(p);
                            up1++;
                        }
                        else if (r < 0)
                        {
                            bots.Add(p);
                            bot1++;
                        }
                        else
                        {
                            //ols.Add(p);
                            ol1++;
                        }
                    }
                    if (p.Color == 2)
                    {
                        if (r > 0)
                        {
                            ups.Add(p);
                            up2++;
                        }
                        else if (r < 0)
                        {
                            bots.Add(p);
                            bot2++;
                        }
                        else
                        {
                            //ols.Add(p);
                            ol2++;
                        }
                    }
                }

                // This usually should always holds, however, since the computer can have computation error.
                // Some cases could be invalid.
                // For instance, if we have y=1/3 *x +2.
                // And if we have a point (3,3), the computer would say 2.9999999999999999... is less than 3.
                // So the point is below y=1/3 *x +2 while actually it should be on the line.
                if (up1 == bot1 && up2 == bot2 && ol1 == 1 && ol2 == 1)
                {
                    var swaps = CalculateSwap(cutDs[i], cutTs[i], ups, bots);

                    // Get the line with minimum swaps.
                    if (swaps.Count < Swaps.Count || Swaps.Count == 0)
                    {
                        CutD = cutDs[i];
                        CutT = cutTs[i];
                        Swaps = swaps;
                    }
                }
            }
        }

        /// <summary>
        /// Since we have in total odd number of points for set 1 and set 2, namely the sets which more points.
        /// And we have even number of points for set 3, namely the set with less points.
        /// We are sure that we can process the set 3 points by pair.
        /// More precisely, the number of points of set 3 in one both sides of the cut is even.
        /// 
        /// Because assume set 1 and set 2 are the main set, set 3 is the small set, whose color will be chaged to 1 and 2.
        /// now we have a cut.
        /// If in the left side of the cut, there is odd number os set 1 points, then there much be odd number of set 1 points in the other side as well.
        /// And the difference is even, which is composed of points from set 3.
        /// If in the left side of the cut, there is even number os set 1 points, then there much be even number of set 1 points in the other side as well.
        /// And the difference is even, which is composed of points from set 3.
        /// The same proof can be applied for set 2.
        /// Therefore, we are sure that the number of points of set 3 in one both sides of the cut is even.     
        /// </summary>
        /// <param name="D"></param>
        /// <param name="T"></param>
        /// <returns></returns>
        private List<Swap> CalculateSwap(double D, double T, List<DataGenerator.DataPoint> ups, List<DataGenerator.DataPoint> bots)
        {
            var swaps = new List<Swap>();

            // TODO: swaps

            return swaps;
        }

        private double OnTopOfLine(DataGenerator.DataPoint data, Line line)
        {
            return data.Y - (line.D * data.X + line.T);
        }

        private string ModifyColor()
        {
            var set1 = _rawdata.Where(x => x.Color == 1).ToList();
            var set2 = _rawdata.Where(x => x.Color == 2).ToList();
            var set3 = _rawdata.Where(x => x.Color == 3).ToList();

            string message = "";
            if (set1.Count <= set2.Count && set1.Count <= set3.Count)
            {
                message +=  MakeRawDataEvenAmount(set1);
                ChangeColor(set1, GetCenterPoint(set1), 2, 3);
                message += MakeRawDataOddAmount(set2);
                message += MakeRawDataOddAmount(set3);
            }
            if (set2.Count <= set1.Count && set1.Count <= set3.Count)
            {
                message += MakeRawDataEvenAmount(set2);
                ChangeColor(set2, GetCenterPoint(set2), 1, 3);
                message += MakeRawDataOddAmount(set1);
                message += MakeRawDataOddAmount(set3);
            }
            if (set3.Count <= set1.Count && set1.Count <= set2.Count)
            {
                message += MakeRawDataEvenAmount(set3);
                ChangeColor(set3, GetCenterPoint(set3), 1, 2);
                message += MakeRawDataOddAmount(set1);
                message += MakeRawDataOddAmount(set2);
            }

            _rawdata.Clear();
            _rawdata = set1.Union(set2.Union(set3)).ToList();
            return message;
        }

        private void ChangeColor(List<DataGenerator.DataPoint> rawdata, List<double> centerPoint, int color1, int color2)
        {
            var distanceDic = new Dictionary<double, List<DataGenerator.DataPoint>>();
            foreach (var d in rawdata)
            {
                List<DataGenerator.DataPoint> data = null;
                var distance = Math.Pow(d.X - centerPoint[0], 2) + Math.Pow(d.Y - centerPoint[1], 2);
                distanceDic.TryGetValue(distance, out data);
                if (data == null)
                {
                    data = new List<DataGenerator.DataPoint>();
                    data.Add(d);
                    distanceDic.Add(distance, data);
                }
                else
                {
                    data.Add(d);
                }
            }

            var keys = distanceDic.Keys.ToList();
            keys.Sort();

            int counter = 0;
            foreach (var key in keys)
            {
                List<DataGenerator.DataPoint> data = new List<DataGenerator.DataPoint>();
                distanceDic.TryGetValue(key, out data);
                foreach (var d in data)
                {
                    counter++;
                    if (counter < rawdata.Count / 2)
                    {
                        d.ModifiedColor = color1;
                    }
                    else
                    {
                        d.ModifiedColor = color2;
                    }
                }
            }
        }

        private List<double> GetCenterPoint(List<DataGenerator.DataPoint> rawdata)
        {
            double sumx = 0.0;
            double sumy = 0.0;
            rawdata.ForEach(x => { sumx += x.X; sumy += x.Y; });
            return new List<double>() { sumx / rawdata.Count, sumy / rawdata.Count };
        }

        // Assume odd number of points, if not remove one.
        private string MakeRawDataOddAmount(List<DataGenerator.DataPoint> data)
        {
            string msg = "";
            if (data.Count % 2 == 0)
            {
                data.RemoveAt(0);
                msg += data[0].ToString() + " is removed. \r\n";
            }
            return msg;
        }

        // Assume odd number of points, if not remove one.
        private string MakeRawDataEvenAmount(List<DataGenerator.DataPoint> data)
        {
            string msg = "";
            if (data.Count % 2 != 0)
            {
                data.RemoveAt(0);
                msg += data[0].ToString() + " is removed. \r\n";
            }
            return msg;
        }
    }

    class Swap
    {
        public DataGenerator.DataPoint PointA { get; set; }
        public DataGenerator.DataPoint PointB { get; set; }
    }
}
