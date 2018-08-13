Shader "Unlit/FireRing"
{
	Properties
	{
		_Gradient ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 100

		//BACK Pass
		Pass
		{
			Blend One OneMinusSrcAlpha			
			Cull Front
			ZWrite ON 

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			#include "FireRing.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float2 uv2 : TEXCOORD1;

			};

			sampler2D _Gradient;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				
				float u0 = frac(o.uv.x);
				float u1 = frac(o.uv.x + 0.5) - 0.5;
				o.uv2 = float2(u0, u1);

				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_Gradient, i.uv);
				// apply fog
				// UNITY_APPLY_FOG(i.fogCoord, col);
				
				float u0 = i.uv2.x;
				float u1 = i.uv2.y;

				i.uv.x = (fwidth(u0) < fwidth(u1) - 0.001) ? u0 : u1;

				return fireRingFragment(i.uv, _Gradient);	
			}

			ENDCG
		}

		Pass
		{

			Blend One OneMinusSrcAlpha		
			Cull Back
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			#include "FireRing.cginc"


			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float2 uv2 : TEXCOORD1;
			};

			sampler2D _Gradient;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				
				float u0 = frac(o.uv.x);
				float u1 = frac(o.uv.x + 0.5) - 0.5;
				o.uv2 = float2(u0, u1);

				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_Gradient, i.uv);
				// apply fog
				// UNITY_APPLY_FOG(i.fogCoord, col);
				
				float u0 = i.uv2.x;
				float u1 = i.uv2.y;

				i.uv.x = (fwidth(u0) < fwidth(u1) - 0.001) ? u0 : u1;

				return fireRingFragment(i.uv, _Gradient);	
			}

			ENDCG
		}
	}
}
