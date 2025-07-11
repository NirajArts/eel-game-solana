using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public float speed = 10f;
    public float jumpForce = 10f;
    public float gravity = -9.81f;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public Transform groundCheck;
    public float sidewaysSpeed = 5f;
    public float smoothSpeed = 10f;
    public Animator animator;

    private Rigidbody rb;
    private Vector3 velocity;
    private bool isGrounded;
    private float targetX;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        targetX = transform.position.x;
        if (animator == null) animator = GetComponent<Animator>();
        if (animator)
        {
            animator.SetTrigger("Run");
        }
    }

    void Update()
    {
        // Ground check using Physics.CheckSphere
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (animator)
        {
            animator.SetBool("IsGrounded", isGrounded);
        }
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Always move forward
        Vector3 move = Vector3.forward * speed;

        // Get horizontal input (old input system)
        float horizontalInput = Input.GetAxis("Horizontal");
        targetX -= horizontalInput * sidewaysSpeed * Time.deltaTime;
        float newX = Mathf.Lerp(transform.position.x, targetX, smoothSpeed * Time.deltaTime);
        move.x = (newX - transform.position.x) / Time.deltaTime;

        // Jump input
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            if (animator)
            {
                animator.SetTrigger("Jump");
            }
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
    }

    void FixedUpdate()
    {
        // Move the player using Rigidbody
        Vector3 move = new Vector3((Mathf.Lerp(transform.position.x, targetX, smoothSpeed * Time.fixedDeltaTime) - transform.position.x) / Time.fixedDeltaTime, 0, speed);
        Vector3 movement = new Vector3(move.x, 0, move.z) * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement + Vector3.up * velocity.y * Time.fixedDeltaTime);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}
