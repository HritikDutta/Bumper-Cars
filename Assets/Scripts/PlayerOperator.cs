using UnityEngine;

public class PlayerOperator : MonoBehaviour, ICarOperator
{
	private float horizontalInput = 0f;
	private float verticalInput = 0f;
	
	private int requestGearChange = 0;
	
	private bool resetInput = false;

	private void Update()
	{
		// Caching values to reduce redundant calculations

		horizontalInput = Input.GetAxisRaw("Horizontal");
		verticalInput = Input.GetAxisRaw("Vertical");
		requestGearChange = 0;

		if (Input.GetKeyDown(KeyCode.E))
			requestGearChange = 1;
		else if (Input.GetKeyDown(KeyCode.Q))
			requestGearChange = -1;

		resetInput = Input.GetKeyDown(KeyCode.R);
	}

	public float HorizontalInput()
	{
		return horizontalInput;
	}

	public float VerticalInput()
	{
		return verticalInput;
	}

	public bool ResetInput()
	{
		return resetInput;
	}

	public int RequestGearChange()
	{
		return requestGearChange;
	}
}
