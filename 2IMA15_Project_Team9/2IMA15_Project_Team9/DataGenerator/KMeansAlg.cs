using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2IMA15_Project_Team9.DataGenerator
{
    class KMeansAlg
    {
        private List<DataPoint> _rawData = new List<DataPoint>();
        private List<DataPoint> _normalizedData = new List<DataPoint>();
        private List<DataPoint> _clusteredData = new List<DataPoint>();
        private int _numberOfClusters = 0;

        public KMeansAlg(int numberOfClusters, List<DataPoint> rawData)
        {
            _numberOfClusters = numberOfClusters;
            _rawData = rawData;
        }
        
        private void NormalizeData()
        {
            double heightSum = 0.0;
            double weightSum = 0.0;
            foreach (DataPoint dataPoint in _rawData)
            {
                heightSum += dataPoint.X;
                weightSum += dataPoint.Y;
            }
            double heightMean = heightSum / _rawData.Count;
            double weightMean = weightSum / _rawData.Count;
            double sumHeight = 0.0;
            double sumWeight = 0.0;
            foreach (DataPoint dataPoint in _rawData)
            {
                sumHeight += Math.Pow(dataPoint.X - heightMean, 2);
                sumWeight += Math.Pow(dataPoint.Y - weightMean, 2);
            }

            double heightSD = sumHeight / _rawData.Count;
            double weightSD = sumWeight / _rawData.Count;

            foreach (DataPoint dataPoint in _rawData)
            {
                _normalizedData.Add(new DataPoint()
                {
                    X = (dataPoint.X - heightMean) / heightSD,
                    Y = (dataPoint.Y - weightMean) / weightSD
                });
            }
        }

        private void Cluster()
        {
            NormalizeData();

            bool _changed = true;
            bool _success = true;

            InitializeCentroids();

            int maxIteration = _normalizedData.Count * 10;
            int _threshold = 0;
            while (_success == true && _changed == true && _threshold < maxIteration)
            {
                _threshold ++;
                _success = UpdateDataPointMeans();
                _changed = UpdateClusterMembership();
            }
        }
        
        private void InitializeCentroids()
        {
            Random random = new Random(_numberOfClusters);
            for (int i = 0; i < _numberOfClusters; ++i)
            {
                _normalizedData[i].Cluster_Id = _rawData[i].Cluster_Id = i;
            }
            for (int i = _numberOfClusters; i < _normalizedData.Count; i++)
            {
                _normalizedData[i].Cluster_Id = _rawData[i].Cluster_Id = random.Next(0, _numberOfClusters);
            }
        }

        private bool UpdateDataPointMeans()
        {
            if (EmptyCluster(_normalizedData)) return false;

            var groupToComputeMeans = _normalizedData.GroupBy(s => s.Cluster_Id).OrderBy(s => s.Key);
            int clusterIndex = 0;
            double height = 0.0;
            double weight = 0.0;
            foreach (var item in groupToComputeMeans)
            {
                foreach (var value in item)
                {
                    height += value.X;
                    weight += value.Y;
                }
                _clusteredData[clusterIndex].X = height / item.Count();
                _clusteredData[clusterIndex].Y = weight / item.Count();
                clusterIndex++;
                height = 0.0;
                weight = 0.0;
            }
            return true;
        }

        private bool EmptyCluster(List<DataPoint> data)
        {
            var emptyCluster =
                data.GroupBy(s => s.Cluster_Id).OrderBy(s => s.Key).Select(g => new { Cluster = g.Key, Count = g.Count() });

            foreach (var item in emptyCluster)
            {
                if (item.Count == 0)
                {
                    return true;
                }
            }
            return false;
        }

        private bool UpdateClusterMembership()
        {
            bool changed = false;

            double[] distances = new double[_numberOfClusters];

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _normalizedData.Count; ++i)
            {

                for (int k = 0; k < _numberOfClusters; ++k)
                {
                    distances[k] = Math.Sqrt(Math.Pow(_normalizedData[i].X - _clusteredData[k].Y, 2) + Math.Pow(_normalizedData[i].X - _clusteredData[k].Y, 2));
                }

                int newClusterId = MinIndex(distances);
                if (newClusterId != _normalizedData[i].Cluster_Id)
                {
                    changed = true;
                    _normalizedData[i].Cluster_Id = _rawData[i].Cluster_Id = newClusterId;
                    sb.AppendLine("Data Point >> Height: " + _rawData[i].X + ", Weight: " +
                                  _rawData[i].Y + " moved to Cluster # " + newClusterId);
                }
                else
                {
                    sb.AppendLine("No change.");
                }
                sb.AppendLine("------------------------------");

            }
            if (changed == false)
                return false;
            if (EmptyCluster(_normalizedData)) return false;
            return true;
        }

        //private double ElucidanDistance(DataPoint dataPoint, DataPoint mean)
        //{
        //    return Math.Sqrt(Math.Pow(dataPoint.X_Value - mean.Y_Value, 2)+ Math.Pow(dataPoint.X_Value - mean.Y_Value, 2));
        //}

        private int MinIndex(double[] distances)
        {
            int _indexOfMin = 0;
            double _smallDist = distances[0];
            for (int k = 0; k < distances.Length; ++k)
            {
                if (distances[k] < _smallDist)
                {
                    _smallDist = distances[k];
                    _indexOfMin = k;
                }
            }
            return _indexOfMin;
        }
    }
}
