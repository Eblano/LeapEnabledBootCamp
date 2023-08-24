using UnityEngine;
using System.Collections;

[System.Serializable]
public class ForestCutscenePlayer : MonoBehaviour
{
    public ForestCutscene forestCutscene;
    public virtual void OnTriggerEnter(Collider other)
    {
        this.PlayScene(other);
    }

    public virtual void OnTriggerStay(Collider other)
    {
        this.PlayScene(other);
    }

    public virtual void OnTriggerExit(Collider other)
    {
        this.PlayScene(other);
    }

    public virtual void PlayScene(Collider other)
    {
        if (other.name.ToLower() == "soldier")
        {
            this.forestCutscene.Play();
            UnityEngine.Object.Destroy(this);
        }
    }

}