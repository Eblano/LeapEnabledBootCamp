using UnityEngine;
using System.Collections;

[System.Serializable]
public class BlueLeaf : MonoBehaviour
{
    private bool registered;
    public AudioClip[] audios;
    private static bool[] played;
    private static int totalPlayed;
    private static int totalAudios;
    private static int lastPlayed;
    public float soundDelay;
    public float volume;
    public virtual void Start()
    {
        this.registered = false;
        if (!(this.audios == null))
        {
            if (this.audios.Length > 0)
            {
                if (BlueLeaf.played == null)
                {
                    BlueLeaf.totalAudios = this.audios.Length;
                    BlueLeaf.totalPlayed = 0;
                    BlueLeaf.played = new bool[BlueLeaf.totalAudios];
                    int i = 0;
                    while (i < BlueLeaf.totalAudios)
                    {
                        BlueLeaf.played[i] = false;
                        i++;
                    }
                }
            }
            else
            {
                BlueLeaf.totalAudios = 0;
            }
        }
        else
        {
            BlueLeaf.totalAudios = 0;
        }
    }

    public virtual void Update()
    {
        if (!this.registered)
        {
            TrainingStatistics.totalBlueLeaf++;
            this.registered = true;
        }
    }

    /*
		if(audio != null)
		{
			audio.PlayOneShot(audios[sAudio]);
		}
		//*/    public virtual void PlaySound()
    {
        if (BlueLeaf.totalAudios <= 0)
        {
            return;
        }
        int sAudio = 0;
        if (BlueLeaf.totalPlayed >= BlueLeaf.totalAudios)
        {
            BlueLeaf.totalPlayed = 0;
            int i = 0;
            while (i < BlueLeaf.totalAudios)
            {
                BlueLeaf.played[i] = false;
                i++;
            }
            BlueLeaf.played[BlueLeaf.lastPlayed] = true;
        }
        sAudio = Random.Range(0, BlueLeaf.totalAudios);
        while (BlueLeaf.played[sAudio])
        {
            sAudio = Random.Range(0, BlueLeaf.totalAudios);
        }
        if (BlueLeaf.totalPlayed == 0)
        {
            BlueLeaf.played[BlueLeaf.lastPlayed] = false;
        }
        BlueLeaf.lastPlayed = sAudio;
        BlueLeaf.played[sAudio] = true;
        BlueLeaf.totalPlayed++;
        GameObject go = new GameObject("_LeafSound");
        AudioSource audioS = go.AddComponent<AudioSource>() as AudioSource;
        audioS.volume = this.volume;
        audioS.playOnAwake = false;
        AutoDestroy ad = go.AddComponent<AutoDestroy>() as AutoDestroy;
        ad.time = (this.audios[sAudio].length + this.soundDelay) + 1f;
        audioS.clip = this.audios[sAudio];
        BlueLeafSound bSound = go.AddComponent<BlueLeafSound>() as BlueLeafSound;
        bSound.delay = this.soundDelay;
    }

    public virtual void Hit(RaycastHit hit)
    {
        TrainingStatistics.blueLeaf++;
        this.PlaySound();
        UnityEngine.Object.Destroy(this);
    }

    public BlueLeaf()
    {
        this.soundDelay = 0.5f;
        this.volume = 0.18f;
    }

}