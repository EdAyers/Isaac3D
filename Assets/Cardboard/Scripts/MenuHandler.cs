using UnityEngine;
using System.Collections;

public class MenuHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

  float startTime;
  const float TIME_THRESHOLD = 0.1f;
  const float POS_THRESHOLD = 0.1f;
  bool timing = false;

	// Update is called once per frame
	void Update () {
    if (Application.loadedLevel != 0)
    {
      if ((Input.acceleration - Vector3.right).magnitude < POS_THRESHOLD)
      {
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
        timing = false;
      }
    }
	}
}
