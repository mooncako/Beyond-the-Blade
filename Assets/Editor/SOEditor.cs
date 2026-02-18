using UnityEditor;
using UnityEngine;

public class SOEditor : EditorWindow
{
    private Vector2 _scroll;
    [SerializeField] private ScriptableObject _so;
    private Editor _editor;

    public static void Open(ScriptableObject so)
    {
        SOEditor window = GetWindow<SOEditor>(true, "Scriptable Object Editor");
        window.SetTarget(so);
        window.minSize = new Vector2(380, 500);
        window.Show();
        window.Focus();
    }

    private void OnDisable()
    {
        if(_editor != null)
        {
            DestroyImmediate(_editor);
            _editor = null;
        }
    }

    private void SetTarget(ScriptableObject so)
    {
        if(_so == so) return;

        _so = so;

        if(_editor != null)
        {
            DestroyImmediate(_editor);
            _editor = null;
        }

        if(_so != null)
        {
            _editor = Editor.CreateEditor(_so);
        }

        titleContent = new GUIContent(_so != null? $"{_so.name}" : "Scriptable Object Editor");

        Repaint();
    }

    void OnGUI()
    {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
        {
            using (new EditorGUI.DisabledScope(true))
            {
                var newSO = (ScriptableObject)EditorGUILayout.ObjectField(
                _so,
                typeof(ScriptableObject),
                false,
                GUILayout.MinWidth(200)
                );

                if (newSO != _so)
                {
                    SetTarget(newSO);
                }
            }




            GUILayout.FlexibleSpace();

            using(new EditorGUI.DisabledScope(_so == null))
            {
                if(GUILayout.Button("Locate", EditorStyles.toolbarButton))
                {
                    EditorGUIUtility.PingObject(_so);
                }
                if(GUILayout.Button("Select", EditorStyles.toolbarButton))
                {
                    Selection.activeObject = _so;
                }
            }
        }

        if(_so == null)
        {
            EditorGUILayout.HelpBox("Assign a scriptable object to edit.", MessageType.Info);
            return;
        }

        if(_so == null || _editor.target != _so)
        {
            _editor = Editor.CreateEditor(_so);
        }

        EditorGUI.BeginChangeCheck();
        _scroll = EditorGUILayout.BeginScrollView(_scroll);
        _editor.OnInspectorGUI();
        EditorGUILayout.EndScrollView();

        if(EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(_so);
            AssetDatabase.SaveAssets();
        }
    }
}
