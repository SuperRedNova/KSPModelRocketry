﻿Shader "ModelRocketry/DiffuseAlpha" 
{
	Properties
	{
		_MainTex("_MainTex (RGB)", 2D) = "white" {}
		_Color("Main Color", Color) = (1,1,1,1)
		_Color2("Secondary Color", Color) = (0,0,0,1)
		_Opacity("_Opacity", Range(0,1)) = 1
		_RimFalloff("_RimFalloff", Range(0.01,5)) = 0.1
		_RimColor("_RimColor", Color) = (0,0,0,0)

		_TemperatureColor("_TemperatureColor", Color) = (0,0,0,0)
		_BurnColor("Burn Color", Color) = (1,1,1,1)
	}

		SubShader
	{
		ZWrite On
		ZTest LEqual
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM

#pragma surface surf Lambert alpha
#pragma target 3.0

		sampler2D _MainTex;

		float4 _Color;
		float4 _Color2;

	float _Opacity;
	float _RimFalloff;
	float4 _RimColor;
	float4 _TemperatureColor;
	float4 _BurnColor;

	struct Input
	{
		float2 uv_MainTex;
		float2 uv_Emissive;
		float3 viewDir;
	};

	void surf(Input IN, inout SurfaceOutput o)
	{
		float4 color = tex2D(_MainTex, (IN.uv_MainTex)) * _Color;
		color = lerp(_Color2, color, color.a);
		color = color * _BurnColor;
		float3 normal = float3(0,0,1);

		half rim = 1.0 - saturate(dot(normalize(IN.viewDir), normal));

		float3 emission = (_RimColor.rgb * pow(rim, _RimFalloff)) * _RimColor.a;
		emission += _TemperatureColor.rgb * _TemperatureColor.a;

		o.Albedo = color.rgb;
		o.Emission = emission;
		o.Gloss = 0;
		o.Specular = 0;
		o.Normal = normal;

		o.Emission *= _Opacity;
		o.Alpha = _Opacity;
	}
	ENDCG
	}
		Fallback "KSP/Diffuse"
}