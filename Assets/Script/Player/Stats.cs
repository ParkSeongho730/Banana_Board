using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public uint hp;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void decreaseHp()
    {
        if (hp > 0) hp--;
    }

    public uint getHp() { return hp; }
}
