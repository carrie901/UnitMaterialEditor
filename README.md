# Unit Material Editor

 A component based material editor framework for unity

 example:
 
```shaderlab
Shader "Custom/Demo-BumpSpec" {
	Properties {
	....
	}
	/*
	#BEGINEDITOR
	[
		{
			"editor" : "OldSchoolSpecular",
		},
		{
			"editor" : "SingleProp",
			"args" : {
				"name" : "_CullMode",
			}
		},
		{
			"editor" : "RenderMode"
		},
		{
			"editor" : "UpdateKeyword",
			"id" : "use_BumpMap",
			"args" : {
				"name" : "_BumpMap",
				"keyword" : "_NORMALMAP"
			}
		},
		{
			"editor" : "UpdateKeyword",
			"id" : "use_External_Alpha",
			"args" : {
				"mode" : "Transparent | Cutout",
				"name" : "_MainTex_Alpha",
				"keyword" : "_USE_EXTERNAL_ALPHA",
			}
		},        
		{
			"editor" : "UpdateKeyword",
			"id" : "use_SpecColor",
			"args" : {
				"name" : "_SpecColor",
				"op" : ">",
				"ref" : [0,0,0],
				"keyword" : "_USE_SPECULAR",
			}
		},
		{
			"editor" : "Logic",
			"id" : "use_Normal_Alpha",
			"args" : {
				"op" : "&&",
				"arg0" : "[use_BumpMap]",
				"arg1" : "[use_External_Alpha]",
			}
		},
		{
			"editor" : "HelpBox",
			"args" : {
				"mode" : "Cutout",
				"if" : "[use_Normal_Alpha]",
				"type" : "info",
				"text" : "Use Normalmap with an external alpha texture."
			}
		},
		{
			"editor" : "GetTextureFormat",
			"id" : "fmt_MainTex",
			"args" : {
				"name" : "_MainTex",
			}
		},
		{
			"editor" : "GetTextureFormat",
			"id" : "fmt_MainTex_Alpha",
			"args" : {
				"name" : "_MainTex_Alpha",
			}
		},
		{
			"editor" : "GetTextureFormat",
			"id" : "fmt_BumpMap",
			"args" : {
				"name" : "_BumpMap",
			}
		},
		{
			"editor" : "GetTextureFormat",
			"id" : "fmt_SpecGlossMap",
			"args" : {
				"name" : "_SpecGlossMap",
			}
		},
		{
			"editor" : "HelpBox",
			"args" : {
				"type" : "info",
				"text" : "Texture Format: _BumpMap = {0}, _SpecGlossMap = {1}, _MainTex_Alpha = {2}",
				"params" : [ "fmt_BumpMap", "fmt_SpecGlossMap", "fmt_MainTex_Alpha" ]
			}
		},
	]
	#ENDEDITOR
	*/
	CustomEditor "ArtistKit.UnitMaterialEditor"
	...
}
```

![image](https://github.com/lujian101/UnitMaterialEditor/blob/master/images/UnitMaterialEditor.jpg)
