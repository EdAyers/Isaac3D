Shader "Custom/VolumeShader" {

  CGINCLUDE
    #include "UnityCG.cginc"

    //implementation strategy based on http://www.daimi.au.dk/~trier/raycasting_shader.cg

    float renderRadius; //< gives the radius in space to render the volume
    float fieldRadius;  //< applies a scaling to the rendering

    ///Stores the data representing the wavefunction.
    ///The x component encodes the field values in the radial direction
    ///The y component encodes the field values in the theta  direction
    ///The floats passed in the texture map seem to not support negative numbers so
    ///The positive field values are encoded in the red channel and the negative in the blue channel.
    sampler2D fieldData;

    ///The distance between calculation steps in the ray marching
    float stepsize;

    ///The absolute value of the quantum number 'm', adds nodes in the phi direction.
    int angularNodes;

    ///Value by which to rotate the m values
    float rotation;

    ///These are two parameters that just change how the colour stacking
    ///is calculated. The idea is that you move a slider in the unity browser
    ///and find the best value for these by eye.
    float alphaFactor;
    float colorFactor;

    //-------HOW IT WORKS-------
    // - The vertex shader finds the object position and ray entry direction
    // - Each ray has the resultant colour hitting the eye calculated by
    //    stepping through the volume and summing the colours and transparencies
    //    at each point on the ray's trajectory through the object.
    // - It does this by taking all of the points of the ray and calculating their
    //    position in polar coordinates. The R and theta values are mapped to a texture with
    //    precalculated values and the phi component is calculated on the spot (since its just a cos function).
    // - This is computationally intensive but amazingly modern phones can handle it without framerate dropping
    //    to perceptible levels. Remarkable.

    ///This is the intermediate data-type between the vertex shader and the fragment shader.
    struct vertex_fragment
    {
       float4 ProjPos	    : POSITION;  //Used by the rasterizer
       float3 Dir         : TEXCOORD2; //Gives the direction of the ray into the cube in the model's coordinate system
       float3 EntryPoint  : TEXCOORD1; //gives the model position of the vertex
    };

    ///output data-type
    struct fragment_out 
    {
      float4 Color	    : COLOR;
    };

    ///Standard vertex shader that also gets the
    ///direction from eye to vertex.
    //A bug here is causing vertices of the mesh to be rendered differently.
    vertex_fragment vertex_main( appdata_img IN)
    {    
      vertex_fragment OUT;
      OUT.ProjPos =  mul( UNITY_MATRIX_MVP, IN.vertex );

      //for the cube this gives [-0.5, 0.5]
      OUT.EntryPoint = IN.vertex.xyz;
      float3 d = - normalize(ObjSpaceViewDir(IN.vertex)); //ObjSpaceViewDir gives a vector facing towards the camera
      OUT.Dir = d;
      
      return OUT;
    }

    ///Converts cartesian to polar coordinates
    float3 xyz_to_rtp(float3 xyz)
    {
      float l = length(float3(xyz.x, xyz.z, 0));
      float phi = atan2(xyz.z, xyz.x)/6.28318530718 + 0.5;
      return float3(length(xyz), atan2(l, xyz.y)/3.14, phi);
    } 

    ///Given the object position at which the ray enters the mesh and
    ///the normalised direction of the sight ray, this function returns the
    ///point on a sphere of radius `renderRadius` at which the ray intersects.
    ///We do this because it helps to ensure that the rendering of the volume doesn't
    ///depend on the mesh it is drawn upon. Tends to go wrong at vertices and oblique angles.
    float4 get_sphere_entry(float3 meshEntryPoint, float3 sightRay)
    {
      float b = dot(sightRay,meshEntryPoint);
      float c = dot(meshEntryPoint,meshEntryPoint) - (renderRadius * renderRadius);
      //p is the distance squared along the sight line that displaces the entry point to the surface of the sphere
      float p = (b*b) - c;
      float4 result;
      if (p <= 0)
      {
        //if the entry point is further in than the sphere then we spit
        //out the dummy and give a stupid value encoding NaN. 
        //There's probably a more proffessional way of doing this but 
        //I seem to remember weird things happen if I tried doing this sensibly.
        result = float4(2000,0,0,0);
      }
      else
      {
        float s = sqrt(p);
        float3 entryPoint = meshEntryPoint + (-b - s) * sightRay;
        float3 exitPoint  = meshEntryPoint + (-b + s) * sightRay; 
        result = float4(entryPoint, 1);
      }
      return result;
    }

    ///Workhorse. Marches rays and finds colour of a pixel.
    fragment_out fragment_main( vertex_fragment IN )
    {
      fragment_out OUT;
      
      //in range [-a to +a]
      float3 start = get_sphere_entry(IN.EntryPoint, IN.Dir); 

      float delta = stepsize * renderRadius;
      float3 delta_dir = IN.Dir * delta;
      float3 vec = start;
      float4 col_acc = float4(0,0,0,0);
      float alpha_acc = 0;
      float length_acc = 0;

      float4 color_sample;
      float alpha_sample;
      float3 rtp;

      //Begin the ray marching
      for(int i = 0; i < 40; i++)
      {
          //spherical textures
          rtp = xyz_to_rtp(vec);
          if (alpha_acc > 1.0 || rtp.x > renderRadius ) break;
          
          float texRadius = saturate(rtp.r /fieldRadius);
          float4 texSample = tex2D(fieldData, float2(texRadius, rtp.y));

          float phiComp = cos(angularNodes * rtp.z * 6.283 + rotation);
          float apc = abs(phiComp) * texSample.a;

          bool pc = phiComp > 0;
          bool tc = texSample.b < 0.5;
          bool b  = (pc && tc) || (!pc && !tc);

          color_sample = b ? float4(0,0,1,apc) : float4(1,0,0,apc);

          alpha_sample = color_sample.a * stepsize * alphaFactor;
          
          col_acc += (1.0 - alpha_acc) * color_sample * colorFactor * alpha_sample;

          alpha_acc += alpha_sample;

          length_acc += delta;
          vec += delta_dir;
      } 

      OUT.Color = col_acc;

      return OUT;
    }
  ENDCG

	Properties {
    fieldData ("Texture containing data about field", 2D) = "green" {} //encodes the R channel as |z| and G as arg(z)
    renderRadius ("Radius of rendering volume", Float) = 0.5
    fieldRadius ("Radius of the field", Float) = 0.5
    stepsize ("Step Size", Float) = 0.035
    angularNodes ("abs(m) quantum number", Int) = 1
    rotation ("phi rotation", Float) = 0.0

    alphaFactor ("alpha factor", Float) = 4
    colorFactor ("color factor", Float) = 3
	}
	SubShader {
    Tags {"Queue" = "Transparent" }
    Pass{
      ZWrite Off
      Cull Back
      Blend SrcAlpha OneMinusSrcAlpha // use alpha blending
		  CGPROGRAM
        #pragma vertex vertex_main
        #pragma fragment fragment_main
   
		  ENDCG
	  } //pass
  } //subshader
	FallBack Off
} //shader
