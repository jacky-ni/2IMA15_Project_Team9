using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2IMA15_Project_Team9
{
    class ThreeColorCut
    {
        // error threshold.
        // For instance, if we have y=1/3 *x +2.
        // And if we have a point (3,3), the computer would say 2.9999999999999999... is less than 3.
        // So the point is below y=1/3 *x +2 while actually it should be on the line.
        private double _errorT = Math.Pow(10, -10);

        // Minimum points required for each color.
        //private int _minimumT = 2;
        
        private List<DataGenerator.DataPoint> _rawData = null;
        private List<DataGenerator.DataPoint> _rawDataBackUp = null;
        public List<DataGenerator.DataPoint> RawDataBackUp { get { return _rawDataBackUp; } }
        public List<DataGenerator.DataPoint> RawData { get { return _rawData; } }

        // y = CutD * x + CutT
        public double CutD { get; private set; }
        public double CutT { get; private set; }

        private List<double> cutDs = new List<double>();
        private List<double> cutTs = new List<double>();

        public List<DataGenerator.DataPoint> OnLines { get; set; }

        public List<Swap> Swaps { get; private set; }
        public string Message { get; private set; }

        public ThreeColorCut(List<DataGenerator.DataPoint> rawdata)
        {
            Swaps = new List<Swap>();

            _rawData = rawdata;
            _rawDataBackUp = new List<DataGenerator.DataPoint>();
            _rawData.ForEach(x =>
            {
                _rawDataBackUp.Add((DataGenerator.DataPoint)x.Clone());
            });
            
            ModifyColor();
            // Calculate two color cut and swap.
            CalculateCut();
            DisplayInformation();
        }
        
        private void CalculateCut()
        {
            var set1 = _rawData.FindAll(x => x.ModifiedColor == 1);
            var set2 = new List<DataGenerator.DataPoint>();
            if (set1.Count == 0)
            {
                set1 = _rawData.FindAll(x => x.ModifiedColor == 2);
                set2 = _rawData.FindAll(x => x.ModifiedColor == 3);
            }
            else
            {
                set2 = _rawData.FindAll(x => x.ModifiedColor == 2);
                if (set2.Count == 0)
                {
                    set2 = _rawData.FindAll(x => x.ModifiedColor == 3);
                }
            }

            TwoColorCut tc = new TwoColorCut(set1, set2);

            foreach (var t in tc.Intersections)
            {
                cutDs.Add(t.IntersectionPointX);
                cutTs.Add(-t.IntersectionPointY);
            }
            
            var ups = new List<DataGenerator.DataPoint>();
            var bots = new List<DataGenerator.DataPoint>();
            var ols = new List<DataGenerator.DataPoint>();

            for (int i = 0; i < cutDs.Count; i++)
            {
                ups.Clear();
                bots.Clear();
                ols.Clear();

                int up1 = 0, ol1 = 0, bot1 = 0;
                int up2 = 0, ol2 = 0, bot2 = 0;

                var line = new Line(cutDs[i], cutTs[i], 0);
                foreach (var p in _rawData)
                {
                    double r = OnTopOfLine(p, line);

                    // This is where the error happens.
                    if (Math.Abs(r) < _errorT)
                    {
                        r = 0;
                    }

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
                            ols.Add(p);
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
                            ols.Add(p);
                            ol2++;
                        }
                    }
                }

                // It should always holds, but due to the possible error in 2 cut, we need this as an extra verification.
                if (up1 == bot1 && up2 == bot2 && ol1 == ol2)
                {
                    var swaps = CalculateSwap(cutDs[i], cutTs[i], ups, bots, ols);

                    // Get the line with minimum swaps.
                    if (i == 0)
                    {
                        CutD = cutDs[i];
                        CutT = cutTs[i];
                        Swaps = swaps;
                        OnLines = ols;
                    }
                    else
                    {
                        if (swaps.Count < Swaps.Count)
                        {
                            CutD = cutDs[i];
                            CutT = cutTs[i];
                            Swaps = swaps;
                            OnLines = ols;
                        }
                    }
                }
                else
                {
                    bool stop = true;
                }
            }
        }

        private void DisplayInformation()
        {
            Message += "\r\nThose two points is the cut.\r\n";
            foreach (var l in OnLines)
            {
                Message += l.ToString() + "\r\n";
            }

            foreach (var s in Swaps)
            {
                s.ExecuteSwap(_rawData);
            }

            int rups = 0, rbots = 0, bups = 0, bbots = 0, gups = 0, gbots = 0;
            var teline = new Line(CutD, CutT, 0);
            
            foreach (var d in _rawData)
            {
                double r = OnTopOfLine(d, teline);

                // This is where the error happens.
                if (Math.Abs(r) < _errorT)
                {
                    r = 0;
                }

                if (d.Color == 1)
                {
                    if (r > 0)
                    {
                        rups++;
                    }
                    else if (r < 0)
                    {
                        rbots++;
                    }
                }
                else if (d.Color == 2)
                {
                    if (r > 0)
                    {
                        bups++;
                    }
                    else if (r < 0)
                    {
                        bbots++;
                    }
                }
                else if (d.Color == 3)
                {
                    if (r > 0)
                    {
                        gups++;
                    }
                    else if (r < 0)
                    {
                        gbots++;
                    }
                }
            }

            Message += "Number of Red points in one side: " + rups +
                "\r\nNumber of Red points in another side: " + rbots +
                "\r\nNumber of Blue points in one side: " + bups +
                "\r\nNumber of Blue points in another side: " + bbots +
                "\r\nNumber of Green points in one side: " + gups +
                "\r\nNumber of Green points in another side: " + gbots + "\r\n";
            
            Message += "\r\nThere are in total " + Swaps.Count + " swaps: \r\n";
            foreach (var sw in Swaps)
            {
                Message += sw.ToString();
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
        /// <param name="D">y=D*x+T</param>
        /// <param name="T">y=D*x+T</param>
        /// <returns>list of swaps</returns>
        private List<Swap> CalculateSwap(double D, double T, List<DataGenerator.DataPoint> ups, List<DataGenerator.DataPoint> bots, List<DataGenerator.DataPoint> ols)
        {
            var swaps = new List<Swap>();

            #region when point whose color is modified is on the cut

            var _modifiedColorData = ols.Where(x => x.Color != x.ModifiedColor).ToList();
            DataGenerator.DataPoint shad = null;
            if (_modifiedColorData.Count == 1)
            {
                var tu = ups.Where(x => x.Color != x.ModifiedColor).ToList();
                var tb = bots.Where(x => x.Color != x.ModifiedColor).ToList();
                var p = ols.Find(x => x.Color != x.ModifiedColor);

                if (tu.Count % 2 != 0)
                {
                    shad = ups.FindAll(y => y.Color == y.ModifiedColor && y.Color == p.ModifiedColor).First();
                    ups.Remove(shad);
                    ups.Add(p);
                }
                if (tb.Count % 2 != 0)
                {
                    shad = bots.FindAll(y => y.Color == y.ModifiedColor && y.Color == p.ModifiedColor).First();
                    bots.Remove(shad);
                    bots.Add(p);
                }
                
                swaps.Add(new Swap(p, shad));
                ols.RemoveAt(0);
                ols.Add(shad);
            }
            if (_modifiedColorData.Count == 2)
            {
                var p1 = ols.FindAll(x => x.Color != x.ModifiedColor).First();
                var p2 = ols.FindAll(x => x.Color != x.ModifiedColor).Last();

                shad = ups.Where(y => y.Color == y.ModifiedColor && y.Color == ols[0].ModifiedColor).First();
                ups.Remove(shad);
                ups.Add(p1);
                swaps.Add(new Swap(ols[0], shad));
                ols.RemoveAt(0);
                ols.Add(shad);

                shad = bots.Where(y => y.Color == y.ModifiedColor && y.Color == ols[0].ModifiedColor).First();
                bots.Remove(shad);
                bots.Add(p2);
                
                swaps.Add(new Swap(ols[0], shad));
                ols.RemoveAt(0);
                ols.Add(shad);
            }

            #endregion
            
            // For example, set1Color = r, set2Color = b, set3Color = g
            var set1Color = 0;
            var set2Color = 0;

            try
            {
                // In case the ups contains only two colors points, which will give "empty sequence exception when calaulating set2Color
                set1Color = ups.Where(x => x.Color == x.ModifiedColor).First().Color;
                set2Color = ups.Where(x => x.Color == x.ModifiedColor && x.Color != set1Color).First().Color;
            }
            catch (Exception)
            {
                set1Color = bots.Where(x => x.Color == x.ModifiedColor).First().Color;
                set2Color = bots.Where(x => x.Color == x.ModifiedColor && x.Color != set1Color).First().Color;
            }
            
            // green being red on top side
            var set3BeSet1ColorInUps = ups.FindAll(x => x.Color != x.ModifiedColor && x.ModifiedColor == set1Color);
            // green being blue on top side
            var set3BeSet2ColorInUps = ups.FindAll(x => x.Color != x.ModifiedColor && x.ModifiedColor == set2Color);

            // green being red on bot side
            var set3BeSet1InBots = bots.FindAll(x => x.Color != x.ModifiedColor && x.ModifiedColor == set1Color);
            // green being blue on bot side
            var set3BeSet2InBots = bots.FindAll(x => x.Color != x.ModifiedColor && x.ModifiedColor == set2Color);

            // process set3BeSetColor1
            var setToBeSwapedA = new List<DataGenerator.DataPoint>();
            var setToBeSwapedB = new List<DataGenerator.DataPoint>();
            if (set3BeSet1ColorInUps.Count > set3BeSet1InBots.Count)
            {
                setToBeSwapedA = set3BeSet1ColorInUps;
                setToBeSwapedB = bots.FindAll(x => x.Color == set1Color);
            }
            else
            {
                setToBeSwapedA = set3BeSet1InBots;
                setToBeSwapedB = ups.FindAll(x => x.Color == set1Color);
            }
            // differ should always be even
            var differ = Math.Abs(set3BeSet1ColorInUps.Count - set3BeSet1InBots.Count);
            if (differ % 2 != 0)
            {
                // Should never reach here.
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
                setToBeSwapedB = bots.FindAll(x => x.Color == set2Color);
            }
            else
            {
                setToBeSwapedA = set3BeSet2InBots;
                setToBeSwapedB = ups.FindAll(x => x.Color == set2Color);
            }
            // differ should always be even
            differ = Math.Abs(set3BeSet2ColorInUps.Count - set3BeSet2InBots.Count);
            if (differ % 2 != 0)
            {
                // Should never reach here.
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

            var set1 = _rawData.Where(x => x.Color == 1).ToList();
            var set2 = _rawData.Where(x => x.Color == 2).ToList();
            var set3 = _rawData.Where(x => x.Color == 3).ToList();

            if (set1.Count <= set2.Count && set1.Count <= set3.Count)
            {
                Message += MakeRawDataAmountDivisibleBy4(set1);
                ChangeColor(set1, GetCenterPoint(set1), 2, 3);

                if ((set2.Count + set1.Where(x => x.ModifiedColor == 2).ToList().Count) % 2 == 0)
                    Message += MakeRawDataOddAmount(set2);
                if ((set3.Count + set1.Where(x => x.ModifiedColor == 3).ToList().Count) % 2 == 0)
                    Message += MakeRawDataOddAmount(set3);
            }
            else if (set2.Count <= set1.Count && set2.Count <= set3.Count)
            {
                Message += MakeRawDataAmountDivisibleBy4(set2);
                ChangeColor(set2, GetCenterPoint(set2), 1, 3);

                if ((set1.Count + set2.Where(x => x.ModifiedColor == 1).ToList().Count) % 2 == 0)
                    Message += MakeRawDataOddAmount(set1);
                if ((set3.Count + set2.Where(x => x.ModifiedColor == 3).ToList().Count) % 2 == 0)
                    Message += MakeRawDataOddAmount(set3);
            }
            else if (set3.Count <= set1.Count && set3.Count <= set2.Count)
            {
                Message += MakeRawDataAmountDivisibleBy4(set3);
                ChangeColor(set3, GetCenterPoint(set3), 1, 2);

                if ((set1.Count + set3.Where(x => x.ModifiedColor == 1).ToList().Count) % 2 == 0)
                    Message += MakeRawDataOddAmount(set1);
                if ((set2.Count + set3.Where(x => x.ModifiedColor == 2).ToList().Count) % 2 == 0)
                    Message += MakeRawDataOddAmount(set2);
            }

            _rawData.Clear();
            _rawData = set1.Union(set2.Union(set3)).ToList();
        }

        private void ChangeColor(List<DataGenerator.DataPoint> rawdata, List<double> centerPoint, int color1, int color2)
        {
            // calculate distance
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
                    if (counter < rawdata.Count / 2)
                    {
                        d.ModifiedColor = color1;
                    }
                    else
                    {
                        d.ModifiedColor = color2;
                    }
                    counter++;
                }
            }

            // Make sure number of points whose color is modified to other two sets are both even
            // so that the number of points of the other two sets is still odd, which satisifies the requirement of two colors cut Alg
            var set = rawdata.FindAll(x => x.ModifiedColor != rawdata.First().ModifiedColor);
            if (set.Count % 2 != 0)
            {
                rawdata.Where(x => x.ModifiedColor != rawdata.First().ModifiedColor).First().ModifiedColor = rawdata.First().ModifiedColor;
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

        // Assume even number of points, if not remove one.
        private string MakeRawDataAmountDivisibleBy4(List<DataGenerator.DataPoint> data)
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

        public Swap(DataGenerator.DataPoint ina, DataGenerator.DataPoint inb)
        {
            PointA = ina;
            PointB = inb;
        }

        public void ExecuteSwap(List<DataGenerator.DataPoint> rawdata)
        {
            var a = rawdata.Find(x => x.ID == PointA.ID);
            var b = rawdata.Find(x => x.ID == PointB.ID);
            var tax = a.X;
            var tay = a.Y;

            // ID follows the position
            var aid = a.ID;
            a.X = b.X;
            a.Y = b.Y;
            a.ID = b.ID;
            b.X = tax;
            b.Y = tay;
            b.ID = aid;

            a.Swapped = true;
            b.Swapped = true;
        }

        public override string ToString()
        {
            return "Point (" + PointA.X + ", " + PointA.Y + ") Color: " + (DataGenerator.PointColor)PointA.Color + ", will be swapped with\r\n" +
                "Point (" + PointB.X + ", " + PointB.Y + ") Color: " + (DataGenerator.PointColor)PointB.Color + " \r\n";
        }
    }
}
