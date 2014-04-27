using UnityEngine;

public class Detonator : IItem
{
	public Texture2D m_texture;
	public int textureSize = 24;
	public float duration = 5.0f;
	public float durationLeft;
	
	public Vector2 Position { get; set; }
	
	public bool Enabled { get; set; }
	
	public Detonator()
	{
		Enabled = true;
		m_texture = Resources.Load<Texture2D>("Textures/Detonator");
		
		durationLeft = duration;
	}
	
	public void Update()
	{
		if (!Enabled)
		{
			return;
		}
		
		durationLeft = Mathf.Max(durationLeft - Time.deltaTime, 0.0f);
		
		if (durationLeft == 0.0f)
		{
			Enabled = false;
		}
	}
	
	public void Display()
	{
		float percentRemaining = durationLeft / duration;
		Color prevColor = GUI.color;
		GUI.color = new Color(1.0f, 1.0f, 1.0f, percentRemaining);
		Rect rect = new Rect(Position.x - textureSize / 2.0f, Position.y - textureSize / 2.0f, textureSize, textureSize);
		GUI.DrawTexture(rect, m_texture);
		GUI.color = prevColor;
	}
}

