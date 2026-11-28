using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneTransitionPoint : MonoBehaviour
{
    [Header("Scene Destination")]
    [SerializeField] private string targetSceneName;
    [SerializeField] private string spawnPointInTargetScene;

    [Header("Interaction UI")]
    [SerializeField] private GameObject prompt;
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private string promptMessage = "Interact";

    private bool playerInRange;
    private bool ignoreUntilExit;

    private void Start()
    {
        if (prompt != null)
            prompt.SetActive(false);
    }

    private void Update()
    {
        if (!playerInRange || ignoreUntilExit) return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (!string.IsNullOrEmpty(targetSceneName))
            {
                GameProgress.NextSpawnPointName = spawnPointInTargetScene;
                SceneManager.LoadScene(targetSceneName);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Abaikan sentuhan pertama jika Player spawn di dalam area trigger
        if (Time.timeSinceLevelLoad < 0.1f)
        {
            ignoreUntilExit = true;
            playerInRange = false;
            return;
        }

        if (ignoreUntilExit) return;

        playerInRange = true;

        if (promptText != null)
            promptText.text = promptMessage;

        if (prompt != null)
            prompt.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        ignoreUntilExit = false;
        playerInRange = false;

        if (prompt != null)
            prompt.SetActive(false);
    }
}