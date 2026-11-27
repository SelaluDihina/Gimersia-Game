using UnityEngine;

public class PromptWobbleEffect : MonoBehaviour
{
    [Header("Pengaturan Geol Kanan-Kiri")]
    [Tooltip("Sudut kemiringan maksimal (derajat)")]
    [SerializeField] private float swayAngle = 8f;

    [Tooltip("Kecepatan ayunan (makin kecil makin pelan)")]
    [SerializeField] private float swaySpeed = 2.5f;

    [Header("Efek Pop-In Saat Muncul")]
    [SerializeField] private bool enablePopIn = true;
    [SerializeField] private float popDuration = 0.15f;

    private Quaternion initialRotation;
    private Vector3 initialScale;
    private float enableTime;

    private void Awake()
    {
        initialRotation = transform.localRotation;
        initialScale = transform.localScale;
    }

    private void OnEnable()
    {
        enableTime = Time.time;

        if (enablePopIn)
            transform.localScale = initialScale * 0.7f;
    }

    private void Update()
    {
        if (enablePopIn)
        {
            float elapsed = Time.time - enableTime;
            if (elapsed < popDuration)
            {
                float t = elapsed / popDuration;
                t = t * t * (3f - 2f * t);
                transform.localScale = Vector3.Lerp(initialScale * 0.7f, initialScale, t);
            }
            else
            {
                transform.localScale = initialScale;
            }
        }

        float angle = Mathf.Sin((Time.time - enableTime) * swaySpeed) * swayAngle;
        transform.localRotation = initialRotation * Quaternion.Euler(0f, 0f, angle);
    }

    private void OnDisable()
    {
        transform.localRotation = initialRotation;
        transform.localScale = initialScale;
    }
}