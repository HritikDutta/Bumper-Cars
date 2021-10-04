using UnityEngine;

public class PlayerOperator : CarOperator
{
	public override float HorizontalInput()
	{
		return Input.GetAxisRaw("Horizontal");
	}

	public override float VerticalInput()
	{
		return Input.GetAxisRaw("Vertical");
	}

	public override bool ResetInput()
	{
		return Input.GetKeyDown(KeyCode.R);
	}
}
