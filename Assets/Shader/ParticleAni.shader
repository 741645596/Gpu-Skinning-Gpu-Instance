//https://docs.unity3d.com/ScriptReference/ParticleSystemRenderer.SetActiveVertexStreams.html
Shader "Unlit/ParticleAni"
{
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_TotalFrame("TotalFrame",float) = 8
		_TotalDir("TotalDir",float) = 8
		//[IntRange] _Dir("Dir",Range(0,7)) = 0
	}
		SubShader{
			Pass
			{
			//不透明物体
			 Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
			 ZWrite Off
			 Blend SrcAlpha OneMinusSrcAlpha
			//Cull Off

		   //CG开始
		   CGPROGRAM
		   #pragma vertex vert
		   #pragma fragment frag

		   #include "UnityCG.cginc"
			//引用变量值
			uniform sampler2D _MainTex;
			fixed4 _MainTex_ST;
			float _TotalFrame;
			float _TotalDir;
			//float _Dir;

			struct vIn {
				half4 vertex:POSITION;
				float3 texcoordAndCustom : TEXCOORD0;

			};
			//顶点输出结构体
			struct VertexOutput
			{
				float4 pos:SV_POSITION;
				float2 uv:TEXCOORD0;
				float customData : TEXCOORD1;
			};

			 VertexOutput vert(vIn input)
			 {
				 VertexOutput v_output;
				 v_output.pos = UnityObjectToClipPos(input.vertex);
				 fixed2 spriteUV = input.texcoordAndCustom.xy;

				 float cellPercentage = 1.0 / _TotalFrame; //每一个小图占百分比;

				 float xValue = spriteUV.x;

				 //UV默认是(0,1)，就是说默认显示整个大图，我们要显示一个小图,UV要指定到(0,1/9)的范围;
				 xValue = xValue * (1.0 / _TotalFrame);

				 //再对时间取整，如果 _Time.y 时间超过了1，就加一个小图这么宽的位移，也就是把x 指到下一个小图的范围。就显示出下一个小图
				 xValue += cellPercentage * ceil(_Time.y * 4);  //_Time.y 等同于 Time.timeSinceLevelLoad,就是游戏运行时间

				 //再赋值;
				 spriteUV.x = xValue;
				 //spriteUV.y = (spriteUV.y + _Dir) / _TotalDir;
				 spriteUV.y = (spriteUV.y + input.texcoordAndCustom.z) / _TotalDir;
				 v_output.uv = TRANSFORM_TEX(spriteUV, _MainTex);
				 v_output.customData = input.texcoordAndCustom.z;
				 return v_output;
			 }

			 float4 frag(VertexOutput input) :COLOR
			 {
				 //获取二维文理坐标color 显示到屏幕      
				 float4 col = tex2D(_MainTex, input.uv);
				 return col;
			  }
				 //CG结束
				 ENDCG
			}
		}
}
