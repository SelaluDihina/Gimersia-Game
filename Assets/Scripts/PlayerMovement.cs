using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        if (!string.IsNullOrEmpty(GameProgress.NextSpawnPointName))
        {
            GameObject spawnPoint = GameObject.Find(GameProgress.NextSpawnPointName);
            if (spawnPoint != null)
            {
                Vector3 targetPos = new Vector3(
                    spawnPoint.transform.position.x,
                    spawnPoint.transform.position.y,
                    transform.position.z
                );
                transform.position = targetPos;
                if (rb != null)
                    rb.position = targetPos;
            }
            else
            {
                Debug.LogWarning("Titik respawn tidak ditemukan: " + GameProgress.NextSpawnPointName);
            }

            GameProgress.NextSpawnPointName = "";
        }
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            moveInput = Vector2.zero;
            UpdateAnimation();
            return;
        }

        float x = 0f;
        float y = 0f;

        if (Keyboard.current.aKey.isPressed ||
            Keyboard.current.leftArrowKey.isPressed)
            x = -1f;

        if (Keyboard.current.dKey.isPressed ||
            Keyboard.current.rightArrowKey.isPressed)
            x = 1f;

        if (Keyboard.current.sKey.isPressed ||
            Keyboard.current.downArrowKey.isPressed)
            y = -1f;

        if (Keyboard.current.wKey.isPressed ||
            Keyboard.current.upArrowKey.isPressed)
            y = 1f;

        moveInput = new Vector2(x, y).normalized;

        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    private void OnDisable()
    {
        moveInput = Vector2.zero;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (anim != null)
        {
            anim.SetFloat("Horizontal", 0f);
            anim.SetFloat("Vertical", 0f);
        }
    }

    private void UpdateAnimation()
    {
        if (anim == null) return;

        anim.SetFloat("Horizontal", moveInput.x);
        anim.SetFloat("Vertical", moveInput.y);

        if (spriteRenderer != null && moveInput.x != 0)
        {
            spriteRenderer.flipX = moveInput.x < 0;
        }
    }
}