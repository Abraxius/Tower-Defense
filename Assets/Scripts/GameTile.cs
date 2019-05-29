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
    //----------------------------------------------------------------------------------------------------------- Methoden
    public bool HasPath => distance != int.MaxValue; //Überprüft ob alle Kacheln einen Pfad haben (Getter Methode)

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
    }

    GameTile GrowPathTo(GameTile neighbor)
    {
        Debug.Assert(HasPath, "Kein Pfad!");
        if (neighbor == null || neighbor.HasPath)
        {
            return null;
        }
        neighbor.distance = distance + 1;
        neighbor.nextOnPath = this;
        return neighbor;
    }

    public GameTile GrowPathNorth() => GrowPathTo(north);   //Methoden um den Pfad in bestimmte Richtungen zu vergrößern

    public GameTile GrowPathEast() => GrowPathTo(east);

    public GameTile GrowPathSouth() => GrowPathTo(south);

    public GameTile GrowPathWest() => GrowPathTo(west);

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


}
