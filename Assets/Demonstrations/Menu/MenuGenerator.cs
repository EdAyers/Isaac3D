using UnityEngine;
using System.Collections;

public class MenuGenerator : MonoBehaviour
{

  public GameObject hexPrefab;
  public GameObject pentagonPrefab;
  public int layers = 5;

  // Use this for initialization
  void Start()
  {
    CreateGrid();
  }

  void EvenPermutations(Vector3 start, int index, Vector3[] result)
  {
    var c = new float[] { start.x, start.y, start.z };
    for (int i = 0; i < 3; i++)
    {
      //for lists of length 3, even permutations are just cycles
      result[index + i] = new Vector3(
          c[(0 + i) % 3],
          c[(1 + i) % 3],
          c[(2 + i) % 3]
        );
    }
  }

  void Negations3(Vector3 start, int index, Vector3[] result)
  {
    for (int i = 0; i < 8; i++)
    {
      result[index + i] = new Vector3(
         (i & 1) == 0 ? start.x : -start.x,
         (i & 2) == 0 ? start.y : -start.y,
         (i & 4) == 0 ? start.z : -start.z
        );
    }
  }

  void CreateGrid()
  {
    float d = Mathf.PI / 2 / (layers - 1);
    //Debug.LogFormat("")

    //vertices on a icosohedron; where the pentagons are
    var a = 1.0f;
    var phi = (1 + Mathf.Sqrt(5)) / 2;
    Vector3[] verts = new Vector3[12];
    EvenPermutations(new Vector3(1, phi) * a, 0, verts);
    EvenPermutations(new Vector3(-1, phi) * a, 3, verts);
    EvenPermutations(new Vector3(-1, -phi) * a, 6, verts);
    EvenPermutations(new Vector3(1, -phi) * a, 9, verts);
    Vector3[] edgeCenters = new Vector3[20];
    var r = verts[0].magnitude;// *0.75f / 0.95f;
    var e1 = r * Vector3.Normalize(verts[0] + verts[1] + verts[4]);
    var e2 = r * Vector3.Normalize(verts[0] + verts[1] + verts[2]);
    Debug.LogFormat("({0},{1},{2})", e1.x, e1.y, e1.z);
    var x = e1.x;
    var y = e1.y;
    EvenPermutations(new Vector3(x, y), 0, edgeCenters);
    EvenPermutations(new Vector3(-x, y), 3, edgeCenters);
    EvenPermutations(new Vector3(-x, -y), 6, edgeCenters);
    EvenPermutations(new Vector3(x, -y), 9, edgeCenters);
    Negations3(e2, 12, edgeCenters);

    for(int i = 0; i < edgeCenters.Length; i++)
    {
      var pos = edgeCenters[i];
      Quaternion quat = Quaternion.LookRotation(pos, new Vector3(0, 1, 0));
      //I'm slightly ashamed of this next bit.
      //I couldnt figure out a slick way of figuring out the rotation of the hexagons
      if (i == 0 || i == 2 || i == 3 || i == 5 || i == 6 || i == 8 || i == 9 || i == 11) 
      {
        quat = Quaternion.AngleAxis(30f, pos) * quat;
      }
      if ( i == 13 || i == 16 || i == 14 || i == 19) 
      {
        quat = Quaternion.AngleAxis(7, pos) * quat;
      }
      if (i == 12 ||  i == 17 || i == 15 || i == 18 ) 
      {
        quat = Quaternion.AngleAxis(-7, pos) * quat;
      }
      var go = (GameObject)Instantiate(hexPrefab, pos * 2, 
        quat);
      go.GetComponentInChildren<UnityEngine.UI.Text>().text = i.ToString();
      go.transform.SetParent(this.transform); 
    }
    for(int i = 0; i < verts.Length; i++)
    {
      var pos = verts[i];
      Vector3 up = new Vector3(0,0,0);
      switch (i%3)
      {
        case 0: up = new Vector3(-pos.x, 0, 0); break;
        case 2: up = new Vector3(0, -pos.y, 0); break;
        case 1: up = new Vector3(0, 0, -pos.z); break;
      }
      var quat = Quaternion.LookRotation(pos,up);
      var go = (GameObject)Instantiate(pentagonPrefab, pos * 2, quat);
      go.transform.SetParent(this.transform);
    }

    //Debug.Log("made it ");

  }

  // Update is called once per frame
  void Update()
  {

  }
}
