Shader "SPPC2D/color margins"
{
    Properties
    {
        tex ("tex", 2D) = "white" {}
        marginsColor ("marginsColor", Color) = (.25, .5, .5, 1)
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

            ///////////////// 
            
            // recuring variables : 

            uniform float ppu;  //  pixels per unit (used for pixel perfect effects)
            uniform float2 ps;  // size of one pixel in uv space

            uniform float width;  //  width of tex
            uniform float heigth; //  heigth of tex

            uniform float cropMarginX;    //  size of x margin
            uniform float cropMarginY;    //  size of y margin

            uniform float scale;

            uniform sampler2D tex;
            uniform float4 tex_ST;

            /////////////////

            // your variables :

            uniform float3 marginsColor;

            /////////////////

            inline float2 UV_TO_ID(float2 uv)
            {
                return float2( floor(uv.x * width), floor(uv.y * heigth) );
            }

            inline float2 ID_TO_UV(float2 id)
            {
                return float2( floor(id.x / width), floor(id.y / heigth) );
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

           float stretch;
            fixed4 frag (v2f i) : SV_Target
            {
                //converts uv to pixel coords
                float2 id = UV_TO_ID(i.uv);

                // quits if we aren't on margins
                if( stretch == 1 || (id.x > cropMarginX - 1 && (width - id.x + 1) > cropMarginX) && (id.y > cropMarginY - 1 && (heigth - id.y + 1) > cropMarginY) ) return tex2D(tex,i.uv);

                float2 smoothUV = i.uv;  //  normal uv coords for math
    
                float2 ppID = float2( scale * floor( id.x / (float)scale ) , scale * floor( id.y / (float)scale ));    // pixel perfect id (before texture upscaling)

                float2 ppUV = float2(ppID.x / (float)width, ppID.y / (float)heigth);        // pixel perfect uv coords for math

                //write your effect here :
   
                float3 marginsColor = float3(0.3,0.75,0.9);  //  SET COLOR

                float4 res = float4(ppUV.x , ppUV.y ,0 , 0);  //my effect displays the pp uv map

                return (half4)res;
            }
            ENDCG
        }
    }
}
