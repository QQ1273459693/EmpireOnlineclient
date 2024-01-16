#ifndef HLSLFALLOFFINC
#define HLSLFALLOFFINC

void falloffRight_float(in float rays, in float start, in float end, in float uv_x, out float res)
{
	if(uv_x < start) res = rays;
	else if(uv_x > end) res = 0;
	else 
	{
		res = rays * ((end - uv_x) / (end-start) );
	}
}

void falloffLeft_float(in float rays, in float start, in float end, in float uv_x, out float res)
{
	if(uv_x < start) res = 0;
	else if(uv_x > end) res = rays;
	else 
	{
		res = rays * (1-((end - uv_x) / (end-start) ));
	}
}

void falloffTop_float(in float rays, in float start, in float end, in float uv_y, out float res)
{
	if(uv_y < start) res = rays;
	else if(uv_y > end) res = 0;
	else 
	{
		res = rays * ((end - uv_y) / (end-start) );
	}
}

void falloffBottom_float(in float rays, in float start, in float end, in float uv_y, out float res)
{
	if(uv_y < start) res = 0;
	else if(uv_y > end) res = rays;
	else 
	{
		res = rays * (1-((end - uv_y) / (end-start) ));
	}
}

#endif