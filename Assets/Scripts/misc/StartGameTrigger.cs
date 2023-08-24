using UnityEngine;
using System.Collections;

[System.Serializable]
public class StartGameTrigger : MonoBehaviour
{
    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.name.ToLower() == "soldier")
        {
            GameManager.running = true;
            UnityEngine.Object.Destroy(this);
        }
    }

}