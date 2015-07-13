using UnityEngine;
using System.Collections;
using System;
namespace Assets
{
  /// <summary>
  /// Causes the gameobject to be rendered as a volume rendering of a scalar field.
  /// Used to visualise the wavefunctions
  /// </summary>
  public class VolumeRenderComponent : MonoBehaviour
  {
    private int volumeTextureSize = 64;

    ///the range over which to evaluate the scalar field
    private float fieldSize = 15.0F;

    [SerializeField]
    private Shader volumeShader; //this field set by unity

    private float fieldMaxValue = 1.0F;
    private float fieldMinValue = -1.0F;

    int calcIndex = 0;

    private Texture2D fieldData;

    private Material volumeShaderMaterial;

    private HydrogenCalc hydrogen;

    private Cardboard cardboardMain;


    void Reset()
    {

    }

    void Awake()
    {
      Screen.sleepTimeout = SleepTimeout.NeverSleep;
      if (volumeShader == null)
      {
        throw new Exception("expecting volumeShader to be set by unity");
      }
      volumeShaderMaterial = new Material(volumeShader);
      var MR = this.gameObject.AddComponent<MeshRenderer>();
      MR.material = volumeShaderMaterial;
      cardboardMain = GetComponentInChildren(typeof(Cardboard)) as Cardboard;
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetFieldData(HydrogenCalc calc)
    {
      hydrogen = calc;
      GenerateSphereTextures();
      volumeShaderMaterial.SetTexture("fieldData", fieldData);
      volumeShaderMaterial.SetInt("angularNodes", hydrogen.AngularNodes);
      volumeShaderMaterial.SetFloat("rotation", hydrogen.Rotation);
    }

    ///Takes the current HydrogenCalc and generates a texture
    ///for sending to the volume rendering shader.
    ///The texture works by mapping values along the r and theta directions
    ///to the U,V texture coordinates. The Phi component is calculated inside
    ///the shader since its just multiplying by cos(phi).
    ///This means we save memory and don't need to use Texture3D which
    ///is not well supported.
    void GenerateSphereTextures()
    {
      if (hydrogen == null) return;
      fieldSize = (float)hydrogen.Size;
      var h = volumeTextureSize;
      var w = volumeTextureSize;
      fieldData = new Texture2D(h, w);
      var sph_colors = new Color[h * w];
      float mag = 0;
      var d = volumeTextureSize;
      for (int i = 0; i < h; i++)
      {
        for (int j = 0; j < w; j++)
        {
          int index = i + (j * h);
          float r = (float)i * fieldSize / d;
          float theta = (float)j / h * Mathf.PI;
          mag = hydrogen.RadialComponent(r) * hydrogen.SphericalHarmonic(theta);
          mag *= 50;
          float red = mag > 0 ? 1.0f : 0.0f;
          float blue = mag < 0 ? 1.0f : 0.0f;
          sph_colors[index] = new Color(red, 0, blue, Math.Abs(mag));
        }
      }

      fieldData.filterMode = FilterMode.Bilinear;
      fieldData.SetPixels(sph_colors);
      fieldData.Apply();
    }

  }

}