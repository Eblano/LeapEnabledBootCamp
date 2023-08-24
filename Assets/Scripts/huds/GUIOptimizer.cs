using UnityEngine;
using System.Collections;

[System.Serializable]
public class GUIOptimizer : MonoBehaviour
{
    public HudWeapons hudWeapons;
    public MainMenuScreen mainMenu;
    public SargeManager sarge;
    public AchievmentScreen achievements;
    public virtual void OnGUI()
    {
        Event evt = Event.current;
        if (this.mainMenu != null)
        {
            this.mainMenu.DrawGUI(evt);
        }
        if (this.achievements != null)
        {
            this.achievements.DrawGUI(evt);
        }
        if (evt.type == EventType.Repaint)
        {
            if (this.hudWeapons != null)
            {
                this.hudWeapons.DrawGUI(evt);
            }
            if (this.sarge != null)
            {
                this.sarge.DrawGUI(evt);
            }
        }
    }

}