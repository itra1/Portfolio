// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/PBR" {
	Properties {
		_BaseColor("Base Color", Color) = (1,1,1,1)
		_NormalVal("Normal Value", Range(0.001,2)) = 1.0
		_RoughnessVal("Roughness Value", Range(0,1)) = 1.0
		_MetallicVal("Metallic Value", Range(0,1)) = 1.0
		
		_AlbedoMap("Albedo Map", 2D) = "white" {}
		_CutOut("Alpha CutOut", Range(0,1)) = 1.0
		_NormalMap("Normal Map", 2D) = "bump" {}
		[MaterialToggle] _InvertRoughnessMap("Invert Roughness Map", Float) = 0
		_RoughnessMap("Roughness Map", 2D) = "black" {}
		[MaterialToggle] _InvertMetallicMap("Invert Metallic Map", Float) = 0
		_MetallicMap("Metallic Map", 2D) = "black" {}		
	}

	SubShader {
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma multi_compile ALBEDO_MAP_ON ALBEDO_MAP_OFF
		#pragma multi_compile ALPHA_CUTOUT_ON ALPHA_CUTOUT_OFF
		#pragma multi_compile NORMAL_MAP_ON NORMAL_MAP_OFF
		#pragma multi_compile ROUGHNESS_MAP_ON ROUGHNESS_MAP_OFF
		#pragma multi_compile METALLIC_MAP_ON METALLIC_MAP_OFF
		#pragma target 3.0
		
		fixed4 _BaseColor;
		half _CutOut;
		half _NormalVal;
		float _InvertRoughnessMap;
		half _RoughnessVal;
		float _InvertMetallicMap;
		half _MetallicVal;
		
		sampler2D _AlbedoMap;
		sampler2D _NormalMap;
		sampler2D _RoughnessMap;
		sampler2D _MetallicMap;

		struct Input {
			float2 uv_AlbedoMap;
			float2 uv_NormalMap;
			float2 uv_RoughnessMap;
			float2 uv_MetallicMap;
		};

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
		// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o) {
			#if ALBEDO_MAP_ON
			fixed4 c = tex2D(_AlbedoMap, IN.uv_AlbedoMap) * _BaseColor;
			#else
			fixed4 c = _BaseColor;
			#endif
			o.Albedo = c.rgb;
			o.Alpha = c.a;

			#if ALPHA_CUTOUT_ON
			clip(c.a - _CutOut);
			#endif

			#if NORMAL_MAP_ON
			fixed3 normal = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap));
			normal.z = normal.z / _NormalVal; 
			o.Normal = normalize(normal);
			#endif
			
			#if ROUGHNESS_MAP_ON
			float4 r_tex = tex2D(_RoughnessMap, IN.uv_RoughnessMap);
			if (_InvertRoughnessMap)
				o.Smoothness = float4(1.0 - r_tex.rgb, 1.0) * _RoughnessVal;
			else
				o.Smoothness = r_tex * _RoughnessVal;
			#else
			o.Smoothness = _RoughnessVal;
			#endif	

			#if METALLIC_MAP_ON
			float4 m_tex = tex2D(_MetallicMap, IN.uv_MetallicMap);
			if (_InvertMetallicMap)
				o.Metallic = float4(1.0 - m_tex.rgb, 1.0) * _MetallicVal;
			else
				o.Metallic = m_tex * _MetallicVal;
			#else
			o.Metallic = _MetallicVal;
			#endif			
		}

		ENDCG
	}

	FallBack "Diffuse"
	CustomEditor "PBR_Inspector"
}