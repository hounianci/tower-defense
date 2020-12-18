Shader "Unlit/RangeShader"
{
	Properties {
	}
	SubShader {
        Pass {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

			struct a2v {
                float4 vertex : POSITION;
				float4 uv : TEXCOORD;
            };
            
            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv:TEXCOORD;
            };
            
            v2f vert(a2v v) {
            	v2f o;
            	o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed checker(float2 uv){
                float2 repeatUV=uv;
                float2 c = floor(repeatUV)/2;
                float checker = frac((repeatUV)*10);
                return checker;
            }

            fixed4 frag(v2f i) : SV_Target {
                fixed col = checker(i.uv);
                return col;
            }

            ENDCG
        }
    }
}
