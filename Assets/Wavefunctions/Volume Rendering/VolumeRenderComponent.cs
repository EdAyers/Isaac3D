﻿using UnityEngine;
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
  [SerializeField]
  private float fieldSize = 10.0F;
  //[SerializeField]
  //private float fieldHeight = 10.0F;
  //[SerializeField]
  //private float fieldDepth = 10.0F;

  [SerializeField]
  private Shader volumeShader;

  private float fieldMaxValue = 1.0F;
  private float fieldMinValue = -1.0F;

  private Texture3D volumeBuffer;
  private Texture2D sphericalBuffer;
  private Texture2D radialBuffer;

  private Material volumeShaderMaterial;

  void Reset()
  {

  }

  void Awake()
  {
    volumeShaderMaterial = new Material(volumeShader);
    
    var MR = this.gameObject.AddComponent<MeshRenderer>();
    MR.material = volumeShaderMaterial;

    GenerateSphereTextures();
    volumeShaderMaterial.SetTexture("sphere_tex", sphericalBuffer);
    volumeShaderMaterial.SetTexture("radial_tex", radialBuffer);

    //GenerateVolumeTexture();
    //volumeShaderMaterial.SetTexture("volume_tex", volumeBuffer);
  }

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {

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
    sphericalBuffer = new Texture2D(h, w);
    var sph_colors = new Color[h * w];

    for (int i = 0; i < h; i++)
    {
      for (int j = 0; j < w; j++)
      {
        int index = i + (j * h);
        float r = (i < 40 && i > 30 && j < 40 && j > 30) ? 1f : 0f;
        sph_colors[index] = new Color( ScalarField(0.5f * i, 0.5f * j,0f),0f,0f , 0f );
      }
    }
    sphericalBuffer.SetPixels(sph_colors);
    sphericalBuffer.Apply();

    var d = volumeTextureSize;
    radialBuffer = new Texture2D(d, 1);
    var r_colors = new Color[d];
    for (int i = 0; i < d; i++)
    {
      r_colors[i] = new Color((i > 10 ) ? 1f : 0f, 0, 0);
    }
    radialBuffer.SetPixels(r_colors);
    radialBuffer.Apply();
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