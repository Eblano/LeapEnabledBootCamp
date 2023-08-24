using UnityEngine;
using System.Collections;

[System.Serializable]
[UnityEngine.RequireComponent(typeof(CharacterController))]
public partial class FPSWalker : MonoBehaviour
{
    public float speed;
    public Transform cam;
    private CharacterController controller;
    public virtual void Start()
    {
        this.controller = (CharacterController) this.GetComponent(typeof(CharacterController));
    }

    public virtual void Update()
    {
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection = this.cam.TransformDirection(moveDirection);
        moveDirection = moveDirection * this.speed;
        this.controller.Move(moveDirection * Time.deltaTime);
    }

    public FPSWalker()
    {
        this.speed = 6f;
    }

}