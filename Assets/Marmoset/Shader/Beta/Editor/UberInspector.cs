// Marmoset Skyshop
// Copyright 2014 Marmoset LLC
// http://marmoset.co

using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections; 
using System.Collections.Generic;
using System.Linq;

public class UberInspector : MaterialEditor {


	// draws a standard material property, optionally disabled, and either compact or full-sized. 
	// minimal flag on texture properties means hide UV tiling.
	private void DrawProperty(MaterialProperty prop, string label, string subLabel, bool disabled) {
		EditorGUI.BeginDisabledGroup(disabled);
		// some controls need more width
		float controlSize = 84;
		EditorGUIUtility.labelWidth = Screen.width - controlSize;
		EditorGUIUtility.fieldWidth = controlSize;
	
		if( prop.type == MaterialProperty.PropType.Texture ) {
			TextureProperty(prop, label);

			if( subLabel.Length > 0 ) {
				Rect r = GUILayoutUtility.GetLastRect();
				r.x = EditorGUIUtility.labelWidth - 21f;
				EditorGUI.BeginDisabledGroup(true);
				EditorGUI.LabelField(r, subLabel);
				EditorGUI.EndDisabledGroup();
			}

			GUILayout.Space(6);
		} else {
			ShaderProperty(prop, label);
		}
		EditorGUI.EndDisabledGroup();
	}

	//Draws a property that's also a section header (usually a checkbox) along with a collapse/expand triangle. Returns true if block is expanded.
	public void DrawPropertyHeader(PropertyBlock block, MaterialProperty prop, string label) {
		float controlSize = 110;
		EditorGUIUtility.fieldWidth = controlSize;
		EditorGUIUtility.labelWidth = 0;
		
		EditorGUILayout.BeginHorizontal();
		
		//draw folding triangle and retrieve state
		Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(0), GUILayout.Height(16));
		block.open = EditorGUI.Foldout(r, block.open, "", false);

		if( !block.label ) {
			//draw a 10-pixel checkbox or a control-sized whatever-else (combo box usually)
			r = EditorGUILayout.GetControlRect(GUILayout.Width(block.checkbox ? 10 : controlSize), GUILayout.Height(16));			
			//draw property with label to the right
			ShaderProperty(r, prop, "");
		}
		EditorGUILayout.LabelField(label);
		
