Shader "GpuCustomAlphaSkin"
{
	Properties{
		_MainTex("texture", 2D) = "black"{}
		_GroupColor("addColor", Color) = (0,0,0,0)
		_AlphaVal("alphaValue",range(0,1)) = 0.5
		_BoneMatrixTex("texture", 2D) = "black"{}
		_frame("frame", Float) = 0
	}

SubShader{
		Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile_instancing

		#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _MainTex_ST;
			sampler2D _BoneMatrixTex;
			half4 _BoneMatrixTex_ST;
			float4 _BoneMatrixTex_TexelSize;
			fixed4 _GroupColor;
			fixed _AlphaVal;

			UNITY_INSTANCING_BUFFER_START(Props)
				UNITY_DEFINE_INSTANCED_PROP(float, _frame)
		    UNITY_INSTANCING_BUFFER_END(Props)

			struct vIn {
				half4 vertex:POSITION;
				float2 texcoord:TEXCOORD0;
				float4 tangent : TANGENT;
				//float2 uv1 : TEXCOORD1;
				//float2 uv2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct vOut {
				half4 pos:SV_POSITION;
				float2 uv:TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			inline float4 indexToUV(float x, float y)
			{
			    float2 meshuv = float2(x * _BoneMatrixTex_TexelSize.x, y);
				meshuv = TRANSFORM_TEX(meshuv, _BoneMatrixTex);
				return float4(meshuv, 0, 0);
			}

			inline float4x4 GetMatrix(float x, float y)
			{
				float4 row0 = tex2Dlod(_BoneMatrixTex, indexToUV(x * 3, y));
				float4 row1 = tex2Dlod(_BoneMatrixTex, indexToUV(x * 3 + 1, y));
				float4 row2 = tex2Dlod(_BoneMatrixTex, indexToUV(x * 3 + 2, y));
				float4 row3 = float4(0, 0, 0, 1);
				float4x4 mat = float4x4(row0, row1, row2, row3);
				return mat;
			}



			vOut vert(vIn v) {
				vOut o;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				//half f = _frame + _Time.y * 0.1f;
				//half f= _frame * _BoneMatrixTex_TexelSize.y;
				half f = UNITY_ACCESS_INSTANCED_PROP(Props, _frame) * _BoneMatrixTex_TexelSize.y;

				half4 pos =
					mul(GetMatrix(v.tangent.x, f), v.vertex) * v.tangent.y
					+ mul(GetMatrix(v.tangent.z, f), v.vertex) * v.tangent.w
					//+ mul(GetMatrix(v.uv1.x, f), v.vertex) * v.uv1.y 
					//+ mul(GetMatrix(v.uv2.x, f), v.vertex) * v.uv2.y
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