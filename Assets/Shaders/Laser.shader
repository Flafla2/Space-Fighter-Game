Shader "Custom/Laser" {
	Properties {
		_Core ("Core Color", Color) = (1,1,1,1)
		_Outer ("Outer Color", Color) = (0,0,1,0.5)
		_CoreSup ("Core Supression", Float) = 2.0
	}

	SubShader {
		Tags {"Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 200

		CGPROGRAM
		#pragma surface surf NoLighting alpha
		#include "UnityCG.cginc"
		
		fixed4 _Core;
		fixed4 _Outer;
		float _CoreSup;

		struct Input {
			float3 viewDir;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = lerp(_Core,_Outer,1-pow(abs(dot(o.Normal,IN.viewDir)/(length(IN.viewDir)*length(o.Normal))),_CoreSup));
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
