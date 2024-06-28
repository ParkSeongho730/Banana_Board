using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command_Manager : MonoBehaviour
{
    private const byte Left = 1;
    private const byte Right = 2;
    private const byte Up = 3;
    private const byte Down = 4;
    private const byte Jump = 5;
    private const byte Dash = 6;
    private const byte Squat = 7;

    [SerializeField]
    private Fixed_Player_Move player;
    [SerializeField]
    private ParabolicJump parabolicJump;
    [SerializeField]
    private Fly fly;
    [SerializeField]
    private PowerDash powerdash;

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
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            commandQueue.Enqueue(Up);
            Debug.Log("위");
            checkCommandQueue();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            commandQueue.Enqueue(Down);
            Debug.Log("아래");
            checkCommandQueue();
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            commandQueue.Enqueue(Jump);
            Debug.Log("점프");
            checkCommandQueue();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            commandQueue.Enqueue(Dash);
            Debug.Log("대쉬");
            checkCommandQueue();
        }
        else if (Input.GetKeyDown(KeyCode.S))
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
                Debug.Log("임시커맨드");
                commandQueue.Clear();
                isCommanding = false;
                player.setIsCommanding_false();
            }
        }
        else if (commandQueue.Count == 3)
        {
            byte[] commands = commandQueue.ToArray();

            if (commands[0] == Down &&
                commands[1] == Up &&
                commands[2] == Jump)
            {
                Debug.Log("트램펄린");
                commandQueue.Clear();
                isCommanding = false;
                player.setIsCommanding_false();
                parabolicJump.StartJump();
            }
            else if (commands[0] == Right &&
                commands[1] == Up &&
                commands[2] == Jump)
            {
                Debug.Log("체공");
                commandQueue.Clear();
                isCommanding = false;
                player.setIsCommanding_false();
                fly.DoFly();
            }
            else if (commands[0] == Right &&
                commands[1] == Right &&
                commands[2] == Dash)
            {
                Debug.Log("파워대쉬");
                commandQueue.Clear();
                isCommanding = false;
                player.setIsCommanding_false();
                powerdash.DoPowerDash();
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