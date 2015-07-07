using UnityEngine;
using System.Collections;

public class MenuGenerator : MonoBehaviour {

  public GameObject UIprefab;
  public int layers = 5;

	// Use this for initialization
	void Start () {
    CreateGrid();
	}

  void CreateGrid()
  {
    float d = Mathf.PI / 2 / (layers - 1);
    //Debug.LogFormat("")
    for (int i = 1 - layers; i < layers; i++)
    {
      var theta = i * d;
      int abs = System.Math.Abs(i);
      if (abs == layers - 1)
      { 
        //don't bother for now
      }
      else
      {
        int radius = (abs == layers - 1) ? 1 : (layers - abs - 1) * 6;
        float dp = 2 * Mathf.PI / radius;
        Debug.Log(radius);
        for (int j = 0; j < radius; j++)
        {
          var phi = j * dp;
          var pos = new Vector3(
            Mathf.Sin(phi) * Mathf.Cos(theta),
            Mathf.Sin(theta),
            Mathf.Cos(phi) * Mathf.Cos(theta));
          pos *= 5;
          var go = (GameObject)Instantiate(UIprefab, pos, Quaternion.LookRotation(pos));
          go.transform.SetParent(this.transform);
          //Debug.Log("made it ");
        }
      }
    }

  }
	
	// Update is called once per frame
	void Update () {
	
	}
}
