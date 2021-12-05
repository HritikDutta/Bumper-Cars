public interface ICarOperator
{
	public float VerticalInput();
	public float HorizontalInput();
	public bool ResetInput();

	public int RequestGearChange();
}
