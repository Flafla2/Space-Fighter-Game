Shader "Custom/Unlit (No Depth)" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_HideColor ("Hidden Color", Color) = (1,1,1,0.5)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	}

	SubShader {
		Tags {"Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 200
		
		ZTest Greater
		ZWrite On
				
		CGPROGRAM								// Second pass - Behind other objects
		#pragma surface surf NoLighting alpha
		
		sampler2D _MainTex;
		fixed4 _HideColor;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _HideColor;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		
		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
	    {
	        fixed4 c;
	        c.rgb = s.Albedo; 
	        c.a = s.Alpha;
	        return c;
	    }
	    
		ENDCG
		
		ZTest LEqual
		ZWrite Off
		
		CGPROGRAM								// First Pass - In front of other objects
		#pragma surface surf NoLighting alpha
		
		sampler2D _MainTex;
		fixed4 _Color;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		
		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
	    {
	        fixed4 c;
	        c.rgb = s.Albedo; 
	        c.a = s.Alpha;
	        return c;
	    }
	    
		ENDCG
	}

	Fallback "Transparent/VertexLit"
}
