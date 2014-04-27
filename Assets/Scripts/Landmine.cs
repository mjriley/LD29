using UnityEngine;
using System;

public class Landmine : ICollidable
{
	Texture2D m_texture;
	Texture2D m_explosion;
	public int textureSize = 24;
	Animator2D m_animator;
	
	public Vector2 Position { get; set; }
	public bool Active { get; set; }
	
	bool m_isAnimating = false;
	
	public Landmine()
	{
		m_texture = Resources.Load<Texture2D>("Textures/LandmineSign");
		m_explosion = Resources.Load<Texture2D>("Textures/BoomSheet");
		
		m_animator = new Animator2D(m_explosion, textureSize, textureSize, 5, 100);
		
		Active = true;
	}
	
	public bool HandleCollisions(Bounds bounds)
	{
		if (!Active)
		{
			return false;
		}
		
		bool explosionOccurred = this.bounds.Intersects(bounds);
		if (explosionOccurred)
		{
			Detonate();
		}
		return explosionOccurred;
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
		if (m_isAnimating)
		{
			m_animator.Update();
		}
	}
	
	public void Detonate()
	{
		m_isAnimating = true;
		Active = false;
	}
	
	public bool HasDetonated()
	{
		return (m_isAnimating && m_animator.IsComplete());
	}
	
	public void Display()
	{
//		Color prevColor = GUI.color;
//		GUI.color = Color.red;
		if (m_isAnimating)
		{
			m_animator.Display(new Rect(Position.x - textureSize / 2.0f, Position.y - textureSize / 2.0f, textureSize, textureSize));
		}
		else
		{
			GUI.DrawTexture(new Rect(Position.x - textureSize / 2.0f, Position.y - textureSize / 2.0f, textureSize, textureSize), m_texture);
		}
		//GUI.color = prevColor;
	}
}
