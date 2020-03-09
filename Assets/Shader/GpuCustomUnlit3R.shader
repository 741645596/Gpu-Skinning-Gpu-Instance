Shader "GpuCustomUnlit3R"
{
	Properties{
		_MainTex("texture", 2D) = "black"{}
		_AniMeshTex("texture", 2D) = "black"{}
		_GroupColor("addColor", Color) = (0,0,0,0)
		_AlphaVal("alphaValue",range(0,1)) = 0.5
		_frame("frame", Int) = 0
	}



	SubShader{
		Pass{
		CGPROGRAM
        
		#pragma vertex vert
		#pragma fragment frag
        #pragma enable_d3d11_debug_symbols

		#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _MainTex_ST;

			sampler2D _AniMeshTex;
			half4 _AniMeshTex_ST;
			float4 _AniMeshTex_TexelSize;
			int _frame;
			fixed4 _GroupColor;
			fixed _AlphaVal;

			struct vIn {
				half4 vertex:POSITION;
				float2 uv:TEXCOORD0;
			};

			struct vOut {
				half4 pos:SV_POSITION;
				float2 uv:TEXCOORD0;
			};

			vOut vert(vIn In) {
				vOut o;

				//half f = _frame + _Time.y * 0.1f;
				half f= _frame * _AniMeshTex_TexelSize.y;
				// 采用顶点
				float2 meshuv;
				meshuv.y = f;


				meshuv.x = In.vertex.x;
				meshuv = TRANSFORM_TEX(meshuv, _AniMeshTex);
				half meshv = tex2Dlod(_AniMeshTex, float4(meshuv, 0, 0));
				In.vertex.x = meshv;
				//
				meshuv.x = In.vertex.y;
				meshuv = TRANSFORM_TEX(meshuv, _AniMeshTex);
				meshv = tex2Dlod(_AniMeshTex, float4(meshuv, 0, 0));
				In.vertex.y = meshv;
				//
				meshuv.x = In.vertex.z;
				meshuv = TRANSFORM_TEX(meshuv, _AniMeshTex);
				meshv = tex2Dlod(_AniMeshTex, float4(meshuv, 0, 0));
				In.vertex.z = meshv;
                //
				o.pos = UnityObjectToClipPos(In.vertex);
				o.uv = TRANSFORM_TEX(In.uv, _MainTex);
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
