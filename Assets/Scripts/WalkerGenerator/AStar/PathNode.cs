public class PathNode
{
    public enum NodeType
    {
        Empty,
        Wall,
        Floor,
        GreenSquare,
        YellowSquare,
    }

    public NodeType nodeType;

    public int x;
    public int y;

    public int gCost;
    public int hCost;
    public int fCost;

    public PathNode nodeBefore;

    public PathNode(NodeType nodeType, int x, int y, int gCost, int hCost, int fCost)
    {
        this.nodeType = nodeType;
        this.x = x;
        this.y = y;
        this.gCost = gCost;
        this.hCost = hCost;
        this.fCost = fCost;
    }

    public override string ToString()
    {
        return x + "," + y;
    }

    public NodeType GetNodeType()
    {
        return nodeType;
    }
}
