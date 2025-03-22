using UnityEngine;

public class StartRainOnSceneLoad : MonoBehaviour
{
    private ParticleSystem rainParticleSystem;

    void Start()
    {
        rainParticleSystem = GetComponent<ParticleSystem>();

        if (rainParticleSystem != null && !rainParticleSystem.isPlaying)
        {
            rainParticleSystem.Play();
        }
    }
}
