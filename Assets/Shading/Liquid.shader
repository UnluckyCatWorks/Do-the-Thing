// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Liquid"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_Level ("Fill height", Float) = 0.5
	}

	SubShader
	{
		Tags { "Queue"="Transparent+1" "RenderType"="Transparent" }
		Cull Off

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert
		struct Input
		{
			float3 worldPos;
			float3 worldCenter;
		};

		fixed4 _Color;
		float _Level;

		void vert (inout appdata_full v, out Input o)
		{
			float3 viewDir = mul(unity_WorldToObject, _WorldSpaceCameraPos).xyz - v.vertex.xyz;
			v.normal = viewDir;
            UNITY_INITIALIZE_OUTPUT (Input, o);
			o.worldCenter = mul (unity_ObjectToWorld, float4(0,0,0, 1));
		}

		void surf (Input IN, inout SurfaceOutputStandard s)
		{
			float3 target = IN.worldCenter + float3(0, _Level, 0);
			float value = target.y - IN.worldPos.y;
			clip(value);

			s.Albedo = _Color;
			s.Alpha = value;
		}

		half4 LightingLiquid (SurfaceOutput s, float3 viewDir, float atten)
		{
			
			return half4(s.Albedo, 1) * atten;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
