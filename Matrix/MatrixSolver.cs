using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Diagnostics;

public class MatrixSolver
{
    public static Vector SolveLinearByLU(Matrix A,Vector b)
    {
        //LU分解して前進後退代入でAx = bを解く
        //疎行列性を活かせていないので微妙
        //Pivotingがだるいのと計算量の制御がきついので一旦放置
        var LU = LU_Decompose(A);
        var L = LU.Item1;
        var U = LU.Item2;
        var result = new Vector(A.W);

        return result;
        
    }
    public static Vector SolveLinearByJacobi(Matrix A,Vector b,int iter = 100,float omega = 1.0f)
    {
        var sw = new Stopwatch();
        sw.Start();
        var A_dash = A.Transpose() * A;
        sw.Stop();
        UnityEngine.Debug.Log("A^T*A time:" + (sw.ElapsedMilliseconds / 1000.0f) + "sec");
        var result = new Vector(A_dash.W);
        var tmp = new Vector(A_dash.W);
        sw.Restart();
        var b_dash = A.Transpose() * b;
        sw.Stop();
        UnityEngine.Debug.Log("A^T*b time:" + (sw.ElapsedMilliseconds / 1000.0f) + "sec");
        sw.Restart();
        //TODO 後で並列化する
        for (int i = 0; i < iter; i++)
        {
            for (int yi = 0; yi < A_dash.H; yi++)
            {
                var sum = new Complex();
                for (int xi = 0; xi < A_dash.W; xi++)
                {
                    if (xi == yi)
                    {
                        continue;
                    }
                    sum += A_dash.val[yi, xi] * result.val[xi];
                }
                tmp.val[yi] = (b_dash.val[yi] - sum) / A_dash.val[yi, yi] * (omega) + result.val[yi]*(1.0f - omega);
            }
            result = new Vector(tmp);
        }
        sw.Stop();
        UnityEngine.Debug.Log("iteration time:" + (sw.ElapsedMilliseconds / 1000.0f) + "sec");
        return result;
    }

    public static Vector SolveLinearByJacobiOnGPU(Matrix A,Vector b,ComputeShader jacobiStep,ComputeShader mulMatrix,int iter = 1000, float omega = 1.0f)
    {
        var sw = new Stopwatch();
        sw.Start();


        var A_dash = MulMatrixOnGPU(A.Transpose(), A,mulMatrix);


        sw.Stop();
        UnityEngine.Debug.Log("A^T*A time:" + (sw.ElapsedMilliseconds / 1000.0f) + "sec");
        sw.Restart();


        var b_dash = A.Transpose() * b;


        sw.Stop();
        UnityEngine.Debug.Log("A^T*b time:" + (sw.ElapsedMilliseconds / 1000.0f) + "sec");
        sw.Restart();


        //ComputeShaderを用いてヤコビ法を実行
        var size = b.Length;
        //2べきが良いので残りは適当な値で埋めてヤコビ法を実行する
        var cellSize = Math.Max(GetCoveredPowerOf2(size),16);
        var dispatchSize = cellSize / 16;
        var kernelId = jacobiStep.FindKernel("JacobiStep");
        //求める対象のxを設定
        var x_src = new ComputeBuffer(cellSize,Marshal.SizeOf(typeof(float)));
        var x_tmp = new float[cellSize];
        for (int i = 0; i < cellSize; i++)
        {
            x_tmp[i] = 0.0f;
        }
        x_src.SetData(x_tmp);
        var x_dest = new ComputeBuffer(cellSize, Marshal.SizeOf(typeof(float)));
        var A_dash_tex = new ComputeBuffer(cellSize * cellSize, Marshal.SizeOf(typeof(float)));
        var A_tmp = new float[cellSize * cellSize];
        for (int yi = 0; yi < cellSize; yi++)
        {
            for (int xi = 0; xi < cellSize; xi++)
            {
                int i = yi * cellSize + xi;
                if(yi < A_dash.H && xi < A_dash.W)
                {
                    A_tmp[i] = A_dash.val[yi,xi].real;
                }
                else
                {
                    A_tmp[i] = 0.0f;
                }
            }
        }
        
        A_dash_tex.SetData(A_tmp);
        var b_dash_tex = new ComputeBuffer(cellSize, Marshal.SizeOf(typeof(float)));
        var b_tmp = new float[cellSize];
        for (int i = 0; i < cellSize; i++)
        {
            if(i < b_dash.Length)
            {
                b_tmp[i] = b_dash.val[i].real;
            }
            else
            {
                b_tmp[i] = 0.0f;

            }
        }
        b_dash_tex.SetData(b_tmp);


        sw.Stop();
        UnityEngine.Debug.Log("Setup ComputeBuffers time:" + (sw.ElapsedMilliseconds / 1000.0f) + "sec");
        sw.Restart();

        for (int i = 0; i < iter; i++)
        {
            jacobiStep.SetBuffer(kernelId, "_x_src", x_src);
            jacobiStep.SetBuffer(kernelId, "_x_dest", x_dest);
            jacobiStep.SetBuffer(kernelId, "_A", A_dash_tex);
            jacobiStep.SetBuffer(kernelId, "_b", b_dash_tex);
            jacobiStep.SetInt("_Size",b_dash.Length);
            jacobiStep.SetInt("_CellSize", cellSize);
            jacobiStep.SetFloat("_omega", omega);
            jacobiStep.Dispatch(kernelId,dispatchSize,1,1);
            //SwapTex
            var tmp = x_src;
            x_src = x_dest;
            x_dest = tmp;
        }


        sw.Stop();
        UnityEngine.Debug.Log("Iteration time:" + (sw.ElapsedMilliseconds / 1000.0f) + "sec");

        var result = new float[cellSize];
        x_src.GetData(result);
        var x = new Vector(b_dash.Length);
        for (int i = 0; i < x.Length; i++)
        {
            x.val[i] = result[i];
        }
        return x;
        



    }
    private static int GetCoveredPowerOf2(int x)
    {
        //x以上で最小の2のべき数を返す
        int y = 1;
        while(y < x)
        {
            y *= 2;
        }
        return y;
    }
    public static Tuple<Matrix,Matrix> LU_Decompose(Matrix A)
    {
        if(A.W != A.H)
        {
            throw new Exception("The target of LUDecomposition must be NxN Shape.");
        }
        //Doolittle法でLU分解を行う
        var N = A.W;
        var L = new Matrix(N,N);
        var U = new Matrix(N, N);
        for (int i = 0; i < N; i++)
        {
            L.val[i, i] = 1.0f;
        }

        for(int i = 0;i < N; i++)
        {
            for (int j = 0; j < N; j++) { 
                // i <= j ならu_ijを、i > jならl_ijを求める
                if(i <= j)
                {
                    //u_ij計算
                    var sum = new Complex();
                    for (int k = 0; k < i; k++)
                    {
                        sum += L.val[i,k] * U.val[k,j];
                    }
                    U.val[i, j] = A.val[i, j] - sum;
                }
                else
                {
                    //l_ij計算
                    var sum = new Complex();
                    for (int k = 0; k < j; k++)
                    {
                        sum += L.val[i, k] * U.val[k, j];
                    }
                    L.val[i, j] = (A.val[i, j] - sum) / U.val[j, j];
                }
            }
        }
        return new Tuple<Matrix, Matrix>(L, U);
    }

