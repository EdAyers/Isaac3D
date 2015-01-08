using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
  class HydrogenCalc
  {
    int n;
    int l;
    int m;

    float harmonicFactor;
    float radialFactor;

    public HydrogenCalc(int n, int l, int m)
    {
      if (l > n) throw new ArgumentException("require n >= l");
      if (Math.Abs(m) > l) throw new ArgumentException("require -l <= m <= l");

      this.n = n;
      this.l = l;
      this.m = m;
      int a = Math.Abs(m);

      //calculate the spherical harmonic factor
      float f = (2 * l + 1) / (4 * Mathf.PI);
      //calculate (l-a)! / (l+a)! which is the same as dividing by all ints between
      // [l+a..l-a)
      for (int i = (l - a + 1); i <= l + a; i++)
      {
        f /= i;
      }
      
      harmonicFactor = Mathf.Sqrt(f);
      
      //select the legendre polynomial

      if      (a == 0 && l == 0) legendrePol = P00;
      else if (a == 0 && l == 1) legendrePol = P01;
      else if (a == 1 && l == 1) legendrePol = P11;
      else if (a == 0 && l == 2) legendrePol = P02;
      else if (a == 1 && l == 2) legendrePol = P12;
      else if (a == 2 && l == 2) legendrePol = P22;
      else if (a == 0 && l == 3) legendrePol = P03;
      else if (a == 1 && l == 3) legendrePol = P13;
      else if (a == 2 && l == 3) legendrePol = P23;
      else if (a == 3 && l == 3) legendrePol = P33;
      else if (a == 0 && l == 4) legendrePol = P04;
      else if (a == 1 && l == 4) legendrePol = P14;
      else if (a == 2 && l == 4) legendrePol = P24;
      else if (a == 3 && l == 4) legendrePol = P34;
      else if (a == 4 && l == 4) legendrePol = P44;
      else if (a == 0 && l == 5) legendrePol = P05;
      else
      {
        throw new ArgumentException("Legendre Polynomials not implemented for l = " + l.ToString());
      }

      if (m < 0)
      {//need to multiply by extra factor
        harmonicFactor *= (a % 2 == 0 ? 1 : -1);
      }

      //TODO calculate radial factor
      //if (n == )
    }

    public Color SphericalHarmonic(float theta, float phi)
    {
      float f = harmonicFactor * legendrePol(Mathf.Cos(theta));
      float mag;
      float arg = m * phi;
      if (f < 0)
      {
        mag = -f;
        arg += Mathf.PI;
      }
      else
      {
        mag = f;
      }
      arg /= (2 * Mathf.PI);
      arg = arg % 1.0f;

      return new Color(mag, arg, 0, 1);
    }

    public Color RadialComponent(float r)
    {
      //HACK
      return new Color( Mathf.Exp(-r / n),0,0,1);
    }

    delegate float Polynomial(float x);
    Polynomial legendrePol;
    Polynomial laguerrePol;

    //For now we'll just hard-code the first few legendre polynomials 
    float P00(float x) { return 1f; }
    float P01(float x) { return x; }
    float P11(float x) { return - Mathf.Sqrt(1 - x * x); }
    float P02(float x) { return 0.5f * (3f * x * x - 1f); }
    float P12(float x) { return -3f * x * Mathf.Sqrt(1f - x * x); }
    float P22(float x) { return 3f * (1f - x * x); }
    float P03(float x) { return 0.5f * x * (5f * x * x - 3f); }
    float P13(float x) { return 1.5f * (1 - 5f * x * x) * Mathf.Sqrt(1f - x * x); }
    float P23(float x) { return 15f * x * (1f - x * x); }
    float P33(float x) { return -15f * Mathf.Pow(1f - x * x, 1.5f); }
    float P04(float x) { return 0.125f * (35f * Mathf.Pow(x,4) - 30f * x * x + 3f); }
    float P14(float x) { return 2.5f * x * (3f - 7f * x * x) * Mathf.Sqrt(1f - x * x); }
    float P24(float x) { return 7.5f * (7f * x * x - 1f) * (1f - x * x) ; }
    float P34(float x) { return -105f * x * Mathf.Pow(1f - x * x, 1.5f); }
    float P44(float x) { return 105f * Mathf.Pow(1 - x * x, 2); }
    float P05(float x) { return 0.125f * x * (63 * Mathf.Pow(x, 4) - 70 * x * x + 15); }

    //Hard-coded Larguerre polynomials
    float L0(float x) { return 1f; }
    float L1(float x) { return 1 - x; }
    float L2(float x) { return 0.5f * (x*x - 4*x + 2); }
    float L3(float x) { return 1/6f * (- x * x * x + 9 * x * x - 18 * x + 6); }
  }
