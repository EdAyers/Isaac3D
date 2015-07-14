using UnityEngine;
using System.Collections;
using Assets;

public class Rotator : MonoBehaviour {

	// Use this for initialization
	void Start () {
    vrc = GetComponent<VolumeRenderComponent>();
	}
  VolumeRenderComponent vrc;
  public bool isRotating = true;
  public float speed = 1f;
  float angle = 0;
	// Update is called once per frame
  void Update()
  {
    if (isRotating)
    {
      float delta = Time.deltaTime * speed;
      if (vrc == null)
      {
        this.transform.Rotate(Vector3.up, delta);
      }
      else
      {
        angle += delta;
        vrc.SetRotation(angle);
      }
    }
  }
}
