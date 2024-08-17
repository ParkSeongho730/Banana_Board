using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fixed_Player_Move : MonoBehaviour
{
    // 이동 및 점프
    public float maxSpeed;
    private float __maxSpeed;
    public float moveSpeed;
    public float stopSpeed;
    public float jumpPower;
    [SerializeField]
    private int jumpCount = 2;
    [SerializeField]
    private bool isSquating;
    [SerializeField]
    private bool isGround;
    [SerializeField]
    private bool canMove;

    // 벽타기 관련
    float slidingSpeed = 0.2f; //벽에서 미끄러져 내려오는 속도
    float wallJumpPower = 15f; //벽에서 점프할 힘
    float raycastDistance = 0.15f; //레이케스트의 길이
    int targetLayer; //벽 레이어마스크
    bool isWall; //플레이어가 벽에 붙어 있는지 확인
    private Bounds bounds;

    // 대쉬 관련
    [SerializeField]
    private int dashCount = 1;
    private float dashPower = 20f;
    private float dashTime = 0.2f;
    private float dashCooldown = 1f;
    private Vector2 leftDashDir;
    private Vector2 rightDashDir;
    private int isRight = 1;
    private bool isDashing = false;

    // 커맨드 관련
    [SerializeField]
    private Command_Manager CommandManager;
    private bool isCommanding;

    // 장애물 관련
    [SerializeField]
    private bool isSlow = false;

    // 스탯 관련
    private Stats stats;

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    private RaycastHit2D hit;

    // Start is called before the first frame update
    void Start()
    {
        stats = GetComponent<Stats>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        __maxSpeed = maxSpeed;
        isSquating = false;
        isGround = false;
        canMove = true;
        isCommanding = false;

        bounds = GetComponent<BoxCollider2D>().bounds;

        targetLayer = 1 << LayerMask.NameToLayer("ClimbableWall");

        leftDashDir = new Vector2(-1, 0);
        rightDashDir = new Vector2(1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // 벽타기
        isWall = Physics2D.BoxCast(transform.position, bounds.size, 0f, Vector2.right * isRight, raycastDistance, targetLayer);
        //중심점, 사이즈, 로테이션, 방향, 길이, 감지할레이어
        if (isWall)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y * slidingSpeed);
            //RigidBody의 velocity Y값에 위에서 선언한 slidingSpeed를 곱해준다.

            if (Input.GetKeyDown(KeyCode.W))
            {
                rigid.velocity = Vector2.zero;
                //점프하기전 속도를 없애줘야 깔끔한 점프가 가능하다.
                Debug.Log("벽점프");
                rigid.velocity = new Vector2(-isRight * wallJumpPower, 1.5f * wallJumpPower);
                //Vector2 climbDir = new Vector2(-isRight, 1);
                //rigid.AddForce(climbDir * wallJumpPower, ForceMode2D.Impulse);
                isRight *= -1;
                //x값은 진행방향의 반대로 해준 후 각각 wallJumpPower를 곱해 힘을 준다.
            }
        }

         
        Jump();
        Dash();
        Squat();
        command();
        

            // 바닥 뚫기 방지
        if (rigid.velocity.y < -0.99f)
        {
            int layerMask = (-1) - (1 << LayerMask.NameToLayer("Player"));
            hit = Physics2D.Raycast(new Vector2(rigid.position.x, rigid.position.y - 1.0f), Vector2.down, rigid.velocity.y, layerMask);
            if (hit.collider != null)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, -0.99f);
                //Debug.Log(hit.collider.name);
            }
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        if (!canMove) return;

        // 커맨드를 입력 받는 중이면 리턴
        if (isCommanding) return;

        Vector2 moveDir;
        float horizontalInput = Input.GetAxis("Horizontal");

        // 대쉬 방향 설정
        if (horizontalInput < 0.0f)
            isRight = -1;
        else if (horizontalInput > 0.0f)
            isRight = 1;

        if (horizontalInput != 0.0f)
        {
            moveDir = new Vector3(horizontalInput, 0, 0);

            if (isSlow && __maxSpeed >= maxSpeed)
            {
                __maxSpeed = maxSpeed / 2.0f;
            }

            rigid.AddForce(moveDir * moveSpeed, ForceMode2D.Impulse);
            if (rigid.velocity.x > __maxSpeed)
            {
                rigid.velocity = new Vector2(__maxSpeed, rigid.velocity.y);
            }
            else if (rigid.velocity.x < __maxSpeed * (-1))
            {
                rigid.velocity = new Vector2(__maxSpeed * (-1), rigid.velocity.y);
            }

            //transform.Translate(moveDir * moveSpeed * Time.deltaTime);  //추후 velocity로 변경 예정
        }
        else if (horizontalInput == 0.0f)
        {
            if (isRight == -1 && rigid.velocity.x < 0)
            {
                //Debug.Log("왼쪽 방향 감속");
                rigid.AddForce(Vector2.right * stopSpeed);
                //Debug.Log("감속 완료");
            }
            else if (isRight == 1 && rigid.velocity.x > 0)
            {
                //Debug.Log("오른쪽 방향 감속");
                rigid.AddForce(Vector2.left * stopSpeed);
                //Debug.Log("감속 완료");
            }
        }
    }

    void Jump()
    {
        if (isWall) return;
        // 커맨드를 입력 받는 중이면 리턴
        if (isCommanding) return;

        if (Input.GetKeyDown(KeyCode.W))
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
        // 커맨드를 입력 받는 중이면 리턴
        if (isCommanding) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(isDashing == false)
            {
                if (isRight == 1)
                    StartCoroutine(__Dash(rightDashDir));
                else
                    StartCoroutine(__Dash(leftDashDir));
            }
        }
    }

    IEnumerator __Dash(Vector2 dashDir)
    {
        if (dashCount > 0)
        {
            isDashing = true;
            __maxSpeed = maxSpeed * 1.5f; 

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

            // 대쉬 쿨타임
            yield return new WaitForSeconds(dashCooldown);

            __maxSpeed = maxSpeed;
            isDashing = false;
        }
    }

    void Squat()
    {
        // 커맨드를 입력 받는 중이면 리턴
        if (isCommanding) return;

        if (Input.GetKeyUp(KeyCode.S) || (isSquating == true && isGround == false))
        {
            isSquating = false;
            gameObject.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 0.25f);
            gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x, 1);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            if (isSquating == false && isGround == true)
            {
                isSquating = true;
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x, 0.5f);
                gameObject.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 0.25f);
            }
        }
    }

    void command()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (!isCommanding) spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            else spriteRenderer.color = new Color(1, 1, 1, 1);  // 플레이어 스프라이트 변경시 필요없음

            isCommanding = !isCommanding;
            CommandManager.setIsCommanding(isCommanding);
        }
    }
    public void setIsCommanding_false()
    {
        Invoke("__setIsCommanding_false", 0.1f);
    }
    void __setIsCommanding_false()
    {
        spriteRenderer.color = new Color(1, 1, 1, 1);  // 플레이어 스프라이트 변경시 필요없음
        isCommanding = false;
    }

    public int getIsRight() { return isRight; }

    public void setCanMove(bool x) { canMove = x; }
    public void setIsSlow(bool x)
    { 
        if(x == false)
        {
            __maxSpeed = maxSpeed;
        }
        isSlow = x;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            isGround = true;
            dashCount = 1;
            jumpCount = 2;
        }
        else if (collision.gameObject.tag == "DamagedObstacle")
        {
            stats.decreaseHp();
            StartCoroutine(getDamaged());
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            isGround = true;
            dashCount = 1;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            isGround = false;
        }
    }

    IEnumerator getDamaged()
    {
        setCanMove(false);
        spriteRenderer.color = new Color(1, 1, 1, 0.5f);

        rigid.velocity = new Vector2(0, 0);
        rigid.AddForce(new Vector2(-7, 10), ForceMode2D.Impulse);

        yield return new WaitForSeconds(1.0f);

        spriteRenderer.color = new Color(1, 1, 1, 1);
        setCanMove(true);
    }
}
