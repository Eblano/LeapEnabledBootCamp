using UnityEngine;
using System.Collections;

[System.Serializable]
public class SargeManager : MonoBehaviour
{
    public Texture2D sarge;
    public Texture2D background;
    public SargeInstruction[] instructions;
    private bool visible;
    private Rect sargeRect;
    private Rect backgroundRect;
    private Vector2 halfScreen;
    private Rect container;
    private float sargeAlpha;
    private float backgroundAlpha;
    private float contentAlpha;
    public float fadeTime;
    private Color auxColor;
    private Color oldColor;
    private Hashtable table;
    private float timeToHide;
    private SargeInstruction currentInstruction;
    public GUIStyle textStyle;
    [UnityEngine.HideInInspector]
    public bool debug;
    private bool audioWasPlaying;
    private object[] messageQueue;
    private bool friendlyFire;
    private SargeInstruction lastInstruction;
    public virtual void Start()
    {
        this.messageQueue = new object[0];
        this.friendlyFire = false;
        this.audioWasPlaying = false;
        this.table = new Hashtable();
        int i = 0;
        while (i < this.instructions.Length)
        {
            if (!(this.instructions[i] == null))
            {
                if (!string.IsNullOrEmpty(this.instructions[i].name))
                {
                    if (!this.table.ContainsKey(this.instructions[i].name.ToLower()))
                    {
                        this.table.Add(this.instructions[i].name.ToLower(), this.instructions[i]);
                    }
                }
            }
            i++;
        }
        this.fadeTime = 1f / this.fadeTime;
        this.sargeAlpha = 0f;
        this.visible = false;
        this.sargeRect = new Rect(0, 0, this.sarge.width, this.sarge.height);
        this.backgroundRect = new Rect(0, 0, this.background.width, this.background.height);
        this.background.wrapMode = TextureWrapMode.Clamp;
        this.container = new Rect(0, 0, this.sarge.width + this.background.width, Mathf.Max(this.sarge.height, this.background.height));
        if (this.GetComponent<AudioSource>() == null)
        {
            this.gameObject.AddComponent<AudioSource>();
        }
        this.GetComponent<AudioSource>().loop = false;
        this.GetComponent<AudioSource>().playOnAwake = false;
    }

    public virtual void StopInstructions()
    {
        if (!(this.messageQueue == null))
        {
            this.messageQueue.Clear();
        }
        this.timeToHide = 0f;
        if (this.GetComponent<AudioSource>().isPlaying)
        {
            this.GetComponent<AudioSource>().Stop();
        }
    }

    public virtual void DrawGUI(Event @event)
    {
        if (this.contentAlpha <= 0f)
        {
            return;
        }
        if ((GameManager.pause || SoldierController.dead) || AchievmentScreen.returningToTraining)
        {
            GUI.color = new Color(0.5f, 0.5f, 0.5f, 0f);
            return;
        }
        this.auxColor = this.oldColor = GUI.color;
        this.halfScreen = new Vector2(Screen.width, Screen.height) * 0.5f;
        this.container.x = this.halfScreen.x - (this.container.width * 0.5f);
        this.container.y = Screen.height - (this.container.height * 0.5f);
        this.sargeRect.x = this.container.x;
        this.sargeRect.y = this.container.y - (this.sargeRect.height * 0.5f);
        this.backgroundRect.x = this.sargeRect.x + this.sargeRect.width;
        this.backgroundRect.y = this.container.y - (this.backgroundRect.height * 0.5f);
        this.auxColor.a = this.backgroundAlpha;
        GUI.color = this.auxColor;
        GUI.DrawTexture(this.backgroundRect, this.background);
        this.auxColor.a = this.sargeAlpha;
        GUI.color = this.auxColor;
        GUI.DrawTexture(this.sargeRect, this.sarge);
        this.DrawInstruction();
        GUI.color = this.oldColor;
    }

    public virtual void DrawInstruction()
    {
        if (this.currentInstruction == null)
        {
            return;
        }
        this.auxColor.a = this.contentAlpha;
        GUI.color = this.auxColor;
        if (this.currentInstruction.texture != null)
        {
            Rect auxRect = new Rect(((this.backgroundRect.width - this.currentInstruction.texture.width) * 0.5f) + this.backgroundRect.x, ((this.backgroundRect.height - this.currentInstruction.texture.height) * 0.5f) + this.backgroundRect.y, this.currentInstruction.texture.width, this.currentInstruction.texture.height);
            GUI.DrawTexture(auxRect, this.currentInstruction.texture);
        }
        else
        {
            GUI.TextArea(new Rect(this.backgroundRect.x + 30, this.backgroundRect.y + 10, this.backgroundRect.width - 60, this.backgroundRect.height - 20), this.currentInstruction.text, this.textStyle);
        }
    }

