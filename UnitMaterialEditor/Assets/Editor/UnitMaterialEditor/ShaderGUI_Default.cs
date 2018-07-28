using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ArtistKit {

    public class ShaderGUI_Default : UnitMaterialEditor {

        List<KeyValuePair<MaterialProperty, ShaderGUI_SingleProp>> m_props = null;

        protected override void OnDrawPropertiesGUI( Material material, MaterialProperty[] props ) {
            if ( m_props != null ) {
                for ( int i = 0; i < m_props.Count; ++i ) {
                    var item = m_props[ i ];
                    var defaultProp = item.Key;
                    var customProp = item.Value;
                    if ( customProp == null ) {
                        if ( ( defaultProp.flags & ( MaterialProperty.PropFlags.HideInInspector | MaterialProperty.PropFlags.PerRendererData ) ) == MaterialProperty.PropFlags.None ) {
                            m_MaterialEditor.SetDefaultGUIWidths();
                            float h = m_MaterialEditor.GetPropertyHeight( defaultProp, defaultProp.displayName );
                            Rect r = EditorGUILayout.GetControlRect( true, h, EditorStyles.layerMaskField );
                            m_MaterialEditor.ShaderProperty( r, defaultProp, defaultProp.displayName );
                            m_MaterialEditor.SetDefaultGUIWidths();
                        }
                    } else {
                        customProp.DrawGUI( material );
                    }
                }
            }
        }

        protected override bool OnInitProperties( MaterialProperty[] props ) {
            if ( m_props == null ) {
                m_props = new List<KeyValuePair<MaterialProperty, ShaderGUI_SingleProp>>( props.Length );
            } else {
                m_props.Clear();
            }
            var excludes = FindPropEditors<ShaderGUI_SingleProp>();
            for ( int i = 0; i < props.Length; ++i ) {
                var name = props[ i ].name;
                var custom = excludes.Find( x => x.propName == name );
                if ( custom != null ) {
                    custom.outsideUse = true;
                }
                m_props.Add( new KeyValuePair<MaterialProperty, ShaderGUI_SingleProp>( props[ i ], custom ) );
            }
            return true;
        }

        public static UnitMaterialEditor Create( UnitMaterialEditor.PropEditorSettings s ) {
            var ret = new ShaderGUI_Default();
            return ret;
        }
    }
}