		EditorGUILayout.EndHorizontal();
	}

	//structure defining the header of a block of material properties. The header can be enabled/disabled by a series of keywords or
	//collapsed/expanded with a GUI foldout.
	public class PropertyBlock {
		public bool open = true; 		//current folded state, collapsed or expanded
		public bool enabled = true;   	//current state of keywords enabling/disabling this block of properties
		public bool checkbox = false;	//if true, this property header is drawn as a checkbox left of the label, otherwise it is drawn as a combo box
		public bool label = false;		//if true, no property gui is drawn for this element, it is just a section label
		public string[] keywords = null;	//list of keywords that will toggle this block as enabled and visible

		public PropertyBlock(string[] keys, bool isCheckbox) {
			this.keywords = keys;
			this.checkbox = isCheckbox;
		}

		public void evalKeywords( string[] matKeywords ) {
			this.enabled = false;
			if( this.keywords == null ) {
				enabled = true;
			} else {
				for(int i=0; i<this.keywords.Length && !this.enabled; ++i) {
					this.enabled |= matKeywords.Contains(this.keywords[i]);
				}
			}
		}
	};

	private Dictionary<string, PropertyBlock> blocks = null;
	public UberInspector() {
		blocks = new Dictionary<string, PropertyBlock>();
		blocks.Add( "Marmo_Diffuse", 	new PropertyBlock(new string[]{	"MARMO_DIFFUSE_ON" }, 	true) );
		blocks.Add( "Marmo_Specular",	new PropertyBlock(new string[]{	"MARMO_SPECULAR_ON" },	true) );
		blocks.Add( "Marmo_Bump",		new PropertyBlock(new string[]{	"MARMO_BUMP_ON" },		true) );
		blocks.Add( "Marmo_Glow",		new PropertyBlock(new string[]{	"MARMO_GLOW_ON" },		true) );
		blocks.Add( "Marmo_Occ", 		new PropertyBlock(new string[]{	"MARMO_OCC_OCCLUSION_MAP",
																		"MARMO_OCC_VERTEX_OCCLUSION",
																		"MARMO_OCC_VERTEX_COLOR" },	false) );
		blocks.Add( "Marmo_Trans", 		new PropertyBlock(new string[]{	"MARMO_TRANS_FADE",
																		"MARMO_TRANS_GLASS"},		false) );
		blocks.Add( "Marmo_Alpha_Test", new PropertyBlock(new string[]{	"MARMO_ALPHA_TEST_ON" },	true) );
		blocks.Add( "Header_Advanced",  new PropertyBlock(null, true ) );
		blocks["Header_Advanced"].label = true;
	}

	public override void OnInspectorGUI() {

		Material targetMat = target as Material;
		string[] keyWords = targetMat.shaderKeywords;

		foreach( KeyValuePair<string, PropertyBlock> itr in blocks ) {
			itr.Value.evalKeywords(keyWords);
		}
		
		//material checkboxes
		//bool diffOn = keyWords.Contains("MARMO_DIFFUSE_ON");
		//bool specOn = keyWords.Contains("MARMO_SPECULAR_ON");
		bool occOn  = keyWords.Contains("MARMO_OCC_OCCLUSION_MAP");
		//bool voccOn = keyWords.Contains("MARMO_OCC_VERTEX_OCCLUSION");
		bool diffSpecOn = keyWords.Contains("MARMO_DIFFUSE_SPECULAR_COMBINED_ON");
		bool transOn = !keyWords.Contains("MARMO_TRANS_OFF");
		bool cutoutOn = keyWords.Contains("MARMO_ALPHA_TEST_ON");
		//bool bumpOn = keyWords.Contains("MARMO_BUMP_ON");
		//bool glowOn = keyWords.Contains("MARMO_GLOW_ON");

		//TODO: transparency and cutout shouldn't be in the same section
		if( transOn ) {
			//Background is 1000, Geometry is 2000, AlphaTest is 2450, Transparent is 3000 and Overlay is 4000
			targetMat.renderQueue = 3000;
			targetMat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		} else if( cutoutOn ) {
			targetMat.renderQueue = 2450;
			targetMat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.Zero);
		} else {
			targetMat.renderQueue = 2000;
			targetMat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.Zero);
		}

		serializedObject.Update();
		SerializedProperty theShader = serializedObject.FindProperty("m_Shader");
		if(isVisible && !theShader.hasMultipleDifferentValues && theShader.objectReferenceValue != null) {

			EditorGUI.BeginChangeCheck();
			MaterialProperty[] props = MaterialEditor.GetMaterialProperties(targets);

			PropertyBlock headerBlock = null;
			bool prevVisible = true;
			for(int i = 0; i < props.Length; i++) {
				if( (props[i].flags & MaterialProperty.PropFlags.HideInInspector) > 0 ) continue;

				bool enabled = true;
				bool visible = true;
				string name = props[i].name;
				string label = props[i].displayName;
				string subLabel = "";

				//is this property a header to a new block?
				bool header = blocks.ContainsKey(name);

				//starting a new header
				if( header ) {
					// if a previous block is still open, add a little space to the end (helps get a little GUI nudge when unrolling empty blocks).
					if( headerBlock != null && headerBlock.open ) {
						GUILayout.Space(3);
					}
					// new block started
					headerBlock = blocks[name];
				}

				if(name == "_MainTex") {
					if( diffSpecOn ) label = "Diffuse(RGB) Specular(A)";
				}

				if(name == "_MainTex" ||
				   name == "_SpecTex" ||
				   name == "_BumpMap" ||
				   name == "_OccTex" ||
				   name == "_Illum") {
					targetMat.SetVector(name + "Tiling", props[i].textureScaleAndOffset);
				}

				// special case visibility for a few parameters
				if(name == "_SpecTex") { visible = !diffSpecOn; }
				if(name == "_OccTex")  { visible = occOn; }

				// link tiling attributes of textures to the textures themselves
				//NOTE: tiling settings must come immediately after texture slots
				if( name.Contains("Tiling") ) { visible = prevVisible; }

				if( header ) {
					visible = true;
				} 
				else if( headerBlock != null ) {
					visible &= headerBlock.enabled;
					visible &= headerBlock.open;
				}

				// draw each header type and update the appropriate flags
				if( visible ) {
					if(header)	DrawPropertyHeader(headerBlock, props[i], label);
					else 		DrawProperty(props[i], label, subLabel, !enabled);
				}
				prevVisible = visible;
				
				//TODO: raise this flag on custom cubes and maybe even SH values!
				//props[i].flags |= MaterialProperty.PropFlags.PerRendererData
			}
			if (EditorGUI.EndChangeCheck()) PropertiesChanged();
		}

		/*
		// if we are not visible... return
		if (!isVisible) return;
		
		// get the current keywords from the material
		Material targetMat = target as Material;
		string[] keyWords = targetMat.shaderKeywords;
		
		// see if redify is set, then show a checkbox
		bool diff = keyWords.Contains ("MARMO_DIFFUSE_ON");
		EditorGUI.BeginChangeCheck();
		diff = EditorGUILayout.Toggle ("Specular toggle", diff);
		if (EditorGUI.EndChangeCheck()) {
			// if the checkbox is changed, reset the shader keywords
			var keywords = new List<string> { diff ? "MARMO_DIFFUSE_ON" : "MARMO_DIFFUSE_OFF"};
			targetMat.shaderKeywords = keywords.ToArray ();
			EditorUtility.SetDirty (targetMat);
		}
		 */
	}
}


