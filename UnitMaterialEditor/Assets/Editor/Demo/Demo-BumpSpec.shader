Shader "Custom/Demo-BumpSpec" {
    Properties {
        _Color( "Main Color", Color ) = ( 1, 1, 1, 1 )
        _Cutoff( "Alpha Cutoff", Range( 0.01, 1.0 ) ) = 0.0
        _ColorKey( "Color Key (RGB) Epsilon (A)", Color ) = ( 1.0, 0, 1.0, 0.004 )

        // _SPECGLOSSMAP
        [NoScaleOffset] _SpecGlossMap( "Specular (R) Gloss (G)", 2D ) = "white" {}
        _SpecColor( "Specular Color", Color ) = ( 0.5, 0.5, 0.5, 1 )
        _Shininess( "Shininess", Range ( 0.03, 1 ) ) = 0.078125
        _MainTex( "Base (RGB) Gloss (A)", 2D ) = "white" {}
        
        // _USE_EXTERNAL_ALPHA
        [NoScaleOffset] _MainTex_Alpha( "Base Alpha (R)", 2D ) = "white" {}
        
        // _NORMALMAP
        _BumpMap( "Normalmap", 2D ) = "bump" {}
        _BumpScale( "Scale", Range( 0.01, 4 ) ) = 1.0

        [HideInInspector] _Mode( "Render Mode", Float ) = 0
        
        [Enum( Add, 0, Sub, 1, RevSub, 2, Min, 3, Max, 4 )]
        _BlendOp( "BlendOp", Float ) = 0
        
        [Enum( UnityEngine.Rendering.BlendMode ) ]
        _SrcBlend( "Src Blend", Float ) = 1

        [Enum( UnityEngine.Rendering.BlendMode ) ]
		_DstBlend( "Dst Blend", Float ) = 0
		
        [Enum( On, 1, Off, 0)]
        [HideInInspector] _ZWrite( "Z Write", Float ) = 1
        [HideInInspector] _ZTest( "Z Test", Float ) = 2
		
		[Enum( UnityEngine.Rendering.CullMode )] 
		[Space( 10 )] _CullMode( "Cull Mode", Float ) = 2

        [Toggle] _AlphaPremultiply( "Premultiply Alpha", Float ) = 0
        [Toggle] _AutoRenderQueue( "Auto Render Queue", Float ) = 1

        [HideInInspector] _InternalData( "Internal Data", Float ) = 0
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

    CGINCLUDE
    sampler2D _MainTex;
    sampler2D _BumpMap;
    fixed4 _Color;
    half _Shininess;
    fixed _Cutoff;
    fixed4 _ColorKey;

    struct Input {
        float2 uv_MainTex;
        float2 uv_BumpMap;
    };

    inline void AlphaCutoff( SurfaceOutput o ) {
        #ifdef _ALPHATEST_ON
            clip( o.Alpha - _Cutoff );
        #elif defined( _ALPHATEST_COLOR_KEY_ON )
            fixed3 ckeyDir = o.Albedo - _ColorKey;
            clip( dot( ckeyDir, ckeyDir ) - _ColorKey.a );
        #endif
    }

    void surf( Input IN, inout SurfaceOutput o ) {
        fixed4 tex = tex2D( _MainTex, IN.uv_MainTex );
        o.Albedo = tex.rgb * _Color.rgb;
        o.Gloss = tex.a;
        o.Alpha = tex.a * _Color.a;
        o.Specular = _Shininess;
        #ifdef _NORMALMAP
            o.Normal = UnpackNormal( tex2D( _BumpMap, IN.uv_BumpMap ) );
        #endif
        AlphaCutoff( o );
    }
    ENDCG

    SubShader {
        LOD 400

        Blend [_SrcBlend] [_DstBlend]
        BlendOp [_BlendOp]
        ZWrite [_ZWrite]
        ZTest [_ZTest]
        Cull [_CullMode]

        CGPROGRAM
        #pragma surface surf BlinnPhong alpha:fade
        #pragma target 3.0
        #pragma shader_feature _ALPHATEST_ON _ALPHATEST_COLOR_KEY_ON
        #pragma shader_feature _USE_EXTERNAL_ALPHA
        #pragma shader_feature _NORMALMAP
        #pragma shader_feature _SPECGLOSSMAP
        ENDCG
    }
}

