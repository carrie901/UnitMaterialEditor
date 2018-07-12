using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ArtistKit {

    public class ShaderGUI_RenderMode : UnitMaterialEditor {

        MaterialProperty m_prop_Mode = null;
        MaterialProperty m_prop_BlendOp = null;
        MaterialProperty m_prop_SrcBlend = null;
        MaterialProperty m_prop_DstBlend = null;
        MaterialProperty m_prop_ZWrite = null;
        MaterialProperty m_prop_ZTest = null;
        MaterialProperty m_prop_AlphaPremultiply = null;
        MaterialProperty m_prop_AutoRenderQueue = null;

        MaterialProperty m_prop_MainTex_Alpha = null;
        MaterialProperty m_prop_Cutoff = null;
        MaterialProperty m_prop_ColorKey = null;

        public RenderMode _Mode {
            get {
                if ( m_prop_Mode != null && m_prop_Mode.type == MaterialProperty.PropType.Float ) {
                    return ( RenderMode )m_prop_Mode.floatValue;
                }
                return RenderMode.Opaque;
            }
            set {
                if ( m_prop_Mode != null && m_prop_Mode.type == MaterialProperty.PropType.Float ) {
                    m_prop_Mode.floatValue = ( float )value;
                }
            }
        }

        public bool _AutoRenderQueue {
            get {
                if ( m_prop_AutoRenderQueue != null && m_prop_AutoRenderQueue.type == MaterialProperty.PropType.Float ) {
                    return m_prop_AutoRenderQueue.floatValue != 0;
                }
                return false;
            }
            set {
                if ( m_prop_AutoRenderQueue != null && m_prop_AutoRenderQueue.type == MaterialProperty.PropType.Float ) {
                    m_prop_AutoRenderQueue.floatValue = value ? 1 : 0;
                }
            }
        }

        public bool _AlphaPremultiply {
            get {
                if ( m_prop_AlphaPremultiply != null && m_prop_AlphaPremultiply.type == MaterialProperty.PropType.Float ) {
                    return m_prop_AlphaPremultiply.floatValue != 0;
                }
                return false;
            }
            set {
                if ( m_prop_AlphaPremultiply != null && m_prop_AlphaPremultiply.type == MaterialProperty.PropType.Float ) {
                    m_prop_AlphaPremultiply.floatValue = value ? 1 : 0;
                }
            }
        }

        public BlendOp _BlendOp {
            get {
                if ( m_prop_BlendOp != null && m_prop_BlendOp.type == MaterialProperty.PropType.Float ) {
                    return ( BlendOp )m_prop_BlendOp.floatValue;
                }
                return BlendOp.Add;
            }
            set {
                if ( m_prop_BlendOp != null && m_prop_BlendOp.type == MaterialProperty.PropType.Float ) {
                    m_prop_BlendOp.floatValue = ( float )value;
                }
            }
        }

        public BlendMode _SrcBlend {
            get {
                if ( m_prop_SrcBlend != null && m_prop_SrcBlend.type == MaterialProperty.PropType.Float ) {
                    return ( BlendMode )m_prop_SrcBlend.floatValue;
                }
                return BlendMode.One;
            }
            set {
                if ( m_prop_SrcBlend != null && m_prop_SrcBlend.type == MaterialProperty.PropType.Float ) {
                    m_prop_SrcBlend.floatValue = ( float )value;
                }
            }
        }

        public BlendMode _DstBlend {
            get {
                if ( m_prop_DstBlend != null && m_prop_DstBlend.type == MaterialProperty.PropType.Float ) {
                    return ( BlendMode )m_prop_DstBlend.floatValue;
                }
                return BlendMode.Zero;
            }
            set {
                if ( m_prop_DstBlend != null && m_prop_DstBlend.type == MaterialProperty.PropType.Float ) {
                    m_prop_DstBlend.floatValue = ( float )value;
                }
            }
        }

        public bool _ZWrite {
            get {
                if ( m_prop_ZWrite != null && m_prop_ZWrite.type == MaterialProperty.PropType.Float ) {
                    return m_prop_ZWrite.floatValue != 0;
                }
                return true;
            }
            set {
                if ( m_prop_ZWrite != null && m_prop_ZWrite.type == MaterialProperty.PropType.Float ) {
                    m_prop_ZWrite.floatValue = value ? 1 : 0;
                }
            }
        }

        public bool useAlpha {
            get {
                return RenderModeNeedsAlpha( _Mode );
            }
        }

        protected override bool OnInitProperties( MaterialProperty[] props ) {
            var _prop_Mode = ShaderGUI.FindProperty( "_Mode", props );
            if ( _prop_Mode != null ) {
                var _prop_BlendOp = FindProperty( "_BlendOp", props );
                var _prop_SrcBlend = FindProperty( "_SrcBlend", props );
                var _prop_DstBlend = FindProperty( "_DstBlend", props );
                var _prop_ZWrite = FindProperty( "_ZWrite", props );
                var _prop_ZTest = FindProperty( "_ZTest", props );
                if ( _prop_SrcBlend != null && _prop_DstBlend != null && _prop_ZWrite != null && _prop_ZTest != null ) {
                    m_prop_Mode = _prop_Mode;
                    m_prop_BlendOp = _prop_BlendOp;
                    m_prop_SrcBlend = _prop_SrcBlend;
                    m_prop_DstBlend = _prop_DstBlend;
                    m_prop_ZWrite = _prop_ZWrite;
                    m_prop_ZTest = _prop_ZTest;
                    m_prop_AlphaPremultiply = FindProperty( "_AlphaPremultiply", props, false );
                    m_prop_AutoRenderQueue = FindProperty( "_AutoRenderQueue", props, false );

                    m_prop_Cutoff = FindProperty( "_Cutoff", props, false );
                    m_prop_ColorKey = FindProperty( "_ColorKey", props, false );
                    m_prop_MainTex_Alpha = FindProperty( "_MainTex_Alpha", props, false );
                    return true;
                }
            }
            return false;
        }

        static bool RenderModeNeedsAlpha( RenderMode mode ) {
            return mode == RenderMode.Cutout ||
                    mode == RenderMode.Transparent;
        }

        static bool IsTransparentMode( RenderMode mode ) {
            return mode == RenderMode.Additive ||
                mode == RenderMode.Transparent;
        }

        bool SetMaterialKeywords( Material material ) {
            var ret = true;
            var mode = _Mode;
            var cutoff = false;
            switch ( mode ) {
            case RenderMode.Cutout: {
                    var alphaTest = m_prop_Cutoff != null;
                    if ( m_prop_Cutoff != null && m_prop_Cutoff.floatValue == 0 ) {
                        if ( m_prop_Cutoff.type == MaterialProperty.PropType.Range ) {
                            m_prop_Cutoff.floatValue = m_prop_Cutoff.rangeLimits.x;
                        }
                    }
                    cutoff = alphaTest;
                    if ( alphaTest ) {
                        SetKeyword( material, "_ALPHATEST_ON", true );
                    } else {
                        SetKeyword( material, "_ALPHATEST_ON", false );
                    }
                    SetKeyword( material, "_ALPHATEST_COLOR_KEY_ON", false );
                }
                break;
            case RenderMode.ColorKey: {
                    cutoff = m_prop_ColorKey != null;
                    SetKeyword( material, "_ALPHATEST_COLOR_KEY_ON", cutoff );
                    SetKeyword( material, "_ALPHATEST_ON", false );
                }
                break;
            case RenderMode.Custom:
                break;
            default:
                cutoff = false;
                SetKeyword( material, "_ALPHATEST_COLOR_KEY_ON", false );
                SetKeyword( material, "_ALPHATEST_ON", false );
                break;
            }
            if ( !cutoff && ( mode == RenderMode.Cutout || mode == RenderMode.ColorKey ) ) {
                _Mode = RenderMode.Opaque;
                SetKeyword( material, "_ALPHATEST_COLOR_KEY_ON", false );
                SetKeyword( material, "_ALPHATEST_ON", false );
                ret = false;
            }
            var hasAlphaTex = useAlpha && m_prop_MainTex_Alpha != null ? m_prop_MainTex_Alpha.textureValue : null;
            SetKeyword( material, "_USE_EXTERNAL_ALPHA", hasAlphaTex != null );
            if ( hasAlphaTex == null && m_prop_MainTex_Alpha != null ) {
                m_prop_MainTex_Alpha.textureValue = null;
            }
            SetKeyword( material, "_MODE_OPAQUE", _Mode == RenderMode.Opaque );
            return ret;
        }

        void SetupMaterialWithRenderingMode( Material material, RenderMode renderingMode, bool modeChanged = true ) {
            var ok = false;
            while ( !ok ) {
                switch ( renderingMode ) {
                case RenderMode.Opaque:
                    material.SetOverrideTag( "RenderType", "Opaque" );
                    _Mode = RenderMode.Opaque;
                    _BlendOp = BlendOp.Add;
                    _SrcBlend = BlendMode.One;
                    _DstBlend = BlendMode.Zero;
                    _ZWrite = true;
                    if ( _AutoRenderQueue ) {
                        material.renderQueue = -1;
                    }
                    SetKeyword( material, "_ALPHAPREMULTIPLY_ON", false );
                    ok = SetMaterialKeywords( material );
                    break;
                case RenderMode.Cutout:
                    material.SetOverrideTag( "RenderType", "TransparentCutout" );
                    _Mode = RenderMode.Cutout;
                    _BlendOp = BlendOp.Add;
                    _SrcBlend = BlendMode.One;
                    _DstBlend = BlendMode.Zero;
                    _ZWrite = true;
                    if ( _AutoRenderQueue ) {
                        material.renderQueue = ( int )UnityEngine.Rendering.RenderQueue.AlphaTest;
                    }
                    SetKeyword( material, "_ALPHAPREMULTIPLY_ON", false );
                    ok = SetMaterialKeywords( material );
                    break;
                case RenderMode.ColorKey:
                    material.SetOverrideTag( "RenderType", "TransparentCutout" );
                    _Mode = RenderMode.ColorKey;
                    _BlendOp = BlendOp.Add;
                    _SrcBlend = BlendMode.One;
                    _DstBlend = BlendMode.Zero;
                    _ZWrite = true;
                    if ( _AutoRenderQueue ) {
                        material.renderQueue = ( int )UnityEngine.Rendering.RenderQueue.AlphaTest;
                    }
                    SetKeyword( material, "_ALPHAPREMULTIPLY_ON", false );
                    ok = SetMaterialKeywords( material );
                    break;
                case RenderMode.Transparent:
                    _Mode = RenderMode.Transparent;
                    material.SetOverrideTag( "RenderType", "Transparent" );
                    _BlendOp = BlendOp.Add;
                    if ( _AlphaPremultiply ) {
                        _SrcBlend = BlendMode.One;
                        SetKeyword( material, "_ALPHAPREMULTIPLY_ON", true );
                    } else {
                        _SrcBlend = BlendMode.SrcAlpha;
                        SetKeyword( material, "_ALPHAPREMULTIPLY_ON", false );
                    }
                    _DstBlend = BlendMode.OneMinusSrcAlpha;
                    _ZWrite = false;
                    if ( _AutoRenderQueue ) {
                        material.renderQueue = ( int )UnityEngine.Rendering.RenderQueue.Transparent;
                    }
                    ok = SetMaterialKeywords( material );
                    break;
                case RenderMode.Additive:
                    _Mode = RenderMode.Additive;
                    material.SetOverrideTag( "RenderType", "Transparent" );
                    _BlendOp = BlendOp.Add;
                    if ( _AlphaPremultiply ) {
                        _SrcBlend = BlendMode.One;
                        SetKeyword( material, "_ALPHAPREMULTIPLY_ON", true );
                    } else {
                        _SrcBlend = BlendMode.SrcAlpha;
                        SetKeyword( material, "_ALPHAPREMULTIPLY_ON", false );
                    }
                    _DstBlend = BlendMode.One;
                    _ZWrite = false;
                    if ( _AutoRenderQueue ) {
                        material.renderQueue = ( int )UnityEngine.Rendering.RenderQueue.Transparent;
                    }
                    ok = SetMaterialKeywords( material );
                    break;
                case RenderMode.SoftAdditive:
                    material.SetOverrideTag( "RenderType", "Transparent" );
                    _Mode = RenderMode.SoftAdditive;
                    _BlendOp = BlendOp.Add;
                    _SrcBlend = BlendMode.OneMinusDstColor;
                    _DstBlend = BlendMode.One;
                    _ZWrite = false;
                    if ( _AutoRenderQueue ) {
                        material.renderQueue = ( int )UnityEngine.Rendering.RenderQueue.Transparent;
                    }
                    SetKeyword( material, "_ALPHAPREMULTIPLY_ON", false );
                    ok = SetMaterialKeywords( material );
                    break;
                case RenderMode.Custom:
                    if ( modeChanged ) {
                        _AutoRenderQueue = false;
                    }
                    _Mode = RenderMode.Custom;
                    SetMaterialKeywords( material );
                    ok = true;
                    break;
                default:
                    ok = true;
                    break;
                }
            }
            if ( modeChanged && _Mode != RenderMode.Custom ) {
                _AutoRenderQueue = true;
            }
        }

        protected override void OnDrawPropertiesGUI( Material material, MaterialProperty[] props ) {
            if ( m_prop_Mode != null ) {
                EditorGUI.showMixedValue = m_prop_Mode.hasMixedValue;
                var oldMode = ( RenderMode )m_prop_Mode.floatValue;
                EditorGUI.BeginChangeCheck();
                var renderingMode = ( RenderMode )EditorGUILayout.EnumPopup( "Render Mode", oldMode, GUILayout.MinWidth( 200 ) );
                EditorGUI.indentLevel++;
                if ( m_prop_AutoRenderQueue != null ) {
                    m_MaterialEditor.ShaderProperty( m_prop_AutoRenderQueue, m_prop_AutoRenderQueue.displayName );
                }
                if ( m_prop_AlphaPremultiply != null && IsTransparentMode( _Mode ) ) {
                    m_MaterialEditor.ShaderProperty( m_prop_AlphaPremultiply, m_prop_AlphaPremultiply.displayName );
                }
                if ( _Mode == RenderMode.Custom ) {
                    if ( m_prop_BlendOp != null ) {
                        var a = ( BlendOp )EditorGUILayout.EnumPopup( m_prop_BlendOp.displayName, ( BlendOp )m_prop_BlendOp.floatValue );
                        m_prop_BlendOp.floatValue = ( float )a;
                    }
                    if ( m_prop_SrcBlend != null ) {
                        var a = ( BlendMode )EditorGUILayout.EnumPopup( m_prop_SrcBlend.displayName, ( BlendMode )m_prop_SrcBlend.floatValue );
                        m_prop_SrcBlend.floatValue = ( float )a;
                    }
                    if ( m_prop_DstBlend != null ) {
                        var a = ( BlendMode )EditorGUILayout.EnumPopup( m_prop_DstBlend.displayName, ( BlendMode )m_prop_DstBlend.floatValue );
                        m_prop_DstBlend.floatValue = ( float )a;
                    }
                    if ( m_prop_ZWrite != null ) {
                        m_MaterialEditor.ShaderProperty( m_prop_ZWrite, m_prop_ZWrite.displayName );
                    }
                    if ( m_prop_ZTest != null ) {
                        var a = ( ZTest )EditorGUILayout.EnumPopup( m_prop_ZTest.displayName, ( ZTest )m_prop_ZTest.floatValue );
                        m_prop_ZTest.floatValue = ( float )a;
                    }
                }
                EditorGUI.indentLevel--;
                if ( EditorGUI.EndChangeCheck() ) {
                    this.m_MaterialEditor.RegisterPropertyChangeUndo( m_prop_Mode.name );
                    this.m_prop_Mode.floatValue = ( float )renderingMode;
                    SetupMaterialWithRenderingMode( material, renderingMode, oldMode != renderingMode );
                }
                EditorGUI.showMixedValue = false;
            }
            EditorGUILayout.Space();
        }

        protected override void OnRefreshKeywords( Material material ) {
            SetMaterialKeywords( material );
        }

        protected override void OnMaterialChanged( Material material ) {
            SetMaterialKeywords( material );
        }

        public static UnitMaterialEditor Create( UnitMaterialEditor.PropEditorSettings s ) {
            return new ShaderGUI_RenderMode();
        }
    }
}
