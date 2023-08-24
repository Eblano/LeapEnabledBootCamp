using System.Collections.Generic;
using UnityEngine;

public enum ObjectState
{
    ENABLED,
    DISABLED,
    ENABLING,
    DISABLING
}

public class ObjectInfo
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

public class DistanceDisable : MonoBehaviour
{
    public int objectsPerFrame = 30;
    public float minObjectDistance = 50.0f;
    public float maxObjectDistance = 100.0f;
    public List<ObjectInfo> objects;
    public Transform soldierRef;
    private int index;
    public Shader alphaShader;
    private float maxObjectDistanceNormal;

    private void Start()
    {
        alphaShader = Shader.Find("Transparent/VertexLit");

        if (soldierRef == null)
        {
            Destroy(this);
        }
        else
        {
            index = 0;

            maxObjectDistance *= maxObjectDistance;
            minObjectDistance *= minObjectDistance;
            maxObjectDistanceNormal = 1.0f / maxObjectDistance;

            objects = new List<ObjectInfo>();

            GetAllChilds(transform);
        }
    }

    private void GetAllChilds(Transform t)
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
            objects.Add(dObject);
        }

        for (int i = 0; i < t.childCount; i++)
        {
            GetAllChilds(t.GetChild(i));
        }
    }

    private void Update()
    {
        if (objects == null) return;

        Vector3 soldierPos = soldierRef.position;

        ObjectInfo cObject;

        for (int i = index; (i < objects.Count) && (i < index + objectsPerFrame); i++)
        {
            cObject = objects[i];

            if (!cObject.gameObject.activeSelf) continue;

            cObject.distance = (cObject.transform.position - soldierPos).sqrMagnitude;

            if (cObject.distance > maxObjectDistance)
            {
                if (cObject.renderer.enabled)
                {
                    cObject.renderer.enabled = false;
                }
            }
            else if (cObject.distance > minObjectDistance)
            {
                if (!cObject.renderer.enabled)
                {
                    cObject.renderer.enabled = true;
                }

                cObject.material.shader = alphaShader;
                cObject.color.a = Mathf.Clamp(1.0f - ((cObject.distance - minObjectDistance) * maxObjectDistanceNormal), 0.0f, 1.0f);
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

            cObject.lastUpdate = Time.time;
        }

        index += objectsPerFrame;

        if (index >= objects.Count) index = 0;
    }
}