using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Causes the gameobject to be rendered as a volume rendering of a scalar field
/// </summary>
public class VolumeRenderComponent : MonoBehaviour {

  private int volumeTextureSize = 64;

  ///the range over which to evaluate the scalar field
  private float fieldSize = 15.0F;

  [SerializeField]
  private Shader volumeShader; //this field set by unity

  private float fieldMaxValue = 1.0F;
  private float fieldMinValue = -1.0F;

  HydrogenCalc[] calcs =
  {
    new HydrogenCalc(3,0,0),
    new HydrogenCalc(3,1,0),
    new HydrogenCalc(3,1,1),
    new HydrogenCalc(3,2,0),
    new HydrogenCalc(3,2,1),
    new HydrogenCalc(3,2,2)
  };
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
    hydrogen = new HydrogenCalc(3, 1, 1);

    if (volumeShader == null)
    {
      throw new Exception("expecting volumeShader to be set by unity");
    }
    volumeShaderMaterial = new Material(volumeShader);
    
    var MR = this.gameObject.AddComponent<MeshRenderer>();
    MR.material = volumeShaderMaterial;

    RefreshFieldData();

    cardboardMain = GetComponentInChildren(typeof(Cardboard)) as Cardboard;
    Debug.Log("cardboardMain = " + cardboardMain);
  }

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
    if (cardboardMain.CardboardTriggered)
    {
      Debug.Log("button pressed");
      hydrogen = calcs[calcIndex];
      RefreshFieldData();
      calcIndex++;
      if (calcIndex >= calcs.Length) calcIndex = 0;
    }
	}

  void RefreshFieldData()
  {
      GenerateSphereTextures();
      volumeShaderMaterial.SetTexture("fieldData", fieldData);
      volumeShaderMaterial.SetInt("angularNodes", hydrogen.AngularNodes);
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
        float red = mag > 0 ? mag : 0.0f;
        float blue = mag < 0 ? -mag : 0.0f;
        //TODO normalise mag for visualisation rather than chemical accuracy
        sph_colors[index] = new Color(red, 0, blue, 1) ;
      }
    }
    Debug.Log(mag);
    fieldData.filterMode = FilterMode.Point;
    fieldData.SetPixels(sph_colors);
    fieldData.Apply();
  }

}
