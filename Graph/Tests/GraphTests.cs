using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GraphTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void TestDistanceCalcBFS()
    {
        var G = new Graph(4,false);
        G.AddEdge(0,1);
        G.AddEdge(0,2);
        G.AddEdge(1, 2);
        G.AddEdge(1, 3);
        var dist = G.CalcDistanceFrom(0);
        Assert.AreEqual(0, dist[0]);
        Assert.AreEqual(1, dist[1]);
        Assert.AreEqual(1, dist[2]);
        Assert.AreEqual(2, dist[3]);

        G = new Graph(3, true);
        G.AddEdge(0, 1);
        G.AddEdge(1, 2);
        G.AddEdge(2, 0);
        dist = G.CalcDistanceFrom(0);
        Assert.AreEqual(0, dist[0]);
        Assert.AreEqual(1, dist[1]);
        Assert.AreEqual(2, dist[2]);
    }
}
