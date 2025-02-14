using UnityEngine;
using System.Collections;

[System.Serializable]
[UnityEngine.ExecuteInEditMode]
public partial class ShowFps : MonoBehaviour
{
    private GUIText gui;
    private float updateInterval;
    private double lastInterval; // Last interval end time
    private int frames; // Frames over current interval
    public virtual void Start()
    {
        this.lastInterval = Time.realtimeSinceStartup;
        this.frames = 0;
    }

    public virtual void OnDisable()
    {
        if (this.gui)
        {
            UnityEngine.Object.DestroyImmediate(this.gui.gameObject);
        }
    }

    public virtual void Update()
    {
        ++this.frames;
        float timeNow = Time.realtimeSinceStartup;
        if (timeNow > (this.lastInterval + this.updateInterval))
        {
            if (!this.gui)
            {
                GameObject go = new GameObject("FPS Display", new System.Type[] {typeof(GUIText)});
                go.hideFlags = HideFlags.HideAndDontSave;
                go.transform.position = new Vector3(0, 0, 0);
                this.gui = go.GetComponent<GUIText>();
                this.gui.pixelOffset = new Vector2(5, 55);
            }
            float fps = (float) (this.frames / (timeNow - this.lastInterval));
            float ms = 1000f / Mathf.Max(fps, 1E-05f);
            this.gui.text = ((ms.ToString("f1") + "ms ") + fps.ToString("f2")) + "FPS";
            this.frames = 0;
            this.lastInterval = timeNow;
        }
    }

    public ShowFps()
    {
        this.updateInterval = 1f;
    }

}