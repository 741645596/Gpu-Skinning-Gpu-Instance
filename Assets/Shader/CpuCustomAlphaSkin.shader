Shader "CpuCustomAlphaSkin"
{
	Properties{
		_MainTex("texture", 2D) = "black"{}
		_GroupColor("addColor", Color) = (0,0,0,0)
		_AlphaVal("alphaValue",range(0,1)) = 0.5
	}

SubShader{
		Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"


		uniform float4x4 _Matrices[24];


			sampler2D _MainTex;
			fixed4 _MainTex_ST;
			fixed4 _GroupColor;
			fixed _AlphaVal;

			struct vIn {
				half4 vertex:POSITION;
				float2 texcoord:TEXCOORD0;
				float4 tangent : TANGENT;
				float2 uv1 : TEXCOORD1;
				float2 uv2 : TEXCOORD2;
			};

			struct vOut {
				half4 pos:SV_POSITION;
				float2 uv:TEXCOORD0;
			};

			vOut vert(vIn v) {
				vOut o;

				half4 pos = 
					mul(_Matrices[v.tangent.x], v.vertex) * v.tangent.y + 
					mul(_Matrices[v.tangent.z], v.vertex) * v.tangent.w
					+ mul(_Matrices[v.uv1.x], v.vertex) * v.uv1.y 
					+ mul(_Matrices[v.uv2.x], v.vertex) * v.uv2.y
					;

				o.pos = UnityObjectToClipPos(pos);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			fixed4 frag(vOut i) :COLOR{

				fixed4 tex = tex2D(_MainTex, i.uv);
				
				if (tex.a > _AlphaVal)
				{
					fixed3 sampleColor;
					float grey = dot(tex.rgb, float3(0.5, 0.5, 0.5));
					sampleColor.rgb = 2*float3(grey, grey, grey) *_GroupColor;

					tex = fixed4(sampleColor.rgb,1);

				}

				
				return tex ;
			}
				ENDCG
			}
	}





}