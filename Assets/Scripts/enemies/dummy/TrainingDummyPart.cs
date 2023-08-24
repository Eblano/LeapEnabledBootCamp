using UnityEngine;
using System.Collections;

[System.Serializable]
public class TrainingDummyPart : object
{
    [UnityEngine.HideInInspector]
    public string name;
    public GameObject gameObject;
    //public var boxCollider : BoxCollider;
    public GameObject[] siblings;
    //public var siblingParts : TrainingDummyPartHelper[];
    public GameObject[] brokeParts;
    public int shootsTaken;
    public DummyPart dummyPart;
    public virtual void Start()
    {
        this.shootsTaken = 0;
    }

}