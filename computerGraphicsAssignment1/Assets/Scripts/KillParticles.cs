using UnityEngine;
[ExecuteInEditMode]
public class ParticleKillOnContact : MonoBehaviour
{
    private ParticleSystem particleSystem;
    

    void Start()
    {
        // Get the particle system attached to the object
        particleSystem = GetComponent<ParticleSystem>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("box"))
        {
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        }
       
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("box"))
        {
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        }

        
    }
}
