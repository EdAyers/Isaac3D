using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
  public struct Complex
  {
    public float real;
    public float imag;

    static public Complex FromRI(float r, float i)
    {
      return new Complex()
      {
        real = r,
        imag = i
      };
    }

    public Complex(float r, float i)
    {
      real = r;
      imag = i;
    }

    public float R
    {
      get { return real; }
      set { real = value; }
    }
    public float I
    {
      get { return imag; }
      set { imag = value; }
    }
    public float Mag
    {
      get { return Mathf.Sqrt(R * R + I * I); }
    }
    public float Arg
    {
      get { return Mathf.Atan2(I, R); }
    }

    static public Complex operator *(Complex a, Complex b)
    {
      return new Complex(a.R * b.R - a.I * b.I, a.R * b.I + a.I * b.R);
    }
    static public Complex operator *(float a, Complex b)
    {
      return new Complex(a * b.R, a * b.I);
    }
    static public Complex operator +(Complex a, Complex b)
    {
      return new Complex(a.R + b.R, a.I + b.I);
    }
    static public Complex operator -(Complex a)
    {
      return new Complex(-a.R, -a.I);
    }
    static public Complex Exp(Complex x)
    {
      Complex arg = new Complex(Mathf.Cos(x.I), Mathf.Sin(x.I));
      float mag = Mathf.Exp(x.R);
      return mag * arg;
    }
    static public Complex Zero()
    {
      return new Complex(0, 0);
    }
  }

  public struct CMatrix2
  {
    public Complex m00;
    public Complex m10;
    public Complex m01;
    public Complex m11;
    public CMatrix2(Complex m00, Complex m10, Complex m01, Complex m11)
    {
      this.m00 = m00;
      this.m10 = m10;
      this.m11 = m11;
      this.m01 = m01;
    }
    public CMatrix2(
      float m00r, float m00i,
      float m10r, float m10i,
      float m01r, float m01i,
      float m11r, float m11i)
    {
      this.m00 = new Complex(m00r, m00i);
      this.m10 = new Complex(m10r, m10i);
      this.m01 = new Complex(m01r, m01i);
      this.m11 = new Complex(m11r, m11i);
    }
    public static CMatrix2 operator *(CMatrix2 a, CMatrix2 b)
    {
      return new CMatrix2(
          a.m01 * b.m10 + a.m00 * b.m00,
          a.m10 * b.m00 + a.m11 * b.m10,
          a.m00 * b.m01 + a.m01 * b.m11,
          a.m11 * b.m11 + a.m10 * b.m01
        );
    }
    public static CMatrix2 operator +(CMatrix2 a, CMatrix2 b)
    {
      return new CMatrix2()
      {
        m00 = a.m00 + b.m00,
        m01 = a.m01 + b.m01,
        m10 = a.m10 + b.m10,
        m11 = a.m11 + b.m11
      };
    }
    public static CMatrix2 ID()
    {
      return new CMatrix2(1, 0, 0, 0, 0, 0, 1, 0);
    }
    public static CMatrix2 QuarterWaveplate()
    {
      return new CMatrix2(1, 0, 0, 0, 0, 0, 0, 1);
    }
    public static CMatrix2 LinearPolariser()
    {
      return new CMatrix2(1, 0, 0, 0, 0, 0, 0, 0);
    }
    public static CMatrix2 Rotation(float theta)
    {
      var c = Mathf.Cos(theta);
      var s = Mathf.Sin(theta);
      return new CMatrix2(c, 0, s, 0, -s, 0, c, 1);
    }
  }

  public struct CVector2
  {
    public Complex X;
    public Complex Y;
    public CVector2(Complex x, Complex y)
    {
      X = x;
      Y = y;
    }
    public CVector2(float xr, float xi, float yr, float yi)
    {
      X = new Complex(xr, xi);
      Y = new Complex(yr, yi);
    }
    public static CVector2 operator *(CMatrix2 m, CVector2 v)
    {
      return new CVector2(m.m00 * v.X + m.m01 * v.Y, m.m10 * v.X + m.m11 * v.Y);
    }
  }
}
