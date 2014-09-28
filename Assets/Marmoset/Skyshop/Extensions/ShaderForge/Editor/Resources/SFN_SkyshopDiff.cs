// Marmoset Skyshop
// Copyright 2014 Marmoset LLC
// http://marmoset.co

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;
using System;

namespace ShaderForge {	
	[System.Serializable]
	public class SFN_SkyshopDiff: SF_Node {
		public bool useRotation = true;
		private bool useBlending = true;
		private bool useLightmapOcc = false;

		public SFN_SkyshopDiff() {	
		}
		
		public override void Initialize() {
			base.Initialize("Diff. IBL");
			base.showColor = true;
			base.UseLowerPropertyBox(true,true);
			base.texture.CompCount = 3;

			string[] allFound = AssetDatabase.FindAssets("sfn_skyshopdiff");
			if (allFound.Length > 0) {
				string path = AssetDatabase.GUIDToAssetPath(allFound[0]); 
				base.texture.iconActive = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
			}

			//property = ScriptableObject.CreateInstance<SFP_SkyshopDiffIBL>().Initialize(this);
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"NRM","Nrm",ConType.cInput,ValueType.VTv3),
				SF_NodeConnector.Create(this,"RGB","RGB",ConType.cOutput,ValueType.VTv3)						.Outputting(OutChannel.RGB),
				SF_NodeConnector.Create(this,"R","R",ConType.cOutput,	ValueType.VTv1)	.WithColor(Color.red)	.Outputting(OutChannel.R),
				SF_NodeConnector.Create(this,"G","G",ConType.cOutput,ValueType.VTv1)	.WithColor(Color.green)	.Outputting(OutChannel.G),
				SF_NodeConnector.Create(this,"B","B",ConType.cOutput,ValueType.VTv1)	.WithColor(Color.blue)	.Outputting(OutChannel.B)
			};

