using UnityEngine;
using System.Collections;
using UnityEngine.UI;

///Script to handle going back when in a demo.
///Providing the requisite canvas with images is attached 
///(comes in the Cardboard/Prefabs/CardboardMain prefab)
///this displays a back GUI and will go back when the user
///tilts the phone to be in the protrait orientation.
public class MenuHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
    canvas = GameObject.Find("BackUICanvas").GetComponent<Canvas>();
    canvas.enabled = true;
    backImage = GameObject.Find("BackImage").GetComponent<RawImage>();
    backTargetImage = GameObject.Find("BackTargetImage").GetComponent<RawImage>();
    backImage.enabled = false;
    backTargetImage.enabled = false;
    Color.TryParseHexString("44AA44FF", out isaacGreen);
    ringColor = isaacGreen;
	}

  Canvas canvas;
  RawImage backImage;
  RawImage backTargetImage;
  Color isaacGreen;
  Color ringColor;
  float startTime;
  const float TIME_THRESHOLD = 0.1f;
  const float POS_THRESHOLD = 0.2f;
  bool timing = false;

  public Vector3 fakeAcc;

	// Update is called once per frame
	void Update () {
    if (Application.loadedLevel != 0)
    {
#if UNITY_EDITOR
      var acc = fakeAcc.normalized;
#else
      var acc = Input.acceleration;
#endif
      float dist = (acc - Vector3.right).magnitude;
      if (dist < 1)
      {
        canvas.enabled = true;
        backImage.enabled = true;
        backTargetImage.enabled = true;
        float a = Mathf.Lerp(0, 1, (0.8f - dist) * 2);
        backTargetImage.color = new Color(
          ringColor.r, 
          ringColor.g, 
          ringColor.b, 
          a);
        backImage.color = new Color(1, 1, 1, a);
        backImage.transform.localPosition = new Vector3(acc.y, acc.z,0) * 5;

      }
      else
      {
        canvas.enabled = false;
      }

      if ((acc - Vector3.right).magnitude < POS_THRESHOLD)
      {
        ringColor = Color.Lerp(Color.red, Color.white, 0.5f);
        if (!timing)
        {
          startTime = Time.time;
          timing = true;
        }
        else
        {
          if ((Time.time - startTime) > TIME_THRESHOLD)
          {
            timing = false;
            Handheld.Vibrate();
            Application.LoadLevel(0);
          }
        }
      }
      else if (timing)
      {
        ringColor = isaacGreen;
        timing = false;
      }
    }
    else
    {
      canvas.enabled = false;
    }
	}
}
