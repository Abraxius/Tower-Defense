using System.Collections.Generic;
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

    GameTileContentFactory contentFactory;

    bool showGrid, showPaths;

    [SerializeField]
    Texture2D gridTexture = default;
    //Methoden-----------------------------
    public void Initialize(Vector2Int size, GameTileContentFactory contentFactory)
    {
        this.size = size;
        this.contentFactory = contentFactory;
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

                tile.Content = contentFactory.Get(GameTileContentType.Empty);   //Gibt jeder Kachel einen leeren Empty Inhalt
            }
        }
        //FindPaths(); //Aufruf der Pfad Methode
        ToggleDestination(tiles[tiles.Length / 2]);
    }

    //Sicherstellung, dass alle Kacheln einen Pfad haben
    Queue<GameTile> searchFrontier = new Queue<GameTile>();

    bool FindPaths()        
    {
        foreach (GameTile tile in tiles)    //Kontrolle ob alle Kacheln einen gültigen Pfad haben, wenn nicht wird der letzte Befehl zurück gesetzt
        {
            if (!tile.HasPath)
            {
                return false;
            }
        }
        foreach (GameTile tile in tiles)
        {
            if (tile.Content.Type == GameTileContentType.Destination)
            {
                tile.BecomeDestination();       //Bestimmt das Ziel der Pfeile
                searchFrontier.Enqueue(tile);   //Bestimmt das Ziel der Pfeile
            } else
            {
                tile.ClearPath();
            }
        }

        if (searchFrontier.Count == 0)
        {
            return false;
        }

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
        if (showPaths) //wird nur angezeigt, wenn bool == true
        {
            foreach (GameTile tile in tiles)
            {
                tile.ShowPath();
            }
        }
        return true;
    }

    public GameTile GetTile(Ray ray)    //Kann passieren, dass ein Ray nichts trifft, also geben wir erstma immer null zurück
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) { //wenn eine Kachel angeklickt wurde, Aufruf legt den Wert der variable fest
            int x = (int)(hit.point.x + size.x * 0.5f);
            int y = (int)(hit.point.z + size.y * 0.5f);
            if (x >= 0 && x < size.x && y >= 0 && y < size.y)   //gibt nur einen Wert zurück, wenn die Kachel im erlaubten Bereich liegt
            {
                return tiles[x + y * size.x];
            }
        }
        return null;
    }

    public void ToggleDestination (GameTile tile)
    {
        if (tile.Content.Type == GameTileContentType.Destination)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Empty);
            if (!FindPaths())   //Falls es zu einem Fehler kommt, dann wird die Änderung rückgängig gemacht
            {
                tile.Content = contentFactory.Get(GameTileContentType.Destination);
                FindPaths();
            }
        } else if (tile.Content.Type == GameTileContentType.Empty)  //macht das ersetzen von Walls durch Zielpkt unmöglich
        {
            tile.Content = contentFactory.Get(GameTileContentType.Destination);
            FindPaths();
        }
    }

    public void ToggleWall(GameTile tile)
    {
        if (tile.Content.Type == GameTileContentType.Wall)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Empty);
            FindPaths();
        }
        else if (tile.Content.Type == GameTileContentType.Empty)      //Wall kann nur gesetzt werden, wenn es kein Zielpunkt ist
        {
            tile.Content = contentFactory.Get(GameTileContentType.Wall);
            if (!FindPaths())
            {
                tile.Content = contentFactory.Get(GameTileContentType.Empty);
                FindPaths();
            }
        }
    }

    public bool ShowPaths   //Setzt das Bool Arrow Anzeigen standardmässig auf false
    {
        get => showPaths;
        set
        {
            showPaths = value;
            if (showPaths)
            {
                foreach (GameTile tile in tiles)
                {
                    tile.ShowPath();
                }
            }
            else
            {
                foreach (GameTile tile in tiles)
                {
                    tile.HidePath();
                }
            }
        }
    }

    public bool ShowGrid    //Macht das Kastenraster sichtbar
    {
        get => showGrid;
        set
        {
            showGrid = value;
            Material m = ground.GetComponent<MeshRenderer>().material;
            if (showGrid)
            {
                m.mainTexture = gridTexture;
                m.SetTextureScale("_MainTex", size);    //verhindert das ein hässlicher Rand entsteht, in dem die Größe passen angepasst wird
            }
            else
            {
                m.mainTexture = null;
            }
        }
    }
}
