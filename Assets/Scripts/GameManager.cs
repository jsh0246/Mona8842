using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Tilemap tileMap;

    private void Awake()
    {
        Instance = this;
    }
}
