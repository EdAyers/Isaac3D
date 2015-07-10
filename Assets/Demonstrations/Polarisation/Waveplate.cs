using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
  ///Class representing a waveplate.
  abstract class Waveplate
  {

    public abstract float StartPosition { get; set; }
    public abstract float FinishPosition { get; }
    public abstract CVector2 GetWave(CVector2 start, float wavenumber, float depth);
    public abstract Color LineColor { get; }
  }

  ///A birefringent waveplate.
  class Birefringent : Waveplate
  {
    public float Thickness { get; set; }
    CVector2 refractiveIndex = new CVector2(1 / 1, 0, 1 / 2f, 0);

    public override Color LineColor { get { return new Color(0f, 0, 1f, 1); } }

    public override float StartPosition { get; set; }

    public override float FinishPosition { get { return StartPosition + Thickness; } }

    public override CVector2 GetWave(CVector2 start, float wavenumber, float depth)
    {
      //high n means it's slow.

      CVector2 phase = -wavenumber * depth * refractiveIndex;
      var rotated = new CVector2(
        start.X * Complex.Rotate(phase.X.R),
        start.Y * Complex.Rotate(phase.Y.R));
      return rotated;
    }
  }
}
