using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fixed_Player_Move : MonoBehaviour
{
    // 이동 및 점프
    public float maxSpeed;
    public float moveSpeed;
    public float stopSpeed;
    public float jumpPower;
    [SerializeField]
    private int jumpCount = 2;
    [SerializeField]
    private bool isSquating;
    [SerializeField]
    private bool isGround;

    // 대쉬 관련
    [SerializeField]
    private int dashCount = 1;
    private float dashPower = 20f;
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

        isSquating = false;
        isGround = false;

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
    }

    void Move()
    {
        Vector2 moveDir;
        float horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput != 0.0f)
        {
            moveDir = new Vector3(horizontalInput, 0, 0);

            rigid.AddForce(moveDir * moveSpeed, ForceMode2D.Impulse);
            if (rigid.velocity.x > maxSpeed)
            {
                rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
            }
            else if (rigid.velocity.x < maxSpeed * (-1))
            {
                rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
            }

            //transform.Translate(moveDir * moveSpeed * Time.deltaTime);  //추후 velocity로 변경 예정

            // 대쉬 방향 설정
            if (horizontalInput < 0.0f)
                isRight = false;
            else if (horizontalInput > 0.0f)
                isRight = true;
        }
        else if (horizontalInput == 0.0f)
        {
            if (isRight == false && rigid.velocity.x < 0)
            {
                Debug.Log("왼쪽 방향 감속");
                rigid.AddForce(Vector2.right * stopSpeed);
                Debug.Log("감속 완료");
            }
            else if (isRight == true && rigid.velocity.x > 0)
            {
                Debug.Log("오른쪽 방향 감속");
                rigid.AddForce(Vector2.left * stopSpeed);
                Debug.Log("감속 완료");
            }
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if(jumpCount > 0)
            {
                jumpCount--;
                rigid.velocity = new Vector2(rigid.velocity.x, 0);
                rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            }
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
        if (dashCount > 0)
        {
            dashCount--;
            rigid.gravityScale = 0;
            rigid.velocity = new Vector2(rigid.velocity.x, 0);

            spriteRenderer.color = new Color(1, 1, 1, 0.5f);

            // 대쉬
            rigid.AddForce(dashDir * dashPower, ForceMode2D.Impulse);
            //rigid.velocity = new Vector2((dashDir.x * dashPower) + rigid.velocity.x, 0f);

            // 대쉬 판정 지속시간
            yield return new WaitForSeconds(dashTime);
            spriteRenderer.color = new Color(1, 1, 1, 1);
            rigid.gravityScale = 1;

            if (isGround) dashCount = 1;

            // 대쉬 쿨타임
            yield return new WaitForSeconds(dashCooldown);
        }
    }

    void Squat()
    {
        
        if (Input.GetKeyUp(KeyCode.X) || (isSquating == true && isGround == false))
        {
            isSquating = false;
            gameObject.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 0.25f);
            gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x, 1);
        }
        else if (Input.GetKey(KeyCode.X))
        {
            if (isSquating == false && isGround == true)
            {
                isSquating = true;
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x, 0.5f);
                gameObject.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 0.25f);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            isGround = true;
            dashCount = 1;
            jumpCount = 2;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            isGround = false;
        }
    }
}
