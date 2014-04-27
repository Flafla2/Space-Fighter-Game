Shader "Custom/GUI Text Messages" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color1 ("Oscillating Color 1", Color) = (1,1,1,1)
		_Color2 ("Oscillating Color 2", Color) = (1,1,0,1)
		_OscTime ("Oscilate Period (s)", Float) = 1
		_ScanSize ("Scanline Size", Float) = 1.0
		_ScanColor ("Scanline Color", Color) = (0.2,0.2,0.2,1)
	}
	SubShader {
		Tags { "IgnoreProjector"="True" "RenderType"="Opaque" "Queue"="Overlay"}
		Fog {Mode Off}
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert finalcolor:modifier
		// We aren't targeting flash, so we will exclude it - prevents shader warnings.
		#pragma exclude_renderers flash
		#include "UnityCG.cginc"
		
		sampler2D _MainTex;
		float _ScanSize;
		float _OscTime;
		float4 _ScanColor;
		float4 _Color1;
		float4 _Color2;

		struct Input {
			float2 uv_MainTex;
			float4 screenPos;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
		}
		
		void modifier (Input IN, SurfaceOutput o, inout fixed4 color) {
			float2 scrCoords = _ScreenParams.xy * (IN.screenPos.xy / IN.screenPos.w);
			
		color *= lerp(_Color1,_Color2,(sin(_Time[1]*6.2832/_OscTime)+1)/2);
			fixed alpha = color.a;
			if((int)(scrCoords.y/max(0.0001,_ScanSize))%2 == 0)
				color = lerp(color,_ScanColor,_ScanColor.a);
			
			color.a = 1;
		}
		
//		fixed4 LightingFixedLight(SurfaceOutput s, fixed3 lightDir, fixed atten)
//	    {
//	        fixed4 c;
//	        c.rgb = s.Albedo; 
//	        c.a = s.Alpha;
//	        
//			float3 L = normalize(_LightDirection);
//			float diffuseLight = max(dot(s.Normal, L), 0);
//			c.rgb *= (_LightColor.rgb * diffuseLight);
//			
//	        return c;
//	    }
		ENDCG
	} 
	FallBack "Diffuse"
}
