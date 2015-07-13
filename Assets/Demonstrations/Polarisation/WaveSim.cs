using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Assets
{
  public class WaveSim : MonoBehaviour
  {
    static Material lineMaterial;
    public CMatrix2 CMatrix2;
    public float time = 0.0f;
    public float startAngle = 0.0f;
    public bool advanceAngle = true;
    Waveplate waveplate;
    Waveplate polariser;
    public float lightSourcePosition;
    public float polariserPosition;

    float initialX = -5.0f;
    float finalX = +5.0f;
    const int STEPS = 1000;
    public float wavenumber = 22.04f;
    float speedOfLight = 0.5f; 

    void Start()
    {
      Screen.sleepTimeout = SleepTimeout.NeverSleep;

      CMatrix2 = CMatrix2.QuarterWaveplate();

      //add  blocks which represent the polariser
      GameObject root = GameObject.Find("waveRoot");

      GameObject cube = GameObject.Find("PlateCube");
      waveplate = (new Birefringent() { StartPosition = -0.5f, Thickness = 1});
      cube.transform.Translate(
        (waveplate.FinishPosition + waveplate.StartPosition) / 2, 0, 0);
      cube.transform.localScale = new Vector3(
        waveplate.FinishPosition - waveplate.StartPosition,
        1,1);
      //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
      //cube.transform.localScale = new Vector3(0.01f, 1f, 1f);

      //add the line
      CreateLineMaterial();
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
      time += 0.1f;
      if (advanceAngle)
      {
        startAngle += 0.001f;
      }
    }

    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
      if (waveplate == null) return;
      CreateLineMaterial();
      // Apply the line material
      lineMaterial.SetPass(0);


      // Draw lines
      GL.Begin(GL.LINES);
 
      var initialVector = Complex.Rotate(time * speedOfLight) * new CVector2(
          Mathf.Cos(startAngle), 0,
          Mathf.Sin(startAngle), 0
        );

      var interface1 = Complex.Rotate((initialX - waveplate.StartPosition) * wavenumber) * initialVector;
      var interface2 = waveplate.GetWave(
        interface1, 
        wavenumber, 
        waveplate.FinishPosition - waveplate.StartPosition);
      GL.Vertex3(initialX, initialVector.X.R, initialVector.Y.R);
      var color = new Color(1,0,0,1);

      for (int i = 0; i < STEPS; i++)
      {
        color = new Color(1, 0, 0, 1);
        var jonesVector = initialVector;
        float x = initialX + (i * (finalX - initialX) / STEPS);
        if (x < waveplate.StartPosition)
        {
          //GL.Color(new Color(1, 0, 0, 1));
          jonesVector = Complex.Rotate((initialX - x) * wavenumber) * initialVector;
        }
        else if (x < waveplate.FinishPosition)
        {
          color = ( waveplate.LineColor);
          jonesVector = waveplate.GetWave(
            interface1, 
            wavenumber, 
            x - waveplate.StartPosition);
        }
        else
        {
          //GL.Color(new Color(1, 0, 0, 1));
          jonesVector = Complex.Rotate((waveplate.FinishPosition - x) * wavenumber) * interface2;
        }
        jonesVector = 0.3f * jonesVector;
        GL.Color(color);
        GL.Vertex3(x, jonesVector.X.R, jonesVector.Y.R);
        GL.Vertex3(x, jonesVector.X.R, jonesVector.Y.R);

        //if (i % 10 == 0)
        //{
        //  GL.Color(color / 2);
        //  GL.Vertex3(x, 0, 0);
        //  GL.Vertex3(x, jonesVector.X.R, jonesVector.Y.R);

        //}


      }

      GL.End();
    }
  }
}