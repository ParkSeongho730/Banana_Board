using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command_Manager : MonoBehaviour
{
    // 키
    private const byte Left = 1;
    private const byte Right = 2;
    private const byte Jump = 3;
    private const byte Dash = 4;
    private const byte Squat = 5;

    [SerializeField]
    private Fixed_Player_Move player;

    private bool isCommanding;

    [SerializeField]
    private Queue<byte> commandQueue = new Queue<byte>();

    // Start is called before the first frame update
    void Start()
    {
        isCommanding = false;
    }

    // Update is called once per frame
    void Update()
    {
        inputCommand();
        //printQueue();
        checkCommandQueue();
    }

    void inputCommand()
    {
        if (!isCommanding) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            commandQueue.Enqueue(Left);
            Debug.Log("왼쪽");
            checkCommandQueue();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            commandQueue.Enqueue(Right);
            Debug.Log("오른쪽");
            checkCommandQueue();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            commandQueue.Enqueue(Jump);
            Debug.Log("점프");
            checkCommandQueue();
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            commandQueue.Enqueue(Dash);
            Debug.Log("대쉬");
            checkCommandQueue();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            commandQueue.Enqueue(Squat);
            Debug.Log("숙이기");
            checkCommandQueue();
        }
    }

    void checkCommandQueue()
    {
        if (commandQueue.Count >= 4)
        {
            byte[] commands = commandQueue.ToArray();

            if (commands[0] == Left &&
                commands[1] == Right &&
                commands[2] == Dash &&
                commands[3] == Dash)
            {
                Debug.Log("커맨드 3 입력 확인!");
                commandQueue.Clear();
                isCommanding = false;
                player.setIsCommanding_false();
            }
        }
        else if (commandQueue.Count == 3)
        {
            byte[] commands = commandQueue.ToArray();

            if (commands[0] == Jump &&
                commands[1] == Jump &&
                commands[2] == Dash)
            {
                Debug.Log("커맨드 1 입력 확인!");
                commandQueue.Clear();
                isCommanding = false;
                player.setIsCommanding_false();
            }
            else if (commands[0] == Squat &&
                commands[1] == Squat &&
                commands[2] == Jump)
            {
                Debug.Log("커맨드 2 입력 확인!");
                commandQueue.Clear();
                isCommanding = false;
                player.setIsCommanding_false();
            }
        }
    }

    void printQueue()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            byte[] commands = commandQueue.ToArray();
            Debug.Log(commands);
        }
    }

    public void setIsCommanding(bool playerState)
    {
        if (playerState == false) commandQueue.Clear();

        isCommanding = playerState;
    }


}
