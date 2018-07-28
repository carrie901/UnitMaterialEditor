using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ArtistKit {

    public class ShaderGUI_UpdateKeyword : UnitMaterialEditor {

        String m_propName = String.Empty;
        MaterialProperty m_prop = null;

        public override bool GetLogicOpResult( out String returnValue ) {
            returnValue = String.Empty;
            return m_prop != null && ShaderGUIHelper.IsModeMatched( this, m_args ) &&
                ShaderGUIHelper.ExcuteLogicOp( this, m_prop, m_args ) == 1;
        }

        protected override bool OnInitProperties( MaterialProperty[] props ) {
            m_prop = ShaderGUI.FindProperty( m_propName, props, false );
            return true;
        }

        protected override void OnMaterialChanged( Material material ) {
            var keyword = String.Empty;
            if ( m_args.GetField( out keyword, "keyword", String.Empty ) ) {
                while ( m_prop != null ) {
                    if ( !ShaderGUIHelper.IsModeMatched( this, m_args ) ) {
                        break;
                    }
                    var cmp = ShaderGUIHelper.ExcuteLogicOp( this, m_prop, m_args );
                    if ( cmp != -1 ) {
                        SetKeyword( material, keyword, cmp == 1 );
                        return;
                    }
                    break;
                }
                // disable keyword for default
                if ( material.IsKeywordEnabled( keyword ) ) {
                    SetKeyword( material, keyword, false );
                }
            }
        }

        public static UnitMaterialEditor Create( UnitMaterialEditor.PropEditorSettings s ) {
            var ret = new ShaderGUI_UpdateKeyword();
            ret.m_propName = s.name;
            return ret;
        }

    }
}
