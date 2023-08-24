#pragma strict
#pragma implicit
#pragma downcast

class MainMenuEffects extends MonoBehaviour
{
    public var customParticleSystem : ParticleSystem;

    function Start()
    {
        if (customParticleSystem != null)
        {
            customParticleSystem.Simulate(10);
        }
    }
}
