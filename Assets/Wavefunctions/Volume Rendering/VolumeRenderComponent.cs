using UnityEngine;
using System.Collections;

/// <summary>
/// Causes the gameobject to be rendered as a volume rendering of a scalar field
/// </summary>
public class VolumeRenderComponent : MonoBehaviour {

  //
  private int volumeTextureSize = 64;
  //[SerializeField]
  //private int volumeHeight = 256;
  //[SerializeField]
  //private int volumeDepth = 256;

  ///the range over which to evaluate the scalar field
  private float fieldSize = 15.0F;
  private float magBoost = 15F;
  //[SerializeField]
  //private float fieldHeight = 10.0F;
  //[SerializeField]
  //private float fieldDepth = 10.0F;

  [SerializeField]
  private Shader volumeShader;

  private float fieldMaxValue = 1.0F;
  private float fieldMinValue = -1.0F;

  private Texture3D volumeBuffer;
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

  float ScalarField(float x, float y, float z)
  {
    return Mathf.Sin(x + y + z);
  }


  void GenerateSphereTextures()
  {
    //for now just make them be their position
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

    //radialBuffer = new Texture2D(d, 1);
    //var r_colors = new Color[d];
    //for (int i = 0; i < d; i++)
    //{
    //  float r = i * fieldSize / d;
    //  Complex x = Complex.FromRI(hydrogen.RadialComponent(r), 0f);
    //  r_colors[i] = new Color(x.mag * magBoost, x.arg / Mathf.PI / 2, 0,1);
    //}
    //radialBuffer.SetPixels(r_colors);
    //radialBuffer.Apply();
    //radialBuffer.filterMode = FilterMode.Point;

  }

  void GenerateVolumeTexture()
  {
    volumeBuffer = new Texture3D(volumeTextureSize, volumeTextureSize, volumeTextureSize, TextureFormat.ARGB32, false);

    var w = volumeBuffer.width;
    var h = volumeBuffer.height;
    var d = volumeBuffer.depth;
    Debug.Log(d);

    var volumeColours = new Color[w * h * d];

    var dx = fieldSize / w;
   // var dy = fieldHeight / h;
   // var dz = fieldDepth / d;

    float x = -fieldSize / 2;
    var y = x; var z = x;
    //var y = -fieldHeight / 2;
   // var z = -fieldDepth / 2;

    for (int i = 0; i < w; i++)
    {
      for (int j = 0; j < h; j++)
      {
        for (int k = 0; k < d; k++)
        {

          var idx = i + (j * w) + (k * w * h);
          //normalise the scalar field
          var value = (ScalarField(x, y, z) - fieldMinValue) / (fieldMaxValue - fieldMinValue);
          value = Mathf.Clamp01(value);
          //make it white with an opacity dictated by field strength
          volumeColours[idx] = new Color(1, 0,0,1);
          z += dx;
        }
          y += dx;
      }
          x += dx;
    }

    volumeBuffer.SetPixels(volumeColours);
    volumeBuffer.Apply();

  }
}
