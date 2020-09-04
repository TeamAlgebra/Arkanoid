using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    private new CustomCollider2D collider;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<CustomCollider2D>();
        collider.onCollisionEnter2D += onCollisionEnter;
    }

    private void onCollisionEnter(CustomCollision col)
    {
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        collider.onCollisionEnter2D -= onCollisionEnter;
    }
}
