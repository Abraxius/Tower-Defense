using UnityEngine;

public class Enemy : MonoBehaviour
{

    EnemyFactory originFactory;

    //Variablen um den Pfad im Auge zu behalten
    GameTile tileFrom, tileTo;
    Vector3 positionFrom, positionTo;
    float progress;

    public EnemyFactory OriginFactory
    {
        get => originFactory;
        set
        {
            Debug.Assert(originFactory == null, "Redefined origin factory!");
            originFactory = value;
        }
    }

    public void SpawnOn(GameTile tile)
    {
        Debug.Assert(tile.NextTileOnPath != null, "Nowhere to go!", this);
        tileFrom = tile;
        tileTo = tile.NextTileOnPath;
        positionFrom = tileFrom.transform.localPosition;
        positionTo = tileFrom.ExitPoint;    //ExitPoint, weil KachelRand nicht Zentrum
        transform.localRotation = tileFrom.PathDirection.GetRotation();
        progress = 0f;
    }

    public bool GameUpdate()    //Kontrolliert ob er noch aktiv ist
    {
        progress += Time.deltaTime;
        while (progress >= 1f)
        {
            tileFrom = tileTo;
            tileTo = tileTo.NextTileOnPath;
            if (tileTo == null) //Kontrolliert ob es vorwärts geht, ansonsten geht man zu
            {
                OriginFactory.Reclaim(this);
                return false;
            }
            positionFrom = positionTo;
            positionTo = tileFrom.ExitPoint;   //ExitPoint, weil KachelRand nicht Zentrum
            transform.localRotation = tileFrom.PathDirection.GetRotation();
            progress -= 1f;
        }
        transform.localPosition = Vector3.LerpUnclamped(positionFrom, positionTo, progress);    //Wenn die Ziel Kachel erreicht ist, verschiebt es sich automatisch zur nächsten und zu nächsten. Immer Stück für stück vorran, damit der Pfad aktualisiert werden kann.
        return true;
    }
}