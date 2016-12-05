using UnityEngine;
using System.Collections;

public class PlayerAvatar : MonoBehaviour
{
    public ParticleSystem shape, trail, burst;
    public GameObject sperm;

    Player player;

	// Use this for initialization
	void Awake ()
    {
        player = transform.root.GetComponent<Player>();
	}

    public void EmittPropellers()
    {

    }

    public void EmittBurst()
    {
        sperm.SetActive(false);
        shape.Stop();
        trail.Stop();
        burst.Emit(burst.maxParticles);
    }

    public void PauseParticles()
    {
        shape.Pause();
        trail.Pause();
        burst.Pause();
    }

    public void StopParticles ()
    {
        shape.Stop();
        trail.Stop();
        burst.Stop();
    }
}
