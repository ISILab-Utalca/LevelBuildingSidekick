using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Kmeans
{
    List<List<Vector2>> clusters = new List<List<Vector2>>();
    public List<List<Vector2>> Clusters => clusters;

    List<Vector2> centroids = new List<Vector2>();
    public List<Vector2> Centroids => centroids;


    public void Init(List<Vector2> points, int k)
    {
        centroids = new List<Vector2>();
        for (int i = 0; i < k; i++)
        {
            centroids.Add(points[i]);
        }
        int counter = 0;
        while (true)
        {
            clusters = new List<List<Vector2>>();
            for (int i = 0; i < k; i++)
            {
                clusters.Add(new List<Vector2>());
            }
            foreach (Vector2 point in points)
            {
                var closestCentroid = centroids.Select((c,i) => new { c, i }).OrderBy((x) => (point - x.c).Distance(DistanceType.CONNECT_4)).First().i; //DistanceType.EUCLIDEAN
                clusters[closestCentroid].Add(point);
            }
            List<Vector2> newCentroids = new List<Vector2>();
            for (int i = 0; i < k; i++)
            {
                if (clusters[i].Count == 0)
                {
                    newCentroids.Add(centroids[i]);
                    continue;
                }

                float avgX = clusters[i].Select(p => p.x).Average();
                float avgY = clusters[i].Select(p => p.y).Average();
                newCentroids.Add(new Vector2(avgX, avgY));
            }
            if (centroids.SequenceEqual(newCentroids) || counter > 1000)
            {
                break;
            }
            centroids = newCentroids;
            counter++;
        }
    }

    public int ElbowMethod(List<Vector2> points)
    {
        List<double> distances = new List<double>();
        for (int k = 1; k <= points.Count; k++)
        {
            Init(points, k);

            var sumDistances = points.Sum(p => centroids.Min(c => (p - c).Distance(DistanceType.CONNECT_4))); //DistanceType.EUCLIDEAN
            distances.Add(sumDistances);
        }
        double maxDistanceReduction = 0;
        int elbow = 0;
        for (int k = 1; k < distances.Count; k++)
        {
            double distanceReduction = distances[k - 1] - distances[k];
            if (distanceReduction > maxDistanceReduction)
            {
                maxDistanceReduction = distanceReduction;
                elbow = k;
            }
        }
        return elbow + 1; // Add 1 to convert from index to count
    }

    public List<List<float>> Dispersion()
    {
        var deviations = new List<List<float>>();
        for (int i = 0; i < Centroids.Count; i++)
        {
            deviations.Add(new List<float>());
            foreach (Vector2 point in Clusters[i])
            {
                float distance = (point - Centroids[i]).Distance(DistanceType.CONNECT_4); //DistanceType.EUCLIDEAN
                deviations[i].Add(distance);
            }
        }
        return deviations;
    }

    public float MaxDispersion(List<Vector2> points)
    {
        return points.Max(p1 => points.Max(p2 => (p1 - p2).Distance(DistanceType.CONNECT_4)));//DistanceType.EUCLIDEAN
    }  

    public float MinDispersion(List<Vector2> points, int k)
    {
        return Mathf.Pow(MaxDispersion(points), 2) / (2 * k * Mathf.Log(points.Count));
    }
    
}
