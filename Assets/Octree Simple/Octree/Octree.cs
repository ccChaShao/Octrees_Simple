using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octree
{
    public OctreeNode rootNode;
    public List<OctreeNode> emptyLeaves = new ();
    public Graph navigationGraph;
    
    public Octree(GameObject[] worldObjects, float minNodeSize, Graph navgraph)
    {
        navigationGraph = navgraph;
        
        // 包围盒创建
        Bounds bounds = new();
        foreach (var wObject in worldObjects)
        {
            bounds.Encapsulate(wObject.GetComponent<Collider>().bounds);
        }
        float maxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        Vector3 sizeVector = new Vector3(maxSize, maxSize, maxSize) * 1.0f;
        bounds.SetMinMax(bounds.center - sizeVector, bounds.center + sizeVector);
        
        // 八叉树创建
        rootNode = new OctreeNode(bounds, minNodeSize, null);
        InitEmptyLeaves();
        AddWorldObject(worldObjects);    
        navigationGraph.ConnectNodeNodeNeighbours();
    }

    public void AddWorldObject(GameObject[] worldObjects)
    {
        foreach (var go in worldObjects)
        {
            rootNode.DivideAndAdd(go);
        }
    }

    public void InitEmptyLeaves()
    {
        emptyLeaves.Clear();
        InitEmptyLeaves(rootNode);       
    }

    public void InitEmptyLeaves(OctreeNode otn)
    {
        if (otn == null)
            return;
        
        // 根节点记录
        if (otn.children == null)
        {
            if (otn.containedObjects.Count <= 0)
            {
                emptyLeaves.Add(otn);           // 根节点记录
                navigationGraph.AddNode(otn);   // 根节点记录
            }
        }
        // 子节点递归查询
        else
        {
            for (int i = 0; i < otn.children.Length; i++)
            {
                InitEmptyLeaves(otn.children[i]);
            }
        }
    }

    public int FindEmptyLeafNode(OctreeNode node, Vector3 position)
    {
        int found = -1;

        if (node == null)
            return -1;
        
        // 查到叶子节点则跳出
        if (node.children == null || node.children.Length <= 0)
        {
            if (node.bounds.Contains(position) && node.containedObjects.Count <= 0)
            {
                return node.id;
            }
        }
        else
        {
            for (int i = 0; i < node.children.Length; i++)
            {
                found = FindEmptyLeafNode(node.children[i], position);
                if (found != -1)
                {
                    break;          
                }
            }
        }
        
        return found;
    }

    public void DrawDebug() { }
}
