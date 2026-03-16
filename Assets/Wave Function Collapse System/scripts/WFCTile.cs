using UnityEngine;

[CreateAssetMenu(fileName = "WFCTile", menuName = "WFC/Tile")]
public class WFCTile : ScriptableObject
{
    public GameObject prefab;

    [Header("Edges")]
    public string top;
    public string right;
    public string bottom;
    public string left;

    [Header("Weight")]
    public float weight = 1f;
}