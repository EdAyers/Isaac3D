using UnityEngine;
using System.Collections;
using Assets;

///Script that sends new Calcs to the VolumeRenderComponent on the
///game object on tapping the trigger.
public class OrbitalCycler : MonoBehaviour {

  VolumeRenderComponent vrc;
  Cardboard cardboardMain;
  int calcIndex = 0;


  UnityEngine.UI.Text orbitalText;
  UnityEngine.UI.Text mText;

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
    orbitalText = GameObject.Find("OrbitalLabel").GetComponent<UnityEngine.UI.Text>();
    mText = GameObject.Find("MLabel").GetComponent<UnityEngine.UI.Text>();
    RefreshData();
	}
	
	// Update is called once per frame
	void Update () {
    if (cardboardMain == null) return;
    if (cardboardMain.CardboardTriggered)
    {
      calcIndex++;
      if (calcIndex >= calcs.Length) { calcIndex = 0; }
      RefreshData();
    }

	}

  void RefreshData()
  {
      current = calcs[calcIndex];
      vrc.SetFieldData(current);
      if (orbitalText != null && mText != null)
      {
        orbitalText.text = current.OrbitalLabel;
        mText.text = current.MLabel;
      }      
  }
}
