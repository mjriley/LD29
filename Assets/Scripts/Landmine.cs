using UnityEngine;
using System;

public class Landmine : ICollidable
{
	Texture2D m_texture;
	public int textureSize = 24;
	
	public Vector2 Position { get; set; }
	
	public Landmine()
	{
		m_texture = Resources.Load<Texture2D>("Textures/LandmineSign");
	}
	
	public bool HandleCollisions(Bounds bounds)
	{
		return this.bounds.Intersects(bounds);
	}
	
	public Bounds bounds { 
		get 
		{ 
			Vector3 position = new Vector3(Position.x, Position.y, 0.0f);
			Vector3 extents = new Vector3(textureSize, textureSize, 0.0f);
			return new Bounds(position, extents);
		}
	}
	
	public void Update()
	{
	}
	
	public void Display()
	{
//		Color prevColor = GUI.color;
//		GUI.color = Color.red;
		GUI.DrawTexture(new Rect(Position.x - textureSize / 2.0f, Position.y - textureSize / 2.0f, textureSize, textureSize), m_texture);
		//GUI.color = prevColor;
	}
}
