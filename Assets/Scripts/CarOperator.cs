using UnityEngine;

public abstract class CarOperator : MonoBehaviour
{
	public abstract float VerticalInput();
	public abstract float HorizontalInput();
	public abstract bool ResetInput();
}
