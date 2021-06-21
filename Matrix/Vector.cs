using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector
{
    private int length;
    public int Length => length;
    public Complex[] val;
    public Vector(int l)
    {
        this.length = l;
        val = new Complex[l];
        for (int i = 0; i < length; i++)
        {
            val[i] = 0.0f;
        }
    }
    public Vector(Vector v)
    {
        this.length = v.length;
        this.val = v.val.Clone() as Complex[];
    }
    public Vector(Complex[] val)
    {
        this.val = val.Clone() as Complex[];
        this.length = val.Length;
    }
    public static Vector operator+(Vector A,Vector B) {
        if(A.length != B.length)
        {
            throw new System.Exception("Not Correct Shape");
        }
        var result = new Vector(A.length);
        for(int i = 0; i < A.length; i++)
        {
            result.val[i] = A.val[i] + B.val[i];
        }
        return result;
    }
    public static Vector operator -(Vector A, Vector B)
    {
        if (A.length != B.length)
        {
            throw new System.Exception("Not Correct Shape");
        }
        var result = new Vector(A.length);
        for (int i = 0; i < A.length; i++)
        {
            result.val[i] = A.val[i] - B.val[i];
        }
        return result;
    }

    public static Vector operator *(Vector A,float b)
    {
        var result = new Vector(A);
        for (int i = 0; i < result.length; i++)
        {
            result.val[i] *= b;
        }
        return result;
    }
    public static Vector operator *(float b, Vector A)
    {
        return A * b;
    }
    public static Vector operator *(Matrix A,Vector b)
    {
        if(A.W != b.length)
        {
            throw new System.Exception("Not Correct Shape");
        }
        var result = new Vector(A.H);
        for (int i = 0; i < A.H; i++)
        {
            for (int j = 0; j < A.W; j++)
            {
                result.val[i] += A.val[i, j] * b.val[j];
            }
        }
        return result;
    }
    public static bool operator == (Vector A,Vector B)
    {
        if(A.length != B.length)
        {
            return false;
        }
        var result = true;
        for (int i = 0; i < A.length; i++)
        {
            result = result && (A.val[i] == B.val[i]);
        }
        return result;
    }
    public static bool operator !=(Vector A, Vector B)
    {
        return !(A == B);
    }
    public override string  ToString()
    {
        var str = "";
        for (int i = 0; i < this.length; i++)
        {
            str += this.val[i].ToString() + ",";
        }
        return str;
    }
}