			base.node_height += 31;
		}

		public override void DrawLowerPropertyBox() {
			GUI.color = Color.white;
			EditorGUI.BeginChangeCheck();
			Rect r = lowerRect;
			useRotation = GUI.Toggle(r, useRotation, "Sky Rotation");
			r.y += 16;
			useBlending = GUI.Toggle(r, useBlending, "Sky Blend");
			r.y += 16;
			EditorGUI.BeginDisabledGroup(!this.editor.ps.catLighting.lightmapped);
			useLightmapOcc = GUI.Toggle(r, useLightmapOcc, "Lightmap Occ");
			EditorGUI.EndDisabledGroup();
			if(EditorGUI.EndChangeCheck()) OnUpdateNode(NodeUpdateType.Hard, true);
		}
		public override string SerializeSpecialData() {			
			string s = "";
			s += "dfrot:" + useRotation.ToString();
			s += ",dfblend:" + useBlending.ToString();
			s += ",dflmocc:" + useLightmapOcc.ToString();
			return s;
		}
		public override void DeserializeSpecialData( string key, string value ) {
			switch( key ) {
			case "dfrot":
				useRotation = bool.Parse(value);
				break;
			case "dfblend":
				useBlending = bool.Parse(value);
				break;
			case "dflmocc":
				useLightmapOcc = bool.Parse (value);
				break;
			}
		}

		public override bool IsUniformOutput() {
			return false;
		}

		public override string Evaluate(OutChannel channel = OutChannel.All) {
			if( !ShouldDefineVariable() ) {
				this.PreDefine();
			}
			string DIR = GetInputIsConnected("NRM") ? GetConnectorByStringID("NRM").TryEvaluate() : "normalDirection"; // "viewReflectDirection";

			bool useLM = useLightmapOcc && this.editor.ps.catLighting.lightmapped;
			if(useLM) {
				return "(lightmap*marmoDiffuse(" + DIR + "))";
			} else {
				return "marmoDiffuse(" + DIR + ")";
			}
		}

		public override string[] TryGetMultiCompilePragmas( out int group ) {
			group = 1;
			return new string[]{
				"MARMO_SKY_BLEND_OFF MARMO_SKY_BLEND_ON",
			};
		}

		public override string GetPrepareUniformsAndFunctions(){

			string str = "";

			SFH_SkyshopCore.exposureCode(ref str);
			SFH_SkyshopCore.skyMatrixCode(ref str);

			bool useLM = useLightmapOcc && this.editor.ps.catLighting.lightmapped;
			if(useLM) SFH_SkyshopCore.lightmapCode(ref str);

			// SH coefficients
			str += "#pragma glsl\n";
			str += "#ifndef MARMO_DIFFUSE_DEFINED\n";
			str += "#define MARMO_DIFFUSE_DEFINED\n";
			str += "uniform float3 _SH0;\n";
			str += "uniform float3 _SH1;\n";
			str += "uniform float3 _SH2;\n";
			str += "uniform float3 _SH3;\n";
			str += "uniform float3 _SH4;\n";
			str += "uniform float3 _SH5;\n";
			str += "uniform float3 _SH6;\n";
			str += "uniform float3 _SH7;\n";
			str += "uniform float3 _SH8;\n\n";

			str += "uniform float3 _SH01;\n";
			str += "uniform float3 _SH11;\n";
			str += "uniform float3 _SH21;\n";
			str += "uniform float3 _SH31;\n";
			str += "uniform float3 _SH41;\n";
			str += "uniform float3 _SH51;\n";
			str += "uniform float3 _SH61;\n";
			str += "uniform float3 _SH71;\n";
			str += "uniform float3 _SH81;\n\n";

			str += "float3 SHLookup(float3 dir) {\n";
			if(useRotation) str += "\tdir = marmoSkyRotate(_SkyMatrix, dir);\n";
			str +=
				"\tdir = normalize(dir);\n" +
				"\tfloat3 band0, band1, band2;\n" +
				"\tband0 = _SH0.xyz;\n" + 
				"\n" + 
				"\tband1 =  _SH1.xyz * dir.y;\n" + 
				"\tband1 += _SH2.xyz * dir.z;\n" + 
				"\tband1 += _SH3.xyz * dir.x;\n" + 
				"\n" + 
				"\tfloat3 swz = dir.yyz * dir.xzx;\n" + 
				"\tband2 =  _SH4.xyz * swz.x;\n" + 
				"\tband2 += _SH5.xyz * swz.y;\n" + 
				"\tband2 += _SH7.xyz * swz.z;\n" + 
				"\tfloat3 sqr = dir * dir;\n" + 
				"\tband2 += _SH6.xyz * ( 3.0*sqr.z - 1.0 );\n" + 
				"\tband2 += _SH8.xyz * ( sqr.x - sqr.y );\n" +
				"\treturn band0 + band1 + band2;\n" +
			"}\n";

			str += "float3 SHLookup1(float3 dir) {\n";
			if(useRotation) str += "\tdir = marmoSkyRotate(_SkyMatrix1, dir);\n";
			str +=
				"\tdir = normalize(dir);\n" +
				"\tfloat3 band0, band1, band2;\n" +
				"\tband0 = _SH01.xyz;\n" + 
				"\n" + 
				"\tband1 =  _SH11.xyz * dir.y;\n" + 
				"\tband1 += _SH21.xyz * dir.z;\n" + 
				"\tband1 += _SH31.xyz * dir.x;\n" + 
				"\n" + 
				"\tfloat3 swz = dir.yyz * dir.xzx;\n" + 
				"\tband2 =  _SH41.xyz * swz.x;\n" + 
				"\tband2 += _SH51.xyz * swz.y;\n" + 
				"\tband2 += _SH71.xyz * swz.z;\n" + 
				"\t//Commented coefficients because of internal Unity PropertyBlock issues.\n" + 
				"\t//float3 sqr = dir * dir;\n" + 
				"\t//band2 += _SH61.xyz * ( 3.0*sqr.z - 1.0 );\n" + 
				"\t//band2 += _SH81.xyz * ( sqr.x - sqr.y );\n" + 
				"\treturn band0 + band1 + band2;\n" +
			"}\n";

			//diffuse function
			str += 
			"float3 marmoDiffuse(float3 dir) {\n" +
				"\tfloat4 exposure = marmoExposureBlended();\n" +
				"\tfloat3 diffIBL = SHLookup(dir);\n";
			if(useBlending) {
			str += 
				"\t#if MARMO_SKY_BLEND_ON\n" +
				"\t\tfloat3 diffIBL1 = SHLookup1(dir);\n" +
				"\t\tdiffIBL = lerp(diffIBL1, diffIBL, _BlendWeightIBL);\n" +
				"\t#endif\n";
			}
			//diffIBL can be eeeever so slightly negative, max(0,diff) would be correct but abs() is often a free instruction
			str += "\treturn (exposure.x * exposure.w) * abs(diffIBL);\n";
			str += "}\n";
			str += "#endif\n\n";

			return str;
		}

	}
}
