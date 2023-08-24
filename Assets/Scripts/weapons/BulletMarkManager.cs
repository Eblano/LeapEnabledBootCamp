using UnityEngine;
using System.Collections;

[System.Serializable]
public class BulletMarkManager : MonoBehaviour
{
    private static BulletMarkManager instance;
    public int maxMarks;
    public object[] marks;
    public object[] pushDistances;
    public virtual void Start()
    {
        if (BulletMarkManager.instance == null)
        {
            BulletMarkManager.instance = this;
        }
    }

    public static float Add(GameObject go)
    {
        float distance = 0.0f;
        if (BulletMarkManager.instance == null)
        {
            GameObject aux = new GameObject("BulletMarkManager");
            BulletMarkManager.instance = aux.AddComponent<BulletMarkManager>() as BulletMarkManager;
            BulletMarkManager.instance.marks = new object[0];
            BulletMarkManager.instance.pushDistances = new object[0];
            BulletMarkManager.instance.maxMarks = 60;
        }
        GameObject auxGO = null;
        Transform auxT = null;
        Transform currentT = go.transform;
        Vector3 currentPos = currentT.position;
        float radius = (((currentT.localScale.x * currentT.localScale.x) * 0.25f) + ((currentT.localScale.y * currentT.localScale.y) * 0.25f)) + ((currentT.localScale.z * currentT.localScale.z) * 0.25f);
        radius = Mathf.Sqrt(radius);
        float realRadius = radius * 2;
        radius = radius * 0.2f;
        if (BulletMarkManager.instance.marks.Length == BulletMarkManager.instance.maxMarks)
        {
            auxGO = BulletMarkManager.instance.marks[0] as GameObject;
            UnityEngine.Object.Destroy(auxGO);
            BulletMarkManager.instance.marks.RemoveAt(0);
            BulletMarkManager.instance.pushDistances.RemoveAt(0);
        }
        float pushDistance = 0.0001f;
        int length = BulletMarkManager.instance.marks.Length;
        int sideMarks = 0;
        int i = 0;
        while (i < length)
        {
            auxGO = BulletMarkManager.instance.marks[i] as GameObject;
            if (auxGO != null)
            {
                auxT = auxGO.transform;
                distance = (auxT.position - currentPos).magnitude;
                if (distance < radius)
                {
                    UnityEngine.Object.Destroy(auxGO);
                    BulletMarkManager.instance.marks.RemoveAt(i);
                    BulletMarkManager.instance.pushDistances.RemoveAt(i);
                    i--;
                    length--;
                }
                else
                {
                    if (distance < realRadius)
                    {
                        float cDist = (float) BulletMarkManager.instance.pushDistances[i];
                        pushDistance = Mathf.Max(pushDistance, cDist);
                    }
                }
            }
            else
            {
                BulletMarkManager.instance.marks.RemoveAt(i);
                BulletMarkManager.instance.pushDistances.RemoveAt(i);
                i--;
                length--;
            }
            i++;
        }
        pushDistance = pushDistance + 0.0005f;
        BulletMarkManager.instance.marks.Add(go);
        BulletMarkManager.instance.pushDistances.Add(pushDistance);
        return pushDistance;
    }

    public static void ClearMarks()
    {
        GameObject go = null;
        if (BulletMarkManager.instance.marks.Length > 0)
        {
            int i = 0;
            while (i < BulletMarkManager.instance.marks.Length)
            {
                go = BulletMarkManager.instance.marks[i] as GameObject;
                UnityEngine.Object.Destroy(go);
                i++;
            }
            BulletMarkManager.instance.marks.Clear();
        }
    }

}