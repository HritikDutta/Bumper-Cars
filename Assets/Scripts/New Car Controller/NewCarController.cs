using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum AxleProperties
{
    NONE     = 0,
    MOTOR    = 1,
    STEERING = 2,
}

[System.Serializable]
public class AxleInfo
{
	public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public AxleProperties properties;
}

public class NewCarController : MonoBehaviour
{
    public List<AxleInfo> axleInfos;
    public float maxMotorTorque = 1000f;
    public float maxSteerAngle = 30f;
    public float turnSlowDownRatio = .5f;

    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
            return;

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation * Quaternion.Euler(0f, 0f, 90f);
    }

    private void FixedUpdate()
	{
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteerAngle * Input.GetAxis("Horizontal");

        foreach (AxleInfo axleInfo in axleInfos)
		{
            if ((axleInfo.properties & AxleProperties.STEERING) != 0)
			{
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }

            if ((axleInfo.properties & AxleProperties.MOTOR) != 0)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }

            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
	}
}
