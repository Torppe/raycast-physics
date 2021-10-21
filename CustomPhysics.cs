using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPhysics : MonoBehaviour
{
    public static readonly float collisionOffset = 0.03f;
    public static float gravity = -50f;

    private List<Solid> solids = new List<Solid>();
    private List<Actor> actors = new List<Actor>();

    private List<Actor> destroyedActors = new List<Actor>();

    private float deltaTime;

    void Start()
    {
        solids.AddRange(FindObjectsOfType<Solid>());
        actors.AddRange(FindObjectsOfType<Actor>());

        Actor.OnAnyDeath += DestroyActor;
    }

    void Update()
    {
        deltaTime = Time.deltaTime;

        solids.ForEach(s => s.CustomUpdate(deltaTime));
        actors.ForEach(a => a.CustomUpdate(deltaTime));

        destroyedActors.ForEach(a => actors.Remove(a));
        destroyedActors.Clear();
    }

    void DestroyActor(Actor actor)
    {
        Debug.Log("called");
        destroyedActors.Add(actor);
    }
}
