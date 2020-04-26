// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ProjectionTextureLocal"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		_LineSize("Line Size", Float) = 1.0
		_LineColor("Line Color", Color) = ( 0, 0, 0, 0 )
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		ZWrite On
		Cull Off

		Pass
		{
			Stencil
			{
				Ref 1
				Comp Always
				Pass Replace
			}

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

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				float sx = length(float3(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0]));
				float sy = length(float3(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1]));
				float2 uv = float2(v.vertex.x * sx, v.vertex.y * sy);
				o.uv = TRANSFORM_TEX(uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return tex2D(_MainTex, i.uv);
			}
			ENDCG
		}
		Pass
		{
			Stencil
			{
				Ref 1
				Comp NotEqual
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};
			
			float _LineSize;
			fixed4 _LineColor;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = mul(unity_ObjectToWorld, v.vertex);
				o.vertex.x += _LineSize;
				o.vertex.y -= _LineSize;
				o.vertex.z += 1.0f;
				o.vertex = mul(UNITY_MATRIX_VP, o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return _LineColor;
			}
			ENDCG
		}
	}
}
