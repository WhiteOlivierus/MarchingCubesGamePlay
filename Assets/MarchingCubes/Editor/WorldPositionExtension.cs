using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Transform))]
public class WorldPositionExtension : Editor
{
    public override void OnInspectorGUI()
    {
        Transform transform = (Transform)target;
        EditorGUILayout.BeginHorizontal();
        transform.position = EditorGUILayout.Vector3Field("Global Position", transform.position);
        EditorGUILayout.EndHorizontal();
        base.OnInspectorGUI();
    }
}
