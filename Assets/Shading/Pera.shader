Shader "Custom/Pera"
{
	Properties
	{
		[NoScaleOffset]
		_Galaxy ("Weird albedo", 2D) = "bump" {}
		_Scale ("Texture scale", Float) = 1.0
		
		[NoScaleOffset] _Mask ("Mask", 2D) = "white" {}

		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo", 2D) = "white" {}
		[NoScaleOffset] _AO ("Ambient Occlusion", 2D) = "white" {}
		[NoScaleOffset] [Normal] _Normal ("Normal map", 2D) = "bump" {}
		[NoScaleOffset] _Smooth ("Smoothness", 2D) = "black" {}
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0
		struct Input
		{
			float2 uv_MainTex;
			float4 screenPos;
			float3 viewDir;
		};

		fixed4 _Color;
		sampler2D _Galaxy;
		float _Scale;
		
		sampler2D _Mask;

		sampler2D _MainTex;
		sampler2D _AO;
		sampler2D _Normal;
		sampler2D _Smooth;

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			fixed mask = tex2D (_Mask, IN.uv_MainTex).x;
			if (mask>0.01)
			{
				float2 uv = (IN.screenPos.xy / IN.screenPos.w) * _Scale + float2( 0, _Time.x);
				fixed3 col = tex2D (_Galaxy, uv).rgb;

				fixed3 ao = tex2D (_AO, IN.uv_MainTex).rgb;
				fixed4 normal = tex2D (_Normal, IN.uv_MainTex);
				o.Normal = UnpackNormal ( normal );

				float3 rim = (0.95 - dot (IN.viewDir, o.Normal)) * 0.05 * _Color;
				o.Emission = col * 8 + rim * 3;
				o.Smoothness = 0;
			}
			else
			{
				fixed3 col = tex2D (_MainTex, IN.uv_MainTex).rgb;
				fixed3 ao = tex2D (_AO, IN.uv_MainTex).rgb;
				fixed4 normal = tex2D (_Normal, IN.uv_MainTex);
				fixed4 smooth = tex2D (_Smooth, IN.uv_MainTex);

				o.Albedo = col * ao;
				o.Normal = UnpackNormal ( normal );
				o.Smoothness = smooth.a;
			}
			o.Metallic = 0;
			o.Alpha = 1;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
