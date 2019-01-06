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

        private List<DataGenerator.DataPoint> _rawdata = null;
        public List<DataGenerator.DataPoint> RawData { get { return _rawdata; } }

        // y = CutD * x + CutT
        public double CutD { get; private set; }
        public double CutT { get; private set; }
        public List<DataGenerator.DataPoint> OnLines { get; set; }

        public List<Swap> Swaps { get; private set; }
        public string Message { get; private set; }

        public ThreeColorCut(List<DataGenerator.DataPoint> rawdata)
        {
            Swaps = new List<Swap>();
            _rawdata = rawdata;
            //CheckDataValidation();
            ModifyColor();
            CalculateCut();
        }

        private void CheckDataValidation()
        {
            var set1 = _rawdata.Where(x => x.Color == 1).ToList();
            var set2 = _rawdata.Where(x => x.Color == 2).ToList();
            var set3 = _rawdata.Where(x => x.Color == 3).ToList();

            //if (set1.Count < _minimumT || set2.Count < _minimumT || set3.Count < _minimumT)
            //    throw new Exception("ThreeCutAlg: Insufficient fata.");
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
            
            // To be removed
            string msgtoShow = "";

            for (int i = 0; i < cutDs.Count; i++)
            {
                var ups = new List<DataGenerator.DataPoint>();
                var bots = new List<DataGenerator.DataPoint>();
                var ols = new List<DataGenerator.DataPoint>();

                int up1 = 0, ol1 = 0, bot1 = 0;
                int up2 = 0, ol2 = 0, bot2 = 0;
                
                var line = new Line(cutDs[i], cutTs[i], 0);
                foreach (var p in _rawdata)
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

                // To be removed
                msgtoShow += "up1: " + up1 + " bot1: " + bot1 + " up2: " + up2 + " bot2: " + bot2 + " ol1: " + ol1 + " ol2 " + ol2 + "\r\n";

                if (up1 == bot1 && up2 == bot2 && ol1 == 1 && ol2 == 1)
                {
                    var swaps = CalculateSwap(cutDs[i], cutTs[i], ups, bots, ols);

                    // Get the line with minimum swaps.
                    if (swaps.Count < Swaps.Count || Swaps.Count == 0)
                    {
                        CutD = cutDs[i];
                        CutT = cutTs[i];
                        Swaps = swaps;
                        OnLines = ols;
                    }
                }
            }

            Message += msgtoShow;
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
        /// <param name="D"></param>
        /// <param name="T"></param>
        /// <returns></returns>
        private List<Swap> CalculateSwap(double D, double T, List<DataGenerator.DataPoint> ups, List<DataGenerator.DataPoint> bots, List<DataGenerator.DataPoint> ols)
        {
            var swaps = new List<Swap>();

            #region when point whose colored is modified is on the cut

            var _modifiedColorData = ols.Where(x => x.Color != x.ModifiedColor).ToList();
            DataGenerator.DataPoint shad = null;
            var tu = ups.Where(x => x.Color != x.ModifiedColor).ToList();
            var tb = bots.Where(x => x.Color != x.ModifiedColor).ToList();
            if (_modifiedColorData.Count == 1)
            {
                if (tu.Count % 2 != 0)
                {
                    shad = ups.Where(y => y.Color == y.ModifiedColor && y.Color == _modifiedColorData[0].ModifiedColor).First();
                }
                if (tb.Count % 2 != 0)
                {
                    shad = bots.Where(y => y.Color == y.ModifiedColor && y.Color == _modifiedColorData[0].ModifiedColor).First();
                }
                shad.Color = _modifiedColorData[0].Color;
                shad.ModifiedColor = _modifiedColorData[0].ModifiedColor;
                _modifiedColorData[0].Color = _modifiedColorData[0].ModifiedColor;
                swaps.Add(new Swap(_modifiedColorData[0], shad));
            }
            if (_modifiedColorData.Count == 2)
            {
                shad = ups.Where(y => y.Color == y.ModifiedColor && y.Color == _modifiedColorData[0].ModifiedColor).First();
                shad.Color = _modifiedColorData[0].Color;
                shad.ModifiedColor = _modifiedColorData[0].ModifiedColor;
                _modifiedColorData[0].Color = _modifiedColorData[0].ModifiedColor;
                swaps.Add(new Swap(_modifiedColorData[0], shad));

                shad = bots.Where(y => y.Color == y.ModifiedColor && y.Color == _modifiedColorData[1].ModifiedColor).First();
                shad.Color = _modifiedColorData[1].Color;
                shad.ModifiedColor = _modifiedColorData[1].ModifiedColor;
                _modifiedColorData[1].Color = _modifiedColorData[1].ModifiedColor;
                swaps.Add(new Swap(_modifiedColorData[1], shad));
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
                //breakpoint here for testing
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
                //breakpoint here for testing
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

            _rawdata.Clear();
            _rawdata = set1.Union(set2.Union(set3)).ToList();
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
            var set = rawdata.Where(x => x.ModifiedColor != rawdata.First().ModifiedColor).ToList();
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
