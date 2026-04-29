using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource runningSoundSource;    // Looping running sound
    public AudioSource jumpSoundSource;       // One shot jump sound
    public AudioSource landSoundSource;       // One shot land sound
    public AudioSource deathSoundSource;      // One shot death sound

    [Header("Audio Clips")]
    public AudioClip runningClip;
    public AudioClip jumpClip;
    public AudioClip landClip;
    public AudioClip deathClip;

    [Header("Settings")]
    public float runningVolume = 0.5f;
    public float jumpVolume = 0.8f;
    public float landVolume = 0.8f;

    private TestCharController controller;
    private bool wasAirborne;
    private bool wasDead;

    private void Start()
    {
        controller = GetComponent<TestCharController>();

        // Setup running audio source as looping
        if (runningSoundSource != null)
        {
            runningSoundSource.clip = runningClip;
            runningSoundSource.loop = true;
            runningSoundSource.volume = runningVolume;
            runningSoundSource.playOnAwake = false;
        }
    }

    private void Update()
    {
        if (controller == null) return;

        bool isAirborne = IsAirborne();

        // Death sound
        if (controller.isDead && !wasDead)
        {
            StopRunning();
            PlayOneShot(deathSoundSource, deathClip, 1f);
            wasDead = true;
            return;
        }

        if (controller.isDead) return;

        // Jump sound — just left ground
        if (isAirborne && !wasAirborne)
        {
            StopRunning();
            PlayOneShot(jumpSoundSource, jumpClip, jumpVolume);
        }

        // Land sound — just hit ground
        if (!isAirborne && wasAirborne)
        {
            PlayOneShot(landSoundSource, landClip, landVolume);
        }

        // Running sound — on ground and alive
        if (!isAirborne)
            StartRunning();
        else
            StopRunning();

        wasAirborne = isAirborne;
    }

    private void StartRunning()
    {
        if (runningSoundSource != null && !runningSoundSource.isPlaying)
            runningSoundSource.Play();
    }

    private void StopRunning()
    {
        if (runningSoundSource != null && runningSoundSource.isPlaying)
            runningSoundSource.Stop();
    }

    private void PlayOneShot(AudioSource source, AudioClip clip, float volume)
    {
        if (source != null && clip != null)
            source.PlayOneShot(clip, volume);
    }

    private bool IsAirborne()
    {
        return (bool)typeof(TestCharController)
            .GetField("isAirborne", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(controller);
    }
}