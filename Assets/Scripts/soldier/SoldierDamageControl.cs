using UnityEngine;
using System.Collections;

[System.Serializable]
public class SoldierDamageControl : MonoBehaviour
{
    public float life;
    public Texture2D hitTexture;
    public Texture2D blackTexture;
    private float hitAlpha;
    private float blackAlpha;
    private float recoverTime;
    public AudioClip[] hitSounds;
    public AudioClip dyingSound;
    public virtual void Start()
    {
        SoldierController.dead = false;
        this.hitAlpha = 0f;
        this.blackAlpha = 0f;
        this.life = 1f;
    }

    public virtual void HitSoldier(string hit)
    {
        if (GameManager.receiveDamage)
        {
            this.life = this.life - 0.05f;
            if (!this.GetComponent<AudioSource>().isPlaying)
            {
                if ((this.life < 0.5f) && (Random.Range(0, 100) < 30))
                {
                    this.GetComponent<AudioSource>().clip = this.dyingSound;
                }
                else
                {
                    this.GetComponent<AudioSource>().clip = this.hitSounds[Random.Range(0, this.hitSounds.Length)];
                }
                this.GetComponent<AudioSource>().Play();
            }
            this.recoverTime = (1f - this.life) * 10f;
            if (hit == "Dummy")
            {
                TrainingStatistics.dummiesHit++;
            }
            else
            {
                if (hit == "Turret")
                {
                    TrainingStatistics.turretsHit++;
                }
            }
            if (this.life <= 0f)
            {
                SoldierController.dead = true;
            }
        }
    }

    public virtual void Update()
    {
        this.recoverTime = this.recoverTime - Time.deltaTime;
        if (this.recoverTime <= 0f)
        {
            this.life = this.life + (this.life * Time.deltaTime);
            this.life = Mathf.Clamp(this.life, 0f, 1f);
            this.hitAlpha = 0f;
        }
        else
        {
            this.hitAlpha = this.recoverTime / ((1f - this.life) * 10f);
        }
        if (!SoldierController.dead)
        {
            return;
        }
        this.blackAlpha = this.blackAlpha + Time.deltaTime;
        if (this.blackAlpha >= 1f)
        {
            Application.LoadLevel(1);
        }
    }

    public virtual void OnGUI()
    {
        Color oldColor = default(Color);
        Color auxColor = default(Color);
        if (!GameManager.receiveDamage)
        {
            return;
        }
        oldColor = auxColor = GUI.color;
        auxColor.a = this.hitAlpha;
        GUI.color = auxColor;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), this.hitTexture);
        auxColor.a = this.blackAlpha;
        GUI.color = auxColor;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), this.blackTexture);
        GUI.color = oldColor;
    }

}