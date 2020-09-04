using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCollision
{
    public readonly Vector3 collisionPoint;
    public readonly CustomCollider2D collider;
    public readonly Vector3 normal;

    public CustomCollision(Vector3 pCol, CustomCollider2D col, Vector3 normal = default)
    {
        collisionPoint = pCol;
        collider = col;
        this.normal = normal;
    }
}
