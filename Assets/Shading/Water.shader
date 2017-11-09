Shader "Custom/Water"
{
	Properties 
	{
		[Header (Color and disortion)]
		_Dark	 ("Dark water color",  Color)		    = (0,0,0,1)
		_Lit	 ("Lit water color",   Color)		    = (1,1,1,1)
		_ColPow  ("Color Balnace",	   Range(0.0, 1.5)) = 0.2
		_Alpha	 ("Transparency",	   Range(0.0, 1.0)) = 1.0
		_BumpAmt ("Distortion Amount", range (0,128))	= 10

		[Header (Fresnel)]
		_R0	("Fresnel R0",	  Range(0.00, 0.1))   = 0.05
		_FPower ("Fresnel power", Range(0.001, 10.0)) = 5.0
		
		[Header (Textures)]
		[Normal]
		_Waves		("Waves (normal map)", 2D)    = "bump" {}
		_WavesMul	("Normal strenght",    float) = 1.0
		_WavesFres	("Fresnel strenght",   float) = 1.0

		_SpeedX ("Waves speed (X)", float) = 0.5
		_SpeedY ("Waves speed (Y)", float) = 0.5
	}
	
	SubShader  
	{
		Tags { "Queue"="Transparent-2" "RenderType"="Transparent" }

		// This pass grabs the screen behind the object into a texture.
		// We can access the result in the next pass as "_Background"
		GrabPass 
		{
			Tags { "LightMode" = "Always" }
			"_Background"
		}

		// Color-Dissortion pass
		CGPROGRAM
		// Some utilities
		#define SPEED(c) _Time.c * float2(_SpeedX, _SpeedY)
		#define UTEX(name) tex2D(name, i.uv##name)

		#pragma surface surf Standard alpha:auto
		#pragma target 3.0
		struct Input 
		{
			float2 uv_Waves;
			float4 screenPos;
			float3 viewDir;
		};
		
		uniform float4 _Dark;
		uniform float4 _Lit;
		uniform float2 _Speed;
		uniform float _BumpAmt;

		uniform float _Alpha;
		uniform float _ColPow;
		uniform float _FPower;
		uniform float _R0;

		uniform sampler2D _Waves;
		uniform float _WavesFres;
		uniform float _WavesMul;

		uniform float _SpeedX;
		uniform float _SpeedY;

		sampler2D _Background;
		float4 _Background_TexelSize;

		void surf (Input i, inout SurfaceOutputStandard s ) 
		{
			// Calculate normal bump
			i.uv_Waves += SPEED(xy);
			s.Normal = UnpackNormal(UTEX(_Waves)) * _WavesFres;

			// Calculate bumped surface
			float3 waves = s.Normal * _WavesMul * _BumpAmt*_BumpAmt;
			float2 offset = waves * _Background_TexelSize.xy;

			// Calculate dissorted UVs
			i.screenPos.xy += offset * i.screenPos.z;
			#if UNITY_UV_STARTS_AT_TOP
				fixed2 sm2Adjust = i.screenPos.xy / i.screenPos.w;
				sm2Adjust.y = 1 - sm2Adjust.y; 
			#endif
			s.Albedo = tex2Dproj( _Background, i.screenPos) * _Dark.a;

			// Calculate fresnel amount
			float fresnel;
			fresnel = saturate ( 1.0 - dot(s.Normal, i.viewDir) );
			fresnel = pow (fresnel, _FPower);
			fresnel = _R0 + (1. - _R0) * fresnel;
			// Lerp color and iluminate fresnel
			s.Albedo += lerp(_Dark, _Lit, _ColPow);
			s.Albedo +=  _Lit * fresnel;
			s.Alpha = _Alpha;
		}
		ENDCG
	}
	FallBack "Diffuse"
}