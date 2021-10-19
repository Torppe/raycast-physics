using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Solid : RaycastBody
{
    public Actor ridingActor;

    private Vector2 velocity;
    private Vector2 input;
    private float speed = 15;

    public override void Start()
    {
        base.Start();
        collisionMask = LayerMask.GetMask("actor");
    }

    public void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    public void CustomUpdate(float deltaTime)
    {
        velocity = input * speed;
        Move(velocity * deltaTime);

        ridingActor?.transform.Translate(velocity * deltaTime);
        ridingActor = null;
    }

    public override void CollideX(ref Vector2 vel)
    {
        var directionX = Mathf.Sign(vel.x);
        var raycastDirection = Vector3.right * directionX;
        float rayLength = Mathf.Abs(vel.x);

        for (int i = 0; i < horizontalRaycount; i++)
        {
            Vector2 raycastPosition = directionX == -1 ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            raycastPosition += Vector2.up * (horizontalRaySpacing * i);

            if (Physics.Raycast(raycastPosition, raycastDirection, out var hit, rayLength, collisionMask))
            {
                float displacementAmount = rayLength - hit.distance + CustomPhysics.collisionOffset;
                hit.transform.Translate(raycastDirection * displacementAmount);

                if (directionX == 1)
                {
                    collisions.right = true;
                }
                else
                {
                    collisions.left = true;
                }
            }

            Debug.DrawRay(raycastPosition, raycastDirection * rayLength, Color.red);
            Debug.DrawRay(raycastPosition, raycastDirection * 1, Color.yellow);
        }

    }

    public override void CollideY(ref Vector2 vel)
    {
        var directionY = Mathf.Sign(vel.y);
        var raycastDirection = Vector3.up * directionY;
        float rayLength = Mathf.Abs(vel.y);

        for (int i = 0; i < verticalRaycount; i++)
        {
            Vector2 raycastPosition = directionY == -1 ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            raycastPosition += Vector2.right * (verticalRaySpacing * i);

            if (Physics.Raycast(raycastPosition, raycastDirection, out var hit, rayLength, collisionMask))
            {
                if (!hit.collider.GetComponent<Actor>() == ridingActor)
                {
                    float displacementAmount = rayLength - hit.distance + CustomPhysics.collisionOffset;
                    hit.transform.Translate(raycastDirection * displacementAmount);
                }

                if (directionY == 1)
                {
                    collisions.above = true;
                }
                else
                {
                    collisions.below = true;
                }
            }

            Debug.DrawRay(raycastPosition, raycastDirection * rayLength, Color.red);
            Debug.DrawRay(raycastPosition, raycastDirection * 1, Color.yellow);
        }
    }
}

// var raycastDirection = Vector3.right * Mathf.Sign(velocity.x);
// var raycastPosition = transform.position + (raycastDirection * 0.5f);
// var velocityX = Mathf.Abs(velocity.x);

// if (Physics.Raycast(raycastPosition, raycastDirection, out var hit, velocityX, collisionMask))
// {
//     float displacementAmount = velocityX - hit.distance + CustomPhysics.collisionOffset;
//     hit.transform.Translate(raycastDirection * displacementAmount);
// }

// Debug.DrawRay(raycastPosition, raycastDirection * Mathf.Abs(velocity.x), Color.red);


// var raycastDirection = Vector3.up * Mathf.Sign(velocity.y);
// var raycastPosition = transform.position + (raycastDirection * 0.5f);
// var velocityY = Mathf.Abs(velocity.y);

// if (Physics.Raycast(raycastPosition, raycastDirection, out var hit, velocityY, collisionMask))
// {
//     float displacementAmount = velocityY - hit.distance + CustomPhysics.collisionOffset;
//     hit.transform.Translate(raycastDirection * displacementAmount);
// }

// Debug.DrawRay(raycastPosition, raycastDirection * Mathf.Abs(velocity.y), Color.red);
