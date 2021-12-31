using UnityEngine;

[CreateAssetMenu(fileName = "Car Config", menuName = "Car Controller/Car Config")]
public class NewCarConfig : ScriptableObject
{
	public float maxSteerAngle = 15f;
	public float maxMotorTorque = 1000f;
}
