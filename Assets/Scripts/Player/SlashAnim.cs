using UnityEngine;

public class SlashAnim : MonoBehaviour
{
    private ParticleSystem ps;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();        
    }

    void Update()
    {
        if (ps && !ps.IsAlive())
        {
            DestroySelf();
        }        
    }


    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
