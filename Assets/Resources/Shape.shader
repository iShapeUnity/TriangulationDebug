Shader "Custom/Shape"
{
    SubShader {
		
        Tags {
			"Queue" = "Transparent+25000"
			"IgnoreProjector" = "True"
			"RenderType" = "Opaque"
			"PreviewType" = "Plane"
		}

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		ZTest Off
        
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata {
                float4 vertex : POSITION;
                float4 color : COLOR;
            };
            
            struct v2f {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };
            
            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target {
                return i.color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"

}
