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
        
        Bounds bounds = new();
        foreach (var wObject in worldObjects)
        {
            bounds.Encapsulate(wObject.GetComponent<Collider>().bounds);
        }
        float maxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        Vector3 sizeVector = new Vector3(maxSize, maxSize, maxSize) * 1.0f;
        bounds.SetMinMax(bounds.center - sizeVector, bounds.center + sizeVector);
        rootNode = new OctreeNode(bounds, minNodeSize, null);
        
        AddWorldObject(worldObjects);
        GetEmptyLeaves(rootNode);           
        navigationGraph.ConnectNodeNodeNeighbours();
    }

    public void AddWorldObject(GameObject[] worldObjects)
    {
        foreach (var go in worldObjects)
        {
            rootNode.AddWorldObject(go);
        }
    }

    public void GetEmptyLeaves(OctreeNode otn)
    {
        if (otn == null)
            return;
        
        // 根节点记录
        if (otn.childrenNodes == null)
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
            for (int i = 0; i < otn.childrenNodes.Length; i++)
            {
                GetEmptyLeaves(otn.childrenNodes[i]);
            }
        }
    }

    public void DrawDebug()
    {
    }
}
