using UnityEngine;
using System.Collections;

public enum HitType
{
    CONCRETE = 0,
    WOOD = 1,
    METAL = 2,
    OLD_METAL = 3,
    GLASS = 4,
    GENERIC = 5
}

[System.Serializable]
public class BulletMarks : MonoBehaviour
{
    public Texture2D[] concrete;
    public Texture2D[] wood;
    public Texture2D[] metal;
    public Texture2D[] oldMetal;
    public Texture2D[] glass;
    public Texture2D[] generic;
    public virtual void GenerateDecal(HitType type, GameObject go)
    {
        int random = 0;
        Texture2D useTexture = null;
        switch (type)
        {
            case HitType.CONCRETE:
                if (this.concrete == null)
                {
                    return;
                }
                if (this.concrete.Length == 0)
                {
                    return;
                }
                random = Random.Range(0, this.concrete.Length);
                useTexture = this.concrete[random];
                break;
            case HitType.WOOD:
                if (this.wood == null)
                {
                    return;
                }
                if (this.wood.Length == 0)
                {
                    return;
                }
                random = Random.Range(0, this.wood.Length);
                useTexture = this.wood[random];
                break;
            case HitType.METAL:
                if (this.metal == null)
                {
                    return;
                }
                if (this.metal.Length == 0)
                {
                    return;
                }
                random = Random.Range(0, this.metal.Length);
                useTexture = this.metal[random];
                break;
            case HitType.OLD_METAL:
                if (this.oldMetal == null)
                {
                    return;
                }
                if (this.oldMetal.Length == 0)
                {
                    return;
                }
                random = Random.Range(0, this.oldMetal.Length);
                useTexture = this.oldMetal[random];
                break;
            case HitType.GLASS:
                if (this.glass == null)
                {
                    return;
                }
                if (this.glass.Length == 0)
                {
                    return;
                }
                random = Random.Range(0, this.glass.Length);
                useTexture = this.glass[random];
                break;
            case HitType.GENERIC:
                if (this.generic == null)
                {
                    return;
                }
                if (this.generic.Length == 0)
                {
                    return;
                }
                random = Random.Range(0, this.generic.Length);
                useTexture = this.generic[random];
                break;
            default:
                if (this.wood == null)
                {
                    return;
                }
                if (this.wood.Length == 0)
                {
                    return;
                }
                random = Random.Range(0, this.wood.Length);
                useTexture = this.wood[random];
                return;
                break;
        }
        this.transform.Rotate(new Vector3(0, 0, Random.Range(-180f, 180f)));
        Decal.dCount++;
        Decal d = (Decal) this.gameObject.GetComponent("Decal");
        d.affectedObjects = new GameObject[1];
        d.affectedObjects[0] = go;
        d.decalMode = DecalMode.MESH_COLLIDER;
        d.pushDistance = 0.009f + BulletMarkManager.Add(this.gameObject);
        Material m = new Material(d.decalMaterial);
        m.mainTexture = useTexture;
        d.decalMaterial = m;
        d.CalculateDecal();
        d.transform.parent = go.transform;
    }

}