using UnityEngine;
using System.Collections;

public class HexagonMeshGenerator : MonoBehaviour {

	// Use this for initialization
	void Start () {
    
    var meshfilter = GetComponent<MeshFilter>() ;
    if (meshfilter == null)
    {
      return;
    }

    Mesh mesh = new Mesh();
    Vector3[] vertices = new Vector3[6];
    Vector3[] normals = new Vector3[6];
    int[] triangles = new int[]
      {
        0,1,2,
        0,2,5,
        5,2,3,
        5,3,4
      };

    for (int i = 0; i < 6; i++)
    {
      float t = (2 * Mathf.PI) * i / 6f;
      vertices[i] = new Vector3(Mathf.Sin(t), Mathf.Cos(t));
      normals[i] = new Vector3(0, 0, 1);
    }

    mesh.vertices = vertices;
    mesh.normals = normals;
    mesh.triangles = triangles;

    meshfilter.mesh = mesh;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
