using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingWall : MonoBehaviour
{
    public float wallBounceAngle = 45f;
    BoxCollider2D box;
    private void Awake()
    {
        box = GetComponent<BoxCollider2D>();
    }

    Vector2 MakeReflection(Vector2 direction)
    {
        Vector2 reflection = Vector2.Reflect(direction, transform.up);
        return reflection;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("newBall"))
        {
            
            Rigidbody2D ballRB = collision.gameObject.GetComponent<Rigidbody2D>();
            Vector2 ballVelocity = ballRB.velocity.normalized;
            Vector2 contactNormalSum = Vector2.zero;

            foreach (ContactPoint2D contactPoint in collision.contacts)
            {
                // Add the contact normal to the sum
                contactNormalSum += contactPoint.normal;
            }

            // Calculate the average contact normal
            Vector2 contactNormal = contactNormalSum / collision.contacts.Length;

            // Flip the normal vector for the left wall
            if (transform.position.x < 0)
            {
                contactNormal *= -1;
            }

            // Calculate the reflection direction using Vector3.Reflect
            Vector3 reflectDirection = Vector3.Reflect(ballVelocity, contactNormal);

            // Calculate the bounce angle based on the reflection direction and the collision angle
            float collisionAngle = Mathf.Atan2(ballVelocity.y, ballVelocity.x) * Mathf.Rad2Deg;
            float reflectAngle = Mathf.Atan2(reflectDirection.y, reflectDirection.x) * Mathf.Rad2Deg;
            float bounceAngle = reflectAngle + wallBounceAngle - collisionAngle;

            // Apply the new direction and bounce force to the ball
            ballRB.velocity = Quaternion.Euler(0, 0, bounceAngle) * reflectDirection * ballRB.velocity.magnitude*2f;
        }
    }

}
