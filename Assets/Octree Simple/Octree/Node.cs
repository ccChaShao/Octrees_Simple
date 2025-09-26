using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public OctreeNode octreeNode;           // 
    public List<Edge> edgeList = new ();    // 以自己为出发的所有正向边界；
    
    // A*
    public Node path;
    public float g, h;          // g：已用代价；h：启发函数代价；
    public float f => g + h;    // f：总代价
    public Node cameFrom;       // 入口节点
    
    public Node(OctreeNode octreeNode)
    {
        path = null;
        this.octreeNode = octreeNode;
    }
}
