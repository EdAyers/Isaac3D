using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuBehaviour : MonoBehaviour {

  public int index = 0;
  public int levelNo = 0;
  Canvas canvas;
  
  Cardboard c;

	// Use this for initialization
	void Start () {
	  //rotate the canvas to be pointing up
    //canvas = this.GetComponentInChildren<Canvas>();
    //canvas.transform.LookAt(transform.position * 3, new Vector3(0, 1, 0));
    c = GameObject.Find("CardboardMain").GetComponent<Cardboard>();
	}
	
	// Update is called once per frame
	void Update () {
	  if (c.CardboardTriggered)
    {
      RaycastHit hit;
      var cam = c.GetComponentInChildren<Camera>();
      if(Physics.Raycast (cam.transform.position, cam.transform.forward, out hit, 500))
      {
        Debug.Log("raycast hit");
        if (hit.collider == this.GetComponentInChildren<MeshCollider>())
        {
          GetComponentInChildren<Text>().text = "LOADING...";
          Application.LoadLevel(levelNo);
        }
      }
    }
	}
}
