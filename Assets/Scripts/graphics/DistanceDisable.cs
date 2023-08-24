using UnityEngine;
using System.Collections;

public enum ObjectState
{
    ENABLED = 0,
    DISABLED = 1,
    ENABLING = 2,
    DISABLING = 3
}

[System.Serializable]
public class ObjectInfo : object
{
    public Transform transform;
    public Renderer renderer;
    public Material material;
    public Shader baseShader;
    public ObjectState state;
    public Color color;
    public float lastUpdate;
    public float distance;
    public Color oColor;
    public GameObject gameObject;
}
[System.Serializable]
public class DistanceDisable : MonoBehaviour
{
    public int objectsPerFrame;
    public float minObjectDistance;
    public float maxObjectDistance;
    public object[] objects;
    public Transform soldierRef;
    private int index;
    public Shader alphaShader;
    private float maxObjectDistanceNormal;
    public virtual void Start()
    {
        this.alphaShader = Shader.Find("Transparent/VertexLit");
        if (this.soldierRef == null)
        {
            UnityEngine.Object.Destroy(this);
        }
        else
        {
            this.index = 0;
            this.maxObjectDistance = this.maxObjectDistance * this.maxObjectDistance;
            this.minObjectDistance = this.minObjectDistance * this.minObjectDistance;
            this.maxObjectDistanceNormal = 1f / this.maxObjectDistance;
            this.objects = new object[0];
            this.GetAllChilds(this.transform);
        }
    }

    public virtual void GetAllChilds(Transform t)
    {
        GameObject auxGO = t.gameObject;
        if (auxGO.GetComponent<Renderer>() != null)
        {
            ObjectInfo dObject = new ObjectInfo();
            dObject.transform = t;
            dObject.renderer = auxGO.GetComponent<Renderer>();
            dObject.state = ObjectState.ENABLED;
            dObject.material = dObject.renderer.material;
            dObject.baseShader = dObject.material.shader;
            dObject.color = dObject.material.color;
            dObject.oColor = dObject.material.color;
            dObject.gameObject = auxGO;
            this.objects.Add(dObject);
        }
        int i = 0;
        while (i < t.childCount)
        {
            this.GetAllChilds(t.GetChild(i));
            i++;
        }
    }

    public virtual void Update()
    {
        if (this.objects == null)
        {
            return;
        }
        Vector3 soldierPos = this.soldierRef.position;
        ObjectInfo cObject = null;
        int i = this.index;
        while ((i < this.objects.Length) && (i < (this.index + this.objectsPerFrame)))
        {
            cObject = (ObjectInfo) this.objects[i];
            if (!cObject.gameObject.active)
            {
                goto Label_for_20;
            }
            cObject.distance = (cObject.transform.position - soldierPos).sqrMagnitude;
            if (cObject.distance > this.maxObjectDistance)
            {
                if (cObject.renderer.enabled)
                {
                    cObject.renderer.enabled = false;
                }
            }
            else
            {
                if (cObject.distance > this.minObjectDistance)
                {
                    if (!cObject.renderer.enabled)
                    {
                        cObject.renderer.enabled = true;
                    }
                    cObject.material.shader = this.alphaShader;
                    cObject.color.a = Mathf.Clamp(1f - ((cObject.distance - this.minObjectDistance) * this.maxObjectDistanceNormal), 0f, 1f);
                    cObject.material.color = cObject.color;
                }
                else
                {
                    if (!cObject.renderer.enabled)
                    {
                        cObject.renderer.enabled = true;
                    }
                    cObject.material.shader = cObject.baseShader;
                    cObject.material.color = cObject.oColor;
                }
            }
            cObject.lastUpdate = Time.time;
            Label_for_20:
            i++;
        }
        this.index = this.index + this.objectsPerFrame;
        if (this.index >= this.objects.Length)
        {
            this.index = 0;
        }
    }

    public DistanceDisable()
    {
        this.objectsPerFrame = 30;
        this.minObjectDistance = 50f;
        this.maxObjectDistance = 100f;
    }

}