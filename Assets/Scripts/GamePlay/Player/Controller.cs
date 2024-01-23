using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField]
    private float speed = 10;
    [SerializeField]
    private float jumpForce = 10;
    private bool isGrounded;
    private Rigidbody2D rb;
    [SerializeField]
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        Jump();
        Rolling();
    }

    void ChagneDirection(float move)
    {
        // Đổi hướng nhân vật
        if (move > 0)
        {
            transform.localScale = new Vector2(1, 1); // Giữ nguyên chiều cao, đổi chiều rộng thành dương
        }
        else if (move < 0)
        {
            transform.localScale = new Vector2(-1, 1); // Giữ nguyên chiều cao, đổi chiều rộng thành âm
        }
    }
    void Move()
    {
        float move = Input.GetAxis("Horizontal");
        Vector2 movement = new Vector2(move, 0);
        if (isGrounded)
        {
            rb.velocity = new Vector2(movement.x * speed, rb.velocity.y);
            animator.SetFloat("Speed", Mathf.Abs(move));
            ChagneDirection(move);
        }
        else
        {
            // Ngừng animation chạy khi nhân vật không di chuyển trên mặt đất
            animator.SetFloat("Speed", 0);
        }

    }
    void Rolling()
    {
        float move = Input.GetAxis("Horizontal");
        Vector2 movement = new Vector2(move, 0);
        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded)
        {
            rb.velocity = new Vector2(movement.x*2*speed, rb.velocity.y);
            animator.SetBool("isRolling", true);
            ChagneDirection(move);
        }
        else
        {
            animator.SetBool("isRolling", false);
        }
    }
    void Jump()
    {
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space)) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            animator.SetBool("isJumping", true);
        }
        else
        {
            // Ngừng animation nhảy khi nhân vật không nhảy
            animator.SetBool("isJumping", false);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            // Kích hoạt animation đứng
            animator.SetBool("isJumping", false);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
