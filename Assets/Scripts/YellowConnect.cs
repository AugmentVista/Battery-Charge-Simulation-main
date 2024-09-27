using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowConnect : MonoBehaviour
{
    private bool Connected = false;
    private Rigidbody2D rb;
    private float speed = 2.0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Initialize the Rigidbody2D in Awake
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Yellow") && !Connected)
        {
            Rigidbody2D rbCollidedWith = collision.GetComponent<Rigidbody2D>(); // Get the Rigidbody2D of the collided object
            Vector2 currentPosition = transform.position;
            Vector2 otherPosition = collision.transform.position;

            rbCollidedWith.velocity = Vector2.zero; // Set the collided object's velocity to zero
            rb.velocity = Vector2.zero; // Set the current object's velocity to zero

            if (Vector2.Distance(currentPosition, otherPosition) < 0.05f)
            {
                collision.transform.SetParent(transform); // Parent the collided object
                Connected = true; // Mark as connected
                rb.velocity = rb.velocity * speed; // Optional: reset the velocity if desired
            }
        }
    }
}
