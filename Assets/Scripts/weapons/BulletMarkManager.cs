using System.Collections.Generic;
using UnityEngine;

public class BulletMarkManager : MonoBehaviour
{
    private static BulletMarkManager instance;

    public int maxMarks;
    public List<GameObject> marks;
    public List<float> pushDistances;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public static float Add(GameObject go)
    {
        if (instance == null)
        {
            GameObject aux = new GameObject("BulletMarkManager");
            instance = aux.AddComponent<BulletMarkManager>();
            instance.marks = new List<GameObject>();
            instance.pushDistances = new List<float>();
            instance.maxMarks = 60;
        }

        GameObject auxGO;
        Transform auxT;
        Transform currentT = go.transform;
        Vector3 currentPos = currentT.position;
        float radius = (currentT.localScale.x * currentT.localScale.x * 0.25f) + (currentT.localScale.y * currentT.localScale.y * 0.25f) + (currentT.localScale.z * currentT.localScale.z * 0.25f);
        radius = Mathf.Sqrt(radius);
        float realRadius = radius * 2;
        radius *= 0.2f;

        float distance;

        if (instance.marks.Count == instance.maxMarks)
        {
            auxGO = instance.marks[0];
            Destroy(auxGO);
            instance.marks.RemoveAt(0);
            instance.pushDistances.RemoveAt(0);
        }

        float pushDistance = 0.0001f;
        int length = instance.marks.Count;
        int sideMarks = 0;
        for (int i = 0; i < length; i++)
        {
            auxGO = instance.marks[i];

            if (auxGO != null)
            {
                auxT = auxGO.transform;
                distance = (auxT.position - currentPos).magnitude;
                if (distance < radius)
                {
                    Destroy(auxGO);
                    instance.marks.RemoveAt(i);
                    instance.pushDistances.RemoveAt(i);
                    i--;
                    length--;
                }
                else if (distance < realRadius)
                {
                    float cDist = instance.pushDistances[i];

                    pushDistance = Mathf.Max(pushDistance, cDist);
                }
            }
            else
            {
                instance.marks.RemoveAt(i);
                instance.pushDistances.RemoveAt(i);
                i--;
                length--;
            }
        }
        pushDistance += 0.0005f;

        instance.marks.Add(go);
        instance.pushDistances.Add(pushDistance);

        return pushDistance;
    }

    public static void ClearMarks()
    {
        GameObject go;

        if (instance.marks.Count > 0)
        {
            for (int i = 0; i < instance.marks.Count; i++)
            {
                go = instance.marks[i];

                Destroy(go);
            }

            instance.marks.Clear();
        }
    }
}