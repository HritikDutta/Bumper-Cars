//#define DONT_DRIVE

using UnityEngine;

public class NPCOperator : MonoBehaviour, ICarOperator
{
	[Header("Target")]
	public Waypoint waypoint;

	[Header("Car")]
	public CarController carController;
	private CarConfig carConfig;
	private Transform carTransform;

	[Header("Settings")]
	public float steerSensitivity = 10f;

	private float horizontalMovement = 0f;
	private float verticalMovement = 0f;

	private void Start()
	{
		carTransform = carController.transform;
		carConfig = carController.carConfig;
	}

	private void FixedUpdate()
	{
		Vector3 vectorToFrontAxle = waypoint.position - carController.frontAxle.position;
		Vector3 vectorToBackAxle  = waypoint.position - carController.backAxle.position;

		float angleBetweenAxles = Vector3.SignedAngle(vectorToBackAxle, vectorToFrontAxle, carTransform.up);
		horizontalMovement = Mathf.Lerp(horizontalMovement, angleBetweenAxles % carConfig.turnAngle, Time.fixedDeltaTime * steerSensitivity);

		float angleToTarget = Vector3.Angle(vectorToFrontAxle, carTransform.forward) - 90f;
		float adjustedMaxSteerAngle = carConfig.maxSteerAngle - 90f;
		if (angleToTarget > adjustedMaxSteerAngle)
		{
			// Re-Align
			horizontalMovement *= -1f;
			verticalMovement = -1f;
			return;
		}

		float distanceLeft = vectorToFrontAxle.magnitude;
		float acceleration = carConfig.acceleration - carController.currentSpeed * carConfig.friction;
		verticalMovement = waypoint.approachSpeed * waypoint.approachSpeed - carController.currentSpeed * carController.currentSpeed - Mathf.Sign(angleToTarget) * 2f * acceleration * distanceLeft;
	}

	public float HorizontalInput()
	{
# if DONT_DRIVE
		return 0f;
# else
		return Mathf.Clamp(horizontalMovement, -1f, 1f);
# endif
	}

	public float VerticalInput()
	{

#if DONT_DRIVE
		return 0f;
#else
		return Mathf.Clamp(verticalMovement, -1f, 1f);
#endif
	}

	public bool ResetInput()
	{
		return Input.GetKeyDown(KeyCode.R);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawLine(carController.frontAxle.position, waypoint.position);
		Gizmos.DrawLine(carController.backAxle.position, waypoint.position);
	}
}
