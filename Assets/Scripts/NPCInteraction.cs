
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class NPCInteraction : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;

    public bool IsDialogueOpen { get; private set; }

    private bool playerInRange;
    private int currentLine;

    private GameObject player;
    private PlayerMovement playerMovement;

    private readonly string[] dialogueLines =
    {
        @"NARASI

Pintu Kios Botani diketuk kencang dari luar. Saat pemain membukanya, Pak Karto berdiri dengan wajah cemas dan napas memburu.",

        @"PAK KARTO

Nak, ayo ikut aku sebentar ke belakang rumah! Kamu harus lihat sendiri kondisi lahanku sekarang.",

        @"NARASI

Pemain mengikuti Pak Karto keluar kios menuju halaman belakang rumahnya yang gersang. Terlihat tanahnya pecah-pecah parah, sementara sumur di dekatnya tampak kering kosong.",

        @"PAK KARTO

Lihat ini. Sumur di belakang rumahku kering total! Dan yang bikin aku bingung, tanah di sini tetap retak-retak dan keras kayak batu, walaupun hujan sudah sering turun belakangan ini.

Air hujannya kayak numpang lewat begitu saja, gak mau meresap ke dalam. Padahal dulu, musim hujan sedikit saja tanah langsung gembur. Entah kenapa alam sekarang jadi rusak begini...",

        @"PEMAIN

Ini bukan salah alam, Pak. Sepertinya semua kekeringan dan kerusakan ini terjadi akibat perluasan kebun sawit PT XXX yang tidak terkontrol. Pembabatan hutan di utara itu pastinya merusak siklus air di desa kita. Akar sawit menyedot habis air tanah, makanya tanah bapak jadi keras dan sumur warga kering.",

        @"PAK KARTO

Perkebunan sawit itu...? Pantas saja sejak mereka buka blok baru di utara, sumur di kampung kita perlahan ambles. Jadi bukan karena kutukan musim ya?

Terus, apa tanah sekeras ini masih bisa diselamatkan? Tanaman apa pun yang kutanam pasti langsung mati.",

        @"PEMAIN

Tenang, Pak. Kita bisa perbaiki struktur tanahnya dari dalam. Tunggu sebentar, aku ambilkan racikan bibitnya di kios.",

        @"RACIKAN TANAMAN

Media Tanam: Tanah Humus
Pohon Utama: Bambu Betung
Tanaman Sela: Akar Wangi / Vetiver",

        @"PEMAIN

Tanam Bambu Betung dan Akar Wangi ini di dekat sumur bapak, pakai media tanah humus yang kubawa.

Akar wanginya nanti bakal menembus dan memecah lapisan tanah yang keras itu supaya pori-porinya terbuka lebar, jadi air hujan bisa meresap ke dalam dan memulihkan kelembapan tanah, sementara batang bambunya bantu nyimpen cadangan air.",

        @"PAK KARTO

Pantesan selama ini hujan deras pun tanahku tetap kering di dalam. Baiklah, langsung kutanam sekarang di dekat sumur.

Kalau perusahaan itu biang keroknya, kita harus buktikan kalau tanah kita bisa subur lagi."
    };

    private void Update()
    {
        if (IntroDialogue.IsPlaying || IntroDialogue.FinishedThisFrame)
            return;

        if (Keyboard.current == null)
            return;

        if (!Keyboard.current.eKey.wasPressedThisFrame)
            return;

        if (IsDialogueOpen)
        {
            NextLine();
        }
        else if (playerInRange)
        {
            StartDialogue();
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
        if (dialoguePanel == null || dialogueText == null)
        {
            Debug.LogError("Dialogue UI NPC belum dihubungkan!");
            return;
        }

        IsDialogueOpen = true;
        currentLine = 0;

        dialoguePanel.SetActive(true);

        if (playerMovement != null)
            playerMovement.enabled = false;

        ShowLine();
    }

    private void NextLine()
    {
        currentLine++;

        if (currentLine >= dialogueLines.Length)
        {
            EndDialogue();
            return;
        }

        ShowLine();
    }

    private void ShowLine()
    {
        dialogueText.text = dialogueLines[currentLine];
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        IsDialogueOpen = false;

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (!playerInRange)
        {
            player = null;
            playerMovement = null;
        }
    }
}
