using UnityEngine;
using System.Collections;

[System.Serializable]
public class SargeInstruction : object
{
    public string name;
    public string text;
    public Texture2D texture;
    public float timeToDisplay;
    public AudioClip audio;
    public bool queuable;
    public bool overridable;
    public float volume;
    public SargeInstruction()
    {
        this.timeToDisplay = 3f;
        this.queuable = true;
        this.volume = 1f;
    }

}