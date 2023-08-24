using UnityEngine;
using UnityEditor;
using System.Collections;

[System.Serializable]
[UnityEditor.CustomEditor(typeof(SargeManager))]
public class SargeHelper : Editor
{
    public override void OnInspectorGUI()
    {
        SargeManager t = this.target as SargeManager;
        this.DrawDefaultInspector();
        if (!EditorApplication.isPlaying)
        {
            return;
        }
        if (!(t.instructions == null))
        {
            t.debug = EditorGUILayout.Foldout(t.debug, "Debug voices");
            if (t.debug)
            {
                int i = 0;
                while (i < t.instructions.Length)
                {
                    string inst = t.instructions[i].name;
                    if (GUILayout.Button(inst, new GUILayoutOption[] {}))
                    {
                        t.gameObject.SendMessage("ShowInstruction", inst);
                    }
                    i++;
                }
            }
        }
    }

}