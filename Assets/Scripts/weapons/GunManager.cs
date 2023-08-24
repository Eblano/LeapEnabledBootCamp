using UnityEngine;
using System.Collections;

[System.Serializable]
public class GunManager : MonoBehaviour
{
    public GunKeyBinder[] guns;
    [UnityEngine.HideInInspector]
    public Gun currentGun;
    [UnityEngine.HideInInspector]
    public int currentWeapon;
    public SoldierController soldier;
    public HudWeapons hud;
    public virtual void Start()
    {
        int i = 0;
        while (i < this.guns.Length)
        {
            this.guns[i].gun.enabled = false;
            i++;
        }
        this.currentWeapon = 0;
        this.guns[0].gun.enabled = true;
        this.currentGun = this.guns[0].gun;
    }

    public virtual void Update()
    {
        int i = 0;
        while (i < this.guns.Length)
        {
            if (Input.GetKeyDown(this.guns[i].keyToActivate))
            {
                this.ChangeToGun(i);
            }
            i++;
        }
        this.hud.selectedWeapon = this.currentWeapon;
        this.hud.ammoRemaining[this.currentWeapon] = this.guns[this.currentWeapon].gun.currentRounds;
    }

    public virtual void ChangeToGun(int gunIndex)
    {
        Gun cGun = this.guns[gunIndex].gun;
        if (cGun.enabled)
        {
            if (this.guns[gunIndex].switchModesOnKey)
            {
                switch (cGun.fireMode)
                {
                    case FireMode.SEMI_AUTO:
                        cGun.fireMode = FireMode.FULL_AUTO;
                        break;
                    case FireMode.FULL_AUTO:
                        cGun.fireMode = FireMode.BURST;
                        break;
                    case FireMode.BURST:
                        cGun.fireMode = FireMode.SEMI_AUTO;
                        break;
                }
            }
        }
        else
        {
            int j = 0;
            while (j < this.guns.Length)
            {
                this.guns[j].gun.enabled = false;
                j++;
            }
            cGun.enabled = true;
            this.currentGun = cGun;
            this.currentWeapon = gunIndex;
        }
    }

}