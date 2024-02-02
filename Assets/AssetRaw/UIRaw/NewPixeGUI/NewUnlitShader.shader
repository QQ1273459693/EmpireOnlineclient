Shader "PostEffect/SeparableGlassBlur" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "" {}
	}

	HLSLINCLUDE
	
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

	struct appdata_img {
		float4 vertex:POSITION;
		float2 texcoord:TEXCOORD0;
	};
	struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;

		float4 uv01 : TEXCOORD1;
		float4 uv23 : TEXCOORD2;
		float4 uv45 : TEXCOORD3;
	};
	
	float4 offsets;
	
	sampler2D _MainTex;
	
	v2f vert (appdata_img v) {
		v2f o;
		VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
		o.pos = vertexInput.positionCS;
		o.uv.xy = v.texcoord.xy;

		o.uv01 =  v.texcoord.xyxy + offsets.xyxy * float4(1,1, -1,-1);
		o.uv23 =  v.texcoord.xyxy + offsets.xyxy * float4(1,1, -1,-1) * 2.0;
		o.uv45 =  v.texcoord.xyxy + offsets.xyxy * float4(1,1, -1,-1) * 3.0;
		return o;
	}
	
	half4 frag (v2f i) : COLOR {
		half4 color = float4 (0,0,0,0);

		color += 0.40 * tex2D (_MainTex, i.uv);
		color += 0.15 * tex2D (_MainTex, i.uv01.xy);
		color += 0.15 * tex2D (_MainTex, i.uv01.zw);
		color += 0.10 * tex2D (_MainTex, i.uv23.xy);
		color += 0.10 * tex2D (_MainTex, i.uv23.zw);
		color += 0.05 * tex2D (_MainTex, i.uv45.xy);
		color += 0.05 * tex2D (_MainTex, i.uv45.zw);
		
		return color;
	}

	ENDHLSL
	
Subshader {
 Pass {
	  ZTest Always Cull Off ZWrite Off

      HLSLPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      ENDHLSL
  }
}

}