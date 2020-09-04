using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
public enum ColliderType
{
    Sphere,
    AABB
}

public class CustomCollider2D : MonoBehaviour
{
    private List<CustomCollider2D> customCollider2DsCollissionning;

    public ColliderType ColliderType;

    public event Action<CustomCollision> onCollisionEnter2D;
    public event Action<CustomCollision> onCollisionExit2D;

    public float width = 0.0f;
    public float heigth = 0.0f;

    public float radius = 0.0f;

    public Vector3 centerOffset;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        switch (ColliderType)
        {
            case ColliderType.Sphere:
                {
                    Gizmos.DrawWireSphere(transform.position + centerOffset, radius);
                }
                break;
            case ColliderType.AABB:
                {
                    Gizmos.DrawWireCube(transform.position + centerOffset, new Vector3(width, heigth, 0));
                }
                break;
            default:
                break;
        }
    }

    private void Awake()
    {
        CollisionManager.GetInstance().RegisterCollider(this);

        onCollisionEnter2D += (CustomCollision col) =>
        {
            Debug.Log("CollisionEnter!");
        };

        onCollisionExit2D += (CustomCollision col) =>
        {
            Debug.Log("CollisionExit!");
        };
    }

    private void OnDestroy()
    {
        CollisionManager.GetInstance().UnregisterCollider(this);
        onCollisionExit2D = null;
        onCollisionEnter2D = null;
    }

    public void CalculateCollisions(IEnumerable<CustomCollider2D> colliders)
    {
        foreach (var collider in colliders)
        {
            if (collider == this) continue;
            switch (ColliderType)
            {
                case ColliderType.Sphere:
                    {
                        switch (collider.ColliderType)
                        {
                            case ColliderType.Sphere:
                                {
                                    if (CircleCollision(this.transform.position + this.centerOffset, this.radius, collider.transform.position + collider.centerOffset, collider.radius))
                                    {
                                        if (AddCollider(collider))
                                            onCollisionEnter2D(new CustomCollision(collider.transform.position + collider.centerOffset, collider));
                                    }
                                    else if (RemoveCollider(collider))
                                        onCollisionExit2D(new CustomCollision(collider.transform.position + collider.centerOffset, collider));
                                }
                                break;
                            case ColliderType.AABB:
                                {
                                    if (CircleAABBCollision(this.transform.position + this.centerOffset, this.radius, collider.transform.position + collider.centerOffset, new Vector2(collider.width, collider.heigth)))
                                    {
                                        if (AddCollider(collider))
                                            onCollisionEnter2D(new CustomCollision(GetCollisionPoint(this, collider), collider, GetCollisionNormal(GetCollisionPoint(this, collider), collider)));
                                    }
                                    else if (RemoveCollider(collider))
                                        onCollisionExit2D(new CustomCollision(GetCollisionPoint(this, collider), collider, GetCollisionNormal(GetCollisionPoint(this, collider), collider)));
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case ColliderType.AABB:
                    {
                        switch (collider.ColliderType)
                        {
                            case ColliderType.Sphere:
                                {
                                    if (CircleAABBCollision(collider.transform.position + collider.centerOffset, collider.radius, this.transform.position + this.centerOffset, new Vector2(width, heigth)))
                                    {
                                        if (AddCollider(collider))
                                            onCollisionEnter2D(new CustomCollision(GetCollisionPoint(collider, this), collider, GetCollisionNormal(GetCollisionPoint(collider, this), this)));
                                    }
                                    else if (RemoveCollider(collider))
                                        onCollisionExit2D(new CustomCollision(GetCollisionPoint(collider, this), collider, GetCollisionNormal(GetCollisionPoint(collider, this), this)));
                                }
                                break;
                            case ColliderType.AABB:
                                {
                                    if (AABBCollision(transform.position + centerOffset, new Vector2(width, heigth), collider.transform.position + collider.centerOffset, new Vector2(collider.width, collider.heigth)))
                                    {
                                        if (AddCollider(collider))
                                            onCollisionEnter2D(new CustomCollision(collider.transform.position + collider.centerOffset, collider));
                                    }
                                    else if (RemoveCollider(collider))
                                        onCollisionExit2D(new CustomCollision(collider.transform.position + collider.centerOffset, collider));
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private bool AddCollider(CustomCollider2D customCollider2D)
    {
        if (customCollider2DsCollissionning == null)
            customCollider2DsCollissionning = new List<CustomCollider2D>();

        if (customCollider2DsCollissionning.Contains(customCollider2D))
            return false;
        customCollider2DsCollissionning.Add(customCollider2D);
        return true;
    }

    private bool RemoveCollider(CustomCollider2D customCollider2D)
    {
        if (customCollider2DsCollissionning == null)
            customCollider2DsCollissionning = new List<CustomCollider2D>();

        if (!customCollider2DsCollissionning.Contains(customCollider2D))
            return false;
        customCollider2DsCollissionning.Remove(customCollider2D);
        return true;
    }

    private bool CircleCollision(Vector2 c1Pos, float c1Radius, Vector2 c2Pos, float c2Radius)
    {
        float distance = Mathf.Sqrt((c1Pos.x - c2Pos.x) * (c1Pos.x - c2Pos.x) + ((c1Pos.y - c2Pos.y) * (c1Pos.y - c2Pos.y)));
        return distance < c1Radius + c2Radius;
    }

    private bool AABBCollision(Vector2 s1Pos, Vector2 s1Size, Vector2 s2Pos, Vector2 s2Size)
    {
        s1Pos = s1Pos - new Vector2(s1Size.x / 2, s1Size.y / 2);
        s2Pos = s2Pos - new Vector2(s2Size.x / 2, s2Size.y / 2);

        return (s1Pos.x < s2Pos.x + s2Size.x &&
                s1Pos.x + s1Size.x > s2Pos.x &&
                s1Pos.y < s2Pos.y + s2Size.y &&
                s1Pos.y + s1Size.y > s2Pos.y);
    }

    private bool CircleAABBCollision(Vector2 circlePosition, float circleRadius, Vector2 aabbPosition, Vector2 aabbSize)
    {
        Vector2 dir = aabbPosition - circlePosition;
        dir = dir.normalized;
        dir = dir * circleRadius;
        Vector2 pos = circlePosition + dir;

        aabbPosition = aabbPosition - new Vector2(aabbSize.x / 2, aabbSize.y / 2);
        return (aabbPosition.x < pos.x &&
                aabbPosition.x + aabbSize.x > pos.x &&
                aabbPosition.y < pos.y &&
                aabbPosition.y + aabbSize.y > pos.y);
    }

    private Vector3 GetCollisionPoint(CustomCollider2D col1, CustomCollider2D col2)
    {
        var dirVector = (col2.transform.position + col2.centerOffset) - (col1.transform.position + col1.centerOffset);
        var closestPoint = dirVector.normalized * col1.radius;

        return col1.transform.position + closestPoint;
    }

    private Vector3 GetCollisionNormal(Vector3 collisionPoint, CustomCollider2D collider) {
        var normal = Vector3.zero;

        switch (collider.ColliderType)
        {
            case ColliderType.Sphere:
                {
                    normal = ((collider.transform.position + collider.centerOffset) - collisionPoint).normalized;
                }
                break;
            case ColliderType.AABB:
                {

                    var center = collider.transform.position + collider.centerOffset;
                    var size = new Vector3(collider.width, collider.heigth);

                    var rect = new Rect(center, size);
                    Vector2 acollisionPoint = collisionPoint;

                    collisionPoint = Rect.NormalizedToPoint(rect, (rect.center - acollisionPoint).normalized);

                    if (rect.max.x == collisionPoint.x)
                        normal = Vector3.right;

                    if (rect.min.x == collisionPoint.x)
                        normal = Vector3.left;

                    if (rect.max.y == collisionPoint.y)
                        normal = Vector3.up;

                    if (rect.min.y == collisionPoint.y)
                        normal = Vector3.right;

                }
                break;
            default:
                normal = Vector3.zero;
                break;
        }

        return normal;
    }
}