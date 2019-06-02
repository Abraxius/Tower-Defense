using UnityEngine;

public class Enemy : MonoBehaviour
{

    EnemyFactory originFactory;

    //Variablen um den Pfad im Auge zu behalten
    GameTile tileFrom, tileTo;
    Vector3 positionFrom, positionTo;
    float progress, progressFactor; //progressFactor -> sorgt für unterschiedliche Geschwindigkeiten der Enemys

    //Variablen für die Richtungswinkel
    Direction direction;
    DirectionChange directionChange;
    float directionAngleFrom, directionAngleTo;

    [SerializeField]
    Transform model = default;

    //------------------------------------------------------Methoden
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
        progress = 0f;
        PrepareIntro(); //Auslagerung von Code von SpawnOn, damit es übersichtlicher wird
    }

    public bool GameUpdate()    //Kontrolliert ob er noch aktiv ist
    {
        progress += Time.deltaTime * progressFactor; ;  //progressFactor -> sorgt für unterschiedliche Geschwindigkeiten der Enemys
        while (progress >= 1f)
        {
            if (tileTo == null) //Kontrolliert ob es vorwärts geht, ansonsten geht man zu
            {
                OriginFactory.Reclaim(this);
                return false;
            }
            progress = (progress - 1f) / progressFactor;
            PrepareNextState();
            progress *= progressFactor;
        }
        if (directionChange == DirectionChange.None)
        {
            transform.localPosition = Vector3.LerpUnclamped(positionFrom, positionTo, progress);    //Wenn die Ziel Kachel erreicht ist, verschiebt es sich automatisch zur nächsten und zu nächsten. Immer Stück für stück vorran, damit der Pfad aktualisiert werden kann.
        } else { 
            float angle = Mathf.LerpUnclamped(
                directionAngleFrom, directionAngleTo, progress
            );
            transform.localRotation = Quaternion.Euler(0f, angle, 0f);
        }
        return true;
    }

    void PrepareNextState()
    {
        tileFrom = tileTo;
        tileTo = tileTo.NextTileOnPath;
        positionFrom = positionTo;
        if (tileTo == null)
        {
            PrepareOutro();
            return;
        }
        positionTo = tileFrom.ExitPoint;
        directionChange = direction.GetDirectionChangeTo(tileFrom.PathDirection);
        direction = tileFrom.PathDirection;
        directionAngleFrom = directionAngleTo;
        switch (directionChange)    //switch richtungswechsel, um zu entscheiden, welche der 4 Methoden aufgerufen werden soll
        {
            case DirectionChange.None: PrepareForward(); break;
            case DirectionChange.TurnRight: PrepareTurnRight(); break;
            case DirectionChange.TurnLeft: PrepareTurnLeft(); break;
            default: PrepareTurnAround(); break;
        }
    }

    void PrepareForward()
    {
        transform.localRotation = direction.GetRotation();
        directionAngleTo = direction.GetAngle();
        model.localPosition = Vector3.zero;
        progressFactor = 1f;//damit die umdrehungen nicht so lang dauern
    }

    void PrepareTurnRight()
    {
        directionAngleTo = directionAngleFrom + 90f;
        model.localPosition = new Vector3(-0.5f, 0f);
        transform.localPosition = positionFrom + direction.GetHalfVector();
        progressFactor = 1f / (Mathf.PI * 0.25f);   //damit die umdrehungen nicht so lang dauern
    }

    void PrepareTurnLeft()
    {
        directionAngleTo = directionAngleFrom - 90f;
        model.localPosition = new Vector3(0.5f, 0f);
        transform.localPosition = positionFrom + direction.GetHalfVector();
        progressFactor = 1f / (Mathf.PI * 0.25f);   //damit die umdrehungen nicht so lang dauern
    }

    void PrepareTurnAround()
    {
        directionAngleTo = directionAngleFrom + 180f;
        model.localPosition = Vector3.zero;
        transform.localPosition = positionFrom;
        progressFactor = 2f;//damit die umdrehungen nicht so lang dauern
    }

    void PrepareIntro() //von zentrum -> rand, kann sich der Winkel nicht ändern
    {
        positionFrom = tileFrom.transform.localPosition;
        positionTo = tileFrom.ExitPoint;
        direction = tileFrom.PathDirection;
        directionChange = DirectionChange.None;
        directionAngleFrom = directionAngleTo = direction.GetAngle();
        transform.localRotation = direction.GetRotation();
        progressFactor = 2f;//damit die umdrehungen nicht so lang dauern
    }

    void PrepareOutro()
    {
        positionTo = tileFrom.transform.localPosition;
        directionChange = DirectionChange.None;
        directionAngleTo = direction.GetAngle();
        model.localPosition = Vector3.zero;
        transform.localRotation = direction.GetRotation();
        progressFactor = 2f;
    }
}