using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPhysics : MonoBehaviour
{
    public static readonly float collisionOffset = 0.03f;
    public static float gravity = -20f;

    public List<Solid> solids = new List<Solid>();
    public List<Actor> actors = new List<Actor>();

    void Update()
    {
        var deltaTime = Time.deltaTime;

        solids.ForEach(s => s.CustomUpdate(deltaTime));
        actors.ForEach(a => a.CustomUpdate(deltaTime));
    }
}
