using UnityEngine;

[System.Flags]
public enum AxleType
{
    None     = 0,
    Motor    = 1,
    Steering = 2,
}

[System.Serializable]
public class WheelInfo
{
    public WheelCollider collider;
    public Transform visual;
}

[System.Serializable]
public class AxleInfo
{
	public WheelInfo leftWheel;
    public WheelInfo rightWheel;
    public AxleType type;
}

[RequireComponent(typeof(ICarOperator), typeof(Rigidbody))]
public class NewCarController : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField]
	private NewCarConfig config;
	[SerializeField]
	private Vector3 centerOfMass;

	[Header("Car Parts")]
    [SerializeField]
    private AxleInfo[] axles;

    private ICarOperator mCarOperator;
    private Rigidbody mRigidbody;

	// Reset Settings
	private Vector3 mStartPosition;
	private Quaternion mStartRotation;

    private void Awake()
	{
        mCarOperator = GetComponent<ICarOperator>();
        mRigidbody = GetComponent<Rigidbody>();

#if UNITY_EDITOR
		// Making sure all wheels' visuals have a mesh
		foreach (AxleInfo axle in axles)
		{
			if (axle.leftWheel.visual.GetComponent<MeshRenderer>() == null ||
				axle.rightWheel.visual.GetComponent<MeshRenderer>() == null)
			{
				Debug.LogWarning("Wheel's visual doesn't have a mesh renderer");
			}
		}
#endif
	}

	private void Start()
	{
		mStartPosition = mRigidbody.position;
		mStartRotation = mRigidbody.rotation;

		ResetCar();
	}

	private void Update()
	{
		if (mCarOperator.ResetInput())
			ResetCar();
	}

	public void ApplyLocalPositionToVisuals(WheelInfo wheel)
	{
		wheel.collider.GetWorldPose(out Vector3 position, out Quaternion rotation);

		wheel.visual.position = position;
		wheel.visual.rotation = rotation * Quaternion.Euler(0f, 0f, 90f);
	}

	private void FixedUpdate()
	{
		float motor = config.maxMotorTorque * mCarOperator.VerticalInput();
		float steering = config.maxSteerAngle * mCarOperator.HorizontalInput();

		foreach (AxleInfo axle in axles)
		{
			if (axle.type.HasFlag(AxleType.Steering))
			{
				axle.leftWheel.collider.steerAngle = steering;
				axle.rightWheel.collider.steerAngle = steering;
			}

			if (axle.type.HasFlag(AxleType.Motor))
			{
				axle.leftWheel.collider.motorTorque = motor;
				axle.rightWheel.collider.motorTorque = motor;
			}

			ApplyLocalPositionToVisuals(axle.leftWheel);
			ApplyLocalPositionToVisuals(axle.rightWheel);
		}

		// Apply DownForce
		float downForce = mRigidbody.velocity.sqrMagnitude * mRigidbody.mass;
		mRigidbody.AddForce(-downForce * transform.up, ForceMode.Force);
	}

	private void ResetCar()
	{
		// To get rid of the warnings
		mRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
		mRigidbody.isKinematic = true;

		mRigidbody.position = mStartPosition;
		mRigidbody.rotation = mStartRotation;
		mRigidbody.ResetInertiaTensor();
		mRigidbody.centerOfMass = centerOfMass;
		mRigidbody.velocity = Vector3.zero;

		mRigidbody.isKinematic = false;
		mRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawSphere(centerOfMass, 0.1f);
	}
}
