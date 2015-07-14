using UnityEngine;
using System.Collections;
using System.Collections.Generic;

interface ILevelFunc
{
  float F(Vector3 v);
}

class LevelSetGen
{
  ILevelFunc func;
  float level;
  const int SAMPLES = 1000;

  float min;
  float max;

  struct Point
  {
    public Vector3 position;
    public Vector3 up;
    public int ID;
  }
  class Cube
  {
    public int i;
    public int j;
    public int k;
    public List<Point> points;
    public int ID
    {
      get
      {
        return Cube.Index(i, j, k);
      }
    }
    public static int Index(int i, int j, int k)
    {
      return (i * SAMPLES * SAMPLES + j * SAMPLES + k);
    }
  }
  Vector3 Pos(int i, int j, int k)
  {
    float x = Mathf.Lerp(min, max, (float)i / SAMPLES);
    float y = Mathf.Lerp(min, max, (float)j / SAMPLES);
    float z = Mathf.Lerp(min, max, (float)k / SAMPLES);
    return new Vector3(x, y, z);
  }
  enum Dir { X, Y, Z }
  Dictionary<int, Cube> cubes;
  List<Point> points;
  void EdgeCalc(Vector3 v0, Vector3 v1, Dir dir, int i, int j, int k)
  {
    var f1 = func.F(v1);
    var f0 = func.F(v0);
    if (    (f1 > level && f0 < level)
         || (f1 < level && f0 > level))
    {
      //surface intersects this cube.
      Point p = new Point()
      {
        position = Vector3.Lerp(v0, v1, Mathf.Abs(level - f0) / Mathf.Abs(f1 - f0)),
        up = (v1 - v0),
        ID = points.Count
      };
      Cube c = new Cube()
      {
        i = i,
        j = j,
        k = k,
        points = new List<Point>()
      };
      c.points.Add(p);
      points.Add(p);
      cubes.Add(c.ID, c);
      //match up other 3 cubes
      if (dir == Dir.X)
      {
        if (k > 0)
        {
          cubes[Cube.Index(i, j, k - 1)].points.Add(p);
        }
        if (j > 0)
        {
          cubes[Cube.Index(i, j - 1, k)].points.Add(p);
        }
        if (j > 0 && k > 0)
        {
          cubes[Cube.Index(i, j - 1, k - 1)].points.Add(p);
        }
      }
      if (dir == Dir.Y)
      {
        if (k > 0)
        {
          cubes[Cube.Index(i, j, k - 1)].points.Add(p);
        }
        if (i > 0)
        {
          cubes[Cube.Index(i - 1, j, k)].points.Add(p);
        }
        if (i > 0 && k > 0)
        {
          cubes[Cube.Index(i - 1, j, k - 1)].points.Add(p);
        }
      } 
      if (dir == Dir.Z)
      {
        if (i > 0)
        {
          cubes[Cube.Index(i- 1, j, k )].points.Add(p);
        }
        if (j > 0)
        {
          cubes[Cube.Index(i, j - 1, k)].points.Add(p);
        }
        if (j > 0 && i > 0)
        {
          cubes[Cube.Index(i - 1, j - 1, k)].points.Add(p);
        }
      }
    }
  }
  Mesh Gen()
  {
    cubes = new Dictionary<int, Cube>();
    points = new List<Point>();
    float d = (max - min) / SAMPLES;
    for (int i = 0; i < SAMPLES; i++)
      for (int j = 0; j < SAMPLES; j++)
        for (int k = 0; k < SAMPLES; k++)
        {
          var v0 = Pos(i, j, k);
          var vx = Pos(i + 1, j, k);
          var vy = Pos(i, j + 1, k);
          var vz = Pos(i, j, k + 1);
          EdgeCalc(v0, vx, Dir.X, i, j, k);
          EdgeCalc(v0, vy, Dir.Y, i, j, k);
          EdgeCalc(v0, vz, Dir.Z, i, j, k);
        }

    List<int> triangles = new List<int>();
    var e = cubes.GetEnumerator();
    
    while (e.MoveNext())
    {
      //generate triangles
      var cube = e.Current.Value;
      if (cube.points.Count == 3)
      {
        var a = cube.points[0].position;
        var b = cube.points[1].position;
        var c = cube.points[2].position;
        //integers need to be provided clockwise looking onto face.
        var normal = Vector3.Cross(b - a, c - a);
        if (Vector3.Angle(normal, cube.points[0].up) < 90)
        {
          triangles.Add(cube.points[0].ID);
          triangles.Add(cube.points[1].ID);
          triangles.Add(cube.points[2].ID);
        }
        else
        {
          triangles.Add(cube.points[0].ID);
          triangles.Add(cube.points[2].ID);
          triangles.Add(cube.points[1].ID);
        }
      }
      if (cube.points.Count == 4)
      {
        //we assume that the surface in cube is close enough to
        //a euclidian plane that we can just consider one triangle
        var a = cube.points[0].position;
        var b = cube.points[1].position;
        var c = cube.points[2].position;
        //integers need to be provided clockwise looking onto face.
        var normal = Vector3.Cross(b - a, c - a);
        if (Vector3.Angle(normal, cube.points[0].up) < 90)
        {
          triangles.Add(cube.points[0].ID);
          triangles.Add(cube.points[1].ID);
          triangles.Add(cube.points[2].ID);

          triangles.Add(cube.points[0].ID);
          triangles.Add(cube.points[2].ID);
          triangles.Add(cube.points[3].ID);
        }
        else
        {
          triangles.Add(cube.points[0].ID);
          triangles.Add(cube.points[2].ID);
          triangles.Add(cube.points[1].ID);

          triangles.Add(cube.points[0].ID);
          triangles.Add(cube.points[3].ID);
          triangles.Add(cube.points[2].ID);
        }
      }
      else
      {
        Debug.Log("found a cube with points length of " + cube.points.Count);
      }
    }

    Vector3[] vertices = new Vector3[points.Count];
    for(int i = 0; i < points.Count; i++)
    {
      vertices[i] = points[i].position;
    }
    var tris = triangles.ToArray();
    Mesh mesh = new Mesh();
    mesh.triangles = tris;
    mesh.vertices = vertices;
    return mesh;
  }

}


public class LevelSetMeshGenerator : MonoBehaviour
{

  // Use this for initialization
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }
}
