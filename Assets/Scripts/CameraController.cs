using UnityEngine;

public class CameraController : MonoBehaviour
{
	InputActions inputActions;

	[SerializeField] Collider boundCollider;
	[SerializeField] float movementSpeed = 1f;

	Bounds cameraBounds;

	// Start is called before the first frame update
	void Start()
	{
		inputActions = GameManager.Instance.InputActions;

		inputActions.MainGameplay.Enable();
		cameraBounds = boundCollider.bounds;
	}

	// Update is called once per frame
	void Update()
	{
		var inputVector = movementSpeed * Time.deltaTime * inputActions.MainGameplay.Movement.ReadValue<Vector2>();
		Vector3 newPosition = transform.position + new Vector3(inputVector.x, 0f, inputVector.y);
		transform.position = cameraBounds.ClosestPoint(newPosition);
	}
}
