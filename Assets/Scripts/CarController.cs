using UnityEngine;

public class CarController : MonoBehaviour
{
	public CarConfig carConfig;
	public ICarOperator carOperator;

	[Header("Car Parts")]
	public Transform frontAxle;
	public Transform backAxle;

	private Rigidbody rb;

	private float currentFwdSpeed = 0f;

	private Vector3 startPosition;
	private Quaternion startRotation;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		startPosition = rb.position;
		startRotation = rb.rotation;
	}

	private void Update()
	{
		if (carOperator.ResetInput())
			ResetPhysics();

		float verticalInput   = carOperator.VerticalInput();
		float horizontalInput = carOperator.HorizontalInput();

		if (verticalInput != 0f)
		{
			float acc = (verticalInput > 0f) ? verticalInput * carConfig.acceleration : verticalInput * carConfig.deceleration;
			currentFwdSpeed += acc * Time.deltaTime;
		}

		float moveSpeedSign = Mathf.Sign(currentFwdSpeed);
		frontAxle.localRotation = Quaternion.Euler(new Vector3(0f, moveSpeedSign * horizontalInput * carConfig.turnAngle, 0f));
	}

	private void FixedUpdate()
	{
		currentFwdSpeed -= carConfig.friction * currentFwdSpeed * Time.fixedDeltaTime;
		float displacement = currentFwdSpeed * Time.fixedDeltaTime;

		Vector3 moveDirection = (frontAxle.forward + backAxle.forward).normalized;

		rb.MovePosition(rb.position + moveDirection * displacement);

		if (Mathf.Abs(displacement) >= carConfig.minTurnSpeed)
			transform.rotation = Quaternion.Lerp(backAxle.rotation, frontAxle.rotation, carConfig.turnSpeed * GetTFactor(displacement));
	}

	private float GetTFactor(float displacement)
	{
		float x = Mathf.Abs(displacement);
		float tFactor = Mathf.Min(Mathf.Exp(x - 2.5f), Mathf.Exp(-3f * x));
		return tFactor;
	}

	private void ResetPhysics()
	{
		// To get rid of the warnings
		rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
		rb.isKinematic = true;

		rb.position = startPosition;
		rb.rotation = startRotation;
		rb.ResetInertiaTensor();
		rb.ResetCenterOfMass();
		rb.velocity = Vector3.zero;
		currentFwdSpeed = 0f;

		rb.isKinematic = false;
		rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
	}

	public float currentSpeed { get { return currentFwdSpeed; } }
}
