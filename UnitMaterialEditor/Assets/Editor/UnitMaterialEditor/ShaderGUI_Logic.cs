using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ArtistKit {

    public class ShaderGUI_Logic : UnitMaterialEditor {

        public override bool GetLogicOpResult() {
            return ShaderGUIHelper.IsModeMatched( this, m_args ) &&
                ShaderGUIHelper.ExcuteLogicOp( this, null, m_args ) == 1;
        }

        protected override bool OnInitProperties( MaterialProperty[] props ) {
            return true;
        }

        public static UnitMaterialEditor Create( UnitMaterialEditor.PropEditorSettings s ) {
            var ret = new ShaderGUI_Logic();
            ret.m_MaterialEditor = s.parent.materialEditor;
            ret.m_parent = s.parent;
            return ret;
        }
    }
}
