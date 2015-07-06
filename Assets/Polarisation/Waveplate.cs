using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
  class Waveplate
  {
    float position;
    
  }

  class Birefringent : Waveplate
  {
    float thickness = 1;
    CVector2 refractiveIndex = new CVector2(1,0,1.1f, 0);

    public CVector2 GetWave(CVector2 startVector, float delta)
    {
      var rotatePower = delta * power;
       
    }
  }
}
