Shader "ModelRocketry/Hue"
{
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
	
	float4 frag(v2f_img i) : SV_Target{
		float4 color = float4(1,1,1,1);
		float hue = i.uv.x * 6;
		int j = floor(hue);
		float up = hue - j;
		float down = 1 - up;
		switch (j) {
		case 0:
			color.r = 1;
			color.g = up;
			color.b = 0;
			break;
		case 1:
			color.r = down;
			color.g = 1;
			color.b = 0;
			break;
		case 2:
			color.r = 0;
			color.g = 1;
			color.b = up;
			break;
		case 3:
			color.r = 0;
			color.g = down;
			color.b = 1;
			break;
		case 4:
			color.r = up;
			color.g = 0;
			color.b = 1;
			break;
		default:
			color.r = 1;
			color.g = 0;
			color.b = down;
			break;
		}
		return color;
	}
			ENDCG
		}
	}
}