using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestVector
{
    // A Test behaves as an ordinary method
    [Test]
    public void Test()
    {
        var a = new Vector(new Complex[] { 1, 2, 3 });
        var b = new Vector(new Complex[] { 1, 2, 3 });
        Assert.IsTrue(a == b);
        Assert.IsTrue(a + b == a * 2);
        Assert.IsTrue(2 * a == a * 2);

    }
}
