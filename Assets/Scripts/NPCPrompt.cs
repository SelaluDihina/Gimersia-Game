
using UnityEngine;

public class NPCPrompt : MonoBehaviour
{
    [SerializeField] private GameObject prompt;

    private bool playerInRange;
    private NPCInteraction npcInteraction;

    private void Awake()
    {
        npcInteraction = GetComponent<NPCInteraction>();
    }

    private void Start()
    {
        if (prompt != null)
            prompt.SetActive(false);
    }

    private void Update()
    {
        if (prompt == null)
            return;

        bool dialogueActive =
            IntroDialogue.IsPlaying ||
            (npcInteraction != null && npcInteraction.IsDialogueOpen);

        prompt.SetActive(playerInRange && !dialogueActive);
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
}
