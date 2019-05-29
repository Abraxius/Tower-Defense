using UnityEngine;
using UnityEditor;

public class GameTileContentFactoryAsset
{
    [MenuItem("Assets/Create/GameTileContentFactory")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<GameTileContentFactory>();
    }
}