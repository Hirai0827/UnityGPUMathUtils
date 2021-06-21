using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Complex
{
    public float real;
    public float imaginary;
    public Complex()
    {
        this.real = 0.0f;
        this.imaginary = 0.0f;
    }
    public Complex(float real,float imaginary)
    {
        this.real = real;
        this.imaginary = imaginary;
    }
    public Complex(Complex comp)
    {
        this.real = comp.real;
        this.imaginary = comp.imaginary;
    }
    public Complex Conjugate()
    {
        var result = new Complex(this);
        result.imaginary *= -1;
        return result;
    }
    public float magnitude()
    {
        return Mathf.Sqrt(this.real * this.real + this.imaginary * this.imaginary);
    }
    public float sqMagnitude()
    {
        return this.real * this.real + this.imaginary * this.imaginary;
    }

    public override bool Equals(object obj)
    {
        return obj is Complex complex &&
               real == complex.real &&
               imaginary == complex.imaginary;
    }

    public override int GetHashCode()
    {
        int hashCode = -1613305685;
        hashCode = hashCode * -1521134295 + real.GetHashCode();
        hashCode = hashCode * -1521134295 + imaginary.GetHashCode();
        return hashCode;
    }

    public override string ToString()
    {
        var str = this.real.ToString() + "+" + this.imaginary.ToString() + "i";
        return str;
    }

    public static Complex operator+(Complex x,Complex y)
    {
        var result = new Complex();
        result.real = x.real + y.real;
        result.imaginary = x.imaginary + y.imaginary;
        return result;
    }
    public static Complex operator -(Complex x, Complex y)
    {
        var result = new Complex();
        result.real = x.real - y.real;
        result.imaginary = x.imaginary - y.imaginary;
        return result;
    }
    //TODO èúêœÇé¿ëï
    public static Complex operator*(Complex x,Complex y)
    {
        var result = new Complex();
        result.real = x.real * y.real - x.imaginary * y.imaginary;
        result.imaginary = x.imaginary * y.real + x.real * y.imaginary;
        return result;
    }
    public static Complex operator *(Complex x, float k)
    {
        var result = new Complex();
        result.real = x.real * k;
        result.imaginary = x.imaginary * k;
        return result;
    }
    public static Complex operator *(float k,Complex x)
    {
        return x * k;
    }
    public static Complex operator /(Complex x,float k)
    {
        return x * (1.0f / k);
    }
    public static Complex operator/(Complex x,Complex y)
    {
        var y_rev = y.Conjugate() / y.sqMagnitude();
        return x * y_rev;
    }
    public static bool operator==(Complex x,Complex y)
    {
        return Mathf.Approximately(x.real, y.real) && Mathf.Approximately(x.imaginary, y.imaginary);
    }
    public static bool operator!=(Complex x,Complex y)
    {
        return !(x == y);
    }
    public static implicit operator Complex(float v)
    {
        return new Complex(v,0.0f);
    }
    public static implicit operator Vector2(Complex complex)
    {
        return new Vector2(complex.real,complex.imaginary);
    }

}
