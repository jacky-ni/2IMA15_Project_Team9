using System;
using System.Collections.Generic;
using System.Linq;

namespace _2IMA15_Project_Team9
{
    class TwoColorCut
    {
        private List<DataGenerator.DataPoint> _rawdata1 = null;
        private List<DataGenerator.DataPoint> _rawdata2 = null;
        public List<Intersection> Intersections { get; private set; }

        public TwoColorCut(List<DataGenerator.DataPoint> rawdata1, List<DataGenerator.DataPoint> rawdata2)
        {
            Intersections = new List<Intersection>();
            _rawdata1 = rawdata1;
            _rawdata2 = rawdata2;
            CalculateCut();
        }

        private void CalculateCut()
        {
            var seg1 = CalculateMiddleLineSegs(_rawdata1);
            var seg2 = CalculateMiddleLineSegs(_rawdata2);
            CalculateIntersectionBetweenSegs(seg1, seg2);
        }

        private void CalculateIntersectionBetweenSegs(List<LineSegment> seg1, List<LineSegment> seg2)
        {
            // Brutal alg, used for verification.
            //foreach (var sg1 in seg1)
            //{
            //    foreach (var sg2 in seg2)
            //    {
            //        var intersec = new Intersection(sg1.Line, sg2.Line);
            //        if (intersec.IntersectionPointX > Math.Max(sg1.BeginPoint, sg2.BeginPoint) &&
            //            intersec.IntersectionPointX < Math.Min(sg1.Endpoint, sg2.Endpoint))
            //        {
            //            Intersections.Add(new Intersection(sg1.Line, sg2.Line));
            //        }
            //    }
            //}

            int index = 0;
            for (int i = 0; i < seg1.Count; i++)
            {
                for (int j = index; j < seg2.Count; j++)
                {
                    var intersec = new Intersection(seg1[i].Line, seg2[j].Line);
                    if (intersec.IntersectionPointX > Math.Max(seg1[i].BeginPoint, seg2[j].BeginPoint) &&
                        intersec.IntersectionPointX < Math.Min(seg1[i].Endpoint, seg2[j].Endpoint))
                    {
                        Intersections.Add(new Intersection(seg1[i].Line, seg2[j].Line));
                    }
                    if (seg1[i].Endpoint >= seg2[j].Endpoint)
                    {
                        index += 1;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        // Calculate the middle line segments
        private List<LineSegment> CalculateMiddleLineSegs(List<DataGenerator.DataPoint> data)
        {
            var segments = new List<LineSegment>();

            var lines = new List<Line>();
            int id = 1;
            foreach (var p in data)
            {
                lines.Add(new Line((double)p.X, -(double)p.Y, id));
                id++;
            }

            // All intersections.
            var intersections = CalculateIntersections(lines);
            intersections = intersections.OrderBy(x => x.IntersectionPointX).ThenBy(y => y.IntersectionPointY).ToList();

            // Sweep line, starting with x= left most intersection's x value -1
            double sweepLine = double.MaxValue;
            foreach (var intersec in intersections)
            {
                if (sweepLine > intersec.IntersectionPointX)
                    sweepLine = intersec.IntersectionPointX;
            }
            sweepLine -= 1;

            var initialIntersections = new Dictionary<double, Line>();
            foreach (var l in lines)
            {
                initialIntersections.Add(l.D * sweepLine + l.T, l);
            }

            var keys = initialIntersections.Keys.ToList();
            keys.Sort();
            int ranker = 1;
            foreach (var key in keys)
            {
                Line l = null;
                initialIntersections.TryGetValue(key, out l);
                l.Rank = ranker;
                ranker += 1;
            }

            var middleLine = lines.Find(x => x.Rank == lines.Count / 2 + 1);
            segments.Add(new LineSegment(middleLine, double.MinValue, intersections[0].IntersectionPointX));

            for (int i = 0; i < intersections.Count; i++)
            {
                // When three or more lines intersect at one point
                var tempIntersecs = new List<Intersection>();
                if (i + 1 < intersections.Count)
                {
                    while ((intersections[i].IntersectionPointX == intersections[i + 1].IntersectionPointX)
                        && (intersections[i].IntersectionPointY == intersections[i + 1].IntersectionPointY))
                    {
                        if (!tempIntersecs.Contains(intersections[i]))
                            tempIntersecs.Add(intersections[i]);

                        i += 1;

                        if (!tempIntersecs.Contains(intersections[i]))
                            tempIntersecs.Add(intersections[i]);

                        // Reach the last element
                        if (i == intersections.Count) break;
                    }
                }

                var tlines = new List<Line>();
                if (tempIntersecs.Count == 0)
                {
                    var temp = intersections[i].Line1.Rank;
                    intersections[i].Line1.Rank = intersections[i].Line2.Rank;
                    intersections[i].Line2.Rank = temp;

                    if (intersections[i].Line1 == middleLine)
                    {
                        middleLine = intersections[i].Line2;
                    }
                    else if (intersections[i].Line2 == middleLine)
                    {
                        middleLine = intersections[i].Line1;
                    }
                }
                // When three or more lines intersect at one point
                else
                {
                    foreach (var t in tempIntersecs)
                    {
                        if (!tlines.Contains(t.Line1)) tlines.Add(t.Line1);
                        if (!tlines.Contains(t.Line2)) tlines.Add(t.Line2);
                    }
                    tlines = tlines.OrderBy(x => x.Rank).ToList();
                    SortMultipleLineIntersection(tlines, intersections[i].IntersectionPointX);

                    if (tlines.Contains(middleLine))
                    {
                        middleLine = tlines.Find(x => x.Rank == lines.Count / 2 + 1);
                    }
                }

                // middleLine = lines.Find(x => x.Rank == lines.Count / 2 + 1);
                if (segments.Last().Line != middleLine)
                {
                    if (segments.Last() != null)
                    {
                        segments.Last().Endpoint = intersections[i].IntersectionPointX;
                    }
                    segments.Add(new LineSegment(middleLine, intersections[i].IntersectionPointX, intersections[i].IntersectionPointX));
                }
            }
            
            segments.Last().Endpoint = double.MaxValue;
            var seg = segments.FindAll(x => x.BeginPoint == x.Endpoint);
            segments.RemoveAll(x => x.BeginPoint == x.Endpoint);

            return segments;
        }

        private void SortMultipleLineIntersection(List<Line> lines, double intersectionX)
        {
            var dic = new Dictionary<double, Line>();
            foreach (var l in lines)
            {
                dic.Add((intersectionX - 1) * l.D + l.T, l);
            }
            var keys = dic.Keys.ToList();
            keys.Sort();

            for (int i = 0; i < keys.Count / 2; i++)
            {
                Line l1 = null;
                dic.TryGetValue(keys[i], out l1);
                Line l2 = null;
                dic.TryGetValue(keys[keys.Count - 1 - i], out l2);

                int rank = l1.Rank;
                l1.Rank = l2.Rank;
                l2.Rank = rank;
            }
        }

        private List<Intersection> CalculateIntersections(List<Line> lines)
        {
            var intersections = new List<Intersection>();

            foreach (var l1 in lines)
            {
                foreach (var l2 in lines)
                {
                    // not parallel, and prevent duplication
                    if (l1.D != l2.D && l1.ID < l2.ID)
                    {
                        var intersec = new Intersection(l1, l2);
                        intersections.Add(intersec);
                    }
                }
            }

            return intersections;
        }
    }

    class Line
    {
        public int ID { get; private set; }

        public double D { get; private set; }
        public double T { get; private set; }
        
        public int Rank { get; set; }

        public Line(double d, double t, int id)
        {
            ID = id;
            D = d;
            T = t;
        }
    }

    class Intersection : IComparable
    {
        public Line Line1 { get; private set; }
        public Line Line2 { get; private set; }
        public double IntersectionPointX { get; private set; }
        public double IntersectionPointY { get; private set; }

        public Intersection(Line l1, Line l2)
        {
            Line1 = l1;
            Line2 = l2;

            CalculateIntersection();
        }

        public Intersection(double intersectionX, double intersectionY)
        {
            IntersectionPointX = intersectionX;
            IntersectionPointY = intersectionY;
        }

        private void CalculateIntersection()
        {
            double x = (Line2.T - Line1.T) / (Line1.D - Line2.D);
            double y = x * Line1.D + Line1.T;
            IntersectionPointX = x;
            IntersectionPointY = y;
        }

        public int CompareTo(object obj)
        {
            var byX = IntersectionPointX.CompareTo(((Intersection)obj).IntersectionPointX);
            return byX;
        }
    }

    class LineSegment
    {
        public Line Line { get; private set; }

        // Domain interval
        public double BeginPoint { get; set; }
        public double Endpoint { get; set; }

        public LineSegment(Line l, double b, double e)
        {
            Line = l;
            BeginPoint = b;
            Endpoint = e;
        }
    }
}
