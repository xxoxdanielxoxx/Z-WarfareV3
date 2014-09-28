// Marmoset Skyshop
// Copyright 2014 Marmoset LLC
// http://marmoset.co

using UnityEngine;
using System.Collections;

public class SFH_SkyshopCore {
	public static void exposureCode(ref string str) {
		// Shared stuff
		str += "#ifndef MARMO_EXPOSURE_IBL_DEFINED\n";
		str += "#define MARMO_EXPOSURE_IBL_DEFINED\n";
		str += "uniform float  _BlendWeightIBL;\n";
		str += "uniform float4 _ExposureIBL;\n";
		str += "uniform float4 _ExposureLM;\n";
		str += "uniform float4 _UniformOcclusion;\n";
		str += "uniform float4 _ExposureIBL1;\n";
		str += "uniform float4 _ExposureLM1;\n";
		str += 
			"inline float4 marmoExposureBlended() {\n" +
				"\tfloat4 exposure = _ExposureIBL;\n" +
				"\t#if !LIGHTMAP_OFF\n" +
				"\t\texposure.xy *= _ExposureLM.xy;\n" +
				"\t#endif\n" +
				"\t#if MARMO_SKY_BLEND_ON\n" +
				"\t\tfloat4 exposure1 = _ExposureIBL1;\n" +
				"\t\t#if !LIGHTMAP_OFF\n" +
				"\t\t\texposure1.xy *= _ExposureLM1.xy;\n" +
				"\t\t#endif\n" +
				"\t\texposure = lerp(exposure1, exposure, _BlendWeightIBL);\n" +
				"\t#endif\n" +
				"\texposure.xy *= _UniformOcclusion.xy;\n" +
				"\treturn exposure;\n" +
			"}\n";
		str += "#endif\n\n";
	}

	public static void skyMatrixCode(ref string str) {
		str += "#ifndef MARMO_SKY_MATRIX_DEFINED\n";
		str += "#define MARMO_SKY_MATRIX_DEFINED\n";

		str += "uniform float4x4 _SkyMatrix;\n";
		str += "uniform float4x4 _InvSkyMatrix;\n";
		str += "uniform float3   _SkySize;\n";
		str += "uniform float3   _SkyMin;\n";
		str += "uniform float3   _SkyMax;\n";			

		str += "uniform float4x4 _SkyMatrix1;\n";
		str += "uniform float4x4 _InvSkyMatrix1;\n";
		str += "uniform float3   _SkySize1;\n";
		str += "uniform float3   _SkyMin1;\n";
		str += "uniform float3   _SkyMax1;\n";
		
		str += 
			"inline float3 mulVec3(uniform float4x4 m, float3 v ) { return float3(dot(m[0].xyz,v.xyz), dot(m[1].xyz,v.xyz), dot(m[2].xyz,v.xyz)); }\n" +
			"inline float3 transposeMulVec3(uniform float4x4 m, float3 v )   { return m[0].xyz*v.x + (m[1].xyz*v.y + (m[2].xyz*v.z)); }\n" +
			"inline float3 transposeMulVec3(uniform float3x3 m, float3 v )   { return m[0].xyz*v.x + (m[1].xyz*v.y + (m[2].xyz*v.z)); }\n" +
			"inline float3 transposeMulPoint3(uniform float4x4 m, float3 p ) { return m[0].xyz*p.x + (m[1].xyz*p.y + (m[2].xyz*p.z + m[3].xyz)); }\n" +
			"inline float3 marmoSkyRotate (uniform float4x4 skyMatrix, float3 R) { return transposeMulVec3(skyMatrix, R); }\n";
		str += "#endif\n\n";
	}

	public static void RGBMCode(ref string str) {		
		str +=  "#ifndef MARMO_RGBM_DEFINED\n";
		str +=  "#define MARMO_RGBM_DEFINED\n";
		str +=  "#define IS_LINEAR ((-3.22581*unity_ColorSpaceGrey.r) + 1.6129)\n";
		str +=  "#define IS_GAMMA  (( 3.22581*unity_ColorSpaceGrey.r) - 0.6129)\n";
		str +=  "inline half  toLinearFast1(half c) { half c2=c*c; return dot(half2(0.7532,0.2468), half2(c2,c*c2)); }\n";
		str +=  "inline half3 fromRGBM(half4 c) { c.a*=6.0; return c.rgb*lerp(c.a, toLinearFast1(c.a), IS_LINEAR); }\n";
		str +=  "#endif\n\n";
	}

	public static void lightmapCode(ref string str) {
		str += "#ifndef MARMO_LIGHTMAP_DEFINED\n";
		str += "#define MARMO_LIGHTMAP_DEFINED\n";
		str += "\t#ifdef LIGHTMAP_OFF\n";
		str += "\t#define lightmap float3(1.0,1.0,1.0)\n";
		str += "\t#endif\n"; 
		str += "#endif\n\n";
	}
}
