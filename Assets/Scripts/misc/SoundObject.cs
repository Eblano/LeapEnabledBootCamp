using UnityEngine;
using System.Collections;

[System.Serializable]
public class SoundObject : MonoBehaviour
{
    [UnityEngine.HideInInspector]
    public int overrideSound;
    private bool playSound;
    private bool generateParticles;
    public float minSpeedToParticle;
    public GameObject genericParticle;
    public GameObject waterParticles;
    public LayerMask waterLayer;
    public bool checkWater;
    private float waterTimer;
    public float minSpeedSound;
    public float maxSpeedSound;
    private float timeToGenerateParticles;
    private float timeToSound;
    private float delta;
    private AudioSource audioPlayer;
    private AudioClip ac;
    public virtual void Start()
    {
        this.playSound = false;
        this.generateParticles = false;
        this.checkWater = false;
        if (SoundObjectManager.instance == null)
        {
            UnityEngine.Object.Destroy(this);
            return;
        }
        SoundObjectManager so = SoundObjectManager.instance;
        this.minSpeedToParticle = so.minSpeedToParticle;
        this.minSpeedToParticle = this.minSpeedToParticle * this.minSpeedToParticle;
        this.minSpeedSound = so.minSpeedSound;
        this.maxSpeedSound = so.maxSpeedSound;
        this.delta = 1f / (this.maxSpeedSound - this.minSpeedSound);
        this.genericParticle = so.genericParticle;
        this.generateParticles = this.genericParticle != null;
        this.waterParticles = so.waterParticles;
        this.waterLayer = so.waterLayer;
        this.checkWater = this.waterParticles != null;
        if (this.overrideSound == -1)
        {
            switch (this.gameObject.tag)
            {
                case "wood":
                    this.ac = so.defaultWoodSound;
                case "metal":
                    this.ac = so.defaultMetalSound;
                case "concrete":
                    this.ac = so.defaultConcreteSound;
                default:
                    this.ac = so.defaultSound;
                    break;
            }
        }
        this.playSound = this.ac != null;
        if (!this.playSound && !this.generateParticles)
        {
            UnityEngine.Object.Destroy(this);
        }
        if (this.GetComponent<Rigidbody>() == null)
        {
            if (this.transform.parent != null)
            {
                if (this.transform.parent.GetComponent<Rigidbody>() != null)
                {
                    SoundObjectAux aux = this.transform.parent.gameObject.AddComponent<SoundObjectAux>() as SoundObjectAux;
                    aux.soundGenerator = this;
                }
                else
                {
                    UnityEngine.Object.Destroy(this);
                }
            }
            else
            {
                UnityEngine.Object.Destroy(this);
            }
        }
    }

    public virtual void Update()
    {
        if (this.waterTimer > 0f)
        {
            this.waterTimer = this.waterTimer - Time.deltaTime;
        }
        this.timeToGenerateParticles = this.timeToGenerateParticles - Time.deltaTime;
        this.timeToSound = this.timeToSound - Time.deltaTime;
    }

    public virtual void OnCollisionEnter(Collision collisionData)
    {
        RaycastHit hitInfo = default(RaycastHit);
        float speed = collisionData.relativeVelocity.sqrMagnitude;
        if (this.checkWater && (this.waterTimer <= 0f))
        {
            if (collisionData.collider.gameObject.GetComponent("Terrain") != null)
            {
                if (Physics.Raycast(this.transform.position + new Vector3(0, 4, 0), -Vector3.up, out hitInfo, 4f, (int) this.waterLayer))
                {
                    if (hitInfo.collider.tag == "water")
                    {
                        this.waterTimer = 1f;
                        GameObject go = GameObject.Instantiate(this.waterParticles, hitInfo.point, Quaternion.identity) as GameObject;
                        if (go.GetComponent<AudioSource>() != null)
                        {
                            go.GetComponent<AudioSource>().Play();
                        }
                        ParticleSystem emitter = null;
                        int i = 0;
                        while (i < go.transform.childCount)
                        {
                            emitter = go.transform.GetChild(i).GetComponent<ParticleSystem>() as ParticleSystem;
                            if (emitter == null)
                            {
                                goto Label_for_60;
                            }
                            ParticleSystem.EmissionModule emissionModule = emitter.emission;
                            emissionModule.enabled = false;
                            emitter.Emit(1); // ���� �� ������ ����������� ������ ���� �������
                            Label_for_60:
                            i++;
                        }
                        AutoDestroy aux = go.AddComponent<AutoDestroy>() as AutoDestroy;
                        aux.time = 2;
                        return;
                    }
                }
            }
        }
        if (this.generateParticles && (this.timeToGenerateParticles <= 0f))
        {
            if (this.minSpeedToParticle < speed)
            {
                this.timeToGenerateParticles = 0.5f;
                GameObject.Instantiate(this.genericParticle, collisionData.contacts[0].point, Quaternion.identity);
            }
        }
        if (this.playSound && (this.timeToSound <= 0f))
        {
            if (this.audioPlayer == null)
            {
                this.audioPlayer = this.gameObject.AddComponent<AudioSource>() as AudioSource;
                this.audioPlayer.playOnAwake = false;
                this.audioPlayer.loop = false;
                this.audioPlayer.clip = this.ac;
            }
            if ((this.minSpeedSound * this.minSpeedSound) < speed)
            {
                speed = Mathf.Sqrt(speed);
                float v = Mathf.Clamp((speed - this.minSpeedSound) * this.delta, 0.05f, 1f);
                this.audioPlayer.volume = v;
                this.timeToSound = 0.2f;
                this.audioPlayer.Play();
            }
        }
    }

    public SoundObject()
    {
        this.overrideSound = -1;
    }

}