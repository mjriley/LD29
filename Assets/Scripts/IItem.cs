using UnityEngine;

public interface IItem
{
	bool Enabled { get; }
	Vector2 Position { get; set; }
	void Update();
	void Display();
}
