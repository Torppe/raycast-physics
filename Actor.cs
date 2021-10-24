using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : RaycastBody
{
    public float movementSpeed = 10f;
    public float jumpForce = 5f;
    public static Action<Actor> OnAnyDeath;

    private Vector2 input;
    private Vector2 velocity;
    private bool jumpInitiated = false;

    public override void Start()
    {
        base.Start();
        collisionMask = LayerMask.GetMask("solid");
    }

    void Update()
    {
        input.x = Input.GetKey(KeyCode.J) ? -1 : Input.GetKey(KeyCode.L) ? 1 : 0;
        input.y = Input.GetKey(KeyCode.I) ? 1 : Input.GetKey(KeyCode.K) ? -1 : 0;
        jumpInitiated |= Input.GetKeyDown(KeyCode.Space);
    }

    public void CustomUpdate(float deltaTime)
    {
        AdjustVelocity(deltaTime);

        Move(velocity * deltaTime);

        if (collisions.platform)
        {
            collisions.platform.ridingActors.Add(this);
            collisions.below = true;
        }

        CheckForSquish();
    }

    public void CheckForSquish()
    {
        if (CheckSquish())
        {
            OnAnyDeath?.Invoke(this);
            Destroy(gameObject);
        }
    }

    public void SetRiding(Solid platform)
    {
        collisions.platform = platform;
        collisions.below = true;
    }

    private void AdjustVelocity(float deltaTime)
    {
        if (collisions.below || collisions.above)
        {
            velocity.y = 0;
        }

        if (jumpInitiated)
        {
            jumpInitiated = false;
            Jump();
        }

        velocity.x = input.x * movementSpeed;
        velocity.y += CustomPhysics.gravity * deltaTime;
    }

    private void Jump()
    {
        if (collisions.below)
        {
            velocity.y += jumpForce;
        }
    }
}
