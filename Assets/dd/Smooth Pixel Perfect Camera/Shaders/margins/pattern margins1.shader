Shader "SPPC2D/color margins"
{
    Properties
    {
        tex ("tex", 2D) = "white" {}
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
            uniform float time;

            /////////////////

            // your variables :

            /////////////////

            inline float2 UV_TO_ID(float2 uv)
            {
                return float2( floor(uv.x * width), floor(uv.y * heigth) );
            }

            inline float2 ID_TO_UV(float2 id)
            {
                return float2( floor(id.x / width), floor(id.y / heigth) );
            }

            float rand(float2 input){
	            return frac(sin(dot(input,float2(23.53,44.0)))*42350.45);
            }

            float perlin(float2 input){
	            float2 i = floor(input);
	            float2 j = frac(input);
	            float2 coord = smoothstep(0.0,1.0,j);
	
	            float a = rand(i);
	            float b = rand(i+float2(1.0,0.0));
	            float c = rand(i+float2(0.0,1.0));
	            float d = rand(i+float2(1.0,1.0));

	            return lerp(lerp(a,b,coord.x),lerp(c,d,coord.x),coord.y);
            }

            float fbm(float2 input){
	            float value = 0.0;
	            float scale = 0.6;
	
	            for(int i = 0; i < 6; i++){
		            value += perlin(input)*scale;
		            input*=2.0;
		            scale*=0.5;
	            }
	            return value;
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
                float4 res = float4(ppUV.x , ppUV.y ,0 , 0);  //my effect displays the pp uv map

                float2 UV = i.uv;

                float mulscale = 12.0;
                float height = 0.6;
                float tide = 0.1;
                float foamthickness = 0.1;
                float timescale = 1.0;
                float waterdeep = 0.3;
                float4 WATER_COL  =  float4(0.04, 0.68, 0.88, 1.0);
                float4 WATER2_COL =  float4(0.04, 0.35, 0.78, 1.0);
                float4 FOAM_COL  = float4(0.8125, 0.9609, 0.9648, 1.0);

	            float newtime = time * timescale;
	            float fbmval = fbm(float2(UV.x*mulscale+0.2*sin(0.3*newtime)+0.15*newtime,-0.05*newtime+UV.y*mulscale+0.1*cos(0.68*newtime)));
	            float fbmvalshadow = fbm(float2(UV.x*mulscale+0.2*sin(-0.6*newtime + 25.0 * UV.y)+0.15*newtime+3.0,-0.05*newtime+UV.y*mulscale+0.13*cos(-0.68*newtime))-7.0+0.1*sin(0.43*newtime));
	            float myheight = height+tide*sin(newtime+5.0*UV.x-8.0*UV.y);
	            float shadowheight = height+tide*1.3*cos(newtime+2.0*UV.x-2.0*UV.y);
	            float withinFoam = step(myheight, fbmval)*step(fbmval, myheight + foamthickness);
	            float shadow = (1.0-withinFoam)*step(shadowheight, fbmvalshadow)*step(fbmvalshadow, shadowheight + foamthickness * 0.7);
	            return  withinFoam*FOAM_COL + shadow*WATER2_COL + ((1.0-withinFoam)*(1.0-shadow))*WATER_COL;

            }
            ENDCG
        }
    }
}
