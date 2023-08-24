using UnityEngine;
using System.Collections;

[System.Serializable]
public class DistanceFade : MonoBehaviour
{
    public bool getDisabled;
    public int objectsToProcessPerFrame;
    public float minDistance;
    public float maxDistance;
    public Transform soldierRef;
    public Shader fadeShader;
    public DistanceFadeObject[] objects;
    private int currentObject;
    private int totalCount;
    private object[] fadingArray;
    private int count;
    private Vector3 sPos;
    private Transform cTransform;
    private float distance;
    private DistanceFadeObject obj;
    public virtual void Start()
    {
        if (this.objects == null)
        {
            UnityEngine.Object.Destroy(this);
            return;
        }
        if (this.objects.Length <= 0)
        {
            UnityEngine.Object.Destroy(this);
            return;
        }
        this.fadingArray = new object[0];
        this.objectsToProcessPerFrame = Mathf.Min(this.objectsToProcessPerFrame, this.objects.Length);
        if (this.objectsToProcessPerFrame == 0)
        {
            this.objectsToProcessPerFrame = this.objects.Length;
        }
        this.totalCount = this.objects.Length;
        this.currentObject = 0;
    }

    /*
		var length : int = fadingArray.length;
		var deltaTime : float = Time.deltaTime;
		for(var i : int = 0; i < length; i++)
		{
			obj = fadingArray[i] as DistanceFadeObject;
			
			if(obj.DoFade(deltaTime))
			{
				fadingArray.RemoveAt(i);
				i--;
				length--;
			}
		}
		//*/    public virtual void Update()
    {
        if (!this.soldierRef.gameObject.active)
        {
            return;
        }
        if (this.maxDistance <= 0f)
        {
            return;
        }
        this.count = 0;
        this.sPos = this.soldierRef.position;
        while (this.count < this.objectsToProcessPerFrame)
        {
            this.count++;
            this.obj = this.objects[this.currentObject];
            if (!(this.obj == null))
            {
                this.cTransform = this.obj.transform;
                if (this.cTransform != null)
                {
                    this.distance = (this.cTransform.position - this.sPos).magnitude;
                    /*
					if(distance > maxDistance && obj.enabled)
					{
						obj.Disable();
					} 
					else if(distance <= maxDistance && !obj.enabled)
					{
						obj.StartFade();
						fadingArray.Add(obj);
					}
					//*/
                    this.obj.DistanceBased(this.distance);
                }
            }
            /*
				else
				{
					//objects[currentObject] = null;
				}
				//*/
            this.NextObject();
        }
    }

    public virtual void NextObject()
    {
        this.currentObject++;
        if (this.currentObject >= this.totalCount)
        {
            this.currentObject = 0;
        }
    }

    public virtual void SetMaxDistance(float d)
    {
        this.maxDistance = d;
        this.minDistance = d - 4f;
        int i = 0;
        while (i < this.totalCount)
        {
            this.objects[i].SetMaxDistance(d);
            i++;
        }
    }

    public DistanceFade()
    {
        this.objectsToProcessPerFrame = 30;
    }

}