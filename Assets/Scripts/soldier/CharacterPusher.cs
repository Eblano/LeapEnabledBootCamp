using UnityEngine;
using System.Collections;

[System.Serializable]
public class CharacterPusher : MonoBehaviour
{
    public float pushPower;
    public virtual void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if ((body == null) || body.isKinematic)
        {
            return;
        }
        if (hit.moveDirection.y < -0.3f)
        {
            return;
        }
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        body.velocity = pushDir * this.pushPower;
    }

    public CharacterPusher()
    {
        this.pushPower = 2f;
    }

}