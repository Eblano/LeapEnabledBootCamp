using UnityEngine;
using System.Collections;

[System.Serializable]
public class EndGameTrigger : MonoBehaviour
{
    public AchievmentScreen achievments;
    public SargeManager sarge;
    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.name.ToLower() == "soldier")
        {
            this.achievments.visible = true;
            GameManager.scores = true;
            GameManager.running = false;
            this.sarge.ShowInstruction("good_work");
            UnityEngine.Object.Destroy(this);
        }
    }

}