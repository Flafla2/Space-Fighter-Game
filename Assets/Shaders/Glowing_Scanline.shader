Shader "Custom/Scanline" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ScanSize ("Scanline Size", Float) = 1.0
		_ScanColor ("Scanline Color", Color) = (0.2,0.2,0.2,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		float _ScanSize;
		float4 _ScanColor;

		struct Input {
			float2 uv_MainTex;
			float4 screenPos;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			float2 scrCoords = _ScreenParams.xy * (IN.screenPos.xy / IN.screenPos.w);
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			
			if((int)(scrCoords.y/max(0,_ScanSize))%2 == 0)
				o.Albedo = lerp(o.Albedo,_ScanColor,_ScanColor.a);
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
