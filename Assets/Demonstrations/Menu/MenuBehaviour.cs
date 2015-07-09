using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuBehaviour : MonoBehaviour
{

  public int index = 0;
  public int levelNo = 0;
  Canvas canvas;

  Cardboard cardboard;
  Collider collider;
  Camera camera;

  // Use this for initialization
  void Start()
  {
    //rotate the canvas to be pointing up
    //canvas = this.GetComponentInChildren<Canvas>();
    //canvas.transform.LookAt(transform.position * 3, new Vector3(0, 1, 0));
    cardboard = GameObject.Find("CardboardMain").GetComponent<Cardboard>();
    camera = cardboard.GetComponentInChildren<Camera>();
    collider = this.GetComponentInChildren<MeshCollider>();
  }

  bool isSelected = false;

  // Update is called once per frame
  void Update()
  {

    RaycastHit hit;
    if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 500))
    {
      if (hit.collider == collider)
      {
        if (cardboard.CardboardTriggered)
        {
            GetComponentInChildren<Text>().text = "LOADING...";
            Application.LoadLevel(levelNo);
        }
        else
        {
          isSelected = true;
        }
      }

    }

  }
}
