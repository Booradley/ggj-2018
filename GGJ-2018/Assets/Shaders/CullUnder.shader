Shader "Custom/underY" {
     Properties {
         _MainTex ("Base (RGB)", 2D) = "white" {}
         _objWorldPosition ("Object World Position (Set in Code)", Vector) = (0, 0, 0, 0)
         _objNormal ("Object Normal (Set in Code)", Vector) = (0, 0, 0, 0)
     }
     SubShader {
         Tags { "RenderType"="Opaque" }
         Cull Off
         CGPROGRAM
         #pragma surface surf Lambert
 
         sampler2D _MainTex;
         uniform float3 _objWorldPosition;
         uniform float3 _objNormal;
 
         struct Input {
             float3 worldPos;
             float2 uv_MainTex;
         };
 
         void surf (Input IN, inout SurfaceOutput o) {
             clip (IN.worldPos.y - _objWorldPosition.y);
             half4 c = tex2D (_MainTex, IN.uv_MainTex);
             o.Albedo = c.rgb;
             o.Alpha = c.a;
         }
         ENDCG
     } 
     FallBack "Diffuse"
 }