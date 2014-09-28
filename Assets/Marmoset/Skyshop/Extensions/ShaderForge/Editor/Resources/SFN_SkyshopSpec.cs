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
	public class SFN_SkyshopSpec : SF_Node {
		
		public	Cubemap cubemapAsset;
		private	Texture2D textureAsset;
		private	CubemapFace previewFace;

		public bool useRotation = true;
		public bool useBlending = true;
		public bool useLightmapOcc = false;

		public SFN_SkyshopSpec() {	
		}
		
		public override void Initialize() {
			base.Initialize("Spec. IBL");
			base.showColor = true;
			base.UseLowerPropertyBox(true,true);
			base.texture.CompCount = 3;

			//property = ScriptableObject.CreateInstance<SFP_SkyshopSpecIBL>().Initialize(this);

			string[] allFound = AssetDatabase.FindAssets("sfn_skyshopspec");
			if (allFound.Length > 0) {
				string path = AssetDatabase.GUIDToAssetPath(allFound[0]); 
				base.texture.iconActive = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
			}


			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"REFL","Refl",ConType.cInput,ValueType.VTv3),
				SF_NodeConnector.Create(this,"WPOS","W.pos",ConType.cInput,ValueType.VTv3),
				SF_NodeConnector.Create(this,"GLOSS","Gloss",ConType.cInput,ValueType.VTv1),
				SF_NodeConnector.Create(this,"RGB","RGB",ConType.cOutput,ValueType.VTv3)						.Outputting(OutChannel.RGB),
				SF_NodeConnector.Create(this,"R","R",ConType.cOutput,	ValueType.VTv1)	.WithColor(Color.red)	.Outputting(OutChannel.R),
				SF_NodeConnector.Create(this,"G","G",ConType.cOutput,ValueType.VTv1)	.WithColor(Color.green)	.Outputting(OutChannel.G),
				SF_NodeConnector.Create(this,"B","B",ConType.cOutput,ValueType.VTv1)	.WithColor(Color.blue)	.Outputting(OutChannel.B)
			};

			extraWidthInput = 6;			
			base.node_height += 31;
			//TODO: This node is also a property node, look at slider-node for serialization
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
			s += "sprot:" + useRotation.ToString();
			s += ",spblend:" + useBlending.ToString();
			s += ",splmocc:" + useLightmapOcc.ToString();
			return s;
		}
		
		public override void DeserializeSpecialData( string key, string value ) {
			switch( key ) {
			case "sprot":
				useRotation = bool.Parse(value);
				break;
				case "spblend":
				useBlending = bool.Parse(value);
				break;
				case "splmocc":
				useLightmapOcc = bool.Parse(value);
				break;
			}
		}

		public override string[] TryGetMultiCompilePragmas( out int group ) {
			group = 1;
			return new string[]{
				"MARMO_SKY_BLEND_OFF MARMO_SKY_BLEND_ON",
				"MARMO_BOX_PROJECTION_OFF MARMO_BOX_PROJECTION_ON"
			};
		}

		//???
		public override bool IsUniformOutput() {
			return false;
		}

		public override string Evaluate(OutChannel channel = OutChannel.All) {
			if( !ShouldDefineVariable() ) {
				this.PreDefine();
			}
			string DIR = GetInputIsConnected("REFL") ? GetConnectorByStringID("REFL").TryEvaluate() : "viewReflectDirection";
			string POS = GetInputIsConnected("WPOS") ? GetConnectorByStringID("WPOS").TryEvaluate() : "i.posWorld.rgb";

			string result;
			if(GetInputIsConnected("GLOSS")) {
				string GLOSS = GetConnectorByStringID("GLOSS").TryEvaluate();
				result = "marmoMipSpecular(" + DIR + ", " + POS + ", " + GLOSS + ")";
			}
			else {
				result = "marmoSpecular(" + DIR + ")";
			}

			bool useLM = useLightmapOcc && this.editor.ps.catLighting.lightmapped;
			if(useLM) return "(lightmap*" + result + ")";
			return result;
		}

		public override string GetPrepareUniformsAndFunctions () {
			string str = "";

			// Shared stuff
			bool useLM = useLightmapOcc && this.editor.ps.catLighting.lightmapped;
			if(useLM) SFH_SkyshopCore.lightmapCode(ref str);
			SFH_SkyshopCore.exposureCode(ref str);
			SFH_SkyshopCore.skyMatrixCode(ref str);
			SFH_SkyshopCore.RGBMCode(ref str);

			str +=  "#ifndef MARMO_SPECULAR_DEFINED\n";
			str +=  "#define MARMO_SPECULAR_DEFINED\n";
			str +=  "uniform samplerCUBE _SpecCubeIBL;\n";
			str +=  "uniform samplerCUBE _SpecCubeIBL1;\n";

			string marmoSkyRotate_R;
			string marmoSkyInvMul_P;
			string marmoSkyRotate_R0_dir;
			string marmoSkyRotate_R1_dir;
			if(useRotation) {
				marmoSkyRotate_R = "R = marmoSkyRotate(skyMatrix, R);\n";
				marmoSkyInvMul_P = "P.w=1.0; P.xyz = mul(invSkyMatrix,P).xyz;\n";
				marmoSkyRotate_R0_dir = "R = marmoSkyRotate(_SkyMatrix, dir);\n";
				marmoSkyRotate_R1_dir = "R = marmoSkyRotate(_SkyMatrix1, dir);\n";
			} else {
				marmoSkyRotate_R = "\n";
				marmoSkyInvMul_P = "\n";
				marmoSkyRotate_R0_dir = "R = dir;\n";
				marmoSkyRotate_R1_dir = "R = dir;\n";
			}
			str += 
			"float3 marmoSkyProject(uniform float4x4 skyMatrix, uniform float4x4 invSkyMatrix, uniform float3 skyMin, uniform float3 skyMax, uniform float3 worldPos, float3 R) {\n" +
				"\t#if MARMO_BOX_PROJECTION_ON\n" +
					"\t\t" + marmoSkyRotate_R +
					"\t\tfloat3 invR = 1.0/R;\n" +
					"\t\tfloat4 P;\n" +
					"\t\tP.xyz = worldPos;\n" +
					"\t\t" + marmoSkyInvMul_P + 
					"\t\tfloat4 rbmax = float4(0.0,0.0,0.0,0.0);\n" +
					"\t\tfloat4 rbmin = float4(0.0,0.0,0.0,0.0);\n" +
					"\t\trbmax.xyz = skyMax - P.xyz;\n" +
					"\t\trbmin.xyz = skyMin - P.xyz;\n" +				
					"\t\tfloat3 rbminmax = invR * lerp(rbmin.xyz, rbmax.xyz, saturate(R*1000000.0));\n" +
					"\t\tfloat fa = min(min(rbminmax.x, rbminmax.y), rbminmax.z);\n" +
					"\t\treturn P.xyz + R*fa;\n" +
				"\t#else\n" + 
					"\t\t" + marmoSkyRotate_R +
					"\t\treturn R;\n" +
				"\t#endif\n" +
			"}\n";

			//specular, no mip
			str +=
			"float3 marmoSpecular(float3 dir) {\n" +
				"\tfloat4 exposure = marmoExposureBlended();\n" +
				"\tfloat3 R;\n" + 
				"\t"+marmoSkyRotate_R0_dir +
				"\tfloat3 specIBL = fromRGBM(texCUBE(_SpecCubeIBL, R));\n";
			if(useBlending) {
				str +=
				"\t#if MARMO_SKY_BLEND_ON\n" +
					"\t\t"+marmoSkyRotate_R1_dir +
					"\t\tfloat3 specIBL1 = fromRGBM(texCUBE(_SpecCubeIBL1, R));\n" +
					"\t\tspecIBL = lerp(specIBL1, specIBL, _BlendWeightIBL);\n" +
				"\t#endif\n";
			}
			str += 
				"\treturn specIBL * (exposure.w * exposure.y);\n" +
			"}\n\n";

			//specular mip
			str += 
			"float3 marmoMipSpecular(float3 dir, float3 worldPos, float gloss) {\n" + 
				"\tfloat4 exposure = marmoExposureBlended();\n" +
				"\tfloat4 lookup;\n" +
				"\tlookup.xyz = marmoSkyProject(_SkyMatrix, _InvSkyMatrix, _SkyMin, _SkyMax, worldPos, dir);\n" +
				"\tlookup.w = (-6.0*gloss) + 6.0;\n" +
				"\tfloat3 specIBL = fromRGBM(texCUBElod(_SpecCubeIBL, lookup));\n";
			if(useBlending) {
				str += 
				"\t#if MARMO_SKY_BLEND_ON\n" +
					"\t\tlookup.xyz = marmoSkyProject(_SkyMatrix1, _InvSkyMatrix1, _SkyMin1, _SkyMax1, worldPos, dir);\n" +
					"\t\tfloat3 specIBL1 = fromRGBM(texCUBElod(_SpecCubeIBL1, lookup));\n" +
					"\t\tspecIBL = lerp(specIBL1, specIBL, _BlendWeightIBL);\n" +
				"\t#endif\n";
			}
			str +=
				"\treturn specIBL * (exposure.w * exposure.y);\n" +
			"}\n";
			str +=  "#endif\n\n";
			return str;
		}
	}
}
