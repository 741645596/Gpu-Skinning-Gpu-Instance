Shader "CustomUnlitAlpha"
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

			sampler2D _MainTex;
			fixed4 _MainTex_ST;
			fixed4 _GroupColor;
			fixed _AlphaVal;

			struct vIn {
				half4 vertex:POSITION;
				float2 texcoord:TEXCOORD0;
				
			};

			struct vOut {
				half4 pos:SV_POSITION;
				float2 uv:TEXCOORD0;
			};

			vOut vert(vIn v) {
				vOut o;
				o.pos = UnityObjectToClipPos(v.vertex);
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
