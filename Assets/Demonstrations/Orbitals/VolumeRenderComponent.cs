using UnityEngine;
using System.Collections;
using System;
namespace Assets
{
  /// <summary>
  /// Causes the gameobject to be rendered as a volume rendering of a scalar field
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

    HydrogenCalc[] calcs =
  {
    new HydrogenCalc(2,1,1),
    new HydrogenCalc(2,1,0),
    //new HydrogenCalc(2,1,-1),
    //new HydrogenCalc(2,1,0), //creates weird dots at the origin
    new HydrogenCalc(3,1,1),
    //new HydrogenCalc(3,1,0),
    //new HydrogenCalc(3,1,-1),
    new HydrogenCalc(3,2,2),
    new HydrogenCalc(3,2,1),
    new HydrogenCalc(3,2,0),
    //new HydrogenCalc(3,2,-1),
    //new HydrogenCalc(3,2,-2),

    //new HydrogenCalc(4,1,1),
    //new HydrogenCalc(4,1,0),
    //new HydrogenCalc(4,1,-1),

    new HydrogenCalc(4,2,2),
    new HydrogenCalc(4,2,1),
    new HydrogenCalc(4,2,0),
    //new HydrogenCalc(4,2,-1),
    //new HydrogenCalc(4,2,-2),

    new HydrogenCalc(4,3, 3),
    new HydrogenCalc(4,3, 2),
    new HydrogenCalc(4,3, 1),
    new HydrogenCalc(4,3, 0),
    //new HydrogenCalc(4,3,-1),
    //new HydrogenCalc(4,3,-2),
    //new HydrogenCalc(4,3,-3)
  };
    int calcIndex = 0;

    private Texture2D fieldData;

    private Material volumeShaderMaterial;

    private HydrogenCalc hydrogen;

    private Cardboard cardboardMain;

    UnityEngine.UI.Text orbitalText;
    UnityEngine.UI.Text mText;

    void Reset()
    {

    }

    void Awake()
    {
      Screen.sleepTimeout = SleepTimeout.NeverSleep;
      orbitalText = GameObject.Find("OrbitalLabel").GetComponent<UnityEngine.UI.Text>();
      mText = GameObject.Find("MLabel").GetComponent<UnityEngine.UI.Text>();

      hydrogen = calcs[0];

      if (volumeShader == null)
      {
        throw new Exception("expecting volumeShader to be set by unity");
      }
      volumeShaderMaterial = new Material(volumeShader);

      var MR = this.gameObject.AddComponent<MeshRenderer>();
      MR.material = volumeShaderMaterial;

      RefreshFieldData();

      cardboardMain = GetComponentInChildren(typeof(Cardboard)) as Cardboard;
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
      if (cardboardMain.CardboardTriggered)
      {
        calcIndex++;
        if (calcIndex >= calcs.Length) calcIndex = 0;
        hydrogen = calcs[calcIndex];
        RefreshFieldData();
      }
    }

    void RefreshFieldData()
    {
      GenerateSphereTextures();
      volumeShaderMaterial.SetTexture("fieldData", fieldData);
      volumeShaderMaterial.SetInt("angularNodes", hydrogen.AngularNodes);
      volumeShaderMaterial.SetFloat("rotation", hydrogen.Rotation);
      orbitalText.text = hydrogen.OrbitalLabel;
      mText.text = hydrogen.MLabel;
    }

    void GenerateSphereTextures()
    {
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