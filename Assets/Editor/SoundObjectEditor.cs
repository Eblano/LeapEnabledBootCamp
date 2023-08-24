using UnityEngine;
using UnityEditor;
using System.Collections;

[System.Serializable]
[UnityEditor.CustomEditor(typeof(SoundObject))]
public class SoundObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SoundObjectManager[] managers = ((SoundObjectManager[]) UnityEngine.Object.FindObjectsOfType(typeof(SoundObjectManager))) as SoundObjectManager[];
        if (!(managers == null))
        {
            if (managers.Length > 0)
            {
                SoundObjectManager m = managers[0];
                if (!(m.additionalSounds == null))
                {
                    string[] optionNames = new string[m.additionalSounds.Length + 1];
                    int[] optionValues = new int[m.additionalSounds.Length + 1];
                    optionNames[0] = "USE TAG";
                    optionValues[0] = -1;
                    int i = 0;
                    while (i < m.additionalSounds.Length)
                    {
                        optionNames[i + 1] = m.additionalSounds[i].name;
                        optionValues[i + 1] = i;
                        i++;
                    }
                    SoundObject so = this.target as SoundObject;
                    so.overrideSound = EditorGUILayout.IntPopup(so.overrideSound, optionNames, optionValues, new GUILayoutOption[] {});
                    this.DrawDefaultInspector();
                }
            }
        }
    }

}