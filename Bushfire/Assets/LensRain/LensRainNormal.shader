Shader "Unlit/LensRainNormal"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;

			fixed3 frag (v2f i) : SV_Target
			{
				float tex = tex2D(_MainTex, i.uv).x; // Sample texture
                float n = tex2D(_MainTex, float2(i.uv.x, i.uv.y + 1.0 / _ScreenParams.y)).x; // Sample with north offset
                float s = tex2D(_MainTex, float2(i.uv.x, i.uv.y - 1.0 / _ScreenParams.y)).x; // Sample with south offset
                float e = tex2D(_MainTex, float2(i.uv.x + 1.0 / _ScreenParams.x, i.uv.y)).x; // Sample with east offset
                float w = tex2D(_MainTex, float2(i.uv.x - 1.0 / _ScreenParams.x, i.uv.y)).x; // Sample with west offset
                
				float3 normal; // Declare normals vector
                normal.x		= s - n; // Get X
                normal.y		= w - e; // Get Y
                normal.z		= 0.05; // Get Z
                
                return normal;
			}
			ENDCG
		}
	}
}
