using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Node path;
    public OctreeNode octreeNode;
    public List<Edge> edgeList = new ();            // 以自己为出发的所有正向边界；
    
    public Node(OctreeNode octreeNode)
    {
        this.octreeNode = octreeNode;
        this.path = null;
    }
}
