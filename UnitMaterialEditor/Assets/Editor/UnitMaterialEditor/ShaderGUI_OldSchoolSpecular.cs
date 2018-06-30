using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ArtistKit {

    public class ShaderGUI_OldSchoolSpecular : UnitMaterialEditor {

        MaterialProperty m_prop_MainTex = null;
        MaterialProperty m_prop_MainTex_Alpha = null;
        MaterialProperty m_prop_Color = null;
        MaterialProperty m_prop_Cutoff = null;
        MaterialProperty m_prop_ColorKey = null;
        MaterialProperty m_prop_BumpScale = null;
        MaterialProperty m_prop_BumpMap = null;
        MaterialProperty m_prop_SpecGlossMap = null;
        MaterialProperty m_prop_SpecColor = null;
        MaterialProperty m_prop_Shininess = null;

        protected override bool OnInitProperties( MaterialProperty[] props ) {
            m_prop_MainTex = FindProperty( "_Mode", props );
            m_prop_MainTex = FindProperty( "_MainTex", props );
            m_prop_MainTex_Alpha = FindProperty( "_MainTex_Alpha", props );
            m_prop_Color = FindProperty( "_Color", props );
            m_prop_Cutoff = FindProperty( "_Cutoff", props );
            m_prop_ColorKey = FindProperty( "_ColorKey", props );
            m_prop_BumpScale = FindProperty( "_BumpScale", props, false );
            m_prop_BumpMap = FindProperty( "_BumpMap", props );
            m_prop_SpecGlossMap = FindProperty( "_SpecGlossMap", props, false );
            m_prop_SpecColor = FindProperty( "_SpecColor", props, false );
            m_prop_Shininess = FindProperty( "_Shininess", props );
            return true;
        }

        public static UnitMaterialEditor Create( UnitMaterialEditor.PropEditorSettings s ) {
            return new ShaderGUI_OldSchoolSpecular();
        }
        
        protected override void OnRefreshKeywords( Material material ) {
            SetKeyword( material, "_NORMALMAP", m_prop_BumpMap != null && m_prop_BumpMap.textureValue != null );
            SetKeyword( material, "_SPECGLOSSMAP", m_prop_SpecGlossMap != null && m_prop_SpecGlossMap.textureValue != null );
        }

        protected override void OnDrawPropertiesGUI( Material material ) {
            EditorGUIUtility.labelWidth = 0f;
            var gui = FindPropEditor<ShaderGUI_RenderMode>();
            var mode = gui != null ? gui._Mode : RenderMode.Opaque;
            var useAlpha = gui != null ? gui.useAlpha : false;
            if ( m_prop_MainTex != null ) {
                m_MaterialEditor.TexturePropertySingleLine(
                    new GUIContent( m_prop_MainTex.displayName ),
                    m_prop_MainTex, m_prop_Color );
                EditorGUI.indentLevel++;
                m_MaterialEditor.TextureScaleOffsetProperty( m_prop_MainTex );
                EditorGUI.indentLevel--;
            }
            if ( useAlpha && m_prop_MainTex_Alpha != null ) {
                EditorGUIUtility.labelWidth = 0f;
                m_MaterialEditor.TexturePropertySingleLine(
                    new GUIContent( m_prop_MainTex_Alpha.displayName ),
                    m_prop_MainTex_Alpha );
            }
            if ( m_prop_BumpMap != null ) {
                EditorGUIUtility.labelWidth = 0f;
                m_MaterialEditor.TexturePropertySingleLine(
                    new GUIContent( m_prop_BumpMap.displayName ),
                    m_prop_BumpMap,
                    m_prop_BumpMap.textureValue != null ?
                        m_prop_BumpScale : null
                );
            }
            if ( m_prop_SpecGlossMap != null ) {
                EditorGUIUtility.labelWidth = 0f;
                m_MaterialEditor.TexturePropertySingleLine(
                    new GUIContent( m_prop_SpecGlossMap.displayName ),
                    m_prop_SpecGlossMap,
                    m_prop_SpecColor
                );
            } else if ( m_prop_SpecColor != null ) {
                m_MaterialEditor.SetDefaultGUIWidths();
                m_MaterialEditor.ShaderProperty( m_prop_SpecColor, m_prop_SpecColor.displayName );
            }
            m_MaterialEditor.SetDefaultGUIWidths();
            if ( m_prop_Shininess != null ) {
                m_MaterialEditor.ShaderProperty( m_prop_Shininess, m_prop_Shininess.displayName );
            }
            if ( mode == RenderMode.Cutout && m_prop_Cutoff != null ) {
                m_MaterialEditor.ShaderProperty( m_prop_Cutoff, m_prop_Cutoff.displayName );
            } else if ( mode == RenderMode.ColorKey ) {
                m_MaterialEditor.ShaderProperty( m_prop_ColorKey, m_prop_ColorKey.displayName );
            }
        }
    }
}
