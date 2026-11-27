using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class NPCInteraction : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Input")]
    [SerializeField] private float doubleClickThreshold = 0.3f;

    public bool IsDialogueOpen { get; private set; }

    private bool playerInRange;
    private int currentLine;

    private float lastClickTime = -1f;

    private GameObject player;
    private PlayerMovement playerMovement;

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

    private readonly DialogueLine[] dialogueLines =
    {
        new DialogueLine(
            "NARASI",
            "Pintu Kios Botani diketuk kencang dari luar. Saat pemain membukanya, Pak Karto berdiri dengan wajah cemas dan napas memburu."
        ),

        new DialogueLine(
            "PAK KARTO",
            "Nak, ayo ikut aku sebentar ke belakang rumah! Kamu harus lihat sendiri kondisi lahanku sekarang."
        ),

        new DialogueLine(
            "NARASI",
            "Pemain mengikuti Pak Karto keluar kios menuju halaman belakang rumahnya yang gersang. Terlihat tanahnya pecah-pecah parah, sementara sumur di dekatnya tampak kering kosong."
        ),

        new DialogueLine(
            "PAK KARTO",
            "Lihat ini. Sumur di belakang rumahku kering total! Dan yang bikin aku bingung, tanah di sini tetap retak-retak dan keras kayak batu, walaupun hujan sudah sering turun belakangan ini."
        ),

        new DialogueLine(
            "PAK KARTO",
            "Air hujannya kayak numpang lewat begitu saja, gak mau meresap ke dalam. Padahal dulu, musim hujan sedikit saja tanah langsung gembur. Entah kenapa alam sekarang jadi rusak begini..."
        ),

        new DialogueLine(
            "PEMAIN",
            "Ini bukan salah alam, Pak. Sepertinya semua kekeringan dan kerusakan ini terjadi akibat perluasan kebun sawit PT Arsari yang tidak terkontrol."
        ),

        new DialogueLine(
            "PEMAIN",
            "Pembabatan hutan di utara itu pastinya merusak siklus air di desa kita. Akar sawit menyedot habis air tanah, makanya tanah bapak jadi keras dan sumur warga kering."
        ),

        new DialogueLine(
            "PAK KARTO",
            "Perkebunan sawit itu...? Pantas saja sejak mereka buka blok baru di utara, sumur di kampung kita perlahan ambles. Jadi bukan karena kutukan musim ya?"
        ),

        new DialogueLine(
            "PAK KARTO",
            "Terus, apa tanah sekeras ini masih bisa diselamatkan? Tanaman apa pun yang kutanam pasti langsung mati."
        ),

        new DialogueLine(
            "PEMAIN",
            "Tenang, Pak. Kita bisa perbaiki struktur tanahnya dari dalam. Tunggu sebentar, aku ambilkan racikan bibitnya di kios."
        ),

        new DialogueLine(
            "RACIKAN TANAMAN",
            "Media Tanam: Tanah Humus\nPohon Utama: Bambu Betung\nTanaman Sela: Akar Wangi / Vetiver"
        ),

        new DialogueLine(
            "PEMAIN",
            "Tanam Bambu Betung dan Akar Wangi ini di dekat sumur bapak, pakai media tanah humus yang kubawa."
        ),

        new DialogueLine(
            "PEMAIN",
            "Akar wanginya nanti bakal menembus dan memecah lapisan tanah yang keras itu supaya pori-porinya terbuka lebar, jadi air hujan bisa meresap ke dalam dan memulihkan kelembapan tanah, sementara batang bambunya bantu nyimpen cadangan air."
        ),

        new DialogueLine(
            "PAK KARTO",
            "Pantesan selama ini hujan deras pun tanahku tetap kering di dalam. Baiklah, langsung kutanam sekarang di dekat sumur."
        ),

        new DialogueLine(
            "PAK KARTO",
            "Kalau perusahaan itu biang keroknya, kita harus buktikan kalau tanah kita bisa subur lagi."
        )
    };

    private readonly DialogueLine[] reminderLines =
    {
        new DialogueLine(
            "PAK KARTO",
            "Jangan lupa racikannya ya Nak: Tanah Humus, Bambu Betung, dan Akar Wangi (Vetiver). Segera siapkan di meja racik!"
        )
    };

    private readonly DialogueLine[] completedLines =
    {
        new DialogueLine(
            "PAK KARTO",
            "Lihat sumurnya, Nak! Airnya sudah mulai naik lagi dan tanahnya perlahan gembur. Terima kasih banyak sudah menyelamatkan lahan desa kita!"
        )
    };

    private DialogueLine[] GetActiveLines()
    {
        if (GameProgress.IsMissionCompleted) return completedLines;
        if (GameProgress.HasTalkedToKarto) return reminderLines;
        return dialogueLines;
    }

    private void Update()
    {
        if (IntroDialogue.IsPlaying || IntroDialogue.FinishedThisFrame)
            return;

        if (!IsDialogueOpen)
        {
            if (playerInRange &&
                Keyboard.current != null &&
                Keyboard.current.eKey.wasPressedThisFrame)
            {
                StartDialogue();
            }

            return;
        }

        bool nextPressed = false;

        if (Keyboard.current != null)
        {
            nextPressed =
                Keyboard.current.eKey.wasPressedThisFrame ||
                Keyboard.current.spaceKey.wasPressedThisFrame;
        }

        if (nextPressed)
        {
            lastClickTime = -1f;
            NextLine();
            return;
        }

        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (lastClickTime >= 0f &&
                Time.time - lastClickTime <= doubleClickThreshold)
            {
                lastClickTime = -1f;
                NextLine();
            }
            else
            {
                lastClickTime = Time.time;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        player = other.gameObject;
        playerMovement = player.GetComponent<PlayerMovement>();
        playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject != player)
            return;

        playerInRange = false;

        if (!IsDialogueOpen)
        {
            player = null;
            playerMovement = null;
        }
    }

    private void StartDialogue()
    {
        if (dialoguePanel == null ||
            speakerNameText == null ||
            dialogueText == null)
        {
            Debug.LogError("Dialogue UI NPC belum dihubungkan!");
            return;
        }

        IsDialogueOpen = true;
        currentLine = 0;
        lastClickTime = -1f;

        dialoguePanel.SetActive(true);

        if (playerMovement != null)
            playerMovement.enabled = false;

        ShowLine();
    }

    private void NextLine()
    {
        currentLine++;

        if (currentLine >= GetActiveLines().Length)
        {
            EndDialogue();
            return;
        }

        ShowLine();
    }

    private void ShowLine()
    {
        DialogueLine line = GetActiveLines()[currentLine];

        speakerNameText.text = line.speaker;
        dialogueText.text = line.text;
    }

    private void EndDialogue()
    {
        GameProgress.HasTalkedToKarto = true;
        dialoguePanel.SetActive(false);
        IsDialogueOpen = false;
        lastClickTime = -1f;

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (!playerInRange)
        {
            player = null;
            playerMovement = null;
        }
    }
}