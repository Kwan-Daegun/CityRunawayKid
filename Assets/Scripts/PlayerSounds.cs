using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource loopSource;
    public AudioSource oneShotSource;

    [Header("Audio Clips")]
    public AudioClip runningClip;
    public AudioClip jumpClip;
    public AudioClip landClip;
    public AudioClip deathClip;
    public AudioClip hitClip;

    [Header("Settings")]
    public float runningVolume = 0.5f;
    public float jumpVolume = 0.8f;
    public float landVolume = 0.8f;
    public float deathVolume = 1f;
    public float hitVolume = 1f;

    private TestCharController controller;
    private bool wasAirborne;
    private bool wasDead;

    private void Start()
    {
        controller = GetComponent<TestCharController>();

        loopSource.loop = true;
        loopSource.playOnAwake = false;
        loopSource.clip = runningClip;
        loopSource.volume = runningVolume;

        oneShotSource.loop = false;
        oneShotSource.playOnAwake = false;
    }

    private void Update()
    {
        if (controller == null) return;

        bool isAirborne = IsAirborne();

        if (controller.isDead && !wasDead)
        {
            wasDead = true;
            loopSource.Stop();
            PlayOneShot(deathClip, deathVolume);
            return;
        }

        if (controller.isDead) return;

        if (isAirborne && !wasAirborne)
            PlayOneShot(jumpClip, jumpVolume);

        if (!isAirborne && wasAirborne)
            PlayOneShot(landClip, landVolume);

        if (!isAirborne)
        {
            if (!loopSource.isPlaying)
                loopSource.Play();

            loopSource.volume = Mathf.MoveTowards(loopSource.volume, runningVolume, Time.deltaTime * 4f);
        }
        else
        {
            loopSource.volume = Mathf.MoveTowards(loopSource.volume, 0f, Time.deltaTime * 8f);
            if (loopSource.volume <= 0f && loopSource.isPlaying)
                loopSource.Stop();
        }

        wasAirborne = isAirborne;
    }

    public void PlayHitSound()
    {
        PlayOneShot(hitClip, hitVolume);
    }

    

    private void PlayOneShot(AudioClip clip, float volume)
    {
        if (clip == null || oneShotSource == null) return;
        oneShotSource.PlayOneShot(clip, volume);
    }

    private bool IsAirborne()
    {
        return (bool)typeof(TestCharController)
            .GetField("isAirborne", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(controller);
    }
}