using UnityEngine;
using UnityEditor;

public class EnemyFactoryAsset
{
    [MenuItem("Assets/Create/EnemyFactory")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<EnemyFactory>();
    }
}