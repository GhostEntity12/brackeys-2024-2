using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : MonoBehaviour
{
    enum State
    {
        Idle,
        ToQuest,
        AtQuest,
        FromQuest
    }

    bool doMovement;
    bool activeQuest;
    PointOfInterest questLocation;
    [SerializeField] float moveSpeed;
    float timeToDestination;
    float questTime = 0;
    State currentState = State.Idle;
    Queue<Vector3> path;
    Vector3 currentWaypoint;

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // if can move
        // if idle
        // return

        // if headed to quest
        // tick timeToDestination
        // move towards quest

        // if at quest
        // if remainingQuestTime < timeToDestination
        // start walking to castle

        // if returning
        // move toward castle
        if (doMovement && activeQuest)
        {
            transform.position = Vector3.MoveTowards(transform.position, questLocation.Location, moveSpeed * Time.deltaTime);
        }

        switch (currentState)
        {
            case State.Idle:
                break;
            case State.ToQuest:
                // Move towards destination
                Move();

				// Increase timeToDestination
				timeToDestination += Time.deltaTime;
                // Decrease remaining quest time
                questTime -= Time.deltaTime;
                if (true)
                {
                    // At destination
                    currentState = State.AtQuest;
                }
                break;
            case State.AtQuest:
                questTime -= Time.deltaTime;
                if (questTime < timeToDestination)
                {
                    currentState = State.FromQuest;
                    path = questLocation.PathFrom();
                }
                break;
            case State.FromQuest:
                // Move to destination
                Move();
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

    void Move()
    {
		if (Vector3.Distance(transform.position, currentWaypoint) < moveSpeed * Time.deltaTime && path.Count > 0)
		{
			currentWaypoint = path.Dequeue();
		}
		transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, moveSpeed * Time.deltaTime);
	}
}
