Shader "SPPC2D/SmoothPixelPerfectCamera"
{
    Properties
    {
        _Tex ("Tex", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Tags { "RenderQueue"="Overlay" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            uniform float marginX;
            uniform float marginY;

            uniform float2 delta;
            uniform float2 ps;
            uniform float2 ps2;

            uniform float ppu;

            uniform float width;         
            uniform float heigth;         

            uniform float width2;        
            uniform float heigth2;        

            uniform int snapMovement;    

            uniform float cropMarginX;   
            uniform float cropMarginY;  

            uniform float stretch;

            sampler2D _Tex;
            float4 _Tex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 id = (i.uv) * float2(width,heigth);

                //crop
                if( stretch == 0 && ( id.x < cropMarginX || (width - id.x + 1) < cropMarginX || id.y < cropMarginY || (heigth - id.y + 1) < cropMarginY ) ) 
                {
                     return float4(0,0,0,0);
                }

                //pixel smooth margin 
                float px = marginX/2;
                float py = marginY/2;

                float resx = px; 
                float resy = py;

                //smooth coords
                resx -= floor(delta.x * ppu * marginX / 2);
                resy -= floor(delta.y * ppu * marginY / 2);

                //snap
                if(snapMovement) 
                {
                    resx = px; 
                    resy = py;
                }

                float2 uv2;
                float2 id2;

                //get final uv's
                if(stretch == 0)
                {
                    id2 = id.xy + int2(resx,resy) - int2(cropMarginX,cropMarginY);
                    uv2 = float2(id2.x / width2 , id2.y / heigth2);
                }
                else 
                {
                    id2 = ( (i.uv) * (float2(width2,heigth2)-float2(marginX,marginY)) ) + float2(resx,resy);
                    uv2 = float2(id2.x / width2 , id2.y / heigth2);
                }

                float4 col = tex2D(_Tex, uv2);
                return  col;
            }
            ENDCG
        }
    }
}
