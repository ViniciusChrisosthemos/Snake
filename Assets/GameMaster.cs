using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static GameMaster Instance { get; private set; }

    [SerializeField] public int boardWidth;
    [SerializeField] public int boardHeight;
    
    private void Awake()
    {
        Instance = this;
    }


}
