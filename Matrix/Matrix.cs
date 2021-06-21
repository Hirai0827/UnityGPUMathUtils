using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Matrix
{
    int w;
    int h;
    public int W => w;
    public int H => h;

    public Complex[,] val;
    public Matrix(int h, int w)
    {
        this.h = h;
        this.w = w;
        this.val = new Complex[h,w];
        for (int yi = 0; yi < h; yi++)
        {
            for (int xi = 0; xi < w; xi++)
            {
                this.val[yi,xi] = 0;
            }
        }

    }
    public Matrix(Matrix mat)
    {
        // Copy Matrix
        this.h = mat.h;
        this.w = mat.w;
        this.val = new Complex[h, w];
        for (int yi = 0; yi < h; yi++)
        {
            for (int xi = 0; xi < w; xi++)
            {
                this.val[yi, xi] = mat.val[yi,xi];
            }
        }

    }
    public Matrix(Complex[,] val)
    {
        this.h = val.GetLength(0);
        this.w = val.GetLength(1);
        this.val = val.Clone() as Complex[,];
    }
    public Matrix(Matrix[,] val)
    {
        //TODO ブロック行列表現が出来るようにしたい
        var h = 0;
        var w = 0;
        for (int yi = 0; yi < val.GetLength(0); yi++)
        {
            var size = val[yi, 0].H;
            for (int xi = 1; xi < val.GetLength(1); xi++)
            {
                if(size != val[yi, xi].H)
                {
                    throw new System.Exception("Not Correct Shape");
                }
            }
            h += size;
        }
        for (int xi = 0; xi < val.GetLength(1); xi++)
        {
            var size = val[0, xi].W;
            for (int yi = 0; yi < val.GetLength(0); yi++)
            {
                if(size != val[yi, xi].W)
                {
                    throw new System.Exception("Not Correct Shape");
                }
            }
            w += size;
        }
        this.h = h;
        this.w = w;
        this.val = new Complex[h, w];
        var yOffset = 0;
        var xOffset = 0;
        for (int yi = 0; yi < val.GetLength(0); yi++)
        {
            xOffset = 0;
            var h_tmp = val[yi, 0].H;
            for (int xi = 0; xi < val.GetLength(1); xi++)
            {
                var mat_tmp = val[yi, xi];
                var w_tmp = mat_tmp.w;
                for (int yj = 0; yj < h_tmp; yj++)
                {
                    for (int xj = 0; xj < w_tmp; xj++)
                    {
                        this.val[yOffset + yj, xOffset + xj] = mat_tmp.val[yj, xj];
                        //Debug.Log((yOffset + yj) + "," + (xOffset + xj) + ":" + this.val[yOffset + yj, xOffset + xj].ToString() + "(" + yj + "," + xj + ")");
                    }
                }
                xOffset += w_tmp;
            }
            yOffset += h_tmp;
        }

    }
    public Complex Det()
    {
        //下三角行列を作ってTraceを計算
        if (this.w != this.h)
        {
        }
        var N = this.w;
        var tmp = new Matrix(this);
        for(int yi = N-1; yi >= 0; yi--)
        {
            for(int xi = N-1; xi > yi; xi--) {
                if(tmp.val[yi,xi] == 0)
                {
                    continue;
                }
                Complex weight = tmp.val[yi, xi]/tmp.val[xi,xi];
                for(int ki = 0; ki < N; ki++)
                {
                    tmp.val[yi, ki] -= tmp.val[xi, ki] * weight;
                }
            }
        }
        Complex result = 1.0f;
        for (int i = 0; i < N; i++)
        {
            result *= tmp.val[i,i];
        }
        return result;
    }
    public Matrix Transpose()
    {
        var h = this.w;
        var w = this.h;
        var result = new Matrix(h, w);
        for (int yi = 0; yi < h; yi++)
        {
            for (int xi = 0; xi < w; xi++)
            {
                result.val[yi, xi] = this.val[xi,yi];
            }
        }
        return result;
    }
    public Matrix SubMatrix(Vector2Int y,Vector2Int x)
    {
        var result = new Matrix(y.y - y.x, x.y - x.x);
        for (int yi = y.x; yi < y.y; yi++)
        {
            for (int xi = x.x; xi < x.y; xi++)
            {
                result.val[yi - y.x, xi - x.x] = this.val[yi, xi];
            }
        }
        return result;
    }
    public static Matrix operator +(Matrix A, Matrix B)
    {
        if (A.w != B.w || A.h != B.h)
        {
            throw new System.Exception("Not Correct Shape");
        }
        int w = A.w; int h = A.h;
        var result = new Matrix(h, w);
        for (int yi = 0; yi < h; yi++)
        {
            for (int xi = 0; xi < w; xi++)
            {
                result.val[yi, xi] = A.val[yi, xi] + B.val[yi, xi];
            }
        }
        return result;
    }
    public static Matrix operator -(Matrix A, Matrix B)
    {
        if (A.w != B.w || A.h != B.h)
        {
            throw new System.Exception("Not Correct Shape");
        }
        int w = A.w; int h = A.h;
        var result = new Matrix(h, w);
        for (int yi = 0; yi < h; yi++)
        {
            for (int xi = 0; xi < w; xi++)
            {
                result.val[yi, xi] = A.val[yi, xi] - B.val[yi, xi];
            }
        }
        return result;
    }
    public static Matrix operator *(Matrix A, Matrix B)
    {
        if (A.w != B.h)
        {
            throw new System.Exception("Not Correct Shape");
        }
        int w = B.w; int h = A.h;
        var result = new Matrix(h, w);
        for (int yi = 0; yi < h; yi++)
        {
            for (int xi = 0; xi < w; xi++)
            {
                for(int j = 0;j < A.w; j++)
                {
                    result.val[yi, xi] += A.val[yi,j] * B.val[j,xi];
                }
            }
        }
        return result;
    }
    public static Matrix operator *(Matrix A, Complex k)
    {
        int w = A.w; int h = A.h;
        var result = new Matrix(h, w);
        for (int yi = 0; yi < h; yi++)
        {
            for (int xi = 0; xi < w; xi++)
            {
                result.val[yi, xi] += A.val[yi, xi] * k;
            }
        }
        return result;
    }
    public static Matrix operator*(Complex k,Matrix A)
    {
        return A * k;
    }
    public static bool operator==(Matrix A,Matrix B)
    {
        if (A.w != B.w || A.h != B.h)
        {
            return false;
        }
        int w = A.w; int h = A.h;
        var result = true;
        for (int yi = 0; yi < h; yi++)
        {
            for (int xi = 0; xi < w; xi++)
            {
                result = result && (A.val[yi, xi] == B.val[yi, xi]);
            }
        }
        return result;

    }
    public static bool operator !=(Matrix A, Matrix B)
    {
        return !(A == B);
    }
    public override string ToString()
    {
        var str = "";
        for (int yi = 0; yi < this.h; yi++)
        {
            for (int xi = 0; xi < this.w; xi++)
            {
                str += this.val[yi, xi].ToString() + ",";
            }
            str += "/n";
        }
        return str;
    }

}
