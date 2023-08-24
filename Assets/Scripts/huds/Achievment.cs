using UnityEngine;
using System.Collections;

[System.Serializable]
public class Achievment : object
{
    public string name;
    public bool enabled;
    public bool done;
    public Texture2D icon;
    public string description;
    public int progress;
    public int maxProgress;
    public bool showProgress;
}