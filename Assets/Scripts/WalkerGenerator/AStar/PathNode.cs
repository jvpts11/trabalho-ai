using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    private Grid[,] grid;

    private int x;
    private int y;

    private int gCost;
    private int hCost;
    private int fCost;

    public PathNode nodeBefore;

    public PathNode(Grid[,] grid, int x, int y, int gCost, int hCost, int fCost)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        this.gCost = gCost;
        this.hCost = hCost;
        this.fCost = fCost;
    }

    public override string ToString()
    {
        return x + ","+ y;
    }
}
