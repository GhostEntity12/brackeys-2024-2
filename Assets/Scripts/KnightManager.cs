using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KnightManager : MonoBehaviour
{
	public int AvailableKnights { get; private set; }
	[SerializeField] int totalKnights = 15;
	Queue<Knight> knightQueue;

	private void Start()
	{
		AvailableKnights = totalKnights;
	}

	public void Dispatch(int count, PointOfInterest location, float time)
	{
		for (int i = 0; i < count; i++)
		{
			Knight k = knightQueue.Dequeue();
			k.SetQuest(location, time);
		}
	}

	public void ReturnKnight(Knight knight)
	{
		knightQueue.Enqueue(knight);
	}
}