// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/SSv2OGL2" {
	Properties {
      _Point ("Player point", Vector) = (0, 0, 0, 1)
	  
	  _Lights ("Lights array", 2D) = "white" { }
	  _LightsSize ("Size of lights array", Int) = 0
   }
	   SubShader {
		 Cull Off
   		 ZWrite Off
		 Blend SrcAlpha OneMinusSrcAlpha
   
      Pass {
         CGPROGRAM

         #pragma vertex vert  
         #pragma fragment frag
         #pragma target 3.0
 
         #include "UnityCG.cginc" 

         uniform float4 _Point;
         uniform float _DistanceNear;
		 
		 uniform int _LightsSize;
		 uniform sampler2D _Lights;
 
         struct vertexInput {
			float4 texcoord : TEXCOORD0;
            float4 vertex : POSITION;
         };
         
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 position_in_world_space : TEXCOORD0;
			float4 textPos : TEXCOORD1;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
			output.textPos = input.vertex;
            output.pos =  UnityObjectToClipPos(input.vertex);
            output.position_in_world_space = mul(unity_ObjectToWorld, input.vertex);
            return output;
         }
		 
		 inline float decode(float4 color)
		 {
			float f = 0;
			f += color.g * 255;
			f += color.b * 2.55;
			f += color.a * 0.0255;
			if (color.r > 0.9)
				f *= -1;
			return f;
		 }
		 
		 float draw(int i, float4 pos)
		 {
		 	float4 colored;
		 	int i3 = i * 3;
		 	int size = _LightsSize * 3;
			colored.x = decode(tex2D(_Lights, float2(0.5, (i3 + 0.5) / size)));
			colored.y = decode(tex2D(_Lights, float2(0.5, (i3 + 1.5) / size)));
			colored.z = 0;
			colored.w = decode(tex2D(_Lights, float2(0.5, (i3 + 2.5) / size)));
			float dist = distance(pos, colored);
			float delta = dist - colored.w;
			if (delta < 0)
				return 0;
			return delta;
		 }
 
         fixed4 frag(vertexOutput input) : COLOR 
         {
			fixed4 trans = fixed4(0.043, 0.29, 0.403,0);
		 
			float drawRes = 0;
			float delta = distance(input.position_in_world_space, _Point) - _Point.w;
			if (delta < 0)
				return trans;
							
			int iter = 0;

			// 7 источников света в кадре, вложенность из-за ошибки компиляции в d3d11
			if (iter < _LightsSize)
			{
				drawRes = draw(iter++, input.position_in_world_space);
				if (drawRes == 0) return trans; else delta = min(delta, drawRes);
				
				if (iter < _LightsSize)
				{
					drawRes = draw(iter++, input.position_in_world_space);
					if (drawRes == 0) return trans; else delta = min(delta, drawRes);
					
					if (iter < _LightsSize)
					{
						drawRes = draw(iter++, input.position_in_world_space);
						if (drawRes == 0) return trans; else delta = min(delta, drawRes);
						
						if (iter < _LightsSize)
						{
							drawRes = draw(iter++, input.position_in_world_space);
							if (drawRes == 0) return trans; else delta = min(delta, drawRes);
							
							if (iter < _LightsSize)
							{
								drawRes = draw(iter++, input.position_in_world_space);
								if (drawRes == 0) return trans; else delta = min(delta, drawRes);
								
								if (iter < _LightsSize)
								{
									drawRes = draw(iter++, input.position_in_world_space);
									if (drawRes == 0) return trans; else delta = min(delta, drawRes);
									
									if (iter < _LightsSize)
									{
										drawRes = draw(iter++, input.position_in_world_space);
										if (drawRes == 0) return trans; else delta = min(delta, drawRes);
										
										if (iter < _LightsSize)
										{
											drawRes = draw(iter++, input.position_in_world_space);
											if (drawRes == 0) return trans; else delta = min(delta, drawRes);
										}
									}
								}
							}
						}
					}
				}
			}
			
			return fixed4(0.043, 0.29, 0.403,min(1, delta * 1.5));
         }
 
         ENDCG  
      }
   }
}
