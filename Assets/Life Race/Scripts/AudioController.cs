using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour {

    public static AudioController Instace;

    public AudioClip menuMusic;
    public AudioClip gameplayMusic;
    public AudioClip winFx;

    AudioSource source;

    void Awake ()
    {
        Instace = this;
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = true;
        source.spatialBlend = 0;
    }
	
	public void PlayMenuSound () {
        source.clip = menuMusic;
        source.Play();
    }

    public void PlayGameSound()
    {
        source.clip = gameplayMusic;
        source.Play();
    }

    public void PlaySoundEffect()
    {
        AudioSource.PlayClipAtPoint(winFx, Vector3.zero);
    }
}
