using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine.UI;

public class OrbitalMeshCycler : MonoBehaviour {

  Cardboard cardboardMain;

  HydrogenCalc[] calcs =
      {
        new HydrogenCalc(1,0,0),
        new HydrogenCalc(2,0,0),
        new HydrogenCalc(2,1,0),
        new HydrogenCalc(2,1,1),
        new HydrogenCalc(2,1,-1),
        //new HydrogenCalc(3,0,0), //doesnt work properly at the moment; only two shells render
        new HydrogenCalc(3,1,0),
        new HydrogenCalc(3,1,1),
        new HydrogenCalc(3,1,-1),
        new HydrogenCalc(3,2,2),
        new HydrogenCalc(3,2,1),
        new HydrogenCalc(3,2,0),
        new HydrogenCalc(3,2,-1),
        new HydrogenCalc(3,2,-2),

        //new HydrogenCalc(4,1,0),
        //new HydrogenCalc(4,1,1),
        //new HydrogenCalc(4,1,-1),
        new HydrogenCalc(4,2,2),
        new HydrogenCalc(4,2,1),
        new HydrogenCalc(4,2,0),
        new HydrogenCalc(4,2,-1),
        new HydrogenCalc(4,2,-2),

        new HydrogenCalc(4,3,3),
        new HydrogenCalc(4,3,2),
        new HydrogenCalc(4,3,1),
        new HydrogenCalc(4,3,0),
        new HydrogenCalc(4,3,-1),
        new HydrogenCalc(4,3,-2),
        new HydrogenCalc(4,3,-3)

      };

  MeshFilter meshFilter;
  Text orbitalText;
  Text mText;

  int index = 0;
	// Use this for initialization
	void Start () {
    cardboardMain = GetComponentInChildren<Cardboard>();
    meshFilter = GetComponentInChildren<MeshFilter>();
    orbitalText = GameObject.Find("OrbitalLabel").GetComponent<UnityEngine.UI.Text>();
    mText = GameObject.Find("MLabel").GetComponent<UnityEngine.UI.Text>();
    RefreshMesh();
	}

  void RefreshMesh()
  {
    var current = calcs[index];
    meshFilter.mesh = current.GetMesh();
    orbitalText.text = current.OrbitalLabel;
    mText.text = current.MLabel;
    meshFilter.transform.localRotation = Quaternion.AngleAxis(current.Rotation * 180 / Mathf.PI, Vector3.up);
  }

	// Update is called once per frame
	void Update () {
    if (cardboardMain == null) return;
    if (cardboardMain.CardboardTriggered)
    {
      index++;
      if (index >= calcs.Length) index = 0;
      RefreshMesh();
    }
    
	}
}
