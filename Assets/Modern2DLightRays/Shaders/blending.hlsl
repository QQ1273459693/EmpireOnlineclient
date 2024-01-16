#ifndef HLSLBLENDINGINC
#define HLSLBLENDINGINC

float4 rays;
float4 screen;
float4 res;
float mode;

void Unity_Blend_Normal(float4 Base, float4 Blend, float Opacity, out float4 Out)
{
    Out = lerp(Base, Blend, Opacity);
}

void Unity_Blend_Burn(float4 Base, float4 Blend, float Opacity, out float4 Out)
{
    Out =  1.0 - (1.0 - Blend)/Base;
    Out = lerp(Base, Out, Opacity);
}

void Unity_Blend_Darken(float4 Base, float4 Blend, float Opacity, out float4 Out)
{
    Out = min(Blend, Base);
    Out = lerp(Base, Out, Opacity);
}

void Unity_Blend_Difference(float4 Base, float4 Blend, float Opacity, out float4 Out)
{
    Out = abs(Blend - Base);
    Out = lerp(Base, Out, Opacity);
}

void Unity_Blend_Dodge(float4 Base, float4 Blend, float Opacity, out float4 Out)
{
    Out = Base / (1.0 - Blend);
    Out = lerp(Base, Out, Opacity);
}

void Unity_Blend_Divide(float4 Base, float4 Blend, float Opacity, out float4 Out)
{
    Out = Base / (Blend + 0.000000000001);
    Out = lerp(Base, Out, Opacity);
}

void Unity_Blend_Exclusion(float4 Base, float4 Blend, float Opacity, out float4 Out)
{
    Out = Blend + Base - (2.0 * Blend * Base);
    Out = lerp(Base, Out, Opacity);
}

void Unity_Blend_HardLight(float4 Base, float4 Blend, float Opacity, out float4 Out)
{
    float4 result1 = 1.0 - 2.0 * (1.0 - Base) * (1.0 - Blend);
    float4 result2 = 2.0 * Base * Blend;
    float4 zeroOrOne = step(Blend, 0.5);
    Out = result2 * zeroOrOne + (1 - zeroOrOne) * result1;
    Out = lerp(Base, Out, Opacity);
}

void Unity_Blend_HardMix(float4 Base, float4 Blend, float Opacity, out float4 Out)
{
    Out = step(1 - Base, Blend);
    Out = lerp(Base, Out, Opacity);
}

void Unity_Blend_Lighten(float4 Base, float4 Blend, float Opacity, out float4 Out)
{
    Out = max(Blend, Base);
    Out = lerp(Base, Out, Opacity);
}

void Unity_Blend_LinearBurn(float4 Base, float4 Blend, float Opacity, out float4 Out)
{
    Out = Base + Blend - 1.0;
    Out = lerp(Base, Out, Opacity);
}

void Unity_Blend_LinearDodge(float4 Base, float4 Blend, float Opacity, out float4 Out)
{
    Out = Base + Blend;
    Out = lerp(Base, Out, Opacity);
}

void Unity_Blend_LinearLight(float4 Base, float4 Blend, float Opacity, out float4 Out)
{
    Out = Blend < 0.5 ? max(Base + (2 * Blend) - 1, 0) : min(Base + 2 * (Blend - 0.5), 1);
    Out = lerp(Base, Out, Opacity);
}

void Unity_Blend_LinearLightAddSub(float4 Base, float4 Blend, float Opacity, out float4 Out)
{
    Out = Blend + 2.0 * Base - 1.0;
    Out = lerp(Base, Out, Opacity);
}

void Unity_Blend_Multiply(float4 Base, float4 Blend, float Opacity, out float4 Out)
{
    Out = Base * Blend;
    Out = lerp(Base, Out, Opacity);
}

void Unity_Blend_Negation(float4 Base, float4 Blend, float Opacity, out float4 Out)
{
    Out = 1.0 - abs(1.0 - Blend - Base);
    Out = lerp(Base, Out, Opacity);
}

void Unity_Blend_Overlay(float4 Base, float4 Blend, float Opacity, out float4 Out)
{
    float4 result1 = 1.0 - 2.0 * (1.0 - Base) * (1.0 - Blend);
    float4 result2 = 2.0 * Base * Blend;
    float4 zeroOrOne = step(Base, 0.5);
    Out = result2 * zeroOrOne + (1 - zeroOrOne) * result1;
    Out = lerp(Base, Out, Opacity);
}

