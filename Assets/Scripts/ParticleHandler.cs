using UnityEngine;

public class ParticleHandler : MonoBehaviour
{
    [SerializeField] private ParticleSystem explosionParticle;

    public void PlayExplosion()
    {
        if (explosionParticle)
            Instantiate(explosionParticle, transform.position, Quaternion.identity);
    }
}