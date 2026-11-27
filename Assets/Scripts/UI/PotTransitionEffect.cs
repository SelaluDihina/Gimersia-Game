using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PotTransitionEffect : MonoBehaviour
{
    [Header("Aset Animasi Asap")]
    [Tooltip("Seret 5 potongan sprite asap berurutan dari Sprite Sheet ke sini")]
    [SerializeField] private Sprite[] smokeFrames;
    [SerializeField] private Image smokeDisplayImage;
    [SerializeField] private float animationSpeed = 0.08f; // Kecepatan ganti frame (detik)

    [Header("Animasi Pop pada Pot")]
    [SerializeField] private bool enablePotPop = true;
    [SerializeField] private float popDuration = 0.25f;

    private RectTransform potRect;
    private Vector3 initialPotScale;
    private Coroutine currentAnimCoroutine;
    private Coroutine popCoroutine;

    private void Awake()
    {
        potRect = GetComponent<RectTransform>();
        initialPotScale = transform.localScale;

        // Pastikan tampilan asap tidak terlihat di awal
        if (smokeDisplayImage != null)
        {
            smokeDisplayImage.gameObject.SetActive(false);
        }
    }

    public void PlayTransition()
    {
        if (!gameObject.activeInHierarchy) return;

        // 1. Jalankan animasi pop halus pada pot
        if (enablePotPop)
        {
            if (popCoroutine != null) StopCoroutine(popCoroutine);
            popCoroutine = StartCoroutine(AnimatePotPop());
        }

        // 2. Putar animasi frame-by-frame asap
        if (smokeDisplayImage != null && smokeFrames != null && smokeFrames.Length > 0)
        {
            if (currentAnimCoroutine != null) StopCoroutine(currentAnimCoroutine);
            currentAnimCoroutine = StartCoroutine(PlaySmokeFrames());
        }
    }

    private IEnumerator PlaySmokeFrames()
    {
        smokeDisplayImage.gameObject.SetActive(true);

        // Putar dari frame pertama sampai terakhir (0 sampai 4)
        for (int i = 0; i < smokeFrames.Length; i++)
        {
            if (smokeFrames[i] != null)
            {
                smokeDisplayImage.sprite = smokeFrames[i];
            }
            yield return new WaitForSeconds(animationSpeed);
        }

        // Sembunyikan kembali setelah selesai animasi
        smokeDisplayImage.gameObject.SetActive(false);
    }

    private IEnumerator AnimatePotPop()
    {
        float elapsed = 0f;
        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / popDuration);

            // Membal kecil (0.94 -> 1.05 -> 1.0)
            float bounce = Mathf.Sin(t * Mathf.PI) * 0.06f;
            potRect.localScale = initialPotScale * (1f + bounce);

            yield return null;
        }

        potRect.localScale = initialPotScale;
    }
}