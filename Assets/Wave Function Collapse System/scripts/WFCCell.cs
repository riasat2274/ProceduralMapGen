using System.Collections.Generic;
using System;

[System.Serializable]
public class WFCCell
{
    public bool collapsed = false;
    public List<int> possibleTiles;

    public int x;
    public int y;

    public WFCCell(int tileCount, int x, int y)
    {
        this.x = x;
        this.y = y;

        possibleTiles = new List<int>();

        for (int i = 0; i < tileCount; i++)
            possibleTiles.Add(i);
    }

    public int Entropy()
    {
        return possibleTiles.Count;
    }
}