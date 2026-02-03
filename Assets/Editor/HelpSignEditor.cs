using UnityEditor;

[CustomEditor(typeof(HelpSign))]
public class HelpSignEditor : Editor
{
    public override void OnInspectorGUI()
    {
        HelpSign script = (HelpSign)target;
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        if (EditorGUI.EndChangeCheck())
        {
            script.OnEditorChange();
        }
    }
}
