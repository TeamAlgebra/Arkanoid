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

    public float racketHitFactor = 1f;
    public float blockHitFactor = .8f;

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

        float x = hitFactor(transform.position, col.collider.transform.position, col.collider.width);
        Bounce(col.normal);

        Vector2 dir = velocity;

        if (col.collider.gameObject.name.Contains("racket"))
            dir = new Vector2(x * racketHitFactor, velocity.y).normalized;
        if (col.collider.gameObject.name.Contains("block"))
            dir = new Vector2(x * blockHitFactor, velocity.y).normalized;

        velocity = dir;
    }

    private float hitFactor(Vector2 ballPos, Vector2 racketPos, float racketWidth)
    {
        return (ballPos.x - racketPos.x) / racketWidth;
    }

    private void Bounce(Vector3 collisionNormal)
    {
        float speed = velocity.magnitude;
        Vector3 direction = Vector3.Reflect(velocity.normalized, collisionNormal).normalized;
        velocity = direction * Mathf.Max(speed, 1);
    }

    private void OnBecameInvisible()
    {
        transform.position = Vector3.zero;
        difficultyMultiplier = 1f;
        velocity = Vector3.up + Vector3.left;
        GameManager.GetInstance().LossLife();
    }
}
