using UnityEditor;

[CustomEditor(typeof(TriggerPlatforms))]
public class TriggerPlatformsEditor : Editor
{
    SerializedProperty triggerType;

    SerializedProperty audioClip;

    //Disolve
    SerializedProperty timeToDisapear;
    SerializedProperty timeToApear;
    SerializedProperty objectToDisapear;

    //Button
    SerializedProperty restartOnPlayerDeath;
    SerializedProperty isToggle;
    SerializedProperty canDeactivateOnExit;
    SerializedProperty targetObjects;

    private void OnEnable()
    {
        triggerType = serializedObject.FindProperty("triggerType");

        audioClip = serializedObject.FindProperty("audioClip");

        //Disolve
        timeToDisapear = serializedObject.FindProperty("timeToDisapear");
        timeToApear = serializedObject.FindProperty("timeToApear");
        objectToDisapear = serializedObject.FindProperty("objectToDisapear");

        //Button
        restartOnPlayerDeath = serializedObject.FindProperty("restartOnPlayerDeath");
        isToggle = serializedObject.FindProperty("isToggle");
        canDeactivateOnExit = serializedObject.FindProperty("canDeactivateOnExit");
        targetObjects = serializedObject.FindProperty("targetObjects");
    }

    public override void OnInspectorGUI()
    {
        TriggerPlatforms script = (TriggerPlatforms)target;

        serializedObject.Update();

        EditorGUILayout.PropertyField(triggerType);

        EditorGUILayout.Space(10);
        switch (script.triggerType)
        {
            case TriggerPlatforms.TriggerType.DisolveFloor:
                EditorGUILayout.PropertyField(audioClip);
                EditorGUILayout.PropertyField(timeToDisapear);
                EditorGUILayout.PropertyField(timeToApear);
                EditorGUILayout.PropertyField(objectToDisapear);
                break;

            case TriggerPlatforms.TriggerType.Button:
                EditorGUILayout.PropertyField(audioClip);
                EditorGUILayout.LabelField("Object to be open or interact with");
                EditorGUILayout.PropertyField(restartOnPlayerDeath);
                EditorGUILayout.PropertyField(isToggle);
                EditorGUILayout.PropertyField(canDeactivateOnExit);
                EditorGUILayout.PropertyField(targetObjects);
                break;

            case TriggerPlatforms.TriggerType.LevelEnd:
                EditorGUILayout.PropertyField(audioClip);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