void Unity_Blend_PinLight(float4 Base, float4 Blend, float Opacity, out float4 Out)
{
    float4 check = step (0.5, Blend);
    float4 result1 = check * max(2.0 * (Base - 0.5), Blend);
    Out = result1 + (1.0 - check) * min(2.0 * Base, Blend);
    Out = lerp(Base, Out, Opacity);
}

void Unity_Blend_Screen(float4 Base, float4 Blend, float Opacity, out float4 Out)
{
    Out = 1.0 - (1.0 - Blend) * (1.0 - Base);
    Out = lerp(Base, Out, Opacity);
}

void Unity_Blend_SoftLight(float4 Base, float4 Blend, float Opacity, out float4 Out)
{
    float4 result1 = 2.0 * Base * Blend + Base * Base * (1.0 - 2.0 * Blend);
    float4 result2 = sqrt(Base) * (2.0 * Blend - 1.0) + 2.0 * Base * (1.0 - Blend);
    float4 zeroOrOne = step(0.5, Blend);
    Out = result2 * zeroOrOne + (1 - zeroOrOne) * result1;
    Out = lerp(Base, Out, Opacity);
}

void Unity_Blend_Subtract(float4 Base, float4 Blend, float Opacity, out float4 Out)
{
    Out = Base - Blend;
    Out = lerp(Base, Out, Opacity);
}

void Unity_Blend_VividLight(float4 Base, float4 Blend, float Opacity, out float4 Out)
{
    float4 result1 = 1.0 - (1.0 - Blend) / (2.0 * Base);
    float4 result2 = Blend / (2.0 * (1.0 - Base));
    float4 zeroOrOne = step(0.5, Base);
    Out = result2 * zeroOrOne + (1 - zeroOrOne) * result1;
    Out = lerp(Base, Out, Opacity);
}

void Blending_float(in float4 rays,in float4 screen, in float opacity, in float mode, out float4 res)
{
   if(mode == 0) Unity_Blend_Normal(screen,rays,opacity,res);
   else if(mode == 1) Unity_Blend_Burn(screen,rays,opacity,res);
   else if(mode == 2) Unity_Blend_Darken(screen,rays,opacity,res);
   else if(mode == 3) Unity_Blend_Difference(screen,rays,opacity,res);
   else if(mode == 4) Unity_Blend_Dodge(screen,rays,opacity,res);
   else if(mode == 5) Unity_Blend_Divide(screen,rays,opacity,res);
   else if(mode == 6) Unity_Blend_Exclusion(screen,rays,opacity,res);
   else if(mode == 7) Unity_Blend_HardLight(screen,rays,opacity,res);
   else if(mode == 8) Unity_Blend_HardMix(screen,rays,opacity,res);
   else if(mode == 9) Unity_Blend_Lighten(screen,rays,opacity,res);
   else if(mode == 10) Unity_Blend_LinearBurn(screen,rays,opacity,res);
   else if(mode == 11) Unity_Blend_LinearDodge(screen,rays,opacity,res);
   else if(mode == 12) Unity_Blend_LinearLight(screen,rays,opacity,res);
   else if(mode == 13) Unity_Blend_LinearLightAddSub(screen,rays,opacity,res);
   else if(mode == 14) Unity_Blend_Multiply(screen,rays,opacity,res);
   else if(mode == 15) Unity_Blend_Negation(screen,rays,opacity,res);
   else if(mode == 16) Unity_Blend_Overlay(screen,rays,opacity,res);
   else if(mode == 17) Unity_Blend_PinLight(screen,rays,opacity,res);
   else if(mode == 18) Unity_Blend_Screen(screen,rays,opacity,res);
   else if(mode == 19) Unity_Blend_SoftLight(screen,rays,opacity,res);
   else if(mode == 20) Unity_Blend_Subtract(screen,rays,opacity,res);
   else if(mode == 21) Unity_Blend_VividLight(screen,rays,opacity,res);
   else Unity_Blend_Normal(screen,rays,opacity,res);
}

#endif