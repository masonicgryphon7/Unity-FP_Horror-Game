using UnityEngine;
using UnityEditor;
using System.Collections;
[CustomEditor(typeof(Transform))]
public class TransformInspector : Editor
{
    public override void OnInspectorGUI()
    {
        Transform t = (Transform)target;

        if (GUILayout.Button("Reset Transforms"))
        {
            Undo.RecordObject(t, "Reset Transforms " + t.name);
            if (t.parent == null)
            {
                t.transform.position = Vector3.zero;
                t.transform.rotation = Quaternion.identity;
                t.transform.localScale = Vector3.one;
            }
            else
            {
                t.transform.localPosition = Vector3.zero;
                t.transform.localRotation = Quaternion.identity;
                t.transform.localScale = Vector3.one;
            }
        }

        if (GUILayout.Button("Reset Position"))
        {
            Undo.RecordObject(t, "Reset Position " + t.name);
            if (t.parent == null)
                t.transform.position = Vector3.zero;
            else
                t.transform.localPosition = Vector3.zero;
        }

        if (GUILayout.Button("Reset Rotation"))
        {
            Undo.RecordObject(t, "Reset Rotation " + t.name);
            if (t.parent == null)
                t.transform.rotation = Quaternion.identity;
            else
                t.transform.localRotation = Quaternion.identity;
        }

        if (GUILayout.Button("Reset Scale"))
        {
            Undo.RecordObject(t, "Reset Scale " + t.name);
            t.transform.localScale = Vector3.one;
        }

        // Replicate the standard transform inspector gui
        EditorGUIUtility.LookLikeControls();
        EditorGUI.indentLevel = 0;
        Vector3 position = EditorGUILayout.Vector3Field("Position", t.localPosition);
        Vector3 eulerAngles = EditorGUILayout.Vector3Field("Rotation", t.localEulerAngles);
        Vector3 scale = EditorGUILayout.Vector3Field("Scale", t.localScale);
        EditorGUIUtility.LookLikeControls();
        if (GUI.changed)
        {
            Undo.RecordObject(t, "Transform Change");
            t.localPosition = FixIfNaN(position);
            t.localEulerAngles = FixIfNaN(eulerAngles);
            t.localScale = FixIfNaN(scale);
        }
    }
    private Vector3 FixIfNaN(Vector3 v)
    {
        if (float.IsNaN(v.x))
            v.x = 0;
        if (float.IsNaN(v.y))
            v.y = 0;
        if (float.IsNaN(v.z))
            v.z = 0;
        return v;
    }
}