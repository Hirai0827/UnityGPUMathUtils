using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestSolver
{
    // A Test behaves as an ordinary method
    [Test]
    public void TestLU()
    {
        var A = new Matrix(new Complex[,] { {5,6,7},{10,20,23},{15,50,67} });
        var LU = MatrixSolver.LU_Decompose(A);
        Debug.Log(LU.Item1);
        Debug.Log(LU.Item2);
        Assert.IsTrue(LU.Item1 * LU.Item2 == A);
    }
    [Test]
    public void TestJacobi()
    {
        var A1 = new Matrix(new Complex[,] {{100,0},{0,1}} );
        var b1 = new Vector(new Complex[] {2,100});
        AssertSolveSuccessfullyOnGPU(A1,b1,100, 0.5f);
        var A2 = new Matrix(new Complex[,] { {3,2,4}, {1,2,0},{2,1,5} });
        var b2 = new Vector(new Complex[] {7,5,8});
        AssertSolveSuccessfully(A2, b2,100000,0.25f);
        AssertSolveSuccessfullyOnGPU(A2, b2, 100000, 0.25f);
    }
    [Test]
    public void TestMul()
    {
        var A1 = new Matrix(new Complex[,] { { 12, 21, 43 }, { 11, 21, 10 }});

        var A2 = new Matrix(new Complex[,] { { 3, 2, 4 }, { 1, 2, 0 }, { 2, 1, 5 } });

        var X = A1 * A2;
        var mulMatrix = (ComputeShader)Resources.Load("MulMatrix");
        var Y = MatrixSolver.MulMatrixOnGPU(A1,A2,mulMatrix);
        Assert.IsTrue(X == Y);

        
    }

    private void AssertSolveSuccessfully(Matrix A,Vector b,int iter,float omega)
    {
        var x = MatrixSolver.SolveLinearByJacobi(A,b,iter,omega);
        Debug.Log(x);
        Assert.IsTrue(Vector.SqrMagnitude(A * x - b) < 0.001);
    }

    private void AssertSolveSuccessfullyOnGPU(Matrix A, Vector b, int iter, float omega)
    {
        var jacobiStep = (ComputeShader)Resources.Load("JacobiStep");
        var mulMatrix = (ComputeShader)Resources.Load("MulMatrix");
        var x = MatrixSolver.SolveLinearByJacobiOnGPU(A, b,jacobiStep, mulMatrix, iter, omega);
        Debug.Log(x);
        Assert.IsTrue(Vector.SqrMagnitude(A * x - b) < 0.001);
    }
}
