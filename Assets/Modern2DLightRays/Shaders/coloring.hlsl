#ifndef HLSLCOLORINC
#define HLSLCOLORINC

void ColorQuad_float(in float rays, in float2 uv, in float3 c1, in float3 c2, in float3 c3, in float3 c4, out float3 col )
{
	float3 c12 = lerp(c1, c2, uv.x);
	float3 c34 = lerp(c3, c4, uv.x);
	col = lerp(c12,c34,uv.y);
}


void ColorQuad_half(in half rays, in half2 uv, in half3 c1, in half3 c2, in half3 c3, in half3 c4, out half3 col )
{
	half3 c12 = lerp(c1, c2, uv.x);
	half3 c34 = lerp(c3, c4, uv.x);
	col = lerp(c12,c34,uv.y);
}

#endif