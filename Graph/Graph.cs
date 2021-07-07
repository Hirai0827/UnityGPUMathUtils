using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Graph
{
    public struct Edge
    {
        public int to;
        public int distance;
    }
    List<List<Edge>> edgeList;
    List<HashSet<int>> hasEdgeSet;
    bool isDirected;
    bool hasMinusWeight;
    bool hasWeight;
    int size;
    public int Size => size;
    public bool IsDirected => isDirected;
    public Graph(int size, bool isDirected)
    {
        this.size = size;
        this.isDirected = isDirected;
        this.hasMinusWeight = false;
        this.hasWeight = false;
        this.edgeList = new List<List<Edge>>();
        this.hasEdgeSet = new List<HashSet<int>>();
        for (int i = 0; i < size; i++)
        {
            this.edgeList.Add(new List<Edge>());
            this.hasEdgeSet.Add(new HashSet<int>());
        }
    }
    public void AddEdge(int from, int to)
    {
        AddEdge(from, to, 1);
    }

    public void AddEdge(int from, int to, int distance)
    {
        if (distance != 1)
        {
            hasWeight = true;
        }
        if (distance < 0)
        {
            hasMinusWeight = true;
        }
        this.edgeList[from].Add(new Edge { to = to, distance = distance });
        this.hasEdgeSet[from].Add(to);
        if (!isDirected)
        {
            this.edgeList[to].Add(new Edge { to = from, distance = distance });
            this.hasEdgeSet[to].Add(from);
        }
    }

    public bool HasEdge(int from, int to)
    {
        return this.hasEdgeSet[from].Contains(to);
    }

    public List<int> TopologicalSort()
    {
        var result = new List<int>();
        var visit = new List<bool>(this.size);
        for (int i = 0; i < this.size; i++)
        {
            visit.Add(false);
        }
        void rec(int v)
        {
            visit[v] = true;
            foreach (var i in edgeList[v])
            {
                if (visit[i.to]) continue;
                rec(i.to);
            }
            result.Add(v);
        };
        for (int i = 0; i < this.size; i++)
        {
            if (visit[i]) continue;
            rec(i);
        }
        result.Reverse();
        return result;
    }

    public bool HasLoop()
    {
        var hasLoop = false;
        var hasVisited = new List<bool>(this.size);
        var hasFinished = new List<bool>(this.size);
        for (int i = 0; i < this.size; i++)
        {
            hasVisited[i] = false;
            hasFinished[i] = false;
        }
        for (int i = 0; i < this.size; i++)
        {
            if (hasVisited[i]) continue;

        }
        throw new NotImplementedException();
    }
    //TODO Utilsを生やす
    public List<List<int>> CalcAllDistances()
    {
        //Calc all distance of two vertices;
        //TODO ワーシャルフロイド法の実装
        throw new Exception();
    }
    public List<int> CalcDistanceFrom(int index)
    {
        //Calc distance from the point
        //TODO 深さ優先探索の実装
        if (!hasWeight)
        {
            return CalcDistanceFromByBFS(index);
        }
        //TODO ダイクストラ法の実装
        if (!hasMinusWeight)
        {
            return CalcDistanceByDijkstra(index);
        }
        //TODO ベルマンフォード法の実装
        throw new Exception();
    }

    private List<int> CalcDistanceFromByBFS(int index)
    {
        var result = new int[this.size].Select(x => int.MaxValue).ToList();
        var hasVisited = new bool[this.size].Select(x => false).ToList();
        var queue = new Queue<int>();
        result[index] = 0;
        hasVisited[index] = true;
        queue.Enqueue(index);
        while(queue.Count != 0)
        {
            var current = queue.Peek();queue.Dequeue();
            foreach (var next in this.edgeList[current])
            {
                if (hasVisited[next.to])
                {
                    continue;
                }
                hasVisited[next.to] = true;
                result[next.to] = result[current] + 1;
                queue.Enqueue(next.to);
            }
        }
        return result;
    }

    private List<int> CalcDistanceByDijkstra(int index)
    {
        throw new NotImplementedException();
        return new List<int>();
    }

    //グラフの直径について
    public struct GraphDiameterInfo
    {
        public int diameter;
        public int edgeA;
        public int edgeB;
    }
    public GraphDiameterInfo CalcDiameter()
    {
        //グラフの直径を計算する
        //TODO Linq動いているか確認
        //TODO グラフに正のループがあるかどうか確認する
        int from = 0;
        var dist_tmp = CalcDistanceFrom(from);
        int farthest = dist_tmp.Select((x, i) => { return new Tuple<int, int>(x, i); }).Max().Item2;
        int pointA = farthest;
        var dist = CalcDistanceFrom(pointA);
        int pointB = dist.Select((x, i) => { return new Tuple<int, int>(x, i); }).Max().Item2;
        return new GraphDiameterInfo { diameter = dist[pointB], edgeA = pointA, edgeB = pointB };
    }

}
