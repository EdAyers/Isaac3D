using UnityEngine;
using System.Collections;

public class TradAtomScript : MonoBehaviour {

  GameObject[] electrons;
  Vector3 a1 = (Vector3.up + Vector3.right).normalized;
  Vector3 a2 = (Vector3.up - Vector3.right).normalized;
	// Use this for initialization
	void Start () {
    electrons = GameObject.FindGameObjectsWithTag("Electron");
    electrons[0].transform.localPosition = a1 * ORBIT_RADIUS;
    electrons[1].transform.localPosition = a2 * ORBIT_RADIUS;
	}
  const float ORBIT_RADIUS = 1f;
	const float SPEED = 100;
	// Update is called once per frame
	void Update () {
    float angle = Time.deltaTime * SPEED;
    electrons[0].transform.RotateAround(this.transform.position, a2, angle);
    electrons[1].transform.RotateAround(this.transform.position, a1, angle);
	}
}
