using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : RaycastBody
{
    // public Solid platform => collisions.platform;

    private Vector2 input;
    private Vector2 velocity;

    public override void Start()
    {
        base.Start();
        collisionMask = LayerMask.GetMask("solid");
    }

    void Update()
    {
        input.x = Input.GetKey(KeyCode.J) ? -1 : Input.GetKey(KeyCode.L) ? 1 : 0;
        input.y = Input.GetKey(KeyCode.I) ? 1 : Input.GetKey(KeyCode.K) ? -1 : 0;
    }

    public void CustomUpdate(float deltaTime)
    {
        if (collisions.below)
            velocity.y = 0;

        velocity.x = input.x * 20f;
        velocity.y += CustomPhysics.gravity * deltaTime;

        Move(velocity * deltaTime);

        if (collisions.platform)
            collisions.platform.ridingActor = this;
    }
}
