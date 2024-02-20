using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField]
    private float speed = 10; // Tốc độ chạy
    [SerializeField]
    private float jumpForce = 10; // Độ cao khi nhảy
    private bool isGrounded; // Kiểm tra khi có sự va chạm vào map
    private Rigidbody2D rb;
    [SerializeField]
    private Animator animator;
    bool Attacked = false; // check xem có đang tấn công hay không
    // Mảng lưu trữ các combo tấn công
    int[] attackCombo = { 1, 2, 3 }; // Đây là một ví dụ về combo tấn công 3 đòn
    // Chỉ số của combo hiện tại
    int currentComboIndex = 0;
    int CurrentWeaponNo = 0; // Theo dõi loại vũ khí hiện tại, mặc định là không cầm vũ khí
    float[] weaponLayerWeights; // Mảng lưu trữ trọng số của các layer vũ khí

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        // Khởi tạo mảng weaponLayerWeights với kích thước bằng với số lượng layer trong Animator
        weaponLayerWeights = new float[animator.layerCount];
    }

    void Update()
    {
        Move();
        Jump();
        Rolling();
        ChangeWeapon();
        Attack();
    }

    void ChangeDirection(float move)
    {
        // Đổi hướng nhân vật
        if (move > 0)
        {
            transform.localScale = new Vector2(1, 1); // Đổi chiều rộng thành dương
        }
        else if (move < 0)
        {
            transform.localScale = new Vector2(-1, 1); // Đổi chiều rộng thành âm
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
            ChangeDirection(move);
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

        // Kiểm tra nếu người chơi nhấn phím "C" và nhân vật đang ở trên mặt đất
        if (Input.GetKeyDown(KeyCode.C) && isGrounded)
        {
            rb.velocity = new Vector2(movement.x * 2 * speed, rb.velocity.y);
            animator.SetBool("isRolling", true);
            ChangeDirection(move);
        }
        else
        {
            // Ngừng animation cuộn khi người chơi không nhấn phím "C" hoặc nhân vật không ở trên mặt đất
            animator.SetBool("isRolling", false);
        }
    }
    void Jump()
    {
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space)) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            animator.SetBool("isJumping", true);
            isGrounded = false; // Cập nhật isGrounded ngay khi nhảy
        }
        else
        {
            // Ngừng animation nhảy khi nhân vật không nhảy
            if (isGrounded)
            {
                animator.SetBool("isJumping", false);
            }
        }
    }
    void ChangeWeapon()
    {
        // Kiểm tra người chơi nhấn các phím tương ứng với vũ khí
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CurrentWeaponNo = 1; // Chuyển sang vũ khí 1
        }
        else if (Input.GetKeyDown(KeyCode.C)) // Sử dụng phím "C" để chuyển đổi vũ khí
        {
            CurrentWeaponNo = 0; // Mặc định không cầm vũ khí
        }

        // Thiết lập trọng số của các layer vũ khí trong mảng weaponLayerWeights
        for (int i = 0; i < weaponLayerWeights.Length; i++)
        {
            // Nếu index của layer trùng với CurrentWeaponNo, thiết lập trọng số là 1
            if (i == CurrentWeaponNo)
            {
                weaponLayerWeights[i] = 1;
            }
            // Ngược lại, thiết lập trọng số là 0
            else
            {
                weaponLayerWeights[i] = 0;
            }

            // Thiết lập trọng số của layer trong Animator bằng giá trị từ mảng weaponLayerWeights
            animator.SetLayerWeight(i, weaponLayerWeights[i]);
        }
    }
    void Attack()
    {
        if (Input.GetMouseButtonDown(0) && CurrentWeaponNo != 0)
        {
            Attacked = true;
            animator.SetBool("isAttacked", Attacked);
            PerformComboAttack();
        }
        else if (!Input.GetMouseButtonDown(0) && Attacked)
        {
            // Dừng hoạt ảnh tấn công nếu không có sự kiện tấn công nào diễn ra
            animator.ResetTrigger("Attack1");
            animator.ResetTrigger("Attack2");
            animator.ResetTrigger("Attack3");
            Attacked = false;
            animator.SetBool("isAttacked", Attacked);
        }
    }
    void PerformComboAttack()
    {
        int currentAttack = attackCombo[currentComboIndex];
        switch (currentAttack)
        {
            case 1:
                animator.SetTrigger("Attack1");
                break;
            case 2:
                animator.SetTrigger("Attack2");
                break;
            case 3:
                animator.SetTrigger("Attack3");
                break;
            default:
                break;
        }

        currentComboIndex++;
        if (currentComboIndex >= attackCombo.Length)
        {
            currentComboIndex = 0;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            // Chỉ kích hoạt animation nhảy khi nhân vật đang ở mặt đất và đang không thực hiện nhảy
            if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.Space))
            {
                animator.SetBool("isJumping", false);
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            animator.SetBool("isJumping", true); // Bật animation nhảy khi nhân vật rời khỏi mặt đất
        }
    }

}
