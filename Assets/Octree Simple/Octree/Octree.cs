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
        
        // 根包围盒
        foreach (var wObject in worldObjects)
        {
            bounds.Encapsulate(wObject.GetComponent<Collider>().bounds);
        }
        float maxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        Vector3 sizeVector = new Vector3(maxSize, maxSize, maxSize) / 2;
        bounds.SetMinMax(bounds.center - sizeVector, bounds.center + sizeVector);
        
        // 根节点构建
        rootNode = new OctreeNode(bounds, minNodeSize);
        AddWorldObject(worldObjects);
        
        // 本地数据更新
        GetEmptyLeaves(rootNode);
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
        
        // 没有子节点
        if (otn.childrenNodes == null)              
        {
            // 没有物体包含
            if (otn.containedObjects.Count <= 0)
            {
                emptyLeaves.Add(otn);
                navigationGraph.AddNode(otn);
            }
        }
        // 检查子节点
        else
        {
            for (int i = 0; i < otn.childrenNodes.Length; i++)
            {
                GetEmptyLeaves(otn.childrenNodes[i]);
            }
        }
        
    }
}
