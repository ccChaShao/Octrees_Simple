using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct OctreeObject
{
    public Bounds bounds;
    public GameObject gameObject;

    public OctreeObject(GameObject gameObject)
    {
        this.gameObject = gameObject;
        this.bounds = gameObject.GetComponent<Collider>().bounds;
    }
}

/// <summary>
/// 八叉树节点
/// </summary>
public class OctreeNode
{
    private float m_MinSize;
    private Bounds m_NodeBounds = new();
    private Bounds[] m_ChildBounds = null;
    
    public OctreeNode[] childrenNodes = null;
    public List<OctreeObject> containedObjects = new();

    public OctreeNode(Bounds bounds, float minSize)
    {
        m_NodeBounds = bounds;
        m_MinSize = minSize;

        if (m_NodeBounds.size.x > m_MinSize)
        {
            // 八向包围盒
            BuildChildBounds();
            // // 子节点分裂
            // DivideChildNode();
        }
    }

    private void BuildChildBounds()
    {
        float quarter = m_NodeBounds.size.x / 4f;
        Vector3 childSize = new Vector3(m_NodeBounds.size.x / 2f, m_NodeBounds.size.x / 2f, m_NodeBounds.size.x / 2f);
        m_ChildBounds = new[]
        {
            new Bounds(m_NodeBounds.center + new Vector3(quarter, quarter, quarter), childSize),
            new Bounds(m_NodeBounds.center + new Vector3(-quarter, -quarter, -quarter), childSize),
            
            new Bounds(m_NodeBounds.center + new Vector3(-quarter, quarter, quarter), childSize),
            new Bounds(m_NodeBounds.center + new Vector3(-quarter, -quarter, quarter), childSize),
            new Bounds(m_NodeBounds.center + new Vector3(quarter, quarter, -quarter), childSize),
            new Bounds(m_NodeBounds.center + new Vector3(quarter, -quarter, -quarter), childSize),
            
            new Bounds(m_NodeBounds.center + new Vector3(quarter, -quarter, quarter), childSize),
            new Bounds(m_NodeBounds.center + new Vector3(-quarter, quarter, -quarter), childSize),
        };
    }

    private void DivideAndAdd(GameObject worldObject)
    {
        // 一路往下放，假如放到没得放了，就保存起来
        OctreeObject octObj = new OctreeObject(worldObject);
        if (m_NodeBounds.size.x <= m_MinSize)
        {
            containedObjects.Add(octObj);
            return;
        }

        if (childrenNodes == null)
        {
            childrenNodes = new OctreeNode[8];
        }
        bool dividing = false;
        for (int i = 0; i < 8; i++)
        {
            if (childrenNodes[i] == null)
            {
                childrenNodes[i] = new OctreeNode(m_ChildBounds[i], m_MinSize);
            }
            if (m_ChildBounds[i].Contains(octObj.bounds.min) && m_ChildBounds[i].Contains(octObj.bounds.max))
            {
                dividing = true;
                childrenNodes[i].DivideAndAdd(worldObject);
            }
        }
        if (!dividing)
        {
            containedObjects.Add(octObj);
        }
    }

    public void AddWorldObject(GameObject worldObject)
    {
        DivideAndAdd(worldObject);
    }

    public void DrawGizoms()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(m_NodeBounds.center, m_NodeBounds.size);
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