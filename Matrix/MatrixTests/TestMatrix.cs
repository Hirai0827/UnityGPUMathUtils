using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestMatrix
{
    // A Test behaves as an ordinary method
    [Test]
    public void Test()
    {
        // Use the Assert class to test conditions
        var A = new Matrix(2,2);
        A.val[0, 0] = 1.0f; A.val[0, 1] = 2.0f;
        A.val[1, 0] = 3.0f; A.val[1, 1] = 4.0f;
        var B = new Matrix(new Complex[,] { {1.0f,2.0f},{2.0f,3.0f}});
        B.val[0, 0] = 1.0f; B.val[0, 1] = 2.0f;
        B.val[1, 0] = 2.0f; B.val[1, 1] = 3.0f;
        var I = new Matrix(2, 2);
        I.val[0, 0] = 1.0f; I.val[0, 1] = 0.0f;
        I.val[1, 0] = 0.0f; I.val[1, 1] = 1.0f;
        Assert.IsTrue((-2.0f == A.Det()));
        Assert.IsTrue((-1.0f == B.Det()));
        Assert.IsTrue(A == A * I);
        Assert.IsTrue(I * A == A * I);
        Assert.IsTrue(B + A == A + B);
    }
    [Test]
    public void TestSubMatrix()
    {
        var A = new Matrix(new Complex[3, 3] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } });
        var B = new Matrix(new Complex[2, 2] { { 1, 2 }, { 4, 5 } });
        var C = new Matrix(new Complex[1, 1] {{5}});
        Assert.IsTrue(A.SubMatrix(new Vector2Int(0, 2), new Vector2Int(0, 2)) == B);
        Assert.IsTrue(A.SubMatrix(new Vector2Int(1, 2), new Vector2Int(1, 2)) == C);

    }
    [Test]
    public void TestBlockExpression()
    {
        var A = new Matrix(new Complex[2, 2] { { 1, 2 }, { 4, 5 } });
        var B = new Matrix(new Complex[2, 1] { {3 }, {6} });
        var C = new Matrix(new Complex[1, 2] { {7,8 } });
        var D = new Matrix(new Complex[1, 1] { {9} });
        Debug.Log(A);
        var E = new Matrix(new Matrix[2, 2] { { A, B },{ C, D } });
        Debug.Log(E);
        var F = new Matrix(new Complex[3, 3] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } });
        Assert.IsTrue(E == F);
    }
}
