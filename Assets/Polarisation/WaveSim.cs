using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Assets
{


  public class WaveSim : MonoBehaviour
  {
    static Material lineMaterial;
    public CMatrix2 CMatrix2;
    // Use this for initialization
    void Start()
    {
      Screen.sleepTimeout = SleepTimeout.NeverSleep;


      CMatrix2 = CMatrix2.QuarterWaveplate();

      //add  blocks which represent the polariser
      GameObject root = GameObject.Find("waveRoot");

      //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
      //cube.transform.localScale = new Vector3(0.01f, 1f, 1f);

      //add the line
      CreateLineMaterial();


      //animate the line.
    }
    static void CreateLineMaterial()
    {
      if (!lineMaterial)
      {
        // Unity has a built-in shader that is useful for drawing
        // simple colored things.
        var shader = Shader.Find("Hidden/Internal-Colored");
        lineMaterial = new Material(shader);
        lineMaterial.hideFlags = HideFlags.HideAndDontSave;
        // Turn on alpha blending
        lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        // Turn backface culling off
        lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        // Turn off depth writes
        lineMaterial.SetInt("_ZWrite", 0);
      }
    }

    // Update is called once per frame
    void Update()
    {
      time += 0.01f;
      startAngle += 0.001f;
    }

    public float time = 0.0f;
    public float startAngle = 0.0f;

    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
      CreateLineMaterial();
      // Apply the line material
      lineMaterial.SetPass(0);

      GL.PushMatrix();
      // Set transformation matrix for drawing to
      // match our transform
      //GL.MultMatrix(transform.localToWorldMatrix);

      // Draw lines
      GL.Begin(GL.LINES);
      GL.Color(new Color(1, 0, 0, 0.8F));
      // One vertex at transform position
      for (int i = 0; i < 1000; i++)
      {
        var x = ((float)i / 500 - 1.0f) * 10;
        var phase = new Complex(1, (time - x) * 10.0f);
        var exp = 0.1f * Complex.Exp(phase);
        var jonesVectorX = Mathf.Cos(startAngle) * exp;
        var jonesVectorY = Mathf.Sin(startAngle) * exp;
        if (i == 0) { GL.Vertex3(x, jonesVectorX.R, jonesVectorY.R); }

        if (x < 0f)
        {
          GL.Vertex3(x, jonesVectorX.R, jonesVectorY.R);
          GL.Vertex3(x, jonesVectorX.R, jonesVectorY.R);
       }
        else
        {
          var result = CMatrix2 * new CMatrix2(jonesVectorX, jonesVectorY, Complex.Zero(), Complex.Zero());
          GL.Vertex3(x, result.m00.R, result.m10.R);
          GL.Vertex3(x, result.m00.R, result.m10.R);
        }

      }
      GL.End();
      GL.PopMatrix();
    }
  }
}