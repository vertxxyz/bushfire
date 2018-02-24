Shader "Unlit/LensRainShader"
{
	Properties
	{
		_RainNormal ("Normal", 2D) = "black" {}
		_Strength ("Strength", Float) = 1
	}
	SubShader
	{
	    //Get rid of the queue for lols
		Tags { "RenderType"="Transparent" "Queue" = "Transparent"}
		LOD 100

        GrabPass {}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 grabPos : TEXCOORD1;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _GrabTexture;
			float4 _GrabTexture_ST;
			sampler2D _RainNormal;
			float4 _RainNormal_ST;
			float _Strength;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _RainNormal);
				o.grabPos = ComputeGrabScreenPos(o.vertex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
			
			    float3 normal = tex2D (_RainNormal, i.uv).rgb;
                float2 uv = float2(normal.x * _ScreenParams.z * _Strength + i.grabPos.x,
                    normal.y * _ScreenParams.z * _Strength + i.grabPos.y);
				// sample the texture
				fixed4 col = tex2Dproj(_GrabTexture, float4(uv.x, uv.y, i.grabPos.z, i.grabPos.w));
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
