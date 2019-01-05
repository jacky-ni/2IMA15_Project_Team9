using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2IMA15_Project_Team9
{
    class ThreeColorCut
    {
        private double _errorT = Math.Pow(10, -10);

        private List<DataGenerator.DataPoint> _rawdata = null;
        public List<DataGenerator.DataPoint> RawData { get { return _rawdata; } }

        // y = CutD * x + CutT
        public double CutD { get; private set; }
        public double CutT { get; private set; }
        public List<Swap> Swaps { get; private set; }
        public string Message { get; private set; }

        public ThreeColorCut(List<DataGenerator.DataPoint> rawdata)
        {
            Swaps = new List<Swap>();
            _rawdata = rawdata;
            ModifyColor();
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
                if (set2.Count == 0)
                {
                    set2 = _rawdata.Where(x => x.ModifiedColor == 3).ToList();
                }
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
            

            string msgtoShow = "";


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

                    // This is where the error happens.
                    // For instance, if we have y=1/3 *x +2.
                    // And if we have a point (3,3), the computer would say 2.9999999999999999... is less than 3.
                    // So the point is below y=1/3 *x +2 while actually it should be on the line.
                    if (Math.Abs(r) < _errorT)
                    {
                        r = 0;
                    }

                    //if (Math.Abs(r) > 0 && Math.Abs(r) < 0.1)
                    //{
                    //    bool stop = true;
                    //}

                    if (p.ModifiedColor == set1.First().ModifiedColor)
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
                    if (p.ModifiedColor == set2.First().ModifiedColor)
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

                msgtoShow += "up1: " + up1 + " bot1: " + bot1 + " up2: " + up2 + " bot2: " + bot2 + " ol1: " + ol1 + " ol2 " + ol2 + "\r\n";

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

                Message += "\r\nThere are in total " + Swaps.Count + " swaps: \r\n";
                foreach (var sw in Swaps)
                {
                    Message += sw.ToString();
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
            // For example, set1Color = r, set2Color = b, set3Color = g
            var set1Color = ups.Where(x => x.Color == x.ModifiedColor).First().Color;
            var set2Color = ups.Where(x => x.Color == x.ModifiedColor && x.Color != set1Color).First().Color;

            // green being red on top side
            var set3BeSet1ColorInUps = ups.Where(x => x.Color != x.ModifiedColor && x.ModifiedColor == set1Color).ToList();
            // green being blue on top side
            var set3BeSet2ColorInUps = ups.Where(x => x.Color != x.ModifiedColor && x.ModifiedColor == set2Color).ToList();

            // green being red on bot side
            var set3BeSet1InBots = bots.Where(x => x.Color != x.ModifiedColor && x.ModifiedColor == set1Color).ToList();
            // green being blue on bot side
            var set3BeSet2InBots = bots.Where(x => x.Color != x.ModifiedColor && x.ModifiedColor == set2Color).ToList();

            // process set3BeSetColor1
            var setToBeSwapedA = new List<DataGenerator.DataPoint>();
            var setToBeSwapedB = new List<DataGenerator.DataPoint>();
            if (set3BeSet1ColorInUps.Count > set3BeSet1InBots.Count)
            {
                setToBeSwapedA = set3BeSet1ColorInUps;
                setToBeSwapedB = bots.Where(x => x.Color == set1Color).ToList();
            }
            else
            {
                setToBeSwapedA = set3BeSet1InBots;
                setToBeSwapedB = ups.Where(x => x.Color == set1Color).ToList();
            }
            // differ should always be even
            var differ = Math.Abs(set3BeSet1ColorInUps.Count - set3BeSet1InBots.Count);
            if (differ % 2 != 0)
            {
                bool stop = true;
            }
            for (int i = 0; i < differ / 2; i++)
            {
                var swap = new Swap(setToBeSwapedA[i], setToBeSwapedB[i]);
                swaps.Add(swap);
            }

            // Similiar process for set3BeSetColor2
            setToBeSwapedA = new List<DataGenerator.DataPoint>();
            setToBeSwapedB = new List<DataGenerator.DataPoint>();
            if (set3BeSet2ColorInUps.Count > set3BeSet2InBots.Count)
            {
                setToBeSwapedA = set3BeSet2ColorInUps;
                setToBeSwapedB = bots.Where(x => x.Color == set2Color).ToList();
            }
            else
            {
                setToBeSwapedA = set3BeSet2InBots;
                setToBeSwapedB = ups.Where(x => x.Color == set2Color).ToList();
            }
            // differ should always be even
            differ = Math.Abs(set3BeSet2ColorInUps.Count - set3BeSet2InBots.Count);
            if (differ % 2 != 0)
            {
                bool stop = true;
            }
            for (int i = 0; i < differ / 2; i++)
            {
                var swap = new Swap(setToBeSwapedA[i], setToBeSwapedB[i]);
                swaps.Add(swap);
            }

            return swaps;
        }

        private double OnTopOfLine(DataGenerator.DataPoint data, Line line)
        {
            return data.Y - (line.D * data.X + line.T);
        }

        private void ModifyColor()
        {
            Message += "\r\nThe Following points are removed: \r\n";

            var set1 = _rawdata.Where(x => x.Color == 1).ToList();
            var set2 = _rawdata.Where(x => x.Color == 2).ToList();
            var set3 = _rawdata.Where(x => x.Color == 3).ToList();
            
            if (set1.Count <= set2.Count && set1.Count <= set3.Count)
            {
                ChangeColor(set1, GetCenterPoint(set1), 2, 3);
                Message +=  MakeRawDataEvenAmount(set1);
                Message += MakeRawDataOddAmount(set2);
                Message += MakeRawDataOddAmount(set3);
            }
            else if (set2.Count <= set1.Count && set2.Count <= set3.Count)
            {
                ChangeColor(set2, GetCenterPoint(set2), 1, 3);
                Message += MakeRawDataEvenAmount(set2);
                Message += MakeRawDataOddAmount(set1);
                Message += MakeRawDataOddAmount(set3);
            }
            else if (set3.Count <= set1.Count && set3.Count <= set2.Count)
            {
                ChangeColor(set3, GetCenterPoint(set3), 1, 2);
                Message += MakeRawDataEvenAmount(set3);
                Message += MakeRawDataOddAmount(set1);
                Message += MakeRawDataOddAmount(set2);
            }

            _rawdata.Clear();
            _rawdata = set1.Union(set2.Union(set3)).ToList();
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
        public DataGenerator.DataPoint PointA { get;private set; }
        public DataGenerator.DataPoint PointB { get;private set; }

        public Swap(DataGenerator.DataPoint a, DataGenerator.DataPoint b)
        {
            PointA = a;
            PointB = b;
        }

        public override string ToString()
        {
            return "Point (" + PointA.X + ", " + PointA.Y + ") Color: " + (DataGenerator.PointColor)PointA.Color + ", will be swapped with\r\n" +
                "Point (" + PointB.X + ", " + PointB.Y + ") Color: " + (DataGenerator.PointColor)PointB.Color + " \r\n";
        }
    }
}
