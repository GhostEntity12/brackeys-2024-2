using System.Collections.Generic;
using UnityEngine;

public class Knight : MonoBehaviour
{
    public delegate void KnightEvent(Knight k);
    public KnightEvent OnFinishQuest;
    enum State
    {
        Idle,
        ToQuest,
        AtQuest,
        FromQuest
    }

    [SerializeField] float moveSpeed;

    bool doMovement;
    float timeToDestination;
    float questTime = 0;
    PointOfInterest questLocation;

    State currentState = State.Idle;
    
    Queue<Vector3> path;
    Vector3 currentWaypoint;

    // Update is called once per frame
    void Update()
    {
        if (!doMovement) return;

        switch (currentState)
        {
            case State.Idle:
                break;
            case State.ToQuest:
                

				// Increase timeToDestination
				timeToDestination += Time.deltaTime;
                // Decrease remaining quest time
                questTime -= Time.deltaTime;
                if (Move())
                {
                    // At destination
                    currentState = State.AtQuest;
                }
                break;
			case State.AtQuest:
				questTime -= Time.deltaTime;
				if (questTime > timeToDestination) break;

				currentState = State.FromQuest;
				path = questLocation.PathFrom();
				break;
			case State.FromQuest:
                if (Move())
                {
                    currentState = State.Idle;
                    OnFinishQuest?.Invoke(this);
                }
                break;
            default:
                break;
        }

    }

    public void SetQuest(PointOfInterest destination, float duration)
    {
        questLocation = destination;
        path = destination.PathTo();
        timeToDestination = 0;
        questTime = duration;
        currentState = State.ToQuest;
        currentWaypoint = path.Dequeue();
    }

    /// <summary>
    /// Move the Lknght. Returns true if at the end of it's path
    /// </summary>
    /// <returns></returns>
    bool Move()
    {
		if (Vector3.Distance(transform.position, currentWaypoint) < moveSpeed * Time.deltaTime)
		{
            if (path.Count > 0) return true;

			currentWaypoint = path.Dequeue();
		}
		transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, moveSpeed * Time.deltaTime);
        return false;
	}
}
