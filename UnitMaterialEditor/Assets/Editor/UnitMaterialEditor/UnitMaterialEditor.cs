using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ArtistKit {

    public class UnitMaterialEditor : ShaderGUI {

        public enum BlendOp {
            Add = 0,
            Subtract = 1,
            ReverseSubtract = 2,
            Min = 3,
            Max = 4,
        }

        public enum ZTest {
            Less,
            Greater,
            LEqual,
            GEqual,
            Equal,
            NotEqual,
            Always,
        }

        public enum RenderMode {
            Opaque = 0,
            Cutout,
            ColorKey,
            Transparent,
            Additive,
            SoftAdditive,
            Custom = 999,
        }

        public static readonly int[] BlendModeValues = ( int[] )Enum.GetValues( typeof( RenderMode ) );
        public static readonly string[] BlendModeNames = Enum.GetNames( typeof( RenderMode ) );
        public static readonly string[] CullModeNames = Enum.GetNames( typeof( CullMode ) );

        List<UnitMaterialEditor> m_props = new List<UnitMaterialEditor>();

        protected MaterialEditor m_MaterialEditor = null;
        protected UnitMaterialEditor m_parent = null;
        protected int m_propIndex = -1;
        protected Vector2 m_uiSpace = new Vector2( 0, 0 );
        protected String m_id = String.Empty;
        protected JSONObject m_args = null;

        bool m_FirstTimeApply = true;
        bool m_userDefaultEditor = false;
        bool m_editorDirty = true;

        public struct PropEditorSettings {
            public String name;
            public UnitMaterialEditor parent;
            public MaterialProperty[] props;
        }

        static String LastShaderPath = String.Empty;
        static DateTime LastShaderModifyTime = new DateTime();
        static JSONObject LastEditorData = null;
        public static Regex Reg_EditorData = new Regex( @"#BEGINEDITOR((.|\n)+)#ENDEDITOR" );
        public static Regex Reg_LogicRef = new Regex( @"^\s*(!)?\s*\[(.+)\]$" );
        static Dictionary<String, Func<PropEditorSettings, UnitMaterialEditor>> ShaderPropGUIFactory = new Dictionary<String, Func<PropEditorSettings, UnitMaterialEditor>>();

        public MaterialEditor materialEditor {
            get {
                return m_MaterialEditor;
            }
        }

        static UnitMaterialEditor() {
            var thisType = typeof( UnitMaterialEditor );
            var assembly = thisType.Assembly;
            var types = assembly.GetTypes();
            var prefix = "ShaderGUI_";
            for ( int i = 0; i < types.Length; ++i ) {
                var t = types[ i ];
                var name = t.Name;
                if ( name.StartsWith( prefix ) && t.IsSubclassOf( thisType ) ) {
                    name = name.Substring( prefix.Length );
                    var creator = t.GetMethod( "Create", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic );
                    if ( creator != null && creator.ReturnType == typeof( UnitMaterialEditor ) ) {
                        ShaderPropGUIFactory[ name ] = s => {
                            return creator.Invoke( null, new object[] { s } ) as UnitMaterialEditor;
                        };
                    }
                }
            }
        }

        static bool IsEmptyShader( Shader shader ) {
            return shader == null ||
                shader.name == "Hidden/InternalErrorShader";
        }

        static JSONObject GetEditorData( String shaderPath, ref bool dirty ) {
            if ( !File.Exists( shaderPath ) ) {
                return null;
            }
            var now = File.GetLastWriteTime( shaderPath );
            if ( LastEditorData == null ||
                LastShaderPath != shaderPath ||
                LastShaderModifyTime != now ) {
                LastShaderModifyTime = now;
                LastShaderPath = shaderPath;
                var source = File.ReadAllText( shaderPath );
                var m = Reg_EditorData.Match( source );
                if ( m.Success && m.Groups.Count > 2 ) {
                    var g = m.Groups[ 1 ].ToString();
                    LastEditorData = JSONObject.Create( g );
                    dirty = true;
                } else {
                    LastEditorData = new JSONObject();
                }
            }
            return LastEditorData;
        }

        protected virtual bool OnInitProperties( MaterialProperty[] props ) {
            m_userDefaultEditor = false;
            var material = m_MaterialEditor.target as Material;
            var shader = material.shader;
            var path = shader != null ? AssetDatabase.GetAssetPath( shader ) : String.Empty;
            if ( !IsEmptyShader( shader ) && !String.IsNullOrEmpty( path ) ) {
                try {
                    var json = GetEditorData( path, ref m_editorDirty );
                    if ( m_editorDirty && json != null && json.IsArray ) {
                        m_props.Clear();
                        m_editorDirty = false;
                        var vals = json.list;
                        for ( int i = 0; i < vals.Count; ++i ) {
                            var val = vals[ i ];
                            if ( val != null && val.IsObject ) {
                                var editor = String.Empty;
                                if ( !val.GetField( out editor, "editor", String.Empty ) ) {
                                    continue;
                                }
                                Func<PropEditorSettings, UnitMaterialEditor> f = null;
                                if ( ShaderPropGUIFactory.TryGetValue( editor, out f ) ) {
                                    var propName = editor;
                                    var args = val.GetField( "args" );
                                    if ( args != null && args.IsObject ) {
                                        args.GetField( out propName, "name", editor );
                                    }
                                    var settings = new PropEditorSettings {
                                        name = propName,
                                        parent = this,
                                        props = props
                                    };
                                    var p = f( settings );
                                    if ( p != null ) {
                                        float space_begin;
                                        float space_end;
                                        val.GetField( out space_begin, "space", 0 );
                                        val.GetField( out space_end, "endspace", 0 );
                                        p.m_uiSpace = new Vector2( space_begin, space_end );
                                        val.GetField( out p.m_id, "id", String.Empty );
                                        p.m_args = args;
                                        p.m_MaterialEditor = m_MaterialEditor;
                                        p.m_parent = this;
                                        try {
                                            if ( p.OnInitProperties( props ) ) {
                                                m_props.Add( p );
                                            }
                                        } catch ( Exception e ) {
                                            Debug.LogException( e );
                                        }
                                    }
                                }
                            }
                        }
                    }
                    m_userDefaultEditor = m_props.Count == 0;
                } catch ( Exception e ) {
                    Debug.LogException( e );
                    m_userDefaultEditor = true;
                }
            } else {
                m_userDefaultEditor = true;
            }
            return true;
        }

        protected void DrawPropertiesGUI( Material material, MaterialProperty[] props ) {
            if ( OnInitProperties( props ) ) {
                OnDrawPropertiesGUI( material, props );
            }
        }

        protected virtual void OnDrawPropertiesGUI( Material material, MaterialProperty[] props ) {
        }

        protected virtual void OnMaterialChanged( Material material ) {
            for ( int i = 0; i < m_props.Count; ++i ) {
                m_props[ i ].OnMaterialChanged( material );
            }
        }

        protected virtual void OnRefreshKeywords( Material material ) {
            for ( int i = 0; i < m_props.Count; ++i ) {
                m_props[ i ].OnRefreshKeywords( material );
            }
        }

        void RefreshKeywords( Material material ) {
            OnRefreshKeywords( material );
        }

        void FindProperties( MaterialProperty[] props ) {
            OnInitProperties( props );
        }

        void MaterialChanged( Material material ) {
            OnMaterialChanged( material );
        }

        void ShaderPropertiesGUI( Material material, MaterialProperty[] props ) {
            EditorGUI.BeginChangeCheck();
            for ( int i = 0; i < m_props.Count; ++i ) {
                GUILayout.Space( m_props[ i ].m_uiSpace.x );
                m_props[ i ].DrawPropertiesGUI( material, props );
                GUILayout.Space( m_props[ i ].m_uiSpace.y );
            }
            EditorGUILayout.Space();
            DrawKeywords( material );
            if ( EditorGUI.EndChangeCheck() ) {
                var targets = m_MaterialEditor.targets;
                for ( int i = 0; i < targets.Length; i++ ) {
                    var mat = targets[ i ] as Material;
                    MaterialChanged( mat );
                }
            }
            EditorGUILayout.Space();
            m_MaterialEditor.RenderQueueField();
        }

        public static void DrawKeywords( Material material ) {
            var _keywords = material.shaderKeywords;
            var keywords = String.Join( "; ", _keywords );
            EditorGUILayout.Separator();
            GUI.enabled = false;
            try {
                var newKeywords = EditorGUILayout.TextArea( keywords );
                if ( newKeywords != keywords ) {
                    var key = newKeywords.Split( ' ', ',', ';' );
                    for ( int i = 0; i < key.Length; ++i ) {
                        key[ i ] = key[ i ].Trim();
                    }
                    material.shaderKeywords = key;
                }
            } finally {
                GUI.enabled = true;
            }
        }

        public override void OnGUI( MaterialEditor materialEditor, MaterialProperty[] props ) {
            m_MaterialEditor = materialEditor;
            FindProperties( props );
            if ( m_userDefaultEditor ) {
                base.OnGUI( materialEditor, props );
            } else {
                Material material = materialEditor.target as Material;
                if ( this.m_FirstTimeApply ) {
                    MaterialChanged( material );
                    this.m_FirstTimeApply = false;
                }
                ShaderPropertiesGUI( material, props );
            }
        }

        public static void SetKeyword( Material m, String keyword, bool state ) {
            if ( state ) {
                m.EnableKeyword( keyword );
            } else {
                m.DisableKeyword( keyword );
            }
        }

        public T FindPropEditor<T>( String id = null ) where T : UnitMaterialEditor {
            if ( m_parent != null ) {
                var t = typeof( T );
                for ( int i = 0; i < m_parent.m_props.Count; ++i ) {
                    if ( t.IsInstanceOfType( m_parent.m_props[ i ] ) &&
                        ( id == null || id == m_parent.m_props[ i ].m_id ) ) {
                        return m_parent.m_props[ i ] as T;
                    }
                }
            }
            return null;
        }

        public List<T> FindPropEditors<T>( List<T> list = null ) where T : UnitMaterialEditor {
            list = list ?? new List<T>();
            if ( m_parent != null ) {
                var t = typeof( T );
                for ( int i = 0; i < m_parent.m_props.Count; ++i ) {
                    if ( t.IsInstanceOfType( m_parent.m_props[ i ] ) ) {
                        list.Add( m_parent.m_props[ i ] as T );
                    }
                }
            }
            return list;
        }

        public T GetPrevEditor<T>() where T : UnitMaterialEditor {
            if ( m_parent != null && m_propIndex > 0 ) {
                var t = typeof( T );
                if ( t.IsInstanceOfType( m_parent.m_props[ m_propIndex - 1 ] ) ) {
                    return m_parent.m_props[ m_propIndex - 1 ] as T;
                }
            }
            return null;
        }

        public T GetNextEditor<T>() where T : UnitMaterialEditor {
            if ( m_parent != null && m_propIndex < m_parent.m_props.Count - 1 ) {
                var t = typeof( T );
                if ( t.IsInstanceOfType( m_parent.m_props[ m_propIndex + 1 ] ) ) {
                    return m_parent.m_props[ m_propIndex + 1 ] as T;
                }
            }
            return null;
        }

        public bool GetLogicOpResult() {
            String returnValue;
            return GetLogicOpResult( out returnValue );
        }

        public virtual bool GetLogicOpResult( out String returnValue ) {
            returnValue = String.Empty;
            return false;
        }

        protected virtual String ComputeReturnValue() {
            return "null";
        }

        public virtual String GetReturnValue() {
            String returnValue;
            if ( GetLogicOpResult( out returnValue ) ) {
                return returnValue;
            }
            return String.Empty;
        }

        public virtual bool GetBoolTestResult() {
            if ( m_args != null ) {
                String _ref;
                m_args.GetField( out _ref, "btest", String.Empty );
                if ( !String.IsNullOrEmpty( _ref ) ) {
                    var m = Reg_LogicRef.Match( _ref );
                    if ( m.Success && m.Groups.Count > 2 ) {
                        var rev = m.Groups[ 1 ].ToString().Trim() == "!";
                        var id = m.Groups[ 2 ].ToString().Trim();
                        if ( !String.IsNullOrEmpty( id ) ) {
                            var ui = FindPropEditor<UnitMaterialEditor>( id );
                            if ( ui != null ) {
                                var returnValue = String.Empty;
                                var b = ui.GetLogicOpResult( out returnValue );
                                if ( rev ) {
                                    b = !b;
                                }
                                return b;
                            }
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}
