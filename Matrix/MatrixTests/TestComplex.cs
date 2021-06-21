using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestComplex
{
    // A Test behaves as an ordinary method
    [Test]
    public void Test()
    {
        var a = new Complex(1,1);
        var b = new Complex(1,-1);
        Assert.IsTrue(a.Conjugate() == b);
        Assert.IsTrue(a * b == new Complex(2, 0));
    }
}
