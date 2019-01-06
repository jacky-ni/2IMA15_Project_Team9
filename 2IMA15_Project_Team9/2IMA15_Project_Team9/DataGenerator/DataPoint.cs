using System;

namespace _2IMA15_Project_Team9.DataGenerator
{
    enum PointColor : int { Blue = 1, Red = 2, Green = 3 }

    class DataPoint : ICloneable
    {
        private int iD;
        public int ID
        {
            get
            {
                return iD;
            }
            set
            {
                iD = value;
                //ModifiedID = iD;
            }
        }

        //public int ModifiedID { get; set; }

        public bool Swapped { get; set; }

        // Location of the data points.
        public double X { get; set; }
        public double Y { get; set; }

        // Cluster ID.
        private int color;
        public int Color
        {
            get { return color; }
            set
            {
                color = value;
                ModifiedColor = color;
            }
        }
        public int ModifiedColor { get; set; }

        public DataPoint(double x, double y)
        {
            Swapped = false;
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return "Position: (X: " + X + ", " + "Y: " + Y + "), Color: " + (PointColor)color;
        }

        public object Clone()
        {
            var p = new DataPoint(X, Y);
            p.Color = color;
            p.Swapped = false;
            p.ID = iD;
            return p;
        }
    }
}
