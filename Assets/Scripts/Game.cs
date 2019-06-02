﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField]
    Vector2Int boardSize = new Vector2Int(11, 11);

    [SerializeField]
    GameBoard board = default;

    [SerializeField]
    GameTileContentFactory tileContentFactory = default;

    [SerializeField]
    EnemyFactory enemyFactory = default;

    [SerializeField, Range(0.1f, 10f)]
    float spawnSpeed = 1f;

    float spawnProgress;

    Ray TouchRay => Camera.main.ScreenPointToRay(Input.mousePosition);

    private void Awake()    //Wird beim öffnen ausgeführt
    {
        board.Initialize(boardSize, tileContentFactory);    //Übergibt den Inhalt an das Board
        board.ShowGrid = true;
    }

    private void OnValidate()   // x und y soll mind. 2 sein, wenn nicht wird es angepasst
    {
        if (boardSize.x < 2)
        {
            boardSize.x = 2;
        }
        if (boardSize.y < 2)
        {
            boardSize.y = 2;
        }
    }

    EnemyCollection enemies = new EnemyCollection();

    void Update()   //Spielereingaben werden hier empfangen
    {
        if (Input.GetMouseButtonDown(0))    //links klick
        {
            HandleTouch();
        }
        else if (Input.GetMouseButtonDown(1))   //rechts klick
        {
            HandleAlternativeTouch();
        }
        if (Input.GetKeyDown(KeyCode.V)) //Ermöglicht ein und Ausblenden der Pfeile
        {
            board.ShowPaths = !board.ShowPaths; //Wechselschalter Trick
        }
        if (Input.GetKeyDown(KeyCode.G))    //Macht das Grid sichtbar
        {
            board.ShowGrid = !board.ShowGrid;
        }

        spawnProgress += spawnSpeed * Time.deltaTime;       //Spawn Prozess der Enemys
        while (spawnProgress >= 1f)
        {
            spawnProgress -= 1f;
            SpawnEnemy();
        }

        enemies.GameUpdate();
    }

    void HandleAlternativeTouch()   //Zielpkt setzen
    {
        GameTile tile = board.GetTile(TouchRay);
        if (tile != null)
        {
            if (Input.GetKey(KeyCode.LeftShift))    //Wenn beim klicken shift gedrückt gehalten wird, wird eien ziel errichtet
            {
                board.ToggleDestination(tile);
            }
            else
            {       //Falls nicht, dann wird ein SpawnPoint gesetzt
                board.ToggleSpawnPoint(tile);
            }
        }
    }

    void HandleTouch()  //Wall bauen
    {
        GameTile tile = board.GetTile(TouchRay);
        if (tile != null)
        {
            //tile.Content = tileContentFactory.Get(GameTileContentType.Destination);
            board.ToggleWall(tile);
        }
    }

    void SpawnEnemy()
    {
        GameTile spawnPoint =
            board.GetSpawnPoint(Random.Range(0, board.SpawnPointCount));
        Enemy enemy = enemyFactory.Get();
        enemy.SpawnOn(spawnPoint);

        enemies.Add(enemy);
    }
}
