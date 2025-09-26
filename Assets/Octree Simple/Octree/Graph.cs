using System.Collections.Generic;
using UnityEngine;

public class Graph
{
    public List<Edge> edgeList = new();
    public List<Node> nodeList = new();
    public List<Node> pathList = new();

    public void AddNode(OctreeNode otn)
    {
        if (FindNode(otn.id) == null)
        {
            Node node = new Node(otn);           // 一个树节点 = 一个路径节点
            nodeList.Add(node);
        }
    }

    public void AddEdge(OctreeNode fromOtn, OctreeNode toOtn)
    {
        Node from = FindNode(fromOtn.id);
        Node to = FindNode(toOtn.id);
        if (from != null && to != null)
        {
            Edge edge = new(from, to);          // 正向
            edgeList.Add(edge);
            from.edgeList.Add(edge);
            
            Edge reverseEdge = new(to, from);   // 反向
            edgeList.Add(reverseEdge);
            to.edgeList.Add(reverseEdge);
        }
    }

    public Node FindNode(int otnId)
    {
        foreach (var node in nodeList)
        {
            if (node.octreeNode.id == otnId)
                return node;
        }
        return null;
    }

    public void DrawDebug()
    {
        // 边界球体绘制
        for (int i = 0; i < nodeList.Count; i++)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(nodeList[i].octreeNode.nodeBounds.center, 0.25f);
        }
        
        // 边界路径绘制
        for (int i = 0; i < edgeList.Count; i++)
        {
            Debug.DrawLine(
                edgeList[i].startNode.octreeNode.nodeBounds.center,
                edgeList[i].endNode.octreeNode.nodeBounds.center,
                Color.yellow
            );
        }
    }
}
