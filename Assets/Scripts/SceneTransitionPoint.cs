
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneTransitionPoint : MonoBehaviour
{
    [Header("Scene Destination")]
    [SerializeField] private string targetSceneName = "MissionScene";

    [Header("Interaction UI")]
    [SerializeField] private GameObject prompt;
    [SerializeField] private TMP_Text promptText;

    private bool playerInRange;
    private bool isLoading;

    private NPCInteraction npcInteraction;

    private void Awake()
    {
        npcInteraction = FindFirstObjectByType<NPCInteraction>();
    }

    private void Start()
    {
        if (prompt != null)
            prompt.SetActive(false);

        if (promptText != null)
            promptText.text = "[E] Pergi";
    }

    private void Update()
    {
        bool dialogueActive =
            IntroDialogue.IsPlaying ||
            (npcInteraction != null && npcInteraction.IsDialogueOpen);

        bool canInteract =
            playerInRange &&
            !dialogueActive &&
            !isLoading;

        if (prompt != null && prompt.activeSelf != canInteract)
            prompt.SetActive(canInteract);

        if (!canInteract || Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
            TransitionScene();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    private void TransitionScene()
    {
        if (!Application.CanStreamedLevelBeLoaded(targetSceneName))
        {
            Debug.LogError(
                "Scene tidak ditemukan atau belum terdaftar: " +
                targetSceneName
            );
            return;
        }

        isLoading = true;

        if (prompt != null)
            prompt.SetActive(false);

        SceneManager.LoadScene(targetSceneName);
    }
}
