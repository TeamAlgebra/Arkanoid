using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallController : MonoBehaviour
{
    //private new Rigidbody2D rigidbody;
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
        //rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<CustomCollider2D>();
        transform = GetComponent<Transform>();

        difficultyMultiplier = 1f;
        velocity = Vector3.up + Vector3.left;
    }

    // Start is called before the first frame update
    void Start()
    {
        collider.onCollisionEnter2D += onCollisionEnter;
    }

    private void FixedUpdate()
    {
        if(difficultyMultiplier < 5)
            difficultyMultiplier += Time.deltaTime / 10;

        transform.position += velocity * 50.0f * Time.deltaTime * difficultyMultiplier;
    }

    private void onCollisionEnter(CustomCollision col)
    {
        AudioSource.PlayClipAtPoint(racket_col_audioclip, Camera.main.transform.position, 1);
        
        if (col.collider.gameObject.name == "racket")
            Debug.Log("golpea la raqueta");

        lastCollisionPoint = col.collisionPoint;
        var normal = col.normal;
        //if (col.collider.name == "border_left")
        //    normal = Vector2.right;
        //if (col.collider.name == "border_right")
        //    normal = Vector2.left;
        //if (col.collider.name == "border_top")
        //    normal = Vector2.down;
        //if (col.collider.name == "racket_left")
        //    normal = Vector2.up;
        //if (col.collider.name == "racket_right")
        //    normal = Vector2.up;

        Bounce(normal);

        if (col.collider.name == "racket_left")
            velocity = (velocity + new Vector3(-0.3f, 0)).normalized;
        if (col.collider.name == "racket_right")
            velocity = (velocity + new Vector3(0.3f, 0)).normalized;
    }

    private void OnBecameInvisible()
    {
        transform.position = Vector3.zero;
        difficultyMultiplier = 1f;
        velocity = Vector3.up + Vector3.left;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
