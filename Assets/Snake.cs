using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject snakeBodyPrefab;
    [SerializeField] private Queue<Transform> snakeBody;

    private float accumTime;
    private Transform bodyDequeued;
    private Transform lastBodyEnqueued;
    private Vector3 lastDirection;

    private int[,] blockedPositions;
    private System.Random random;

    [SerializeField] private GameObject foodPrefab;
    private GameObject foodInstance;

    private void Awake()
    {
        random = new System.Random();
    }

    private void Start()
    {
        accumTime = 0;

        // Init snake body
        GameObject instance;
        snakeBody = new Queue<Transform>();
        int initX = GameMaster.Instance.boardWidth / 2;
        int initY = GameMaster.Instance.boardHeight / 2;

        for (int i = -2; i < 2; i++)
        {
            instance = Instantiate(snakeBodyPrefab, new Vector3(initX, initY + i, 0), Quaternion.identity);
            if (i == 1)
                lastBodyEnqueued = instance.GetComponent<Transform>();

            snakeBody.Enqueue(instance.GetComponent<Transform>());
        }

        lastDirection = Vector3.up;

        // Init matriz of blocked positions
        blockedPositions = new int[GameMaster.Instance.boardWidth + 2, GameMaster.Instance.boardHeight + 2];

        for (int x = 0; x < GameMaster.Instance.boardWidth + 2; x++)
        {
            for (int y = 0; y < GameMaster.Instance.boardHeight + 2; y++)
            {
                if (x == 0 || x == GameMaster.Instance.boardWidth + 1 || y == 0 || y == GameMaster.Instance.boardHeight + 1)
                    blockedPositions[x, y] = 1;
            }
        }

        //  Apply body to matriz
        foreach (Transform bodyPart in snakeBody.ToArray())
        {
            blockedPositions[(int)bodyPart.position.x, (int)bodyPart.position.y] = 1;
        }

        SpawnFood();
    }

    private void Update()
    {
        accumTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.A))
        {
            MoveSnake(Vector3.left);

        } else if (Input.GetKeyDown(KeyCode.D))
        {
            MoveSnake(Vector3.right);

        } else if (Input.GetKeyDown(KeyCode.W))
        {
            MoveSnake(Vector3.up);

        }else if (Input.GetKeyDown(KeyCode.S))
        {
            MoveSnake(Vector3.down);

        }

        if (accumTime >= speed)
        {
            accumTime = 0;

            MoveSnake(lastDirection);
        }
    }

    private void MoveSnake(Vector3 _direction)
    {
        if (_direction + lastDirection == Vector3.zero)
            return;


        bodyDequeued = snakeBody.Dequeue();
        blockedPositions[(int)bodyDequeued.position.x, (int)bodyDequeued.position.y] = 0;

        bodyDequeued.rotation = lastBodyEnqueued.rotation;
        bodyDequeued.position = lastBodyEnqueued.position + _direction;

        if (blockedPositions[(int)bodyDequeued.position.x, (int)bodyDequeued.position.y] == 1) 
        {
            Destroy(foodInstance);
            DeleteBody();
            Start();
        }else
        {
            if (blockedPositions[(int)bodyDequeued.position.x, (int)bodyDequeued.position.y] == 2)
            {
                Debug.Log("FOI!!");
                Destroy(foodInstance);
                SpawnFood();
            }

            blockedPositions[(int)bodyDequeued.position.x, (int)bodyDequeued.position.y] = 1;
            snakeBody.Enqueue(bodyDequeued);
            lastBodyEnqueued = bodyDequeued;
            lastDirection = _direction;
        }
    }

    public void DeleteBody()
    {
        Transform[] body = snakeBody.ToArray();

        for (int i = 0; i < body.Length; i++)
        {
            Destroy(body[i].gameObject);
        }

        Destroy(bodyDequeued.gameObject);
    }

    private void SpawnFood()
    {
        List<Vector2> possiblePositions = new List<Vector2>();

        for (int x = 0; x < GameMaster.Instance.boardWidth + 2; x++)
        {
            for (int y = 0; y < GameMaster.Instance.boardHeight + 2; y++)
            {
                if (blockedPositions[x, y] == 0)
                    possiblePositions.Add(new Vector2(x, y));
            }
        }

        Vector2 positionChosen = possiblePositions[random.Next(possiblePositions.Count)];

        blockedPositions[(int)positionChosen.x, (int)positionChosen.y] = 2;
        foodInstance = Instantiate(foodPrefab, positionChosen, Quaternion.identity);
    }
}
