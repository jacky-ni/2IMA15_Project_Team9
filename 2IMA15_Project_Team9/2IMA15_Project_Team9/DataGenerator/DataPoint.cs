using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace _2IMA15_Project_Team9.DataGenerator
{
    enum PointColor : int { Blue = 1, Red = 2, Green = 3 }

    class DataPoint
    {
        // Location of the data points.
        public double X { get; set; }
        public double Y { get; set; }
        public int Color { get; set; }

        // Cluster ID.
        public int Cluster_Id { get; set; }

        public DataPoint(double x, double y)
        {
            X = x;
            Y = y;
            Cluster_Id = 0;
        }

        public DataPoint()
        {

        }
    }
}
