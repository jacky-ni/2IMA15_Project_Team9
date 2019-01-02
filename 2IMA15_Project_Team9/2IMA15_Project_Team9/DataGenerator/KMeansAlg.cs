using System;
using System.Collections.Generic;
using System.Linq;

namespace _2IMA15_Project_Team9.DataGenerator
{
    class KMeansAlg
    {
        private List<DataPoint> _rawData = new List<DataPoint>();
        private List<DataPoint> _normalizedData = new List<DataPoint>();
        private List<DataPoint> _clusters = new List<DataPoint>();
        private int _numberOfClusters = 0;

        public KMeansAlg(int numberOfClusters, List<DataPoint> rawData)
        {
            _numberOfClusters = numberOfClusters;
            _rawData = rawData;
        }

        private void Normalization()
        {
            var xSum = 0.0;
            var ySum = 0.0;
            _rawData.ForEach(x => xSum += x.X);
            _rawData.ForEach(x => ySum += x.Y);

            var xMean = xSum / _rawData.Count;
            var yMean = ySum / _rawData.Count;

            var t_xSum = 0.0;
            var t_ySum = 0.0;

            _rawData.ForEach(x => t_xSum += Math.Pow(x.X - xMean, 2));
            _rawData.ForEach(x => t_ySum += Math.Pow(x.Y - yMean, 2));
            
            var heightSD = t_xSum / _rawData.Count;
            var weightSD = t_ySum / _rawData.Count;

            _rawData.ForEach(x => _normalizedData.Add(new DataPoint((x.X - xMean) / heightSD, (x.Y - yMean) / weightSD)));
        }

        public void InitializeCentroids(List<DataPoint> centroids)
        {
            for (int i = 0; i < centroids.Count; i++)
            {
                centroids[i].Cluster_Id = i;
                _clusters.Add(centroids[i]);
            }
        }

        public void Cluster()
        {
            Normalization();
            Initialize();

            bool changed = true;
            bool success = true;
            int maxIteration = _normalizedData.Count * 10;
            int counter = 0;

            // Maximum iteration is 10*n times.
            while (success == true && changed == true && counter < maxIteration)
            {
                counter ++;
                success = UpdateDataMeans();
                changed = UpdateCluster();
            }
            Console.WriteLine("K-means takes " + counter + " iterations in total.");
        }
        
        private void Initialize()
        {
            Random r = new Random();
            // Make sure that each there are at least one element in each cluster.
            for (int i = 0; i < _numberOfClusters; i++)
            {
                _normalizedData[i].Cluster_Id = i;
                _rawData[i].Cluster_Id = i;
            }
            for (int i = _numberOfClusters; i < _normalizedData.Count; i++)
            {
                var id = r.Next(0, _numberOfClusters);
                _normalizedData[i].Cluster_Id = id;
                _rawData[i].Cluster_Id = id;
            }
        }
        
        private bool UpdateDataMeans()
        {
            // In case one cluster becomes empty.
            if (EmptyCluster(_normalizedData)) return false;

            var clusters = _normalizedData.GroupBy(a => a.Cluster_Id).OrderBy(a => a.Key);
            int clusterIndex = 0;
            double x = 0;
            double y = 0;
            foreach (var c in clusters)
            {
                foreach (var v in c)
                {
                    x += v.X;
                    y += v.Y;
                }
                _clusters[clusterIndex].X = x / c.Count();
                _clusters[clusterIndex].Y = y / c.Count();
                clusterIndex++;
                x = 0;
                y = 0;
            }
            return true;
        }

        private bool EmptyCluster(List<DataPoint> data)
        {
            var emptyCluster = data.GroupBy(a => a.Cluster_Id).OrderBy(a => a.Key).Select(a => new { Cluster = a.Key, Count = a.Count() });

            foreach (var ec in emptyCluster)
            {
                if (ec.Count == 0)
                    return true;
            }
            return false;
        }

        private bool UpdateCluster()
        {
            bool changed = false;
            
            for (int i = 0; i < _normalizedData.Count; i++)
            {
                var distances = new List<double>();
                for (int k = 0; k < _numberOfClusters; k++)
                {
                    distances.Add(Math.Sqrt(Math.Pow(_normalizedData[i].X - _clusters[k].Y, 2) + Math.Pow(_normalizedData[i].X - _clusters[k].Y, 2)));
                }

                int newClusterId = distances.IndexOf(distances.Min());
                if (newClusterId != _normalizedData[i].Cluster_Id)
                {
                    changed = true;
                    _normalizedData[i].Cluster_Id = _rawData[i].Cluster_Id = newClusterId;
                }
            }

            if (EmptyCluster(_normalizedData)) return false;
            return changed;
        }
    }
}
