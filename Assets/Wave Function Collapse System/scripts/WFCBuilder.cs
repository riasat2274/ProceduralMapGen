using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class WFCBuilder : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float spacing = 2f;

    public List<WFCTile> tiles;

    WFCCell[,] grid;
    public UnityEvent onGenerationComplete;


    void Start()
    {
        //InitializeGrid();
        //RunWFC();
    }

    public void GenerateNew()
    {
        if (spawnedTiles.Count > 0)
        {
            for (int i = 0; i < spawnedTiles.Count; i++)
            {
                Destroy(spawnedTiles[i]);
            }
            spawnedTiles.Clear();
        }
        InitializeGrid();
        RunWFC();
    }
    public void ClearOldTiles()
    {
        if (spawnedTiles.Count > 0)
        {
            for (int i = 0; i < spawnedTiles.Count; i++)
            {
                Destroy(spawnedTiles[i]);
            }
            spawnedTiles.Clear();
        }
    }
    void InitializeGrid()
    {
        grid = new WFCCell[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = new WFCCell(tiles.Count, x, y);
            }
        }
    }

    void RunWFC()
    {
        while (true)
        {
            WFCCell cell = GetLowestEntropyCell();

            if (cell == null)
                break;

            CollapseCell(cell);
            Propagate(cell);
        }

        //SpawnTiles();
        StartCoroutine(SpawnTilesRoutine());
    }
    

    WFCCell GetLowestEntropyCell()
    {
        WFCCell best = null;
        int lowestEntropy = int.MaxValue;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                WFCCell cell = grid[x, y];

                if (cell.collapsed)
                    continue;

                int entropy = cell.Entropy();

                if (entropy < lowestEntropy && entropy > 0)
                {
                    lowestEntropy = entropy;
                    best = cell;
                }
            }
        }

        return best;
    }

    void CollapseCell(WFCCell cell)
    {
        int tileIndex = cell.possibleTiles[
            Random.Range(0, cell.possibleTiles.Count)
        ];

        cell.possibleTiles.Clear();
        cell.possibleTiles.Add(tileIndex);
        cell.collapsed = true;
    }

    void Propagate(WFCCell startCell)
    {
        Queue<WFCCell> queue = new Queue<WFCCell>();
        queue.Enqueue(startCell);

        while (queue.Count > 0)
        {
            WFCCell cell = queue.Dequeue();

            List<WFCCell> neighbors = GetNeighbors(cell);

            foreach (var neighbor in neighbors)
            {
                if (neighbor.collapsed)
                    continue;

                bool changed = ReduceEntropy(cell, neighbor);

                if (changed)
                    queue.Enqueue(neighbor);
            }
        }
    }

    bool ReduceEntropy(WFCCell source, WFCCell target)
    {
        bool changed = false;

        List<int> validTiles = new List<int>();

        foreach (int targetTileIndex in target.possibleTiles)
        {
            WFCTile targetTile = tiles[targetTileIndex];

            bool valid = false;

            foreach (int sourceTileIndex in source.possibleTiles)
            {
                WFCTile sourceTile = tiles[sourceTileIndex];

                if (Match(source, target, sourceTile, targetTile))
                {
                    valid = true;
                    break;
                }
            }

            if (valid)
                validTiles.Add(targetTileIndex);
        }

        if (validTiles.Count != target.possibleTiles.Count)
        {
            target.possibleTiles = validTiles;
            changed = true;
        }

        return changed;
    }

    bool Match(WFCCell a, WFCCell b, WFCTile tileA, WFCTile tileB)
    {
        int dx = b.x - a.x;
        int dy = b.y - a.y;

        if (dx == 1)
            return tileA.right == tileB.left;

        if (dx == -1)
            return tileA.left == tileB.right;

        if (dy == 1)
            return tileA.top == tileB.bottom;

        if (dy == -1)
            return tileA.bottom == tileB.top;

        return false;
    }

    List<WFCCell> GetNeighbors(WFCCell cell)
    {
        List<WFCCell> neighbors = new List<WFCCell>();

        int x = cell.x;
        int y = cell.y;

        if (x > 0) neighbors.Add(grid[x - 1, y]);
        if (x < width - 1) neighbors.Add(grid[x + 1, y]);
        if (y > 0) neighbors.Add(grid[x, y - 1]);
        if (y < height - 1) neighbors.Add(grid[x, y + 1]);

        return neighbors;
    }
    private List<GameObject> spawnedTiles = new List<GameObject>();
    void SpawnTiles()
    {
        spawnedTiles = new List<GameObject>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int tileIndex = grid[x, y].possibleTiles[0];

                Vector3 pos = new Vector3(x * spacing, 0, y * spacing);

                spawnedTiles.Add(Instantiate(
                    tiles[tileIndex].prefab,
                    pos,
                    tiles[tileIndex].prefab.transform.rotation
                ));
            }
        }
    }
    IEnumerator SpawnTilesRoutine()
    {
        spawnedTiles = new List<GameObject>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int tileIndex = grid[x, y].possibleTiles[0];

                Vector3 pos = new Vector3(x * spacing, 0, y * spacing);

                spawnedTiles.Add(Instantiate(
                    tiles[tileIndex].prefab,
                    pos,
                    tiles[tileIndex].prefab.transform.rotation
                ));
                yield return new WaitForSeconds(0.05f);
            }
        }
        yield return null;
        onGenerationComplete?.Invoke();
    }
}
[CustomEditor(typeof(WFCBuilder))]
public class WFCBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        WFCBuilder bb = (WFCBuilder)target;
        if (GUILayout.Button("Genarate"))
        {
            bb.GenerateNew();
        }
        //if (GUILayout.Button("Clear"))
        //{

        //}
    }
}