using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct OctreeObject
{
    public Bounds bounds;
    public GameObject go;

    public OctreeObject(GameObject gameObject)
    {
        go = gameObject;
        bounds = go.GetComponent<Collider>().bounds;
    }
}

/// <summary>
/// 八叉树节点
/// </summary>
public class OctreeNode
{
    public float minSize;
    public Bounds nodeBounds = new();
    public Bounds[] childBounds = null;
    
    public OctreeNode[] childrenNodes = null;
    public List<OctreeObject> containedObjects = new();

    public OctreeNode(Bounds bounds, float minSize)
    {
        nodeBounds = bounds;
        this.minSize = minSize;
        BuildChildBounds();
    }

    private void BuildChildBounds()
    {
        float quarter = nodeBounds.size.x / 4f;
        Vector3 childSize = new Vector3(nodeBounds.size.x / 2f, nodeBounds.size.x / 2f, nodeBounds.size.x / 2f);
        childBounds = new[]
        {
            // 4 2 1
            new Bounds(nodeBounds.center + new Vector3(-quarter, -quarter, -quarter), childSize),     // 0
            new Bounds(nodeBounds.center + new Vector3(-quarter, -quarter, quarter), childSize),      // 1    
            new Bounds(nodeBounds.center + new Vector3(-quarter, quarter, -quarter), childSize),      // 2
            new Bounds(nodeBounds.center + new Vector3(-quarter, quarter, quarter), childSize),       // 3
            new Bounds(nodeBounds.center + new Vector3(quarter, -quarter, -quarter), childSize),      // 4
            new Bounds(nodeBounds.center + new Vector3(quarter, -quarter, quarter), childSize),       // 5
            new Bounds(nodeBounds.center + new Vector3(quarter, quarter, -quarter), childSize),       // 6
            new Bounds(nodeBounds.center + new Vector3(quarter, quarter, quarter), childSize),        // 7
        };
    }

    private void DivideAndAdd(GameObject worldObject)
    {
        OctreeObject octObj = new OctreeObject(worldObject);
        if (!CheckBoundContains(nodeBounds, octObj.bounds))
        {
            return;
        }
        // 最底层，直接添加
        if (nodeBounds.size.x <= minSize)
        {
            containedObjects.Add(octObj);           
            return;
        }

        // 内部分割
        bool dividing = false;
        if (childrenNodes == null) childrenNodes = new OctreeNode[8];
        for (int i = 0; i < 8; i++)
        {
            if (childrenNodes[i] == null) childrenNodes[i] = new OctreeNode(childBounds[i], minSize);

            if (CheckBoundContains(childrenNodes[i].nodeBounds, octObj.bounds))
            {
                dividing = true;
                childrenNodes[i].DivideAndAdd(worldObject);
            }
        }
        if (!dividing)
        {
            containedObjects.Add(octObj);           // 没有小结点能够包含，则直接包含；
            // childrenNodes = null;
        }
    }

    private bool CheckBoundContains(Bounds a, Bounds b)
    {
       bool isContains = a.Contains(b.center) &&    // 中心包含
                         a.Contains(b.min) &&       // 最大值包含
                         a.Contains(b.max);         // 最小值包含
       return isContains;
    }

    public void AddWorldObject(GameObject worldObject)
    {
        DivideAndAdd(worldObject);
    }

    public void DrawGizoms()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(nodeBounds.center, nodeBounds.size);
        if (childrenNodes != null)
        {
            for (int i = 0; i < childrenNodes.Length; i++)
            {
                if (childrenNodes[i] != null)
                {
                    childrenNodes[i].DrawGizoms();
                }
            }
        }
    }
}