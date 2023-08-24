using UnityEngine;
using System.Collections;

[System.Serializable]
public class CutsceneManager : MonoBehaviour
{
    public HelicopterCutscene heliCutscene;
    public ForestCutscene forestCutscene;
    public virtual void Awake()
    {
        if (this.forestCutscene != null)
        {
            this.forestCutscene.gameObject.SetActiveRecursively(true);
        }
    }

    public virtual void PlayHeli(int step)
    {
        if (this.heliCutscene == null)
        {
            return;
        }
        this.heliCutscene.Play(step);
    }

    public virtual void PlayForest()
    {
        if (this.forestCutscene == null)
        {
            return;
        }
        this.forestCutscene.Play();
    }

    public virtual void HeliCutsceneEnd(int step)
    {
        switch (step)
        {
            case 0:
                this.PlayHeli(1);
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
        }
    }

    public virtual void Disable(string cutsceneName)
    {
        switch (cutsceneName)
        {
            case "Helicopter":
                this.heliCutscene.DestroyScene();
        }
    }

}