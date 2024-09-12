using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public enum Resource
	{
		Gold,
		Wood,
		Medicine,
		Food,
		People
	}

	private int gold = 0;
	private int wood = 0;
	private int medicine = 0;
	private int food = 0;
	private int people = 0;

	public int GetResourceQuantity(Resource resource) => resource switch
	{
		Resource.Gold => gold,
		Resource.Wood => wood,
		Resource.Medicine => medicine,
		Resource.Food => food,
		Resource.People => people,
		_ => throw new System.NotImplementedException(),
	};

	public int AddResource(Resource resource, int quantity) {
		switch (resource)
		{
			case Resource.Gold:
				gold += quantity;
				return gold;
			case Resource.Wood:
				wood += quantity;
				return wood;
			case Resource.Medicine:
				medicine += quantity;
				return medicine;
			case Resource.Food:
				food += quantity;
				return food;
			case Resource.People:
				people += quantity;
				return people;
			default:
				throw new System.NotImplementedException();
		}
	}
}