    public Texture2D tex;
    public virtual void Update()
    {
        if ((GameManager.pause || SoldierController.dead) || AchievmentScreen.returningToTraining)
        {
            if (this.GetComponent<AudioSource>().isPlaying)
            {
                this.audioWasPlaying = true;
                this.GetComponent<AudioSource>().Pause();
            }
            return;
        }
        else
        {
            if (this.audioWasPlaying)
            {
                this.GetComponent<AudioSource>().Play();
                this.audioWasPlaying = false;
            }
        }
        if (!this.visible)
        {
            if (this.contentAlpha > 0f)
            {
                this.contentAlpha = this.contentAlpha - (Time.deltaTime * this.fadeTime);
            }
            else
            {
                if (this.backgroundAlpha > 0f)
                {
                    this.backgroundAlpha = this.backgroundAlpha - (Time.deltaTime * this.fadeTime);
                }
                else
                {
                    if (this.sargeAlpha > 0f)
                    {
                        this.sargeAlpha = this.sargeAlpha - (Time.deltaTime * this.fadeTime);
                    }
                }
            }
        }
        else
        {
            if (this.sargeAlpha < 1f)
            {
                this.sargeAlpha = this.sargeAlpha + (Time.deltaTime * this.fadeTime);
            }
            else
            {
                if (this.backgroundAlpha < 1f)
                {
                    this.backgroundAlpha = this.backgroundAlpha + (Time.deltaTime * this.fadeTime);
                }
                else
                {
                    if (this.contentAlpha < 1f)
                    {
                        this.contentAlpha = this.contentAlpha + (Time.deltaTime * this.fadeTime);
                    }
                }
            }
            if (this.timeToHide >= 0)
            {
                this.timeToHide = this.timeToHide - Time.deltaTime;
                if (this.timeToHide < 0f)
                {
                    if (this.friendlyFire)
                    {
                        this.friendlyFire = false;
                        if (!(this.lastInstruction == null))
                        {
                            this.ShowInstruction(this.lastInstruction.name);
                            this.lastInstruction = null;
                        }
                    }
                    else
                    {
                        if (this.messageQueue.Length > 0)
                        {
                            string m = this.messageQueue[0] as string;
                            this.messageQueue.RemoveAt(0);
                            this.ShowInstruction(m);
                        }
                        else
                        {
                            this.visible = false;
                        }
                    }
                }
            }
        }
    }

    public virtual void FriendlyFire()
    {
        if (this.friendlyFire)
        {
            return;
        }
        if (this.GetComponent<AudioSource>().isPlaying)
        {
            int i = Random.Range(0, 2);
            string m = null;
            if (i == 0)
            {
                m = "friendly_fire1";
            }
            else
            {
                m = "friendly_fire2";
            }
            if (this.table.ContainsKey(m.ToLower()))
            {
                this.lastInstruction = this.currentInstruction;
                this.friendlyFire = true;
                this.currentInstruction = (SargeInstruction) this.table[m];
                this.timeToHide = this.currentInstruction.timeToDisplay + ((((1f - this.sargeAlpha) + (1f - this.backgroundAlpha)) + (1f - this.contentAlpha)) * (1f / this.fadeTime));
                if (this.currentInstruction.audio != null)
                {
                    this.GetComponent<AudioSource>().clip = this.currentInstruction.audio;
                    this.GetComponent<AudioSource>().volume = this.currentInstruction.volume;
                    this.GetComponent<AudioSource>().Play();
                }
                this.visible = true;
            }
        }
    }

    public virtual void ShowInstruction(string instruction)
    {
        if (this.table == null)
        {
            return;
        }
        if (this.table.ContainsKey(instruction.ToLower()))
        {
            if ((this.timeToHide > 0f) || this.friendlyFire)
            {
                if (!this.currentInstruction.overridable)
                {
                    if ((this.table[instruction] as SargeInstruction).queuable)
                    {
                        this.messageQueue.Add(instruction);
                    }
                    return;
                }
            }
            this.currentInstruction = (SargeInstruction) this.table[instruction];
            this.timeToHide = this.currentInstruction.timeToDisplay + ((((1f - this.sargeAlpha) + (1f - this.backgroundAlpha)) + (1f - this.contentAlpha)) * (1f / this.fadeTime));
            if (this.currentInstruction.audio != null)
            {
                this.GetComponent<AudioSource>().clip = this.currentInstruction.audio;
                this.GetComponent<AudioSource>().volume = this.currentInstruction.volume;
                this.GetComponent<AudioSource>().Play();
            }
            this.visible = true;
        }
    }

}