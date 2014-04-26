using UnityEngine;

public class Player
{
	Texture2D m_playerTexture;
	
	public Player()
	{
		IsUp = false;
		Position = new Vector2(0.0f, 0.0f);
		
		m_playerTexture = Resources.Load<Texture2D>("Textures/white_circle");
	}
	
	public Vector2 Position { get; set; }
	public bool IsUp { get; set; }
	public int playerSize = 32;
	
	public void Update()
	{
		
	}
	
	public void Display()
	{
		if (IsUp)
		{
			Color prevColor = GUI.color;
			GUI.color = Color.red;
			GUI.DrawTexture(new Rect(Position.x - playerSize / 2.0f, Position.y - playerSize / 2.0f, playerSize, playerSize), m_playerTexture);
			GUI.color = prevColor;
		}
	}
}
