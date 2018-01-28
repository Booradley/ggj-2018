Shader "Custom/CullUnder" {
     Properties {
         [Header(Basic Settings)]
         [Space]
         _Color ("Main Color", Color) = (1,1,1,1)
         _Shininess ("Shininess", Range (0.03, 1)) = 0.078125
         _MainTex ("Base (RGB)", 2D) = "white" {}
         [Space(50)]
         [Header(Cull Settings)]
         [Space]
         _objWorldPosition ("Object World Position (Set in Code)", Vector) = (0, 0, 0, 0)
         _objNormal ("Object Normal (Set in Code)", Vector) = (0, 0, 0, 0)
     }
     SubShader {
         Tags { "RenderType"="Opaque" "Queue" = "Transparent" }
         Cull Off
         CGPROGRAM
         #pragma surface surf Lambert
 
         fixed4 _Color;
         half _Shininess;
         sampler2D _MainTex;
         uniform float3 _objWorldPosition;
         uniform float3 _objNormal;
 
         struct Input {
             float3 worldPos;
             float2 uv_MainTex;
         };

         float dot(in float3 v1, in float3 v2) {
             return v1.x*v2.x + v1.y*v2.y + v1.z*v2.z;
         }

         float3 getVector(in float3 v1, in float3 v2) {
             return float3(v2.x-v1.x, v2.y-v1.y, v2.z-v1.z);
         }
 
         void surf (Input IN, inout SurfaceOutput o) {
             clip (dot(_objNormal, getVector(_objWorldPosition, IN.worldPos)));
             half4 tex = tex2D (_MainTex, IN.uv_MainTex);
             o.Albedo = tex.rgb * _Color.rgb;
             o.Alpha = tex.a * _Color.a;
             o.Specular = _Shininess;
         }
         ENDCG
     } 
     FallBack "Specular"
 }