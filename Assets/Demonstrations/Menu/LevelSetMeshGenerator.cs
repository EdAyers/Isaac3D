using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Assets;

interface ILevelFunc
{
  float F(Vector3 v);
}

class SphereLevel : ILevelFunc
{
  public float F(Vector3 v)
  {
      var r = v.magnitude;
      return r;
  }
}

public class LevelSetMeshGenerator : MonoBehaviour
{
  ILevelFunc func;
  float level = 0.5f;
  const int SAMPLES = 124;

  public float min = -1;
  public float max = 1;

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
    public int Key
    {
      get
      {
        return Cube.CalcKey(i, j, k);
      }
    }
    public static int CalcKey(int i, int j, int k)
    {
      return (i * SAMPLES * SAMPLES + j * SAMPLES + k);
    }
    public Cube(int i, int j, int k)
    {
      this.i = i; this.j = j; this.k = k;
      this.points = new List<Point>();
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
  Cube GetOrNewCube(int i, int j, int k)
  {
    if (i < 0 || i >= SAMPLES) return null;
    if (j < 0 || j >= SAMPLES) return null;
    if (k < 0 || k >= SAMPLES) return null;
    int key = Cube.CalcKey(i, j, k);
    Cube cube;
    if (cubes.TryGetValue(key, out cube))
    {
      return cube;
    }
    else
    {
      cube = new Cube(i, j, k);
      cubes.Add(key, cube);
      return cube;
    }
  }
  void AddAPointToCube(int i, int j, int k, Point point)
  {
    var cube = GetOrNewCube(i, j, k);
    if (cube != null)
    {
      cube.points.Add(point);
    }
  }
  
  Vector3 Normal(Vector3 pos)
  {
    float delta = (max - min) / SAMPLES * 2;
    float x = func.F(pos + Vector3.right * delta) 
      - func.F(pos - Vector3.right * delta);
    float y = func.F(pos + Vector3.up * delta) 
      - func.F(pos - Vector3.up * delta);
    float z = func.F(pos + Vector3.forward * delta) 
      - func.F(pos - Vector3.forward * delta);
    return new Vector3(x, y, z).normalized;
  }
  void EdgeCalc(Vector3 v0, Vector3 v1, Dir dir, int i, int j, int k)
  {
    var f1 = func.F(v1);
    var f0 = func.F(v0);
    if ((f1 > level && f0 < level)
         || (f1 < level && f0 > level))
    {
      //surface intersects this cube.
      var pos = Vector3.Lerp(v0, v1, Mathf.Abs(level - f0) / Mathf.Abs(f1 - f0));
      var norm = Normal(pos);
      Point p = new Point()
      {
        position = pos,
        up = norm,
        ID = points.Count
      };
      Cube c = GetOrNewCube(i, j, k);
      c.points.Add(p);
      points.Add(p);
      //match up other 3 cubes
      if (dir == Dir.X)
      {
        AddAPointToCube(i, j, k - 1, p);
        AddAPointToCube(i, j - 1, k - 1, p);
        AddAPointToCube(i, j - 1, k, p);
      }
      else if (dir == Dir.Y)
      {
        AddAPointToCube(i - 1, j, k, p);
        AddAPointToCube(i - 1, j, k - 1, p);
        AddAPointToCube(i, j, k - 1, p);
      }
      else if (dir == Dir.Z)
      {
        AddAPointToCube(i - 1, j, k, p);
        AddAPointToCube(i - 1, j - 1, k, p);
        AddAPointToCube(i, j - 1, k, p);

      }
    }
  }
  float AngleAbout(Vector3 axis, Vector3 startPos, Vector3 x)
  {
    var a = axis.normalized;
    var projStart = Vector3.ProjectOnPlane(startPos, a);
    var projX = Vector3.ProjectOnPlane(x, a);
    float c = Vector3.Dot(projStart, projX);
    float s = Vector3.Dot(Vector3.Cross(projStart, projX), a);
    return Mathf.Atan2(s, c);
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
      var cube = e.Current.Value;
      if (cube.points.Count >= 3)
      {
        //sort the points into a clockwise order
        Vector3 mean = Vector3.zero;
        Vector3 normal = Vector3.zero;
        
        foreach (Point p in cube.points)
        {
          mean += (p.position / cube.points.Count);
          normal += p.up;
        }
        var a = cube.points[0].position - mean;
        bool rev = false;
        var ordered = cube.points.OrderBy(x =>
               AngleAbout(normal, a, x.position - mean)
          ).Select(x => x.ID).ToArray();
        for (int n = 0; n < cube.points.Count - 2; n++)
        {
          if (rev)
          {
            triangles.Add(ordered[0]);
            triangles.Add(ordered[n + 1]);
            triangles.Add(ordered[n + 2]);
          }
          else
          {
            triangles.Add(ordered[n + 1]);
            triangles.Add(ordered[0]);
            triangles.Add(ordered[n + 2]);
          }
        }
      }
    }

    Vector3[] vertices = new Vector3[points.Count];
    Vector3[] normals = new Vector3[points.Count];
    for (int i = 0; i < points.Count; i++)
    {
      vertices[i] = points[i].position;
      normals[i] =  points[i].up;
    }
    var tris = triangles.ToArray();
    Mesh mesh = new Mesh();
    mesh.vertices = vertices;
    mesh.normals = normals;
    mesh.triangles = tris;
    //mesh.RecalculateNormals();
    mesh.Optimize();
    Debug.Log(vertices.Length);
    return mesh;
  }

  MeshFilter mf;
  // Use this for initialization
  void Start()
  {
    mf = GetComponent<MeshFilter>();
    var calc = new HydrogenCalc(4, 2, 2);
    level = calc.GetLevelSetValue(0.5f);
    func = calc;
    mf.mesh = Gen();
  }

  // Update is called once per frame
  void Update()
  {

  }
}
