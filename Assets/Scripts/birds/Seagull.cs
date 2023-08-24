using UnityEngine;
using System.Collections;
using UnityEngine.WSA;

[System.Serializable]
public partial class Seagull : MonoBehaviour
{
    public AudioClip[] sounds;
    public float soundFrequency;
    public float animationSpeed;
    public float minSpeed;
    public float turnSpeed;
    public float randomFreq;
    public float randomForce;
    public float toOriginForce;
    public float toOriginRange;
    public float damping;
    public float gravity;
    public float avoidanceRadius;
    public float avoidanceForce;
    public float followVelocity;
    public float followRadius;
    public float bankTurn;
    public bool raycast;
    public float bounce;
    private SeagullFlightPath target;
    private Transform origin;
    private Vector3 velocity;
    private Vector3 normalizedVelocity;
    private Vector3 randomPush;
    private Vector3 originPush;
    private Vector3 gravPush;
    private RaycastHit hit;
    private Transform[] objects;
    private Seagull[] otherSeagulls;
    private Animation animationComponent;
    private Transform transformComponent;
    private bool gliding;
    private float bank;
    private AnimationState glide;
    private bool paused;
    public virtual void Start()
    {
        this.randomFreq = 1f / this.randomFreq;
        this.paused = false;
        this.gameObject.tag = this.transform.parent.gameObject.tag;
        this.animationComponent = (Animation) this.GetComponentInChildren(typeof(Animation));
        this.animationComponent["Take 001"].speed = this.animationSpeed;
        this.animationComponent.Blend("Take 001");
        this.animationComponent["Take 001"].normalizedTime = Random.value;
        this.glide = this.animationComponent["Take 001"];
        this.origin = this.transform.parent;
        this.target = (SeagullFlightPath) this.origin.GetComponent(typeof(SeagullFlightPath));
        this.transform.parent = null;
        this.transformComponent = this.transform;
        Component[] tempSeagulls = new Component[0];
        if (transform.parent)
            tempSeagulls = transform.parent.GetComponentsInChildren<Seagull>();
        objects = new Transform[tempSeagulls.Length];
        otherSeagulls = new Seagull[tempSeagulls.Length];
        for (int i = 0; i < tempSeagulls.Length; i++)
        {
            objects[i] = tempSeagulls[i].transform;
            otherSeagulls[i] = (Seagull)tempSeagulls[i];
        }

        UpdateRandom();
    }

    public virtual IEnumerator UpdateRandom()
    {
        while (true)
        {
            this.randomPush = Random.insideUnitSphere * this.randomForce;
            yield return new WaitForSeconds(this.randomFreq + Random.Range(-this.randomFreq / 2, this.randomFreq / 2));
        }
    }

    void Update()
    {
        if (origin == null)
        {
            Destroy(gameObject);
            return;
        }

        if (GameManager.pause)
        {
            if (!paused)
            {
                paused = true;
                animationComponent.Stop();
            }
            return;
        }
        else
        {
            if (paused)
            {
                paused = false;
                animationComponent.Blend("Take 001");
            }
        }

        float speed = velocity.magnitude;
        Vector3 avoidPush = Vector3.zero;
        Vector3 avgPoint = Vector3.zero;
        int count = 0;
        float f = 0.0f;
        Vector3 myPosition = transformComponent.position;

        Vector3 forceV; // Объявляем forceV до цикла
        float d; // Объявляем d до цикла
        for (int i = 0; i < objects.Length; i++)
        {
            Transform o = objects[i];
            if (o != transformComponent)
            {
                Vector3 otherPosition = o.position;
                avgPoint += otherPosition;
                count++;

                forceV = myPosition - otherPosition; // Присваиваем значение forceV
                d = forceV.magnitude; // Присваиваем значение d
                if (d < followRadius)
                {
                    if (d < avoidanceRadius)
                    {
                        f = 1.0f - (d / avoidanceRadius);
                        if (d > 0) avoidPush += (forceV / d) * f * avoidanceForce;
                    }

                    f = d / followRadius;
                    Seagull otherSealgull = otherSeagulls[i];
                    avoidPush += otherSealgull.normalizedVelocity * f * followVelocity;
                }
            }
        }

        Vector3 toAvg; // Объявляем toAvg до условного оператора
        if (count > 0)
        {
            avoidPush /= count;
            toAvg = (avgPoint / count) - myPosition;
        }
        else
        {
            toAvg = Vector3.zero;
        }

        //Vector3 forceV; // Объявляем forceV
        if (target != null)
        {
            forceV = origin.position + target.offset - myPosition;
        }
        else
        {
            forceV = origin.position - myPosition;
        }

        float d2 = forceV.magnitude;
        f = d2 / toOriginRange;
        if (d2 > 0) originPush = (forceV / d2) * f * toOriginForce;

        if (speed < minSpeed && speed > 0)
        {
            velocity = (velocity / speed) * minSpeed;
        }

        Vector3 wantedVel = velocity;
        wantedVel -= wantedVel * damping * Time.deltaTime;
        wantedVel += randomPush * Time.deltaTime;
        wantedVel += originPush * Time.deltaTime;
        wantedVel += avoidPush * Time.deltaTime;
        wantedVel += toAvg.normalized * gravity * Time.deltaTime;


        Vector3 diff = transformComponent.InverseTransformDirection(wantedVel - velocity).normalized;
        bank = Mathf.Lerp(bank, diff.x, Time.deltaTime * 0.8f);
        velocity = Vector3.RotateTowards(velocity, wantedVel, turnSpeed * Time.deltaTime, 100.00f);

        transformComponent.rotation = Quaternion.FromToRotation(Vector3.right, velocity);
        //transformComponent.Rotate(0, 0, -bank * bankTurn);

        // Raycast
        float distance = speed * Time.deltaTime;
        if (raycast && distance > 0.00f && Physics.Raycast(myPosition, velocity, out hit, distance))
        {
            velocity = Vector3.Reflect(velocity, hit.normal) * bounce;
        }
        else
        {
            transformComponent.Translate(velocity * Time.deltaTime, Space.World);
        }

        // Sounds
        if (sounds != null && sounds.Length > 0)
        {
            if (SeagullSoundHeat.heat < Mathf.Pow(Random.value, 1 / soundFrequency / Time.deltaTime))
            {
                AudioSource.PlayClipAtPoint(sounds[Random.Range(0, sounds.Length)], myPosition, 0.90f);
                SeagullSoundHeat.heat += (1 / soundFrequency) / 10;
            }
        }

        normalizedVelocity = velocity.normalized;
    }

public Seagull()
    {
        this.sounds = new AudioClip[0];
        this.soundFrequency = 1f;
        this.animationSpeed = 1f;
        this.bounce = 0.8f;
    }

}