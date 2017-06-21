﻿Shader "Custom/SimpleToonShader" {
	Properties{
		_Color("Color", Color) = (1, 1, 1, 1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	}
	SubShader{
		Tags{
//			"Queue" = "Transparent"
			"RenderType" = "Transparent"
		}
		LOD 200

		Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
		#pragma surface surf CelShadingForward alpha
		#pragma target 3.0

		half4 LightingCelShadingForward(SurfaceOutput s, half3 lightDir, half atten) {
			half NdotL = dot(s.Normal, lightDir);
			if (NdotL <= 0.025) NdotL = 0;
			else if (NdotL <= 0.2755) NdotL = 0.25;
			else if (NdotL <= 0.5) NdotL = 0.5;
			else NdotL = 1;
			//NdotL = smoothstep(0, 0.025f, NdotL)*0.25 + smoothstep(0.25, 0.275f, NdotL)*0.25 + smoothstep(0.5, 0.525f, NdotL)*0.5;
			half4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten * 2);
			c.a = s.Alpha;
			return c;
		}

		sampler2D _MainTex;
		fixed4 _Color;

		struct Input {
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
		FallBack "Diffuse"
}
