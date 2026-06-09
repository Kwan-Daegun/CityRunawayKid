using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { SpeedBoost, Invincibility, Combo, AntiGravity }

    [Header("Settings")]
    public PowerUpType powerUpType = PowerUpType.SpeedBoost;
    public float rotateSpeed = 90f;
    public float bobSpeed = 2f;
    public float bobAmount = 0.3f;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        transform.position = startPos + Vector3.up * Mathf.Sin(Time.time * bobSpeed) * bobAmount;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!col.CompareTag("Player")) return;

        switch (powerUpType)
        {
            case PowerUpType.SpeedBoost:
                PowerUpManager.Instance.ActivateSpeedBoost();
                break;
            case PowerUpType.Invincibility:
                PowerUpManager.Instance.ActivateInvincibility();
                break;
            case PowerUpType.Combo:
                PowerUpManager.Instance.ActivateCombo();
                break;
            case PowerUpType.AntiGravity:
                AntiGravityManager.Instance.Activate();
                break;
        }

        Destroy(gameObject);
    }
}