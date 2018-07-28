using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using System.Text.RegularExpressions;

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

        protected override void OnDrawPropertiesGUI( Material material, MaterialProperty[] props ) {
            if ( !ShaderGUIHelper.IsModeMatched( this, m_args ) || !GetBoolTestResult() ) {
                return;
            }
            if ( !String.IsNullOrEmpty( m_text ) ) {
                var jo = m_args.GetField( "params" );
                List<object> pars = null;
                if ( jo != null ) {
                    if ( jo.IsArray ) {
                        for ( int i = 0; i < jo.Count; ++i ) {
                            var o = jo[ i ];
                            if ( o != null && !o.isContainer ) {
                                pars = pars ?? new List<object>();
                                pars.Add( ShaderGUIHelper.JSONValueToString( o ) );
                            }
                        }
                    } else if ( jo.IsObject == false ) {
                        pars = pars ?? new List<object>();
                        pars.Add( ShaderGUIHelper.JSONValueToString( jo ) );
                    }
                }
                for ( ; ; ) {
                    var m = Regex.Matches( m_text, @"\{(\d+)\}" );
                    if ( m.Count > 0 ) {
                        var maxIndex = 0;
                        for ( int i = 0; i < m.Count; ++i ) {
                            var groups = m[ i ].Groups;
                            if ( groups.Count == 2 ) {
                                var index = -1;
                                if ( int.TryParse( groups[ 1 ].ToString(), out index ) ) {
                                    if ( index > maxIndex && index >= 0 ) {
                                        maxIndex = index;
                                    }
                                }
                            }
                        }
                        if ( maxIndex >= 0 ) {
                            pars = pars ?? new List<object>();
                            if ( pars.Count < maxIndex + 1 ) {
                                pars.Resize( maxIndex + 1 );
                            }
                            for ( int i = 0; i < pars.Count; ++i ) {
                                var ref_id = pars[ i ] as String;
                                if ( !String.IsNullOrEmpty( ref_id ) ) {
                                    var prop = FindPropEditor<UnitMaterialEditor>( ref_id );
                                    if ( prop != null ) {
                                        var v = prop.GetReturnValue();
                                        if ( String.IsNullOrEmpty( v ) ) {
                                            v = "null";
                                        }
                                        pars[ i ] = v;
                                    }
                                }
                            }
                            EditorGUILayout.HelpBox( String.Format( m_text, pars.ToArray() ), m_type );
                        }
                        break;
                    }
                    EditorGUILayout.HelpBox( m_text, m_type );
                    break;
                }
            }
        }

        public static UnitMaterialEditor Create( UnitMaterialEditor.PropEditorSettings s ) {
            return new ShaderGUI_HelpBox();
        }

    }
}
//EOF
