Shader "Custom/VolumeShader" {

  CGINCLUDE
    #include "UnityCG.cginc"

    //copied from http://www.daimi.au.dk/~trier/raycasting_shader.cg

    float sphereRadius; //gives the radius in space to render the volume
    sampler2D sphere_tex;
    sampler2D radial_tex;
    float stepsize;

    // Define the interface between the vertex- and the fragment programs
    struct vertex_fragment
    {
       float4 ProjPos	  : POSITION; // For the rasterizer
       float3 Dir       : TEXCOORD0; //Gives the direction of the ray into the cube in the model's coordinate system
       float3 EntryPoint       : TEXCOORD1; //gives the model position of the vertex
    };

    struct fragment_out 
    {
      float4 Color	    : COLOR;
    };

    // Raycasting vertex program implementation
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

    float3 xyz_to_rtp(float3 xyz)
    {
      float l = length(float3(xyz.x, xyz.y, 0));
      return float3(length(xyz), atan2(l, xyz.z)/3.14, atan2(xyz.y, xyz.x)/6.28 + 0.5);
    }

    float4 get_sphere_entry(float3 x, float3 d)
    {//assume d is normalised
      //x is the mesh position in object space
      //we can find the point where the ray starting at x in direction d first
      //intersects our sphere of interest
      float b = dot(d,x);
      float c = dot(x,x) - (sphereRadius * sphereRadius);
      float p = (b*b) - c;
      float4 result;
      if (p <= 0)
      {
        result = float4(110,0,0,0);
      }
      else
      {
        float s = sqrt(p);
        float3 entryPoint = x + (-b - s) * d;
        float3 exitPoint  = x + (-b + s) * d; 
        result = float4(entryPoint, 1);
      }
      return result;
    }

    // Raycasting fragment program implementation
    fragment_out fragment_main( vertex_fragment IN )
    {
      

      fragment_out OUT;
      
      //in range [-a to +a]
      float3 start = get_sphere_entry(IN.EntryPoint, IN.Dir); 

      float delta = stepsize * sphereRadius;
      float3 delta_dir = IN.Dir * delta;
      float3 vec = start;
      float4 col_acc = float4(0,0,0,0);
      float alpha_acc = 0;
      float length_acc = 0;

      float sample_mag;
      float sample_arg;
      float4 color_sample;
      float alpha_sample;
      float3 rtp;
      for(int i = 0; i < 30; i++)
      {
           
          //spherical textures
          rtp = xyz_to_rtp(vec);
          if (alpha_acc > 1.0 || rtp.x > sphereRadius ) break;
          float4 sample_r = tex2D(radial_tex, float2(rtp.x / sphereRadius, 0));
          float4 sample_y = tex2D(sphere_tex, float2(rtp.y, rtp.z));
          sample_mag = sample_r.x * sample_y.x; //assume always less than one
          sample_arg = fmod(sample_r.y + sample_y.y, 6.28); 
          
          color_sample = float4(0.8,0.8,1,sample_mag);
          alpha_sample = color_sample.a * stepsize;
          
          col_acc   += (1.0 - alpha_acc) * color_sample * alpha_sample * 3;

          alpha_acc += alpha_sample;

          length_acc += delta;
          vec += delta_dir;
      } 

      OUT.Color = float4((start) + float3(0.5,0.5,0.5),1);
      OUT.Color = col_acc;

      return OUT;
    }
  ENDCG

	Properties {
    //volume_tex ("Volume Texture", 3D) = "" {}
    sphere_tex ("Spherical Texture", 2D) = "blue" {} //encodes the R channel as |z| and G as arg(z)
    radial_tex ("Radial Texture", 2D) = "blue" {}
    //render_sphere ("Render Spherical Textures", int) = 0
    sphereRadius ("Radius of Volume to be rendered", Float) = 0.55
    stepsize ("Step Size", Float) = 0.05
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
