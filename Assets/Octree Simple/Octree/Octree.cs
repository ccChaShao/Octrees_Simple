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
        rootNode = new OctreeNode(bounds, minNodeSize, null);
        AddWorldObject(worldObjects);
        
        // 本地数据更新
        GetEmptyLeaves(rootNode);
        ProcrssConnections();

        Debug.Log("charsiew : [Octree] : -----------------"+navigationGraph.edgeList.Count);
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
        // 子节点查询
        else
        {
            for (int i = 0; i < otn.childrenNodes.Length; i++)
            {
                // 递归处理
                GetEmptyLeaves(otn.childrenNodes[i]);
                
                // // 节点路径绘制
                // for (int j = 0; j < otn.childrenNodes.Length; j++)
                // {
                //     if (i == j)
                //         continue;
                //     // 同层级路径连接
                //     navigationGraph.AddEdge(otn.childrenNodes[i], otn.childrenNodes[j]);
                // }
            }
        }
    }

    public void ProcrssConnections()
    {
        Dictionary<int, int> subGraphConnections = new();
        
        foreach (var otnI in emptyLeaves)
        {
            foreach (var otnJ in emptyLeaves)
            {
                if(otnI.id == otnJ.id)
                    continue;

                // 同层级连接
                if (otnI.parent.id == otnJ.parent.id)
                {
                    navigationGraph.AddEdge(otnI, otnJ);
                }
                // 不同层级连接
                else
                {
                    if (subGraphConnections.TryAdd(otnI.parent.id, otnJ.parent.id))         // 假如已经连接过，就不需要再连接了，暂时这样处理
                    {
                        Vector3 direction = otnJ.nodeBounds.center - otnI.nodeBounds.center;
                        float accuracy = 1;
                        if (!Physics.SphereCast(otnI.nodeBounds.center, accuracy, direction, out RaycastHit hitInfo))
                        {
                            navigationGraph.AddEdge(otnI, otnJ);
                        }
                    }
                }
            }
        }
    }
}
