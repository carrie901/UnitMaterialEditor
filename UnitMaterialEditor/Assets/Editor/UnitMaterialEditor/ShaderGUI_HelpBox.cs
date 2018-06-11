using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ArtistKit {

    public class ShaderGUI_HelpBox : UnitMaterialEditor {

        MessageType m_type = MessageType.None;
        String m_text = String.Empty;

        protected override bool OnInitProperties( MaterialProperty[] props ) {
            if ( m_args != null ) {
                m_args.GetField( out m_text, "text", String.Empty );
                String type;
                if ( m_args.GetField( out type, "type", "none" ) ) {
                    try {
                        m_type = ( MessageType )Enum.Parse( typeof( MessageType ), type, true );
                    } catch ( Exception e ) {
                        Debug.LogException( e );
                    }
                }
                return !String.IsNullOrEmpty( m_text );
            }
            return false;
        }

        protected override void OnDrawPropertiesGUI( Material material ) {
            if ( !ShaderGUIHelper.IsModeMatched( this, m_args ) || !GetBoolTestResult() ) {
                return;
            }
            if ( !String.IsNullOrEmpty( m_text ) ) {
                EditorGUILayout.HelpBox( m_text, m_type );
            }
        }

        public static UnitMaterialEditor Create( UnitMaterialEditor.PropEditorSettings s ) {
            return new ShaderGUI_HelpBox();
        }

    }
}
//EOF
