// Marmoset Skyshop
// Copyright 2013 Marmoset LLC
// http://marmoset.co

Shader "Marmoset/Beta/Refractive IBL" {
	Properties {
		_Color   ("Diffuse Color", Color) = (1,1,1,1)
		_SpecColor ("Specular Color", Color) = (1,1,1,1)
		_SpecInt ("Specular Intensity", Float) = 1.0
		_Shininess ("Specular Sharpness", Range(2.0,8.0)) = 4.0
		_Fresnel ("Fresnel Strength", Range(0.0,1.0)) = 0.0
		_IOR     ("Index of Refraction", Range(1,2)) = 1.0
		_MainTex ("Diffuse(RGB) Alpha(A)", 2D) = "white" {}
		_SpecTex ("Specular(RGB) Gloss(A)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) 	= "bump" {}
		
		//slots for custom lighting cubemaps
		//_DiffCubeIBL ("Custom Diffuse Cube", Cube) = "black" {}
		
	}
	
	SubShader {
		Tags {
			"Queue"="Geometry"
			"RenderType"="Opaque"
		}
		LOD 400
		//diffuse LOD 200
		//diffuse-spec LOD 250
		//bumped-diffuse, spec 350
		//bumped-spec 400
		
		//mac stuff
		CGPROGRAM
		#ifdef SHADER_API_OPENGL
			#pragma glsl
		#endif
				
		#pragma target 3.0
		#pragma exclude_renderers gles d3d11_9x flash
		#pragma surface MarmosetSurf MarmosetDirect vertex:MarmosetVert fullforwardshadows  exclude_path:prepass
				
		#pragma multi_compile MARMO_BOX_PROJECTION_OFF MARMO_BOX_PROJECTION_ON
		#if MARMO_BOX_PROJECTION_ON	
			#define MARMO_BOX_PROJECTION
		#endif
		
		#pragma multi_compile MARMO_SKY_BLEND_OFF MARMO_SKY_BLEND_ON
		#if MARMO_SKY_BLEND_ON			
			#define MARMO_SKY_BLEND
		#endif
		
		#define MARMO_HQ
		#define MARMO_SKY_ROTATION
		#define MARMO_DIFFUSE_IBL
		#define MARMO_SPECULAR_IBL
		#define MARMO_DIFFUSE_DIRECT
		#define MARMO_SPECULAR_DIRECT
		#define MARMO_NORMALMAP
		#define MARMO_MIP_GLOSS

		//#define MARMO_GLOW
		//#define MARMO_PREMULT_ALPHA
		//#define MARMO_OCCLUSION
		//#define MARMO_VERTEX_OCCLUSION
		//#define MARMO_VERTEX_COLOR
		//#define MARMO_SPECULAR_FILTER
		#define MARMO_SPECULAR_REFRACTION
		
		#include "../MarmosetInput.cginc"		
		
		uniform float _IOR;
		//inputs i: -eye vector, n: normal vector, fresnel: parameterized fresnel term that gets loosely converted into index of refraction
		//outputs xyz: refraction/reflection vector, w: weight of reflection to refraction
		float4 specularRefract( float3 i, float3 n, float fresnel) {
			float eta = _IOR;
			float3 r = i - 2.0 * n * dot(n,i);
			float cosi = dot(-i, n);
			float cost2 = 1.0f - eta * eta * (1.0f - cosi*cosi);
			float4 result;
			
			result.xyz = eta*i + ((eta*cosi - sqrt(abs(cost2))) * n);			
			result.w = saturate(0.5*cost2 + 0.5);
			result.xyz = normalize(lerp(r, result.xyz, result.w));
			return result;
		}
				
		#include "../MarmosetCore.cginc"
		#include "../MarmosetDirect.cginc"
		#include "../MarmosetSurf.cginc"

		ENDCG
	}
	
	SubShader {
		Tags {
			"Queue"="Geometry"
			"RenderType"="Opaque"
		}
		LOD 400
		//diffuse LOD 200
		//diffuse-spec LOD 250
		//bumped-diffuse, spec 350
		//bumped-spec 400
		
		//mac stuff
		CGPROGRAM
		#ifdef SHADER_API_OPENGL
			#pragma glsl
		#endif
				
		#pragma target 3.0
		#pragma only_renderers gles d3d11_9x
		#pragma surface MarmosetSurf MarmosetDirect vertex:MarmosetVert fullforwardshadows  exclude_path:prepass
				
		#pragma multi_compile MARMO_BOX_PROJECTION_OFF MARMO_BOX_PROJECTION_ON
		#if MARMO_BOX_PROJECTION_ON	
			#define MARMO_BOX_PROJECTION
		#endif
		
		#pragma multi_compile MARMO_SKY_BLEND_OFF MARMO_SKY_BLEND_ON
		#if MARMO_SKY_BLEND_ON			
			#define MARMO_SKY_BLEND
		#endif
		
		#define MARMO_HQ
		#define MARMO_SKY_ROTATION
		#define MARMO_DIFFUSE_IBL
		#define MARMO_SPECULAR_IBL
		#define MARMO_DIFFUSE_DIRECT
		#define MARMO_SPECULAR_DIRECT
		#define MARMO_NORMALMAP
		//#define MARMO_MIP_GLOSS

		//#define MARMO_GLOW
		//#define MARMO_PREMULT_ALPHA
		//#define MARMO_OCCLUSION
		//#define MARMO_VERTEX_OCCLUSION
		//#define MARMO_VERTEX_COLOR
		//#define MARMO_SPECULAR_FILTER
		#define MARMO_SPECULAR_REFRACTION
		
		#include "../MarmosetInput.cginc"		
		
		uniform float _IOR;
		//inputs i: -eye vector, n: normal vector, fresnel: parameterized fresnel term that gets loosely converted into index of refraction
		//outputs xyz: refraction/reflection vector, w: weight of reflection to refraction
		float4 specularRefract( float3 i, float3 n, float fresnel) {
			float eta = _IOR;
			float3 r = i - 2.0 * n * dot(n,i);
			float cosi = dot(-i, n);
			float cost2 = 1.0f - eta * eta * (1.0f - cosi*cosi);
			float4 result;
			
			result.xyz = eta*i + ((eta*cosi - sqrt(abs(cost2))) * n);			
			result.w = saturate(0.5*cost2 + 0.5);
			result.xyz = normalize(lerp(r, result.xyz, result.w));
			return result;
		}
		
		
		
		#include "../MarmosetCore.cginc"
		#include "../MarmosetDirect.cginc"
		#include "../MarmosetSurf.cginc"

		ENDCG
	}
	
	FallBack "Marmoset/Bumped Specular IBL"
}
