using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ArtistKit {

    public class ShaderGUI_SingleProp : UnitMaterialEditor {

        String m_propName = String.Empty;
        MaterialProperty m_prop = null;
        bool m_outsideUse = false;

        public String propName {
            get {
                return m_propName ?? String.Empty;
            }
        }

        public bool outsideUse {
            get {
                return m_outsideUse;
            }
            set {
                m_outsideUse = value;
            }
        }

        public void DrawGUI( Material material ) {
            if ( !ShaderGUIHelper.IsModeMatched( this, m_args ) ) {
                return;
            }
            m_MaterialEditor.SetDefaultGUIWidths();
            float h = m_MaterialEditor.GetPropertyHeight( m_prop, m_prop.displayName );
            Rect r = EditorGUILayout.GetControlRect( true, h, EditorStyles.layerMaskField );
            m_MaterialEditor.ShaderProperty( r, m_prop, m_prop.displayName );
        }

        protected override void OnDrawPropertiesGUI( Material material ) {
            if ( !m_outsideUse ) {
                DrawGUI( material );
            }
        }
        
        protected override bool OnInitProperties( MaterialProperty[] props ) {
            m_prop = ShaderGUI.FindProperty( m_propName, props );
            return m_prop != null;
        }

        public static UnitMaterialEditor Create( UnitMaterialEditor.PropEditorSettings s ) {
            var ret = new ShaderGUI_SingleProp();
            ret.m_propName = s.name;
            return ret;
        }
    }
}