    public static Matrix MulMatrixOnGPU(Matrix A,Matrix B,ComputeShader mulMatrix)
    {

        if (A.W != B.H)
        {
            throw new System.Exception("Not Correct Shape");
        }
        var kernelId = mulMatrix.FindKernel("MulMatrix");
        var w = B.W; var h = A.H;
        var cellSize_A_W = Math.Max(16, GetCoveredPowerOf2(A.W));
        var cellSize_A_H = Math.Max(16, GetCoveredPowerOf2(A.H));
        var cellSize_B_W = Math.Max(16, GetCoveredPowerOf2(B.W));
        var cellSize_B_H = Math.Max(16, GetCoveredPowerOf2(B.H));
        var dispatchSize_X = cellSize_A_H/16;
        var dispatchSize_Y = cellSize_B_W/16;
        mulMatrix.SetInt("_CellSize_A_W", cellSize_A_W);
        mulMatrix.SetInt("_CellSize_A_H", cellSize_A_H);
        mulMatrix.SetInt("_CellSize_B_W", cellSize_B_W);
        mulMatrix.SetInt("_CellSize_B_H", cellSize_B_H);
        mulMatrix.SetInt("_Size_A_W", A.W);
        mulMatrix.SetInt("_Size_A_H", A.H);
        mulMatrix.SetInt("_Size_B_W", B.W);
        mulMatrix.SetInt("_Size_B_H", B.H);
        var bufferA = new ComputeBuffer(cellSize_A_W * cellSize_A_H, Marshal.SizeOf(typeof(float)));
        var bufferB = new ComputeBuffer(cellSize_B_W * cellSize_B_H, Marshal.SizeOf(typeof(float)));
        var bufferResult = new ComputeBuffer(cellSize_A_H * cellSize_B_W, Marshal.SizeOf(typeof(float)));
        var tmpA = new float[cellSize_A_W * cellSize_A_H];
        for (int yi = 0; yi < cellSize_A_H; yi++)
        {
            for (int xi = 0; xi < cellSize_A_W; xi++)
            {
                if(yi < A.H && xi < A.W)
                {
                    tmpA[yi * cellSize_A_W + xi] = A.val[yi, xi].real;
                }
                else
                {
                    tmpA[yi * cellSize_A_W + xi] = 0;
                }
            }
        }
        var tmpB = new float[cellSize_B_W * cellSize_B_H];
        for (int yi = 0; yi < cellSize_B_H; yi++)
        {
            for (int xi = 0; xi < cellSize_B_W; xi++)
            {
                if (yi < B.H && xi < B.W)
                {
                    tmpB[yi * cellSize_B_W + xi] = B.val[yi, xi].real;
                }
                else
                {
                    tmpB[yi * cellSize_B_W + xi] = 0;
                }
            }
        }
        bufferA.SetData(tmpA);
        bufferB.SetData(tmpB);
        mulMatrix.SetBuffer(kernelId, "_A", bufferA);
        mulMatrix.SetBuffer(kernelId, "_B", bufferB);
        mulMatrix.SetBuffer(kernelId, "_Result", bufferResult);
        mulMatrix.Dispatch(kernelId,dispatchSize_X,dispatchSize_X,1);
        var result = new float[cellSize_A_H * cellSize_B_W];
        bufferResult.GetData(result);
        var X = new Matrix(A.H,B.W);
        for (int yi = 0; yi < A.H; yi++)
        {
            for (int xi = 0; xi < B.W; xi++)
            {
                X.val[yi, xi] = result[yi * cellSize_B_W + xi];
            }
        }
        return X;
    }

}
