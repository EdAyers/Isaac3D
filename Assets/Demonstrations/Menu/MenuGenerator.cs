using UnityEngine;
using System.Collections;

///This code was over-ambitious; The idea was to find points of the
///the face centers for the hexagons of a multiply truncated
///iscosohedron. This would render as a globe of hexagons which the
///viewer would be inside and be able to look around and click on menu items.
///I've decided instead to manually place the hexagons because this was too much faff
///for what it's worth.
public class MenuGenerator : MonoBehaviour
{
  public GameObject hexPrefab;
  public GameObject pentagonPrefab;
  public int layers = 5;
  public float magicSeven = 0;
  public float magicThirty = 0;
  public float platesep;

  Vector3[] verts;
  Vector3[] edgeCenters;

  void Start()
  {
    CreateGrid();
    MakeElements();
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

  //Potential employers reading this code:
  //This is super-hacky. Instead of judging me, please
  //see this as proof that I can adapt my coding style for
  //speed over readability, functionality, dignity for
  //appropriate situations. I'm versatile!

  void CreateGrid()
  {
    float d = Mathf.PI / 2 / (layers - 1);
    //Debug.LogFormat("")

    //vertices on a icosohedron; where the pentagons are
    var a = 1.0f;
    var phi = (1 + Mathf.Sqrt(5)) / 2;
    verts = new Vector3[12];
    EvenPermutations(new Vector3(1, phi) * a, 0, verts);
    EvenPermutations(new Vector3(-1, phi) * a, 3, verts);
    EvenPermutations(new Vector3(-1, -phi) * a, 6, verts);
    EvenPermutations(new Vector3(1, -phi) * a, 9, verts);
    edgeCenters = new Vector3[20];
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


  }

  private Quaternion FindZRotate(int i, Vector3 pos)
  {
    Quaternion zRotate = Quaternion.identity;
    //I'm slightly ashamed of this next bit.
    //I couldnt figure out a slick way of finding out the rotation of the hexagons.
    //so I put labels on them and hand wrote the results.
    if (i == 0 || i == 2 || i == 3 || i == 5 || i == 6 || i == 8 || i == 9 || i == 11)
    {
      zRotate = Quaternion.AngleAxis(magicThirty, pos);
    }
    if (i == 13 || i == 16 || i == 14 || i == 19)
    {
      //I don't really know why 7 works.
      zRotate = Quaternion.AngleAxis(magicSeven, pos);
    }
    if (i == 12 || i == 17 || i == 15 || i == 18)
    {
      zRotate = Quaternion.AngleAxis(-magicSeven, pos);
    }
    return zRotate;
  }

  void PositionElements()
  {
    for (int i = 0; i < edgeCenters.Length; i++)
    {
      var pos = edgeCenters[i];

      for (int j = 0; j < 3; j++)
      {
        float theta = ((float)j) * Mathf.PI * 2 / 3;
        var posOffset = platesep * new Vector3(Mathf.Cos(theta), Mathf.Sin(theta));
        posOffset =
          Quaternion.FromToRotation(Vector3.forward, pos)
          * FindZRotate(i, Vector3.forward)
          * posOffset;
        var finalPosition = Vector3.Normalize(5 * pos + posOffset) * 5;
        var finalRotation =
            FindZRotate(i, finalPosition)
            * Quaternion.FromToRotation(Vector3.forward, finalPosition);

        var go = items[i * 3 + j];
        go.transform.localPosition = finalPosition;
        go.transform.localRotation = finalRotation;

      }

    }
  }

  System.Collections.Generic.List<GameObject> items = new System.Collections.Generic.List<GameObject>();

  void MakeElements()
  {
    for (int i = 0; i < edgeCenters.Length; i++)
    {
      var pos = edgeCenters[i];

      for (int j = 0; j < 3; j++)
      {
        var go = (GameObject)Instantiate(hexPrefab, Vector3.zero, Quaternion.identity);
        items.Add(go);
        var behav = go.GetComponent<MenuBehaviour>();
        behav.index = i * 3 + j;
        go.transform.SetParent(this.transform);

      }

    }
    for (int i = 0; i < verts.Length; i++)
    {
      var pos = verts[i];
      Vector3 up = new Vector3(0, 0, 0);
      switch (i % 3)
      {
        case 0: up = new Vector3(-pos.x, 0, 0); break;
        case 2: up = new Vector3(0, -pos.y, 0); break;
        case 1: up = new Vector3(0, 0, -pos.z); break;
      }
      var quat = Quaternion.LookRotation(pos, up);
      var go = (GameObject)Instantiate(pentagonPrefab, pos * 2, quat);
      go.transform.SetParent(this.transform);
    }
  }

  // Update is called once per frame
  void Update()
  {
    PositionElements();
  }
}
