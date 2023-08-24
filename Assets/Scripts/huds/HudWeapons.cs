using UnityEngine;
using System.Collections;

[System.Serializable]
public class HudWeapons : MonoBehaviour
{
    public Texture2D[] weapon;
    public Texture2D[] ammunition;
    public Texture2D[] ammunitionBackground;
    public int selectedWeapon;
    public int[] maxAmmo;
    public int[] ammoRemaining;
    public int[] maxIcons;
    public int[] clipsRemaining;
    private Vector2 startCorner;
    private Rect[] weaponRect;
    private Rect[] ammunitionRect;
    public GUIStyle totalAmmoStyle;
    public GUIStyle ammoRemainingStyle;
    private float alphaWeapon;
    private float alphaAmmo;
    private Color auxColor;
    private Color cColor;
    private int state;
    private int currentWeapon;
    private int currentAmmo;
    private float hideTime;
    public float fadeTime;
    public float showTime;
    public virtual void Start()
    {
        int i = 0;
        this.fadeTime = 1f / this.fadeTime;
        this.state = 0;
        this.alphaWeapon = 0f;
        this.alphaAmmo = 0f;
        this.currentWeapon = this.selectedWeapon;
        this.currentAmmo = this.ammoRemaining[this.selectedWeapon];
        this.weaponRect = new Rect[this.weapon.Length];
        i = 0;
        while (i < this.weaponRect.Length)
        {
            this.weaponRect[i] = new Rect(0, 0, this.weapon[i].width, this.weapon[i].height);
            i++;
        }
        this.ammunitionRect = new Rect[this.ammunitionBackground.Length];
        i = 0;
        while (i < this.ammunitionBackground.Length)
        {
            this.ammunitionRect[i] = new Rect(0, 0, this.ammunitionBackground[i].width, this.ammunitionBackground[i].height);
            i++;
        }
    }

    public virtual void DrawGUI(Event @event)
    {
        if (this.alphaAmmo <= 0f)
        {
            return;
        }
        this.auxColor = this.cColor = GUI.color;
        this.startCorner = new Vector2(Screen.width, Screen.height) - new Vector2(5, 5);
        this.selectedWeapon = Mathf.Clamp(this.selectedWeapon, 0, 1);
        this.ShowAmmunition();
        this.ShowSelectedWeapon();
        GUI.color = this.cColor;
    }

    public virtual void Update()
    {
        this.selectedWeapon = Mathf.Clamp(this.selectedWeapon, 0, 1);
        switch (this.state)
        {
            case 0:
                if (this.alphaAmmo > 0f)
                {
                    this.alphaAmmo = this.alphaAmmo - (Time.deltaTime * this.fadeTime);
                }
                if (this.alphaWeapon > 0f)
                {
                    this.alphaWeapon = this.alphaWeapon - (Time.deltaTime * this.fadeTime);
                }
                break;
            case 1:
                this.alphaAmmo = 0f;
                if (this.alphaWeapon < 1f)
                {
                    this.alphaWeapon = this.alphaWeapon + (Time.deltaTime * this.fadeTime);
                }
                break;
            case 2:
                this.alphaWeapon = 0f;
                if (this.alphaAmmo < 1f)
                {
                    this.alphaAmmo = this.alphaAmmo + (Time.deltaTime * this.fadeTime);
                }
                break;
        }
        if (this.selectedWeapon != this.currentWeapon)
        {
            this.currentWeapon = this.selectedWeapon;
            this.currentAmmo = this.ammoRemaining[this.selectedWeapon];
            this.state = 1;
            this.hideTime = this.showTime + ((1f - this.alphaWeapon) * (1 / this.fadeTime));
        }
        else
        {
            if (this.currentAmmo != this.ammoRemaining[this.selectedWeapon])
            {
                this.currentAmmo = this.ammoRemaining[this.selectedWeapon];
                this.state = 2;
                this.hideTime = this.showTime + ((1f - this.alphaAmmo) * (1 / this.fadeTime));
            }
            else
            {
                if (this.hideTime > 0f)
                {
                    this.hideTime = this.hideTime - Time.deltaTime;
                    if (this.hideTime <= 0f)
                    {
                        this.state = 0;
                    }
                }
            }
        }
    }

    public virtual void ShowAmmunition()
    {
        this.auxColor.a = this.alphaAmmo;
        GUI.color = this.auxColor;
        this.ammunitionRect[this.selectedWeapon].x = this.startCorner.x - this.ammunitionRect[this.selectedWeapon].width;
        this.ammunitionRect[this.selectedWeapon].y = this.startCorner.y - this.ammunitionRect[this.selectedWeapon].height;
        GUI.DrawTexture(this.ammunitionRect[this.selectedWeapon], this.ammunitionBackground[this.selectedWeapon]);
        float delta = Mathf.Clamp(this.ammoRemaining[this.selectedWeapon], 0, this.maxAmmo[this.selectedWeapon]);
        delta = delta / this.maxAmmo[this.selectedWeapon];
        delta = delta * this.maxIcons[this.selectedWeapon];
        int length = (int) delta;
        int i = 0;
        while (i < length)
        {
            GUI.DrawTexture(new Rect((this.ammunitionRect[this.selectedWeapon].x + 40) + (i * (this.ammunition[this.selectedWeapon].width - 1)), this.ammunitionRect[this.selectedWeapon].y + 28, this.ammunition[this.selectedWeapon].width, this.ammunition[this.selectedWeapon].height), this.ammunition[this.selectedWeapon]);
            i++;
        }
        Rect auxRect = new Rect(this.ammunitionRect[this.selectedWeapon].x + 40, this.ammunitionRect[this.selectedWeapon].y + 2, 20, 20);
        GUI.Label(auxRect, this.clipsRemaining[this.selectedWeapon].ToString(), this.totalAmmoStyle);
        auxRect.x = auxRect.x + 17;
        auxRect.y = auxRect.y - 1;
        GUI.Label(auxRect, "|", this.totalAmmoStyle);
        auxRect.x = auxRect.x + 6;
        auxRect.y = auxRect.y + 4;
        GUI.Label(auxRect, this.ammoRemaining[this.selectedWeapon].ToString(), this.ammoRemainingStyle);
    }

    public virtual void ShowSelectedWeapon()
    {
        this.auxColor.a = this.alphaWeapon;
        GUI.color = this.auxColor;
        this.weaponRect[this.selectedWeapon].x = this.startCorner.x - this.weaponRect[this.selectedWeapon].width;
        this.weaponRect[this.selectedWeapon].y = this.startCorner.y - this.weaponRect[this.selectedWeapon].height;
        GUI.DrawTexture(this.weaponRect[this.selectedWeapon], this.weapon[this.selectedWeapon]);
    }

    public HudWeapons()
    {
        this.fadeTime = 0.2f;
        this.showTime = 2f;
    }

}