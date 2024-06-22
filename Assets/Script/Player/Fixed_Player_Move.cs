using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fixed_Player_Move : MonoBehaviour
{
    // 이동 및 점프
    public float moveSpeed;
    public float jumpPower;

    // 대쉬 관련
    private float dashPower = 12.5f;
    private float dashTime = 0.2f;
    private float dashCooldown = 1f;
    private Vector2 leftDashDir;
    private Vector2 rightDashDir;
    private bool isRight = true;

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        leftDashDir = new Vector2(-1, 0);
        rightDashDir = new Vector2(1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Jump();
        Dash();
        Squat();
    }

    void FixedUpdate()
    {
        Move();
        //Jump();
    }

    void Move()
    {
        Vector2 moveDir;
        float horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput != 0.0f)
        {
            moveDir = new Vector2(horizontalInput, 0);

            transform.Translate(moveDir * moveSpeed * Time.deltaTime);  //추후 velocity로 변경 예정

            // 대쉬 방향 설정
            if (horizontalInput < 0.0f)
                isRight = false;
            else if (horizontalInput > 0.0f)
                isRight = true;
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            rigid.velocity = new Vector2(rigid.velocity.x, 0);
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
    }

    void Dash()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (isRight)
                StartCoroutine(__Dash(rightDashDir));
            else
                StartCoroutine(__Dash(leftDashDir));
        }
    }

    IEnumerator __Dash(Vector2 dashDir)
    {
        // 대쉬
        rigid.velocity = new Vector2(dashDir.x * dashPower, 0f);

        // 대쉬 판정 지속시간
        spriteRenderer.color = new Color(1, 1, 1, 0.5f);  // 대쉬 중에 중력의 영향을 받는 문제 추후 해결
        yield return new WaitForSeconds(dashTime);
        spriteRenderer.color = new Color(1, 1, 1, 1);

        // 대쉬 쿨타임
        yield return new WaitForSeconds(dashCooldown);
    }

    void Squat()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x, 0.5f);
            gameObject.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 0.25f);
        }
        else if (Input.GetKeyUp(KeyCode.X))
        {
            gameObject.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 0.25f);
            gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x, 1);
        }
    }
}
