using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RaycastBody : MonoBehaviour
{
    public int horizontalRaycount = 3;
    public int verticalRaycount = 3;
    protected float horizontalRaySpacing = 0.5f;
    protected float verticalRaySpacing = 0.5f;

    protected RaycastOrigins raycastOrigins = new RaycastOrigins();
    protected CollisionInfo collisions = new CollisionInfo();
    protected BoxCollider boxCollider;
    protected LayerMask collisionMask;

    public virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(-CustomPhysics.collisionOffset * 2);
        horizontalRaySpacing = bounds.size.y / 2;
        verticalRaySpacing = bounds.size.x / 2;
    }

    public void UpdateRaycastOrigins()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(-CustomPhysics.collisionOffset * 2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    public virtual void Move(Vector2 vel)
    {
        collisions.Clear();
        UpdateRaycastOrigins();

        if (vel.x != 0)
            CollideX(ref vel);
        if (vel.y != 0)
            CollideY(ref vel);

        transform.Translate(vel);
    }

    public virtual void CollideX(ref Vector2 vel)
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
                rayLength = hit.distance;
                vel.x = (hit.distance - CustomPhysics.collisionOffset) * directionX;

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

    public virtual void CollideY(ref Vector2 vel)
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
                rayLength = hit.distance;
                vel.y = (hit.distance - CustomPhysics.collisionOffset) * directionY;

                if (directionY == 1)
                {
                    collisions.above = true;
                }
                else
                {
                    collisions.below = true;

                    if (!collisions.platform || collisions.platform.transform != hit.transform)
                        collisions.platform = hit.collider.GetComponent<Solid>();
                }
            }

            Debug.DrawRay(raycastPosition, raycastDirection * rayLength, Color.red);
            Debug.DrawRay(raycastPosition, raycastDirection * 1, Color.yellow);
        }
    }

    public bool CheckSquish()
    {
        var bounds = boxCollider.bounds;
        bounds.Expand(-CustomPhysics.collisionOffset * 10);
        return Physics.OverlapBox(bounds.center, bounds.size * 0.5f, Quaternion.identity, collisionMask).Length > 0;
    }

    public struct CollisionInfo
    {
        public bool left;
        public bool right;
        public bool above;
        public bool below;
        public bool any => left || right || above || below;

        public Solid platform;

        public void Clear()
        {
            left = right = above = below = false;
            platform = null;
        }
    }

    public struct RaycastOrigins
    {
        public Vector2 bottomLeft;
        public Vector2 bottomRight;
        public Vector2 topLeft;
        public Vector2 topRight;
    }
}