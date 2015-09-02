Shader "Custom/ScrollMaskShaderForText"
{ 
	Properties
	{ 
	   _MainTex ("Font Texture", 2D) = "white" {} 
	   _Color ("Text Color", Color) = (1,1,1,1)
	   _Clip ("ViewportClipRect", Vector) = (0,0,0,0)
	} 

	SubShader
	{ 
	   Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" } 
	   Lighting Off Cull Off ZWrite Off Fog { Mode Off } 
	   Blend SrcAlpha OneMinusSrcAlpha 
	   
	   
	   
	   Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            float4 _Color;
            sampler2D _MainTex;
             float4 _MainTex_ST;
             float4 _Clip;

            struct vertOut {
                float4 pos:SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float4 scrPos;
            };

            vertOut vert(appdata_base v) {
                vertOut o;
                o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
                o.texcoord = v.texcoord;
                o.scrPos = ComputeScreenPos(o.pos);
                return o;
            }

            fixed4 frag(vertOut i) : COLOR {
                float2 wcoord = (i.scrPos.xy/i.scrPos.w);
                //fixed4 color = 
				
				fixed4 texcol = tex2D(_MainTex, i.texcoord) * _Color;
				
				if((wcoord.x < _Clip.x)||(wcoord.x > (_Clip.x + _Clip.z))||(wcoord.y > _Clip.y)||(wcoord.y < (_Clip.y - _Clip.w)))
				{
					texcol.r = 1;
					texcol.b = 0;
					texcol.g = 0;
					texcol.a = 0;// = (1,1,0,1);
				}
				
				return texcol;
            }

            ENDCG
	   
	       Color [_Color] 
	      	SetTexture [_MainTex] {   combine primary, texture * primary   } 
	   } 
	} 
}