using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueConnect : MonoBehaviour
{
    private bool Connected = false;
    private Rigidbody2D rb;
    private float speed = 2.0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Blue") && !Connected) // wants to connect to blue
        {
            Rigidbody2D rbCollidedWith = collision.GetComponent<Rigidbody2D>();
            Vector2 currentPosition = transform.position;
            Vector2 otherPosition = collision.transform.position;

            rbCollidedWith.velocity = Vector2.zero;
            rb.velocity = Vector2.zero;

            if (Vector2.Distance(currentPosition, otherPosition) < 0.05f)
            {
                collision.transform.SetParent(transform);
                Connected = true;
                rb.velocity = rb.velocity * speed;
            }
        }

        if (collision.gameObject.CompareTag("Red") && Connected) // wants to avoid red
        {
            Rigidbody2D rbCollidedWith = collision.GetComponent<Rigidbody2D>();
            Vector2 currentPosition = transform.position;
            Vector2 otherPosition = collision.transform.position;

            rbCollidedWith.velocity = Vector2.zero;

            if (Vector2.Distance(currentPosition, otherPosition) < 0.05f)
            {
                rb.velocity = rb.velocity * -1.5f;
            }
        }
    }
}
