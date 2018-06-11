# Unit Material Editor
 A component based material editor framework for unity

 example:
 
```
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
            "editor" : "RenderMode",
        },
        {
            "editor" : "UpdateKeyword",
            "id" : "id_normalmap",
            "args" : {
                "name" : "_BumpMap",
                "keyword" : "_NORMALMAP"
            }
        },
        {
            "editor" : "UpdateKeyword",
            "id" : "id_use_alpha_texture",
            "args" : {
                "mode" : "Transparent | Cutout",
                "name" : "_MainTex_Alpha",
                "keyword" : "_USE_EXTERNAL_ALPHA",
            }
        },
        {
            "editor" : "UpdateKeyword",
            "id" : "id_use_specglossmap",
            "args" : {
                "name" : "_SpecGlossMap",
                "keyword" : "_SPECGLOSSMAP"
            }
        },
        {
            "editor" : "Logic",
            "id" : "id_use_normal_alpah",
            "args" : {
                "op" : "&&",
                "arg0" : "[id_normalmap]",
                "arg1" : "[id_use_alpha_texture]",
            }
        },
        {
            "editor" : "HelpBox",
            "args" : {
                "mode" : "Cutout",
                "btest" : "[id_use_normal_alpah]",
                "type" : "info",
                "text" : "Use Normalmap with an external alpha texture."
            }
        },
    ]
    #ENDEDITOR
    */
    
    CustomEditor "ArtistKit.UnitMaterialEditor"
    ...
}
```
 
