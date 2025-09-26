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
    // 节点信息
    public int id;
    public float minSize;
    public Bounds nodeBounds = new();
    public Bounds[] childNodeBounds = null;
    
    // 孩子节点信息
    public bool isContainedChild = false;
    public OctreeNode[] childrenNodes = null;
    
    // 世界物体包含信息
    public List<OctreeObject> containedObjects = new();

    public OctreeNode(Bounds nodeBounds, float minSize)
    {
        this.id = Utils.idInt++;
        this.nodeBounds = nodeBounds;
        this.minSize = minSize;
        BuildChildBounds();
    }

    private void BuildChildBounds()
    {
        float quarter = nodeBounds.size.x / 4f;
        Vector3 childSize = new Vector3(nodeBounds.size.x / 2f, nodeBounds.size.x / 2f, nodeBounds.size.x / 2f);
        childNodeBounds = new[]
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
        // 最底层，直接添加
        if (nodeBounds.size.x <= minSize)
        {
            containedObjects.Add(octObj);           
            return;
        }

        // 内部分割
        if (childrenNodes == null)
            childrenNodes = new OctreeNode[8];
        for (int i = 0; i < 8; i++)
        {
            if (childrenNodes[i] == null) 
                childrenNodes[i] = new OctreeNode(childNodeBounds[i], minSize);
            if (childrenNodes[i].nodeBounds.Intersects(octObj.bounds))
            {
                isContainedChild = true;
                childrenNodes[i].DivideAndAdd(worldObject);
            }
        }
        if (!isContainedChild)
        {
            childrenNodes = null;
            containedObjects.Add(octObj);           // 没有小结点能够包含，则直接包含；
        }
    }

    public void AddWorldObject(GameObject worldObject)
    {
        DivideAndAdd(worldObject);
    } 

    public void DrawDebug()
    {
        // draw my bounds
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(nodeBounds.center, nodeBounds.size);
        // draw contain cube
        if (containedObjects.Count > 0)
        {
            Gizmos.color = new Color(0, 0, 1, 0.75f);
            Gizmos.DrawCube(nodeBounds.center, nodeBounds.size);

            foreach (var obj in containedObjects)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(obj.bounds.center, obj.bounds.size);
            }
        }
        // draw child bounds
        if (childrenNodes != null)
        {
            for (int i = 0; i < childrenNodes.Length; i++)
            {
                if (childrenNodes[i] != null)
                {
                    childrenNodes[i].DrawDebug();
                }
            }
        }
    }
}