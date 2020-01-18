Shader "BattlePhazeScreen/3D/Toggle OU SBS"
{
	Properties
	{
		_MainTex("Screen Texture", 2D) = "white" {}
		[Toggle(_)]OverUnder("Over Under", Int) = 0
		[Toggle(_)]SwapEyes("Swap Eyes", Int) = 0
	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha vertex:vertexDataFunc 
		struct Input
		{
			float2 UVMainTex;
		};
		uniform sampler2D _MainTex;
		uniform int SwapEyes;
		uniform int OverUnder;
		void vertexDataFunc( inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float AdjustedEyeIndex = lerp( unity_StereoEyeIndex , ( -unity_StereoEyeIndex + 1.0 ) , (float)SwapEyes);
			float TexLerp = lerp(v.texcoord.xy * (0.5).xx, v.texcoord.x, AdjustedEyeIndex);
			float2 UVLerp = (float2(v.texcoord.x , lerp( v.texcoord.y , v.texcoord.y , AdjustedEyeIndex)));
			o.UVMainTex = lerp( (float2(TexLerp, v.texcoord.y)) , UVLerp , (float)OverUnder);
		}
		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}
		void surf( Input i , inout SurfaceOutput o )
		{
			o.Emission = tex2D( _MainTex, i.UVMainTex ).rgb;
			o.Alpha = 1;
		}
		ENDCG
	}
	Fallback "Diffuse"
}