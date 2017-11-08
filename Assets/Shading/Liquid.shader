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
		#pragma surface surf Liquid fullforwardshadows vertex:vert
		struct Input
		{
			float3 worldPos;
			float3 objectCenter;
		};

		fixed4 _Color;
		float _Level;

		void vert (inout appdata_full v, out Input o)
		{
			v.normal *= -1;
            UNITY_INITIALIZE_OUTPUT (Input, o);
			o.objectCenter = mul (unity_ObjectToWorld, float4 (0,0,0,1));
		}

		void surf (Input IN, inout SurfaceOutput s)
		{
			float3 target = IN.objectCenter + float3(0, _Level, 0);
			float value = target.y - IN.worldPos.y;

			s.Albedo = _Color;
			s.Alpha = value;
		}

		half4 LightingLiquid (SurfaceOutput s, float3 lightDir, float atten)
		{
			clip(s.Alpha);
			return half4(s.Albedo, 1) * atten;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
