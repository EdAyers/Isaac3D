using UnityEngine;
using System.Collections;

///Makes meshes for regular 2D polygons.
///Attach this behaviour to an element, make the mesh and then use MeshSaver to save it to assets.
public class HexagonMeshGenerator : MonoBehaviour {
  public int n = 6;
	// Use this for initialization
	void Start () {
    var meshfilter = GetComponent<MeshFilter>() ;
    if (meshfilter == null)
    {
      return;
    }
    meshfilter.mesh = MakeNGon(n);
	}
  Mesh MakeNGon(int n)
  {
    if (n < 3)
    {
      Debug.LogError("n was less than 3");
      return null;
    }
    Mesh mesh = new Mesh();
    Vector3[] vertices = new Vector3[n];
    Vector3[] normals = new Vector3[n];
    int[] triangles = new int[3 * (n - 2)];

    for (int i = 0; i < n; i++)
    {
      float t = (2 * Mathf.PI) * i / n;
      vertices[i] = new Vector3(Mathf.Sin(t), Mathf.Cos(t));
      normals[i] = new Vector3(0, 0, 1);
    }
    for (int i = 0; i < (n - 2); i++ )
    {
      var j = i * 3;
      triangles[j + 0] = 0;
      triangles[j + 1] = i + 1;
      triangles[j + 2] = i + 2;
    }
    mesh.vertices = vertices;
    mesh.normals = normals;
    mesh.triangles = triangles;
    return mesh;
  }
}
