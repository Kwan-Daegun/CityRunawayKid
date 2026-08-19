using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { SpeedBoost, Invincibility, Combo, AntiGravity }

    [Header("Settings")]
    public PowerUpType powerUpType = PowerUpType.SpeedBoost;
    public float rotateSpeed = 90f;

    [Header("Sound")]
    public AudioClip pickupSound;
    public float pickupVolume = 1f;

    private void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!col.CompareTag("Player")) return;

        switch (powerUpType)
        {
            case PowerUpType.SpeedBoost:
                if (PowerUpManager.Instance != null)
                    PowerUpManager.Instance.ActivateSpeedBoost();
                break;
            case PowerUpType.Invincibility:
                if (PowerUpManager.Instance != null)
                    PowerUpManager.Instance.ActivateInvincibility();
                break;
            case PowerUpType.Combo:
                if (PowerUpManager.Instance != null)
                    PowerUpManager.Instance.ActivateCombo();
                break;
            case PowerUpType.AntiGravity:
                if (AntiGravityManager.Instance != null)
                {
                    if (AntiGravityManager.Instance.IsAntiGravity)
                        AntiGravityManager.Instance.ReturnToNormal();
                    else
                        AntiGravityManager.Instance.Activate();
                }
                break;
        }

        // Play sound before destroying
        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, pickupVolume);

        Destroy(gameObject);
    }
}