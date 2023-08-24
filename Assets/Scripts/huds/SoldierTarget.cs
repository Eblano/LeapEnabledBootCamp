using UnityEngine;
using System.Collections;

[System.Serializable]
public class SoldierTarget : MonoBehaviour
{
    public Texture2D target;
    public Texture2D targetOver;
    public bool overEnemy;
    private bool _overEnemy;
    private GUITexture gui;
    public bool aim;
    private bool _aim;
    public LayerMask enemyLayer;
    public LayerMask otherLayer;
    public float enemyDistance;
    public Camera soldierCam;
    public Transform soldierTarget;
    public SoldierController soldierController;
    public SoldierCamera soldierCamera;
    public virtual void OnEnable()
    {
        this.soldierTarget.parent = null;
        this.gui = this.GetComponent<GUITexture>();
        this.gui.pixelInset = new Rect(-this.target.width * 0.5f, -this.target.height * 0.5f, this.target.width, this.target.height);
        this.gui.texture = this.target;
        this.gui.color = new Color(0.5f, 0.5f, 0.5f, 0.15f);
    }

    public virtual void Update()
    {
        RaycastHit hit1 = default(RaycastHit);
        RaycastHit hit2 = default(RaycastHit);
        if (!this.soldierCam.gameObject.active)
        {
            this.gui.color = new Color(0.5f, 0.5f, 0.5f, 0f);
            return;
        }
        this.aim = Input.GetButton("Fire2");
        Ray ray = this.soldierCam.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
        this.overEnemy = Physics.Raycast(ray.origin, ray.direction, out hit1, this.enemyDistance, (int) this.enemyLayer);
        if (this.overEnemy)
        {
            if (Physics.Raycast(ray.origin, ray.direction, out hit2, this.enemyDistance, (int) this.otherLayer))
            {
                this.overEnemy = hit1.distance < hit2.distance;
            }
        }
        float delta = 1f - ((this.soldierCamera.y + 85) * 0.0058823529f);
        if (!this.soldierController.crouch)
        {
            if (this.soldierController.aim)
            {
                this.soldierTarget.position = this.soldierCam.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, Screen.height * (0.3f + (delta * 0.24f)), 10));
            }
            else
            {
                this.soldierTarget.position = this.soldierCam.ScreenToWorldPoint(new Vector3(Screen.width * 0.6f, Screen.height * (0.4f + (delta * 0.16f)), 10));
            }
        }
        else
        {
            if (this.soldierController.aim)
            {
                this.soldierTarget.position = this.soldierCam.ScreenToWorldPoint(new Vector3(Screen.width * 0.7f, Screen.height * (0.3f + (delta * 0.24f)), 10));
            }
            else
            {
                this.soldierTarget.position = this.soldierCam.ScreenToWorldPoint(new Vector3(Screen.width * 0.7f, Screen.height * (0.4f + (delta * 0.16f)), 10));
            }
        }
        if (this.overEnemy != this._overEnemy)
        {
            this._overEnemy = this.overEnemy;
            if (this.overEnemy)
            {
                this.gui.texture = this.targetOver;
            }
            else
            {
                this.gui.texture = this.target;
            }
        }
        if (this.aim != this._aim)
        {
            this._aim = this.aim;
            if (this.aim)
            {
                this.gui.color = new Color(0.5f, 0.5f, 0.5f, 0.75f);
            }
            else
            {
                this.gui.color = new Color(0.5f, 0.5f, 0.5f, 0.15f);
            }
        }
    }

    public SoldierTarget()
    {
        this.enemyDistance = 50f;
    }

}