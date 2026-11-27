using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class IntroDialogue : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Player")]
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Input")]
    [SerializeField] private float doubleClickThreshold = 0.3f;

    public static bool IsPlaying { get; private set; }
    public static bool FinishedThisFrame { get; private set; }

    private int currentLine;
    private float lastClickTime = -1f;

    private class DialogueLine
    {
        public string speaker;
        public string text;

        public DialogueLine(string speaker, string text)
        {
            this.speaker = speaker;
            this.text = text;
        }
    }

    private readonly DialogueLine[] introLines =
    {
        new DialogueLine(
            "PEMAIN (MONOLOG)",
            "Suhu udara akhir-akhir ini benar-benar tidak masuk akal... Rasanya seperti dipanggang hidup-hidup, padahal masih jam tiga siang."
        ),

        new DialogueLine(
            "PEMAIN (MONOLOG)",
            "Untungnya, di dalam kios ini suhunya masih agak mendingan."
        ),

        new DialogueLine(
            "PEMAIN (MONOLOG)",
            "Berkat koleksi pepohonan dan tanaman polikultur yang kutanam di sekitar toko, setidaknya tempat ini bisa sedikit meredam panasnya kondisi lingkungan di luar."
        ),

        new DialogueLine(
            "PEMAIN (MONOLOG)",
            "Tapi, perlindungan sekecil ini tidak akan bertahan lama kalau daerah sekitarnya terus dibiarkan seperti ini."
        ),

        new DialogueLine(
            "PEMAIN (MONOLOG)",
            "Semua panas dan kekeringan ekstrem ini sepertinya terjadi akibat ulah PT Arsari. Deforestasi untuk membuka lahan sawit secara masif itu pastinya telah merusak kondisi di desa ini."
        ),

        new DialogueLine(
            "PEMAIN (MONOLOG)",
            "Rasanya, akar-akar sawit itu menyedot habis air tanah tanpa menyisakan apa pun, makanya wajar kalau sumur-sumur warga sekarang kering dan tanah berubah panas seperti gurun."
        )
    };

    private void Awake()
    {
        IsPlaying = true;
        FinishedThisFrame = false;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    private void Start()
    {
        if (GameProgress.HasWatchedIntro)
        {
            IsPlaying = false;
            if (dialoguePanel != null) dialoguePanel.SetActive(false);
            if (playerMovement != null) playerMovement.enabled = true;
            return;
        }

        if (dialoguePanel == null ||
            speakerNameText == null ||
            dialogueText == null)
        {
            Debug.LogError("Intro Dialogue UI belum dihubungkan!");
            IsPlaying = false;
            return;
        }

        dialoguePanel.SetActive(true);

        if (playerMovement != null)
            playerMovement.enabled = false;

        currentLine = 0;
        ShowLine();
    }

    private void Update()
    {
        if (!IsPlaying)
            return;

        if (Keyboard.current != null &&
            (Keyboard.current.eKey.wasPressedThisFrame ||
             Keyboard.current.spaceKey.wasPressedThisFrame))
        {
            NextLine();
            return;
        }

        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (lastClickTime >= 0f &&
                Time.time - lastClickTime <= doubleClickThreshold)
            {
                NextLine();
                lastClickTime = -1f;
            }
            else
            {
                lastClickTime = Time.time;
            }
        }
    }

    private void NextLine()
    {
        currentLine++;

        if (currentLine >= introLines.Length)
        {
            EndIntro();
            return;
        }

        ShowLine();
    }

    private void ShowLine()
    {
        DialogueLine line = introLines[currentLine];

        speakerNameText.text = line.speaker;
        dialogueText.text = line.text;
    }

    private void EndIntro()
    {
        GameProgress.HasWatchedIntro = true;
        dialoguePanel.SetActive(false);

        if (playerMovement != null)
            playerMovement.enabled = true;

        IsPlaying = false;
        FinishedThisFrame = true;
    }

    private void LateUpdate()
    {
        FinishedThisFrame = false;
    }
}