using UnityEngine;
using System.Collections;

[System.Serializable]
public class SpeakOnTrigger : MonoBehaviour
{
    public TriggerInstruction[] instructions;
    private bool playing;
    private float timer;
    private int instructionsToPlay;
    public virtual void Start()
    {
        this.playing = false;
        this.gameObject.layer = 2;
        this.instructionsToPlay = this.instructions.Length;
    }

    public virtual void Update()
    {
        if (this.playing && (this.instructionsToPlay > 0))
        {
            this.timer = this.timer + Time.deltaTime;
            int i = 0;
            while (i < this.instructions.Length)
            {
                if (!this.instructions[i].playing)
                {
                    if (this.instructions[i].instructionDelay < this.timer)
                    {
                        this.instructions[i].playing = true;
                        this.instructionsToPlay--;
                        this.SendMessageUpwards("ShowInstruction", this.instructions[i].instructionName, SendMessageOptions.DontRequireReceiver);
                    }
                }
                i++;
            }
        }
        else
        {
            if (this.instructionsToPlay <= 0)
            {
                UnityEngine.Object.Destroy(this.gameObject);
            }
        }
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (!this.playing)
        {
            if (other.name.ToLower() == "soldier")
            {
                this.playing = true;
                this.timer = 0f;
            }
        }
    }

}
[System.Serializable]
public class TriggerInstruction : object
{
    public string instructionName;
    public float instructionDelay;
    [UnityEngine.HideInInspector]
    public bool playing;
}