using UnityEngine;

public class Treat
{
	public int Value { get; set; }
	
	Texture2D m_texture;
	public int treatSize = 24;
	public float duration = 5.0f;
	public float durationLeft;
	public Vector2 Position { get; set; }
	
	public bool Enabled { get; set; }
	
	public Treat()
	{
		Value = 200;
		m_texture = Resources.Load<Texture2D>("Textures/white_circle");
		Position = new Vector2(0.0f, 0.0f);
		Enabled = true;
		
		durationLeft = duration;
	}
	
	public void Update()
	{
		durationLeft = Mathf.Max(durationLeft - Time.deltaTime, 0.0f);
		
		if (durationLeft == 0.0f)
		{
			Enabled = false;
		}
	}
	
	public void Display()
	{
		if (Enabled)
		{
			float percentRemaining = durationLeft / duration;
			Color prevColor = GUI.color;
			GUI.color = new Color(0.0f, 0.0f, 0.0f, percentRemaining);
			GUI.DrawTexture(new Rect(Position.x - treatSize / 2.0f, Position.y - treatSize / 2.0f, treatSize, treatSize), m_texture);
			GUI.color = prevColor;
		}
	}
}

