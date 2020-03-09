Shader "GpuCustomUnlitRGBA"
{
	Properties{
		_MainTex("texture", 2D) = "black"{}
		_AniMeshTex("texture", 2D) = "black"{}
		_GroupColor("addColor", Color) = (0,0,0,0)
		_AlphaVal("alphaValue",range(0,1)) = 0.5
		_frame("frame", Float) = 0
	}



	SubShader{
		Pass{
		CGPROGRAM
        
		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile_instancing
		//#pragma enable_d3d11_debug_symbols
		//#pragma target 3.0
		#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _MainTex_ST;

			sampler2D _AniMeshTex;
			half4 _AniMeshTex_ST;
			float4 _AniMeshTex_TexelSize;
			fixed4 _GroupColor;
			fixed _AlphaVal;

			UNITY_INSTANCING_BUFFER_START(Props)
				UNITY_DEFINE_INSTANCED_PROP(float, _frame)
			UNITY_INSTANCING_BUFFER_END(Props)

			struct vIn {
				half4 vertex:POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct vOut {
				half4 pos:SV_POSITION;
				float2 uv:TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			vOut vert(vIn In) {
				vOut o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				// 采取uv
				float2 uv = float2(In.vertex.x, In.vertex.y);

				// 采用顶点
				half f= UNITY_ACCESS_INSTANCED_PROP(Props, _frame) * _AniMeshTex_TexelSize.y;
				float2 meshuv = float2(In.vertex.z, f);
				meshuv = TRANSFORM_TEX(meshuv, _AniMeshTex);
				half4 meshv = tex2Dlod(_AniMeshTex, float4(meshuv, 0, 0));
				//
				o.pos = UnityObjectToClipPos(meshv.rgb);
				o.uv = TRANSFORM_TEX(uv, _MainTex);
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