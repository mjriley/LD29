public interface IState
{
	bool Active { get; set; }
	void Update();
	void Display();
	void Reset();
}
