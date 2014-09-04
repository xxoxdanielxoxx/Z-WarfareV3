// Shader created with Shader Forge Beta 0.36 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.36;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:5,bsrc:0,bdst:1,culm:0,dpts:2,wrdp:False,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:31572,y:32866|diff-154-OUT,amdfl-154-OUT;n:type:ShaderForge.SFN_Tex2d,id:2,x:34494,y:32675,ptlb:node_2,ptin:_node_2,tex:e540d5ecd223f8947a58783caeb85c93,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:3,x:34527,y:32874,ptlb:node_3,ptin:_node_3,tex:e540d5ecd223f8947a58783caeb85c93,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Normalize,id:4,x:34299,y:32675|IN-2-RGB;n:type:ShaderForge.SFN_Normalize,id:5,x:34299,y:32888|IN-3-RGB;n:type:ShaderForge.SFN_Dot,id:6,x:34055,y:32675,dt:0|A-4-OUT,B-9-OUT;n:type:ShaderForge.SFN_Dot,id:8,x:34080,y:32888,dt:0|A-5-OUT,B-9-OUT;n:type:ShaderForge.SFN_LightVector,id:9,x:34818,y:32798;n:type:ShaderForge.SFN_Vector1,id:11,x:33748,y:32812,v1:0.8;n:type:ShaderForge.SFN_Clamp01,id:12,x:33811,y:32675|IN-6-OUT;n:type:ShaderForge.SFN_Clamp01,id:14,x:33823,y:32898|IN-8-OUT;n:type:ShaderForge.SFN_Power,id:15,x:33499,y:32703|VAL-12-OUT,EXP-11-OUT;n:type:ShaderForge.SFN_Power,id:17,x:33523,y:32895|VAL-14-OUT,EXP-11-OUT;n:type:ShaderForge.SFN_Multiply,id:18,x:33238,y:32734|A-15-OUT,B-20-OUT;n:type:ShaderForge.SFN_Multiply,id:19,x:33320,y:32895|A-17-OUT,B-22-OUT;n:type:ShaderForge.SFN_Vector3,id:20,x:33487,y:32417,v1:0.5137255,v2:0.6980392,v3:0.8156863;n:type:ShaderForge.SFN_Vector3,id:22,x:33505,y:32568,v1:0.9558824,v2:0.01068025,v3:0;n:type:ShaderForge.SFN_Add,id:23,x:33267,y:32506|A-20-OUT,B-22-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:24,x:32661,y:32728,ptlb:node_24,ptin:_node_24,on:True|A-15-OUT,B-25-OUT;n:type:ShaderForge.SFN_Divide,id:25,x:32871,y:32826|A-26-OUT,B-23-OUT;n:type:ShaderForge.SFN_Add,id:26,x:33135,y:32895|A-18-OUT,B-19-OUT;n:type:ShaderForge.SFN_Multiply,id:35,x:32307,y:32783|A-24-OUT,B-36-OUT;n:type:ShaderForge.SFN_Vector1,id:36,x:32559,y:32995,v1:1;n:type:ShaderForge.SFN_Multiply,id:37,x:32307,y:32919|A-93-OUT,B-38-RGB;n:type:ShaderForge.SFN_Tex2d,id:38,x:32535,y:32586,ptlb:node_38,ptin:_node_38,tex:77936cffdb5c0e24bb9a953599bc7822,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:41,x:32307,y:33059|A-38-RGB,B-36-OUT;n:type:ShaderForge.SFN_Vector1,id:85,x:32199,y:33107,v1:1;n:type:ShaderForge.SFN_Vector4,id:93,x:32548,y:32889,v1:1,v2:1,v3:1,v4:1;n:type:ShaderForge.SFN_Multiply,id:151,x:32037,y:32832|A-35-OUT,B-37-OUT;n:type:ShaderForge.SFN_Multiply,id:154,x:32000,y:33020|A-41-OUT,B-85-OUT;proporder:2-3-24-38;pass:END;sub:END;*/

Shader "Shader Forge/skinTest" {
    Properties {
        _node_2 ("node_2", 2D) = "bump" {}
        _node_3 ("node_3", 2D) = "bump" {}
        [MaterialToggle] _node_24 ("node_24", Float ) = 0.4119016
        _node_38 ("node_38", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One Zero
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _node_38; uniform float4 _node_38_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Normals:
                float3 normalDirection =  i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor + UNITY_LIGHTMODEL_AMBIENT.rgb;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                float2 node_173 = i.uv0;
                float4 node_38 = tex2D(_node_38,TRANSFORM_TEX(node_173.rg, _node_38));
                float node_36 = 1.0;
                float3 node_41 = (node_38.rgb*node_36);
                float node_85 = 1.0;
                float3 node_154 = (node_41*node_85);
                diffuseLight += node_154; // Diffuse Ambient Light
                finalColor += diffuseLight * node_154;
/// Final Color:
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            ZWrite Off
            
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _node_38; uniform float4 _node_38_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Normals:
                float3 normalDirection =  i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                float2 node_174 = i.uv0;
                float4 node_38 = tex2D(_node_38,TRANSFORM_TEX(node_174.rg, _node_38));
                float node_36 = 1.0;
                float3 node_41 = (node_38.rgb*node_36);
                float node_85 = 1.0;
                float3 node_154 = (node_41*node_85);
                finalColor += diffuseLight * node_154;
/// Final Color:
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
