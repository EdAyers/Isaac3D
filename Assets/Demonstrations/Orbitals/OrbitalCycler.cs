using UnityEngine;
using System.Collections;
using Assets;

///Script that sends new Calcs to the VolumeRenderComponent on the
///game object on tapping the trigger.
public class OrbitalCycler : MonoBehaviour {

  VolumeRenderComponent vrc;
  Cardboard cardboardMain;
  int calcIndex = 0;
  HydrogenCalc current;
  HydrogenCalc[] calcs =
      {
        new HydrogenCalc(2,1,1),
        new HydrogenCalc(2,1,0),
        //new HydrogenCalc(2,1,-1),
        new HydrogenCalc(3,1,1),
        //new HydrogenCalc(3,1,0),
        //new HydrogenCalc(3,1,-1),
        new HydrogenCalc(3,2,2),
        new HydrogenCalc(3,2,1),
        new HydrogenCalc(3,2,0),
        //new HydrogenCalc(3,2,-1),
        //new HydrogenCalc(3,2,-2),

        //new HydrogenCalc(4,1,1), //TODO these ones don't render correctly because the texture is not hi-fid enough.
        //new HydrogenCalc(4,1,0), //so just don't include them and hope nobody notices.
        //new HydrogenCalc(4,1,-1),

        new HydrogenCalc(4,2,2),
        new HydrogenCalc(4,2,1),
        new HydrogenCalc(4,2,0),
        //new HydrogenCalc(4,2,-1),
        //new HydrogenCalc(4,2,-2),

        new HydrogenCalc(4,3, 3),
        new HydrogenCalc(4,3, 2),
        new HydrogenCalc(4,3, 1),
        new HydrogenCalc(4,3, 0),
        //new HydrogenCalc(4,3,-1),
        //new HydrogenCalc(4,3,-2),
        //new HydrogenCalc(4,3,-3)
      };

	// Use this for initialization
	void Start () {
    vrc = GetComponent<VolumeRenderComponent>();
    cardboardMain = GetComponentInChildren<Cardboard>();
    current = calcs[calcIndex];
    vrc.SetFieldData(current);
	}
	
	// Update is called once per frame
	void Update () {
	
    if (cardboardMain.CardboardTriggered)
    {
      calcIndex++;
      if (calcIndex >= calcs.Length) calcIndex = 0;
      current = calcs[calcIndex];
      vrc.SetFieldData(current);
    }

	}
}
