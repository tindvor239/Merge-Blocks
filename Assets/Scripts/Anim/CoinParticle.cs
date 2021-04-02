using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinParticle : MonoBehaviour
{
	public Transform target;

    public Vector3 Target { 
		get 
		{
			Vector3 pos = target.position;
			
			return pos;
		} 
	}

    private ParticleSystem system;

	private static ParticleSystem.Particle[] particles = new ParticleSystem.Particle[30];

	int count;

	[SerializeField]
	private float waitTime = 0.5f;
	[SerializeField]
	private float speed = 1f;

	void OnEnable()
	{
		
		if (system == null)
			system = GetComponent<ParticleSystem>();

		if (system == null)
		{
			this.enabled = false;
		}else
        {
			system.Clear();
		}
		
	}

	public void PlayParticles(Vector3 pos, int burstCount)
    {
		system.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
		ParticleSystem.Burst burst = system.emission.GetBurst(0);
		burst.count = burstCount;
		system.emission.SetBurst(0, burst);
		system.transform.position = pos;
		StopAllCoroutines();
		StartCoroutine(CorPlayParticles());
    }

	IEnumerator CorPlayParticles()
    {
		system.Play();
		
		yield return new WaitForSeconds(waitTime);

		StartCoroutine(MoveToTarget());
		
	}

	IEnumerator MoveToTarget()
    {
		
		while(true)
        {
			
			count = system.GetParticles(particles);
			int deadCount = 0;
			for (int i = 0; i < count; i++)
			{
				
				
				ParticleSystem.Particle particle = particles[i];
				particle.velocity = Vector3.zero;
				
				particle.position = Vector3.MoveTowards(particle.position, Target, Time.deltaTime * speed);
				
                if (Vector3.Distance(particle.position, Target) < 0.02f)
                {
					
                    deadCount++;
                }
				particles[i] = particle;
			}
			
            if (deadCount >= count)
			{
				//SoundLibrary.instance.PlaySound(SoundLibrary.instance.CoinSound());
				system.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
				break;
			}

			system.SetParticles(particles, count);
			yield return null;
		}
		
	}


}
