using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class GunKeyBinder : object
{
    public Gun gun;
    public KeyCode keyToActivate;
    public bool switchModesOnKey;
}