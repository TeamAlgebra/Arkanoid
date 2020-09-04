using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    private static CollisionManager instance;
    private List<CustomCollider2D> customCollider2Ds;

    public bool collideSameLayer;

    public static CollisionManager GetInstance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<CollisionManager>();
            if (instance == null)
            {
                GameObject container = new GameObject("CollisionManager");
                instance = container.AddComponent<CollisionManager>();
            }
        }

        return instance;
    }

    public void RegisterCollider(CustomCollider2D customCollider2D) {
        if (customCollider2Ds == null)
            customCollider2Ds = new List<CustomCollider2D>();
        
        if (customCollider2Ds.Contains(customCollider2D))
            return;

        customCollider2Ds.Add(customCollider2D);
    }

    public void UnregisterCollider(CustomCollider2D customCollider2D) {
        if (customCollider2Ds == null)
            return;

        customCollider2Ds.Remove(customCollider2D);
    }

    private void FixedUpdate()
    {
        if (customCollider2Ds == null) return;

        foreach (var collider in customCollider2Ds)
        {
            collider.CalculateCollisions(customCollider2Ds);
        }
    }
}
