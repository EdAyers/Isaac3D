using UnityEngine;
using System.Collections;

public class tesselate : MonoBehaviour
{

  public GameObject prefab;
  public float spacing = Mathf.PI * 2;
  public int size = 4;

  Cardboard cardboardMain;
  CardboardHead cardboardHead;
  // Use this for initialization
  void Start()
  {
    var c = spacing * (size - 1) / 2;
    Vector3 center = new Vector3(c, c, c);
    for (int i = 0; i < size; i++)
    {
      for (int j = 0; j < size; j++)
      {
        for (int k = 0; k < size; k++)
        {
          Vector3 pos = new Vector3(i * spacing, j * spacing, k * spacing) - center;
          var go = (GameObject) Instantiate(prefab, pos, Quaternion.identity);
          go.transform.SetParent(this.transform);
        }
      }
    }
    cardboardMain = GetComponentInChildren<Cardboard>();
    cardboardHead = GetComponentInChildren<CardboardHead>();
  }

  bool lookMode;

  // Update is called once per frame
  void Update()
  {

  }
}
