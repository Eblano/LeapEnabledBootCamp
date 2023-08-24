using UnityEngine;
using System.Collections;

[System.Serializable]
public class HudLeap : MonoBehaviour
{
    public Texture2D[] quadrant;
    public Texture2D monitor;
    public Texture2D dot;
    private int inQuadrant;
    private float mouseX;
    private float mouseY;
    private Color auxColor;
    private Color cColor;
    private Vector2 startCornerMonitor;
    private Vector2 startCornerNav;
    public virtual void Start()
    {
    }

    public virtual void OnGUI()
    {
        this.auxColor = this.cColor = GUI.color;
        // top right corner
        this.startCornerMonitor = new Vector2(Screen.width, 0) - new Vector2(this.monitor.width, 0);
        this.startCornerNav = (new Vector2(Screen.width, 0) - new Vector2(this.monitor.width, 0)) + new Vector2(0, this.monitor.height + 2);
        this.ShowQuadrant(this.quadrant[this.inQuadrant]);
        this.ShowMonitor();
        GUI.color = this.cColor;
    }

    public virtual void Update()// print("x = " + x.ToString() + ", z = " + z.ToString() + "' inQuadrant = " + inQuadrant.ToString());
    {
        float x = pxsLeapInput.GetHandAxisStep("Horizontal");
        float z = pxsLeapInput.GetHandAxisStep("Depth");
        this.mouseX = pxsLeapInput.GetHandAxis("Mouse X");
        this.mouseY = pxsLeapInput.GetHandAxis("Mouse Y");
        if (z == 1f)
        {
            if (x == -1f)
            {
                this.inQuadrant = 0;
            }
            else
            {
                if (x == 0f)
                {
                    this.inQuadrant = 1;
                }
                else
                {
                    this.inQuadrant = 2;
                }
            }
        }
        else
        {
            if (z == 0f)
            {
                if (x == -1f)
                {
                    this.inQuadrant = 3;
                }
                else
                {
                    if (x == 0f)
                    {
                        this.inQuadrant = 4;
                    }
                    else
                    {
                        this.inQuadrant = 5;
                    }
                }
            }
            else
            {
                if (x == -1f)
                {
                    this.inQuadrant = 6;
                }
                else
                {
                    if (x == 0f)
                    {
                        this.inQuadrant = 7;
                    }
                    else
                    {
                        this.inQuadrant = 8;
                    }
                }
            }
        }
    }

    public virtual void ShowMonitor()
    {
        GUI.color = this.auxColor;
        float x = this.startCornerMonitor.x;
        float y = this.startCornerMonitor.y;
        Rect graphicRect = new Rect(x, y, this.monitor.width, this.monitor.height);
        GUI.DrawTexture(graphicRect, this.monitor);
        x = (this.startCornerMonitor.x + (this.monitor.width / 2)) + (((this.monitor.width - this.dot.width) * this.mouseX) / 2);
        y = (this.startCornerMonitor.y + (this.monitor.height / 2)) + (((this.monitor.height - this.dot.height) * this.mouseY) / 2);
        graphicRect = new Rect(x, y, this.dot.width, this.dot.height);
        GUI.DrawTexture(graphicRect, this.dot);
    }

    public virtual void ShowQuadrant(Texture2D aQuadrant)
    {
        GUI.color = this.auxColor;
        float x = this.startCornerNav.x;
        float y = this.startCornerNav.y;
        Rect graphicRect = new Rect(x, y, aQuadrant.width, aQuadrant.height);
        GUI.DrawTexture(graphicRect, aQuadrant);
    }

}