using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
  ///Class that calculates the wavefunction properties of a hydrogenic orbital
  ///given the 3 quantum numbers. It is very likely that there is a mistake somewhere
  ///in calculating values, especially in their normalisation and size, where extra scaling is added
  ///ad-hoc to make the renderings have roughly consistent brightness and size.
  ///The 'm' quantum number is abused somewhat because
  ///the +m and -m wavefunctions are linearly combined to give real-valued wavefunctions to
  ///align with what A-level students are used to.
  ///The +m is mapped to the + linear combination and -m similarly.
  public class HydrogenCalc : ILevelFunc
  {
    int n;
    int l;
    int m;

    float harmonicFactor;
    float hydrogenFactor;

    ///Value used to give a general order-of-magnitude
    ///spatial size of the orbital so the renderer knows
    ///how big it is.
    public float Size
    {
      get
      {
        return Mathf.Pow(2, n) * 3;
      }
    }

    ///Spectroscopic label of the orbital. eg ("4s", "3d").
    public string OrbitalLabel
    {
      get
      {
        StringBuilder sb = new StringBuilder();
        sb.Append(n.ToString());
        switch (l)
        {
          case 0: sb.Append("s "); break;
          case 1:
            sb.Append("p ");
            break;
          case 2:
            sb.Append("d ");
            break;
          case 3:
            sb.Append("f ");
            break;
          default:
            sb.Append(((char)('f' + l - 3)) + " ");
            break;
        }
        return sb.ToString();
      }
    }

    ///Label usually written as a subscript indicating the m value
    public string MLabel
    {
      get
      {
        switch (l)
        {
          case 0: return "";
          case 1:
            if (m == 1) return "x";
            if (m == 0) return "z";
            if (m == -1) return "y";
            break;
          case 2:
            if (m == 2) return "xy";
            if (m == 1) return "xz";
            if (m == 0) return "zz";
            if (m == -1) return "yz";
            if (m == -2) return "(xx-yy)";
            break;
          case 3:
            if (m == 3) return "x(xx-3yy)";
            if (m == 2) return "xyz";
            if (m == 1) return "xzz";
            if (m == 0) return "zzz";
            if (m == -1) return "yzz";
            if (m == -2) return "z(xx-yy)";
            if (m == -3) return "y(3xx-yy)";
            break;
          default:
            return " (m = " + m + ")";
        }
        return "???";
      }
    }

    ///Gives the number of nodes in azimuthal (phi) direction.
    ///(doesn't include 
    public int AngularNodes { get { return Math.Abs(m); } }
    ///When we calculate the wavefunctions, we find the two real solutions
    ///by finding two linear combos of functions with the same |m| value.
    ///These two solutions differ only by a rotation, so we can use the same
    ///texture to show both and then tell the engine to rotate one.
    ///This is slightly abusive of notation, since the displayed functions are
    ///not actually eigenstates of m but of |m|, but we reassign the
    ///meaning of the sign of m to tell us which rotation to look at.
    public float Rotation
    {
      get
      {
        if (m >= 0) { return 0.0f; }
        else { return (float)Math.PI / (2 * m); }
      }
    }

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

      if (a == 0 && l == 0) legendrePol = P00;
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
      int lagN = n - l - 1;
      if (lagN == 0) laguerrePol = L0;
      else if (lagN == 1) laguerrePol = L1;
      else if (lagN == 2) laguerrePol = L2;
      else if (lagN == 3) laguerrePol = L3;
      else
      {
        throw new ArgumentException("Laguerre Polynomials not implemented for n - l - 1 = " + lagN.ToString());
      }

      f = Mathf.Pow(2f / n, 3f) / (2 * n);

      for (int i = n - l; i <= n + l; i++)
      {
        f /= i;
      }
      hydrogenFactor = Mathf.Sqrt(f);
    }

    public float SphericalHarmonic(float theta)
    {
      float f = harmonicFactor * legendrePol(Mathf.Cos(theta));

      return f;
    }

    public float RadialComponent(float r)
    {
      var rho = 2 * r / n;
      float part1 = Mathf.Exp(-rho / 2);
      float part2 = Mathf.Pow(rho, l);
      float part3 = laguerrePol(rho, 2 * l + 1);

      var result = part1 * part2 * part3 * hydrogenFactor;
      //HACK fudge factor to get brightnesses roughly the same
      result *= (float)(Math.Pow(2, n) / Math.Pow(2, 3));
      return result;
    }

    ///Calculates the wavefunction value at the given position.
    public Complex Wavefunction(float r, float theta, float phi)
    {
      float mag_Y = SphericalHarmonic(theta);
      float mag = mag_Y * RadialComponent(r);
      float phase = phi * m;
      mag *= Mathf.Cos(phase);
      return Complex.FromMagArg(mag, 0);
    }

    delegate float Polynomial(float x);
    delegate float LagPoly(float x, int k);
    Polynomial legendrePol;
    LagPoly laguerrePol;

    //Hard-coded first few Legendre polynomials, almost certainly a mistake somewhere.
    float P00(float x) { return 1f; }
    float P01(float x) { return x; }
    float P11(float x) { return -Mathf.Sqrt(1 - x * x); }
    float P02(float x) { return 0.5f * (3f * x * x - 1f); }
    float P12(float x) { return -3f * x * Mathf.Sqrt(1f - x * x); }
    float P22(float x) { return 3f * (1f - x * x); }
    float P03(float x) { return 0.5f * x * (5f * x * x - 3f); }
    float P13(float x) { return 1.5f * (1 - 5f * x * x) * Mathf.Sqrt(1f - x * x); }
    float P23(float x) { return 15f * x * (1f - x * x); }
    float P33(float x) { return -15f * Mathf.Pow(1f - x * x, 1.5f); }
    float P04(float x) { return 0.125f * (35f * Mathf.Pow(x, 4) - 30f * x * x + 3f); }
    float P14(float x) { return 2.5f * x * (3f - 7f * x * x) * Mathf.Sqrt(1f - x * x); }
    float P24(float x) { return 7.5f * (7f * x * x - 1f) * (1f - x * x); }
    float P34(float x) { return -105f * x * Mathf.Pow(1f - x * x, 1.5f); }
    float P44(float x) { return 105f * Mathf.Pow(1 - x * x, 2); }
    float P05(float x) { return 0.125f * x * (63 * Mathf.Pow(x, 4) - 70 * x * x + 15); }

    //Hard-coded Larguerre polynomials
    float L0(float x, int k) { return 1f; }
    float L1(float x, int k) { return 1 - x + k; }
    float L2(float x, int k) { return 0.5f * (x * x - 2 * (k + 1) * x + (k + 1) * (k + 2)); }
    float L3(float x, int k) { return 1 / 6f * (-x * x * x + 3 * (k + 3) * x * x - 3 * (k + 2) * (k + 3) * x + (k + 3) * (k + 2) * (k + 1)); }

    ///Used for implementation of ILevelFunc
    public float F(Vector3 v)
    {
      if (n != 1 && l == 0 && v.z < 0) return 0;

      var r = v.magnitude * Size;
      var rho = Mathf.Sqrt(v.x * v.x + v.z * v.z);
      var theta = Mathf.Atan2(rho, v.y);
      var phi = Mathf.Atan2(v.z, v.x);
      float result = Wavefunction(r, theta, phi).MagSquared;
      return result;
    }

    public float GetLevelSetValue(float requiredIntegration)
    {
      //check the integration range
      const int STEPS = 30;
      float range = Size / 16;

      float start = - range / 2;
      var startV = new Vector3(start, start, start);
      float delta = range / STEPS;
      float testValue = 1;

      float maxTotal = Integrate(STEPS, startV, delta, 0);
      Debug.Log("maxtotal = " + maxTotal.ToString());

      StringBuilder sb = new StringBuilder();
      for (int n = 0; n < STEPS; n++)
      {
        var pos = startV + new Vector3(n, n, n) * delta;
        float val = F(pos);
        sb.Append(val);
        sb.Append(" ; ");
      }
      Debug.Log(sb);

      for (int n = 0; n < 20; n++)
      {


        float total = Integrate(STEPS, startV, delta, testValue);
        total /= maxTotal;

        if (Mathf.Abs(total - requiredIntegration) < 0.01f) { break; }
        else if (total < requiredIntegration)
        {
          testValue -= Mathf.Pow(2, - 1 - n);
        }
        else
        {
          testValue += Mathf.Pow(2, -1 - n);
        }
        Debug.Log("test value = " + testValue.ToString());
        Debug.Log("total = " + total.ToString());
      }
      
      return testValue;
    }

    private float Integrate(int STEPS, Vector3 startV, float delta, float testValue)
    {
      float total = 0;
      for (int i = 0; i < STEPS; i++)
        for (int j = 0; j < STEPS; j++)
          for (int k = 0; k < STEPS; k++)
          {
            var pos = startV + new Vector3(i, j, k) * delta;
            float val = F(pos);
            if (val > testValue)
            {
              total += val * Mathf.Pow(delta, 3);
            }
          }
      return total;
    }
  }
}