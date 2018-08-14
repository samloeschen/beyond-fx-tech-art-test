Shader "Unlit/FireRing"
{
	Properties
	{
		_Gradient ("Fire LUT", 2D) = "white" {}
		_VertexPerlin ("Vertex Perlin", 2D) = "white" {}
		_Voronoise1 ("Voronoise 1", 2D) = "white" {}
		_Voronoise2 ("Voronoise 2", 2D) = "white" {}
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
			
			#include "UnityCG.cginc"
			#include "FireRing.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _Gradient;
			sampler2D _Voronoise1;
			sampler2D _Voronoise2;
			sampler2D _VertexPerlin;
			
			v2f vert (appdata v)
			{
				v2f o;
				v.vertex.xyz += fireRingVertOffset(v.uv, v.normal.xyz, _VertexPerlin);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_Gradient, i.uv);
				return fireRingFragment(i.uv, _Gradient, _Voronoise1, _Voronoise2);	
			}

			ENDCG
		}

		//FRONT Pass
		Pass
		{
			Blend One OneMinusSrcAlpha		
			Cull Back
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "FireRing.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _Gradient;
			sampler2D _Voronoise1;
			sampler2D _Voronoise2;
			sampler2D _VertexPerlin;


			v2f vert (appdata v)
			{
				v2f o;
				v.vertex.xyz += fireRingVertOffset(v.uv, v.normal.xyz, _VertexPerlin);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_Gradient, i.uv);
				return fireRingFragment(i.uv, _Gradient, _Voronoise1, _Voronoise2);	
			}

			ENDCG
		}
	}
}
