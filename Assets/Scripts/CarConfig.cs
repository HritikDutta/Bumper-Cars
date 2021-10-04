using UnityEngine;

[CreateAssetMenu(fileName = "Car Config", menuName = "Driving/Car Config")]
public class CarConfig : ScriptableObject
{
	public float acceleration = 10f;
	public float deceleration = 5f;
	public float friction = 2.5f;

	public float turnAngle = 10f;
	public float turnSpeed = 0.001f;
	public float minTurnSpeed = 0.01f;
	public float maxSteerAngle = 60f;
}
