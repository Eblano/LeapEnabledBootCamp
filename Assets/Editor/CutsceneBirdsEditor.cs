using UnityEngine;
using UnityEditor;
using System.Collections;

[System.Serializable]
[UnityEditor.CustomEditor(typeof(CutsceneBirds))]
public class CutsceneBirdsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        this.DrawDefaultInspector();
        if (GUILayout.Button("SET BIRDS PROPERTIES", new GUILayoutOption[] {}))
        {
            this.SetupBirds(this.target as CutsceneBirds);
        }
    }

    public virtual void SetupBirds(CutsceneBirds cBirds)
    {
        GameObject go = cBirds.gameObject;
        Transform birdGroup = null;
        Seagull bird = null;
        int j = 0;
        while (j < go.transform.childCount)
        {
            birdGroup = go.transform.GetChild(j);
            int i = 0;
            while (i < birdGroup.childCount)
            {
                bird = (Seagull) birdGroup.GetChild(i).gameObject.GetComponent("Seagull");
                if (bird == null)
                {
                    goto Label_for_2;
                }
                bird.sounds = cBirds.sounds;
                bird.soundFrequency = cBirds.soundFrequency;
                bird.minSpeed = cBirds.minSpeed;
                bird.turnSpeed = cBirds.turnSpeed;
                bird.randomFreq = cBirds.randomFreq;
                bird.randomForce = cBirds.randomForce;
                bird.toOriginForce = cBirds.toOriginForce;
                bird.toOriginRange = cBirds.toOriginRange;
                bird.damping = cBirds.damping;
                bird.gravity = cBirds.gravity;
                bird.avoidanceRadius = cBirds.avoidanceRadius;
                bird.avoidanceForce = cBirds.avoidanceForce;
                bird.followVelocity = cBirds.followVelocity;
                bird.followRadius = cBirds.followRadius;
                bird.bankTurn = cBirds.bankTurn;
                bird.raycast = cBirds.raycast;
                bird.bounce = cBirds.bounce;
                bird.animationSpeed = cBirds.animationSpeed;
                Label_for_2:
                i++;
            }
            j++;
        }
    }

}