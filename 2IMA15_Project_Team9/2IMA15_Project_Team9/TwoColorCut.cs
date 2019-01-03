using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            //var set1 = _rawdata.Where(x => x.Color == 1).ToList();
            //var set2 = _rawdata.Where(x => x.Color == 2).ToList();
            var seg1 = CalculateMiddleLine(_rawdata1);
            var seg2 = CalculateMiddleLine(_rawdata2);
            CheckIntersection(seg1, seg2);
        }

        private void CheckIntersection(List<LineSegment> seg1, List<LineSegment> seg2)
        {
            // Check intersection for the first segment.
            if (seg1[0].Line.D != seg2[0].Line.D)
            {
                var intersec = new Intersection(seg1[0].Line, seg2[0].Line);
                if (intersec.IntersectionPoint.X < Math.Max(seg1[0].Endpoint, seg2[0].Endpoint))
                {
                    Intersections.Add(new Intersection(seg1[0].Line, seg2[0].Line));
                }
            }

            // Check intersection for the last segment.
            if (seg1.Last().Line.D != seg2.Last().Line.D)
            {
                var intersec = new Intersection(seg1.Last().Line, seg2.Last().Line);
                if (intersec.IntersectionPoint.X < Math.Min(seg1.Last().BeginPoint, seg2.Last().BeginPoint))
                {
                    Intersections.Add(new Intersection(seg1.Last().Line, seg2.Last().Line));
                }
            }

            int index = 1;
            for (int i = 1; i < seg1.Count-1; i++)
            {
                for (int j = index; j < seg2.Count; j++)
                {
                    var intersec = new Intersection(seg1[i].Line, seg2[j].Line);
                    if (intersec.IntersectionPoint.X >= Math.Min(seg1[i].BeginPoint, seg2[j].BeginPoint) &&
                        intersec.IntersectionPoint.X <= Math.Min(seg1[i].Endpoint, seg2[j].Endpoint))
                    {
                        Intersections.Add(new Intersection(seg1[i].Line, seg2[j].Line));
                    }
                    if (seg1[i].Endpoint > seg2[j].Endpoint)
                    {
                        index++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        // Generate the middle line segments
        private List<LineSegment> CalculateMiddleLine(List<DataGenerator.DataPoint> data)
        {
            var segments = new List<LineSegment>();

            var lines = new List<Line>();
            int id = 1;
            foreach (var p in data)
            {
                lines.Add(new Line((float)p.X, -(float)p.Y, id));
                id++;
            }

            // All intersections.
            var intersections = GenerateIntersections(lines);

            // Sweep line, starting with x= left most intersection's x value -1
            float sweepLine = float.MaxValue;
            foreach (var intersec in intersections)
            {
                if (sweepLine > intersec.IntersectionPoint.X)
                    sweepLine = intersec.IntersectionPoint.X;
            }
            sweepLine -= 1;

            var initialIntersections = new Dictionary<float, Line>();
            foreach (var l in lines)
            {
                initialIntersections.Add(l.D * sweepLine + l.T, l);
            }

            var keys = initialIntersections.Keys.ToList();
            keys.Sort();
            int ranker = 0;
            foreach (var key in keys)
            {
                Line l = null;
                initialIntersections.TryGetValue(key, out l);
                ranker += 1;
                l.Rank = ranker;
            }

            var line = lines.Find(x => x.Rank == lines.Count / 2);
            segments.Add(new LineSegment(line, float.MinValue, intersections[0].IntersectionPoint.X));
            for (int i = 1; i < intersections.Count; i++)
            {
                var temp = intersections[i].Line1.Rank;
                intersections[i].Line1.Rank = intersections[i].Line2.Rank;
                intersections[i].Line2.Rank = temp;

                var l = lines.Find(x => x.Rank == lines.Count / 2);
                if (segments.Last().Line != l)
                    segments.Add(new LineSegment(l, intersections[i - 1].IntersectionPoint.X, intersections[i].IntersectionPoint.X));
                else
                    segments.Last().Endpoint = intersections[i].IntersectionPoint.X;
            }
            segments.Last().Endpoint = float.MaxValue;

            return segments;
        }

        private List<Intersection> GenerateIntersections(List<Line> lines)
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
                        //if (intersec.IntersectionPoint.X <= _width && intersec.IntersectionPoint.X >= 0
                        //    && intersec.IntersectionPoint.Y >= 0 && intersec.IntersectionPoint.Y <= _height)
                        //{
                            intersections.Add(intersec);
                        //}
                    }
                }
            }
            intersections.Sort();

            return intersections;
        }
    }

    class Line
    {
        public int ID { get; private set; }

        public float D { get; private set; }
        public float T { get; private set; }

        //public Intersection IntersectionWithInitialSweepLine { get; set; }
        public int Rank { get; set; }

        public Line(float d, float t, int id)
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
        public PointF IntersectionPoint { get; private set; }

        public Intersection(Line l1, Line l2)
        {
            Line1 = l1;
            Line2 = l2;

            CalculateIntersection();
        }

        public Intersection(PointF intersection)
        {
            IntersectionPoint = intersection;
        }

        private void CalculateIntersection()
        {
            float x = (Line2.T - Line1.T) / (Line1.D - Line2.D);
            float y = x * Line1.D + Line1.T;
            IntersectionPoint = new PointF(x, y);
        }

        public int CompareTo(object obj)
        {
            return this.IntersectionPoint.X.CompareTo(((Intersection)obj).IntersectionPoint.X);
        }
    }

    class LineSegment
    {
        public Line Line { get; private set; }

        // Domain interval
        public float BeginPoint { get; set; }
        public float Endpoint { get; set; }

        public LineSegment(Line l, float b, float e)
        {
            Line = l;
            BeginPoint = b;
            Endpoint = e;
        }
    }
}
