using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTileContent : MonoBehaviour    
    //Stellt ein was für eine Art Kachel die Kachel ist, zB Zielkachel
{
    [SerializeField]
    GameTileContentType type = default;

    public GameTileContentType Type => type;

    GameTileContentFactory originFactory;

    //Methoden----------------------------
    public GameTileContentFactory OriginFactory
    {
        get => originFactory;
        set
        {
            Debug.Assert(originFactory == null, "Redefined origin factory!");
            originFactory = value;
        }
    }

    public void Recycle()
    {
        originFactory.Reclaim(this);
    }

}
