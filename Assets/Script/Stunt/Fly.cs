using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly : MonoBehaviour
{
    public Vector2 targetPosition;
    public float jumpPower;
    public float speed;
    [SerializeField]
    private bool isFlying;
    private bool isJumping;

    private int targetLayer;
    private float raycastDistance = 3.0f;
    private RaycastHit2D isCommandObstacle;

    private Fixed_Player_Move player;
    private Rigidbody2D rigid;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Fixed_Player_Move>();
        rigid = GetComponent<Rigidbody2D>();

        targetLayer = 1 << LayerMask.NameToLayer("FlyObstacle");

        isJumping = false;
        isFlying = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(isJumping && transform.position.y > targetPosition.y)
        {
            rigid.gravityScale = 0;
            isJumping = false;
            isFlying = true;
        }

        if (isFlying)
        {
            if (transform.position.x > targetPosition.x)
            {
                isFlying = false;
                rigid.velocity = Vector2.zero;
                rigid.gravityScale = 1;
                player.setCanMove(true);
            }
            else
            {
                DoFly();
            }
        }
        
    }

    public void CheckObstacle()
    {
        isCommandObstacle = Physics2D.Raycast(transform.position, Vector2.right, raycastDistance, targetLayer);
        if (isCommandObstacle.collider != null)
        {
            GameObject Obstacle = isCommandObstacle.collider.gameObject;
            Transform ObstacleTransform = Obstacle.transform;
            Transform target = ObstacleTransform.Find("1");
            targetPosition = new Vector2(target.position.x, target.position.y);

            DoFly();
        }
        else
        {
            return;
        }
    }

    public void DoFly()
    {
        player.setCanMove(false);

        if (!isFlying)
            __jump();

        if (!isJumping)
            rigid.velocity = new Vector2(speed, 0);
    }

    private void __jump()
    {
        isJumping = true;

        //점프
        rigid.velocity = Vector2.zero;
        rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
    }
}
