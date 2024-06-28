using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pos
{
    public Vector2 startPosition;    // 시작 위치
    public Vector2 targetPosition;   // 도착 위치
    public Vector2 apexPosition;     // 최고점 위치
    public float speed;         // x축 속도
}

public class ParabolicJump : MonoBehaviour
{
    public Pos[] pos;
    int it = 0;
    public bool isJumping = false;   // 점프 상태

    private Fixed_Player_Move player;
    private Rigidbody2D rb;
    WaitForSeconds _wait = new WaitForSeconds(0.5f);

    void Start()
    {
        player = GetComponent<Fixed_Player_Move>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isJumping)
        {
            // 목표 지점에 도달했는지 체크
            if (Vector2.Distance(transform.position, pos[it].targetPosition) < 0.1f)
            {
                if(it < 1)
                {
                    Debug.Log("한번 더 점프");
                    rb.velocity = Vector2.zero;
                    it++;
                    StartJump();
                }
                else
                {
                    Debug.Log("점프 종료");
                    isJumping = false;
                    rb.velocity = Vector2.zero;
                    player.setCanMove(true);
                }
                
            }
        }
    }

    public void StartJump()
    {
        player.setCanMove(false);
        rb.isKinematic = true; // 수동으로 속도와 힘을 제어하기 위해 Kinematic 모드로 설정
        StartCoroutine(__jump());
    }

    IEnumerator __jump()
    {
        // 시작 위치 지정
        pos[it].startPosition = transform.position;

        // x축 거리 및 방향 계산
        float distanceX = pos[it].targetPosition.x - pos[it].startPosition.x;
        float directionX = distanceX > 0 ? 1 : -1;

        // y축 초기 속도 계산
        float distanceY = pos[it].apexPosition.y - pos[it].startPosition.y;
        float gravity = Mathf.Abs(Physics2D.gravity.y);
        float initialVelocityY = Mathf.Sqrt(2 * gravity * distanceY);


        // 쪼그리는 모션
        gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x, 0.5f);
        gameObject.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 0.25f);
        yield return _wait;
        gameObject.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 0.25f);
        gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x, 1);


        // x축 속도 설정
        rb.isKinematic = false;
        rb.velocity = new Vector2(pos[it].speed * directionX, 0);

        // y축 초기 속도 설정
        rb.velocity = new Vector2(rb.velocity.x, initialVelocityY);

        isJumping = true;
    }
}
