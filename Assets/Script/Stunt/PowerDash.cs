using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerDash : MonoBehaviour
{
    public float dashPower = 20.0f;
    private bool isDashing = false;

    private Vector2 leftDashDir;
    private Vector2 rightDashDir;

    private Fixed_Player_Move player;
    private Rigidbody2D rigid;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Fixed_Player_Move>();
        rigid = GetComponent<Rigidbody2D>();

        leftDashDir = new Vector2(-1, 0);
        rightDashDir = new Vector2(1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DoPowerDash()
    {
        if (player.getIsRight() == 1)
            StartCoroutine(__Dash(rightDashDir));
        else
            StartCoroutine(__Dash(leftDashDir));
    }

    IEnumerator __Dash(Vector2 dashDir)
    {
        player.setCanMove(false);
        isDashing = true;

        rigid.gravityScale = 0;
        rigid.velocity = Vector2.zero;

        // 대쉬
        rigid.AddForce(dashDir * dashPower, ForceMode2D.Impulse);

        rigid.gravityScale = 1;

        yield return null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "PowerDashWall")
        {
            Destroy(collision.gameObject);
            if (player.getIsRight() == 1)
                StartCoroutine(__Dash(rightDashDir));
            else
                StartCoroutine(__Dash(leftDashDir));
            player.setCanMove(true);
        }
    }
}
