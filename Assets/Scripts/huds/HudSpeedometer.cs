using UnityEngine;
using System.Collections;

[System.Serializable]
public class HudSpeedometer : MonoBehaviour
{
    public float currentSpeed;
    public int currentGear;
    public float currentRPM;
    public float minSpeed;
    public float maxSpeed;
    public float minSpeedAngle;
    public float maxSpeedAngle;
    public Texture pointer;
    public GUIText rpmText;
    public GUIText gearText;
    private float targetAngle;
    private float _maxSpeed;
    private Vector2 pointerPos;
    private Vector2 pivot;
    public virtual void Start()
    {
        this._maxSpeed = 1f / this.maxSpeed;
    }

    public virtual void Update()
    {
        this.currentSpeed = Mathf.Clamp(this.currentSpeed, this.minSpeed, this.maxSpeed);
        this.targetAngle = Mathf.Lerp(this.minSpeedAngle, this.maxSpeedAngle, (this.currentSpeed - this.minSpeed) * this._maxSpeed);
        this.rpmText.text = UnityScript.Lang.UnityBuiltins.parseInt(this.currentRPM).ToString();
        if (this.currentGear > 0)
        {
            this.gearText.text = this.currentGear.ToString();
        }
        else
        {
            if (this.currentGear == 0)
            {
                this.gearText.text = "R";
            }
            else
            {
                if (this.currentGear < 0)
                {
                    this.gearText.text = "N";
                }
            }
        }
    }

    public virtual void OnGUI()
    {
        this.pointerPos = new Vector2(Screen.width - 110, Screen.height - 84);
        GUIUtility.RotateAroundPivot(this.targetAngle, this.pointerPos + this.pivot);
        GUI.DrawTexture(new Rect(this.pointerPos.x, this.pointerPos.y, 103, 34), this.pointer);
    }

    public HudSpeedometer()
    {
        this.pivot = new Vector2(17, 17);
    }

}