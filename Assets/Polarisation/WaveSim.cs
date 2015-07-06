using UnityEngine;
using System.Collections;

public struct Waveplate
{
  public float position;
  public Complex m00;
  public Complex m10;
  public Complex m01;
  public Complex m11;
  public Waveplate(Complex m00, Complex m10, Complex m01, Complex m11)
  {
    this.position = 0;
    this.m00 = m00;
    this.m10 = m10;
    this.m11 = m11;
    this.m01 = m01;
  }
  public Waveplate(
    float m00r, float m00i, 
    float m10r, float m10i, 
    float m01r, float m01i, 
    float m11r, float m11i)
  {
    this.position = 0;
    this.m00 = new Complex(m00r,m00i);
    this.m10 = new Complex(m10r,m10i);
    this.m01 = new Complex(m01r,m01i);
    this.m11 = new Complex(m11r,m11i);
  }
  public static Waveplate operator *(Waveplate a, Waveplate b)
  {
    return new Waveplate(
        a.m01 * b.m10 + a.m00 * b.m00,
        a.m10 * b.m00 + a.m11 * b.m10,
        a.m00 * b.m01 + a.m01 * b.m11,
        a.m11 * b.m11 + a.m10 * b.m01
      );
  }
  public static Waveplate operator +(Waveplate a, Waveplate b)
  {
    return new Waveplate()
    {
      m00 = a.m00 + b.m00,
      m01 = a.m01 + b.m01,
      m10 = a.m10 + b.m10,
      m11 = a.m11 + b.m11
    };
  }
  public static Waveplate ID()
  {
    return new Waveplate(1, 0, 0, 0, 0, 0, 1, 0);
  }
  public static Waveplate QuarterWaveplate()
  {
    return new Waveplate(1, 0, 0, 0, 0, 0, 0, 1);
  }
  public static Waveplate LinearPolariser()
  {
    return new Waveplate(1, 0, 0, 0, 0, 0, 0, 1);
  }
  public static Waveplate Rotation(float theta)
  {
    var c = Mathf.Cos(theta);
    var s = Mathf.Sin(theta);
    return new Waveplate(c, 0, s, 0, -s, 0, c, 1);
  }
}

public class WaveSim : MonoBehaviour {

	// Use this for initialization
	void Start () {
	  
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
