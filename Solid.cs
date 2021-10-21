using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Solid : RaycastBody
{
    public Actor ridingActor;

    private Vector2 velocity;
    private Vector2 input;
    public float speed = 15;

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

        if (ridingActor)
        {
            ridingActor.transform.Translate(velocity * deltaTime);
            ridingActor = null;
        }
    }

    public override void CollideX(ref Vector2 vel)
    {
        var directionX = Mathf.Sign(vel.x);
        var raycastDirection = Vector3.right * directionX;
        float rayLength = Mathf.Abs(vel.x) + CustomPhysics.collisionOffset;

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
        float rayLength = Mathf.Abs(vel.y) + CustomPhysics.collisionOffset;

        for (int i = 0; i < verticalRaycount; i++)
        {
            Vector2 raycastPosition = directionY == -1 ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            raycastPosition += Vector2.right * (verticalRaySpacing * i + vel.x);

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