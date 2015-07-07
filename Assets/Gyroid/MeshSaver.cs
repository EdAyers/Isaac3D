
using UnityEditor;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class MeshSaverEditor
{

  [MenuItem("CONTEXT/MeshFilter/Save Mesh...")]
  public static void SaveMeshInPlace(MenuCommand menuCommand)
  {
    MeshFilter mf = menuCommand.context as MeshFilter;
    Mesh m = mf.sharedMesh;
    SaveMesh(m, m.name, false, true);
  }

  [MenuItem("CONTEXT/MeshFilter/Save Mesh As New Instance...")]
  public static void SaveMeshNewInstanceItem(MenuCommand menuCommand)
  {
  }

  [MenuItem("CONTEXT/MeshFilter/Invert and save...")]
  public static void InvertMeshAndSave(MenuCommand menuCommand)
  {
    MeshFilter mf = menuCommand.context as MeshFilter;
    Mesh mesh = Object.Instantiate(mf.sharedMesh);

    Vector3[] normals = mesh.normals;
    for (int i = 0; i < normals.Length; i++)
      normals[i] = -normals[i];
    mesh.normals = normals;

    for (int m = 0; m < mesh.subMeshCount; m++)
    {
      int[] triangles = mesh.GetTriangles(m);
      for (int i = 0; i < triangles.Length; i += 3)
      {
        int temp = triangles[i + 0];
        triangles[i + 0] = triangles[i + 1];
        triangles[i + 1] = temp;
      }
      mesh.SetTriangles(triangles, m);
    }

    SaveMesh(mesh, mesh.name + "_inverted", false, true);

  }

  public static void SaveMesh(Mesh mesh, string name, bool makeNewInstance, bool optimizeMesh)
  {
    string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", name, "asset");
    if (string.IsNullOrEmpty(path)) return;

    path = FileUtil.GetProjectRelativePath(path);

    Mesh meshToSave = (makeNewInstance) ? Object.Instantiate(mesh) as Mesh : mesh;

    if (optimizeMesh)
      meshToSave.Optimize();

    AssetDatabase.CreateAsset(meshToSave, path);
    AssetDatabase.SaveAssets();
  }

}
