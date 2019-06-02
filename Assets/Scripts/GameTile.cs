using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour
{

    [SerializeField]
    Transform arrow = default;  //default, damit kein compilierungs error erscheint, weil kein wert zugewiesen ist

    GameTile north, east, south, west, nextOnPath;

    int distance;

    public bool IsAlternative { get; set; } //Alternative ermöglicht Zick Zack Muster (Diagonal)

    GameTileContent content;

    public Vector3 ExitPoint { get; private set; }

    public Direction PathDirection { get; private set; }    //Eigenschaft für die Pfadrichtung
    //----------------------------------------------------------------------------------------------------------- Methoden
    public bool HasPath => distance != int.MaxValue; //Überprüft ob alle Kacheln einen Pfad haben (Getter Methode)

    public GameTile NextTileOnPath => nextOnPath; //Getter Methode die den Feinden hilft herauszufinden wo sie lang müssen
    public static void MakeEastWestNeighbors(GameTile east, GameTile west)
    {
        Debug.Assert(west.east == null && east.west == null, "Neudefinierte Nachbarn!");
        west.east = east;
        east.west = west;
    }

    public static void MakeNorthSouthNeighbors(GameTile north, GameTile south)
    {
        Debug.Assert(south.north == null && north.south == null, "Neudefinierte Nachbarn!");
        south.north = north;
        north.south = south;
    }

    public void ClearPath() //stellt den alten Status von Path wieder her
    {
        distance = int.MaxValue;
        nextOnPath = null;
    }

    public void BecomeDestination() //stellt den neuen Pfad ein
    {
        distance = 0;
        nextOnPath = null;
        ExitPoint = transform.localPosition;        //Bei der Zielposition ist der Endpunkt das Kachel Zentrum, bei allen anderen ist der Endpunkt der Rand. Weil in der Mitte der Ausgang ist
    }

    GameTile GrowPathTo(GameTile neighbor, Direction direction)
    {
        Debug.Assert(HasPath, "Kein Pfad!");
        if (neighbor == null || neighbor.HasPath)
        {
            return null;
        }
        neighbor.distance = distance + 1;
        neighbor.nextOnPath = this;
        neighbor.ExitPoint = neighbor.transform.localPosition + direction.GetHalfVector();   //Sollen sich von Kanten zu Kanten bewegen und nicht von Zentrum zu Zentrum der Kacheln EDIT: Winkelberechnung nun
        neighbor.PathDirection = direction;
        return neighbor.Content.Type != GameTileContentType.Wall ? neighbor : null; //+ Kontrolle, dass Wände nicht in den Weg mit einbezogen werdenn
    }

    public GameTile GrowPathNorth() => GrowPathTo(north, Direction.South);   //Methoden um den Pfad in bestimmte Richtungen zu vergrößern

    public GameTile GrowPathEast() => GrowPathTo(east, Direction.West);

    public GameTile GrowPathSouth() => GrowPathTo(south, Direction.North);  //xx, Direction..) weil, sich die Blickrichtung umdrehen soll, wenn sich der Pfad umkehrt

    public GameTile GrowPathWest() => GrowPathTo(west, Direction.East);

    static Quaternion   //Statische Quaternion, die die Drehrichtung der Pfeile festlegen
        northRotation = Quaternion.Euler(90f, 0f, 0f),
        eastRotation = Quaternion.Euler(90f, 90f, 0f),
        southRotation = Quaternion.Euler(90f, 180f, 0f),
        westRotation = Quaternion.Euler(90f, 270f, 0f);

    public void ShowPath()  //Bestimmt, wenn es kein Ziel zum drauf zeigen gibt, wird der Pfeil deaktiviert, ansonsten dreht es den Pfeil in die richtige Richtung durch nextOnPath
    {
        if (distance == 0)
        {
            arrow.gameObject.SetActive(false);
            return;
        }
        arrow.gameObject.SetActive(true);
        arrow.localRotation =
            nextOnPath == north ? northRotation :
            nextOnPath == east ? eastRotation :
            nextOnPath == south ? southRotation :
            westRotation;
    }

    public GameTileContent Content
    {
        get => content;
        set
        {
            Debug.Assert(value != null, "Kein Inhalt zugewiesen!"); //Der Einzige Ort an dem wir überprüfen müssen ob Inhalt in den Kachel vorhanden ist
            if (content != null)
            {
                content.Recycle();
            }
            content = value;
            content.transform.localPosition = transform.localPosition;
        }
    }

    public void HidePath()  //Versteckt die Pfeile
    {
        arrow.gameObject.SetActive(false);
    }


}
