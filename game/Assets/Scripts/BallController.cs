using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallController : MonoBehaviour
{
    private new CustomCollider2D collider;
    private new Transform transform;
    private Vector3 velocity;

    private Vector3 lastCollisionPoint;
    private float difficultyMultiplier;

    public AudioClip racket_col_audioclip;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(lastCollisionPoint, 1);
        if(transform != null)
            Gizmos.DrawLine(transform.position, transform.position + velocity * 15);
    }

    private void Awake()
    {
        collider = GetComponent<CustomCollider2D>();
        transform = GetComponent<Transform>();

        difficultyMultiplier = 1f;
        velocity = Vector3.up + Vector3.left;
    }

    void Start()
    {
        collider.onCollisionEnter2D += onCollisionEnter;
    }

    private void Update()
    {
        if(difficultyMultiplier < 5)
            difficultyMultiplier += Time.deltaTime / 10;

        transform.position += velocity * 50.0f * Time.deltaTime * difficultyMultiplier;
    }

    private void onCollisionEnter(CustomCollision col)
    {
        AudioSource.PlayClipAtPoint(racket_col_audioclip, Camera.main.transform.position, 1);
        
        //if (col.collider.gameObject.name == "racket")
        //    Debug.Log("golpea la raqueta");

        lastCollisionPoint = col.collisionPoint;
        var normal = col.normal;

        Bounce(normal);
    }

    private void OnBecameInvisible()
    {
        transform.position = Vector3.zero;
        difficultyMultiplier = 1f;
        velocity = Vector3.up + Vector3.left;
    }

    private void Bounce(Vector3 collisionNormal)
    {
        var speed = velocity.magnitude;
        var direction = Vector3.Reflect(velocity.normalized, collisionNormal).normalized;

        Debug.Log("Out Direction: " + direction);
        Debug.DrawRay(transform.position, velocity.normalized, Color.cyan, 2f);
        Debug.DrawRay(transform.position, collisionNormal, Color.magenta, 2f);
        velocity = direction * Mathf.Max(speed, 1);
    }
}
