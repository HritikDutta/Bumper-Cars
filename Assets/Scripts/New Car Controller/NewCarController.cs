using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Flags]
public enum AxleProperties
{
    NONE     = 0,
    MOTOR    = 1,
    STEERING = 2,
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
    public AxleProperties properties;
}

[RequireComponent(typeof(ICarOperator), typeof(Rigidbody))]
public class NewCarController : MonoBehaviour
{
    public float maxMotorTorque = 3000f;
    public float maxSteerAngle = 15f;
    public float turnSlowDownRatio = .5f;

    public Transform centerOfMass;

    public List<AxleInfo> axleInfos;

    public List<float> gearRatios = new List<float> { 1f, 2f, 4f, 8f, 16f };
    private int gearIndex = 0;

    private ICarOperator carOperator;

    private Rigidbody rb;
    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Awake()
	{
        carOperator = GetComponent<ICarOperator>();
        rb = GetComponent<Rigidbody>();
    }

	private void Start()
	{
        startPosition = transform.position;
        startRotation = transform.rotation;

        rb.centerOfMass = centerOfMass.localPosition;
    }

    private void Update()
    {
        if (carOperator.ResetInput())
		{
            ResetCar();
            return;
		}

        int requestedChange = carOperator.RequestGearChange();
        gearIndex = Mathf.Clamp(gearIndex + requestedChange, 0, gearRatios.Count - 1);
    }

    public void ApplyLocalPositionToVisuals(WheelInfo wheel)
    {
		wheel.collider.GetWorldPose(out Vector3 position, out Quaternion rotation);

		wheel.visual.position = position;
        wheel.visual.rotation = rotation * Quaternion.Euler(0f, 0f, 90f);
    }

	private void FixedUpdate()
	{
        float motor = maxMotorTorque * carOperator.VerticalInput();
        float steering = maxSteerAngle * carOperator.HorizontalInput();

        foreach (AxleInfo axleInfo in axleInfos)
		{
            if ((axleInfo.properties & AxleProperties.STEERING) != 0)
			{
                axleInfo.leftWheel.collider.steerAngle = steering;
                axleInfo.rightWheel.collider.steerAngle = steering;
            }

            float mult = ((axleInfo.properties & AxleProperties.MOTOR) != 0) ? 1f : 0.25f;
            axleInfo.leftWheel.collider.motorTorque = mult * motor;
            axleInfo.rightWheel.collider.motorTorque = mult * motor;

            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
	}

    private void ResetCar()
    {
        // To get rid of the warnings
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.isKinematic = true;

        rb.position = startPosition;
        rb.rotation = startRotation;
        rb.ResetInertiaTensor();
        rb.ResetCenterOfMass();
        rb.velocity = Vector3.zero;

        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        gearIndex = 0;
    }
}
