Shader "ModelRocketry/ColorPicker"
{
	Properties
	{
		_Color("Hue",Color) = (1,0,0,1)
	}
		SubShader
	{
		Tags
		{
		"RenderType" = "Opaque"
		"PreviewType" = "Plane"
		}
		Pass
		{
			CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
#pragma target 3.0

#include "UnityCG.cginc"

	float4 _Color;
	
	float4 frag(v2f_img i) : SV_Target
	{
		float4 color;
		color = lerp(float4(1, 1, 1, 1), _Color, i.uv.x);
		color = lerp(float4(0, 0, 0, 1), color, i.uv.y);
		return color;
	}
			ENDCG
		}
	}
}