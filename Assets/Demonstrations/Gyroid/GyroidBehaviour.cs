using UnityEngine;
using System.Collections;

///Code to handle changing the view of the camera in the gyroid demo.
public class GyroidBehaviour : MonoBehaviour
{

  Cardboard cardboardMain;
  CardboardHead cardboardHead;
  bool lookMode;

  // Use this for initialization
  void Start()
  {
    cardboardMain = GetComponent<Cardboard>();
    cardboardHead = GetComponentInChildren<CardboardHead>();
    ChangeView();
  }

  // Update is called once per frame
  void Update()
  {
    if (cardboardMain.CardboardTriggered)
    {
      ChangeView();
    }
  }

  private void ChangeView()
  {
    var go = cardboardMain.gameObject;
    if (lookMode)
    {
      var size = GameObject.Find("tesselator").GetComponent<tesselate>().size;
      cardboardHead.lookAroundDistance = 10 * size;
      go.transform.localPosition = new Vector3(0, 0, 0);
    }
    else
    {
      cardboardHead.lookAroundDistance = 0;
      var t = 1f;
      go.transform.localPosition = new Vector3(1, 1.5f, 0);
    }
    lookMode = !lookMode;
  }
}
