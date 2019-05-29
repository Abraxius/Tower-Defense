﻿using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    //Nachbarn!<s

    [SerializeField]
    Transform ground = default;

    [SerializeField]
    GameTile tilePrefab = default;

    Vector2Int size;

    GameTile[] tiles;

    public void Initialize(Vector2Int size)
    {
        this.size = size;
        ground.localScale = new Vector3(size.x, size.y, 1f);

        Vector2 offset = new Vector2(
            (size.x - 1) * 0.5f, (size.y - 1) * 0.5f );

        tiles = new GameTile[size.x * size.y];
        for (int i = 0, y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++, i++)
            {
                GameTile tile = tiles[i] = Instantiate(tilePrefab); //erstellt sie und schreibt sie gleichzeitig in ein array, in einer zeile!!!
                tile.transform.SetParent(transform, false);
                tile.transform.localPosition = new Vector3(x - offset.x, 0f, y - offset.y);

                //Erstellt Nord Süd/ West Ost beziehungen
                if (x > 0)
                {
                    GameTile.MakeEastWestNeighbors(tile, tiles[i - 1]);
                }
                if (y > 0)
                {
                    GameTile.MakeNorthSouthNeighbors(tile, tiles[i - size.x]);
                }

                tile.IsAlternative = (x & 1) == 0;
                				if ((y & 1) == 0) {
					tile.IsAlternative = !tile.IsAlternative;
				}
            }
        }
        FindPaths(); //Aufruf der Pfad Methode
    }

    //Sicherstellung, dass alle Kacheln einen Pfad haben
    Queue<GameTile> searchFrontier = new Queue<GameTile>();

    void FindPaths()
    {
        foreach (GameTile tile in tiles)
        {
            tile.ClearPath();
        }
        tiles[tiles.Length / 2].BecomeDestination();       //Bestimmt das Ziel der Pfeile
        searchFrontier.Enqueue(tiles[tiles.Length / 2]);   //Bestimmt das Ziel der Pfeile

        while (searchFrontier.Count > 0)    //Solange wiederholen, solang sich die Kacheln in den Grenzen befinden
        {
            GameTile tile = searchFrontier.Dequeue();
            if (tile != null)   //überprüfen ob davor noch eine Kachel ist
            {
                if (tile.IsAlternative)
                {
                    searchFrontier.Enqueue(tile.GrowPathNorth());
                    searchFrontier.Enqueue(tile.GrowPathSouth());
                    searchFrontier.Enqueue(tile.GrowPathEast());
                    searchFrontier.Enqueue(tile.GrowPathWest());
                }
                else
                {
                    searchFrontier.Enqueue(tile.GrowPathWest());
                    searchFrontier.Enqueue(tile.GrowPathEast());
                    searchFrontier.Enqueue(tile.GrowPathSouth());
                    searchFrontier.Enqueue(tile.GrowPathNorth());
                }
            }
        }

        foreach (GameTile tile in tiles)
        {
            tile.ShowPath();
        }
    }

    public GameTile GetTile(Ray ray)    //Kann passieren, dass ein Ray nichts trifft, also geben wir erstma immer null zurück
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) { //wenn eine Kachel getroffen wurde, Aufruf legt den Wert der variable fest
            int x = (int)(hit.point.x + size.x * 0.5f);
            int y = (int)(hit.point.z + size.y * 0.5f);
            if (x >= 0 && x < size.x && y >= 0 && y < size.y)   //gibt nur einen Wert zurück, wenn die Kachel im erlaubten Bereich liegt
            {
                return tiles[x + y * size.x];
            }
        }
        return null;
    }
}