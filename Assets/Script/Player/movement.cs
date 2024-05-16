using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class movement : MonoBehaviour
{
    public Vector2 inputvector;
    Rigidbody2D rigid;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

   
    void Update() 
    {
        inputvector.x = Input.GetAxisRaw("Horizontal");
        inputvector.y = Input.GetAxisRaw("Vertical");

    }
    void FixedUpdate()
    {
   
        rigid.MovePosition(rigid.position + inputvector);
        
    }
}
