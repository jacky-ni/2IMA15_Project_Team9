namespace _2IMA15_Project_Team9.DataGenerator
{
    enum PointColor : int { Blue = 1, Red = 2, Green = 3 }

    class DataPoint
    {
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
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return "Position: (X: " + X + ", " + "Y: " + Y + "), Color: " + (PointColor)color;
        }
    }
}
