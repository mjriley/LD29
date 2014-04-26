using UnityEngine;
using System;
using System.Collections.Generic;

public class Player : ICollidable
{
	Texture2D m_playerTexture;
	
	float m_historyDisplayTime = 3.0f;
	
	public event EventHandler PoppedUp;
	
	public Player()
	{
		Position = new Vector2(0.0f, 0.0f);
		
		m_playerTexture = Resources.Load<Texture2D>("Textures/Gopher");
		Speed = 5.0f;
	}
	
	public Vector2 Position { get; set; }
	public Vector2 Destination { get; set; }
	public float Speed { get; set; }
	//public bool IsUp { get; set; }
	public int playerSize = 32;
	
	public Bounds bounds { 
		get 
		{ 
			Vector3 position = new Vector3(Position.x, Position.y, 0.0f);
			Vector3 extents = new Vector3(playerSize, playerSize, 0.0f);
			return new Bounds(position, extents);
		} 
	}
	
	public bool IsUp()
	{
		return AtDestination();
	}
	
	private Queue<HistoricalPoint> m_trailPoints = new Queue<HistoricalPoint>();
	
	private bool AtDestination()
	{
		return ((Mathf.Abs(Destination.x - Position.x) <= float.Epsilon) && (Mathf.Abs(Destination.y - Position.y) <= float.Epsilon));
	}
	
	public void UpdatePosition()
	{
		float xdiff = Destination.x - Position.x;
		float ydiff = Destination.y - Position.y;
		
		float distance = Mathf.Sqrt(xdiff * xdiff + ydiff * ydiff);
		
		float xdistance = Speed * xdiff / distance;
		float ydistance = Speed * ydiff / distance;
		
		float newX;
		if (xdistance > 0)
		{
			newX = Mathf.Min(Position.x + xdistance, Destination.x);
		}
		else
		{
			newX = Mathf.Max(Position.x + xdistance, Destination.x);
		}
		
		float newY;
		if (ydistance > 0)
		{
			newY = Mathf.Min(Position.y + ydistance, Destination.y);
		}
		else
		{
			newY = Mathf.Max(Position.y + ydistance, Destination.y);
		}
		
		Position = new Vector2(newX, newY);
		
		if (AtDestination() && PoppedUp != null)
		{
			PoppedUp(this, EventArgs.Empty);
		}
	}
	
	public void Update()
	{
		float timeSinceLastUpdate = Time.deltaTime;
		
		Queue<HistoricalPoint> nextQueue = new Queue<HistoricalPoint>();
		foreach (HistoricalPoint point in m_trailPoints)
		{
			if (point.remaining - timeSinceLastUpdate > float.Epsilon)
			{
				nextQueue.Enqueue(new HistoricalPoint(point.point, point.remaining - timeSinceLastUpdate));
			}
		}
		
		m_trailPoints = nextQueue;
		
		if (!AtDestination())
		{
			// enqueue the historical position
			m_trailPoints.Enqueue(new HistoricalPoint(Position, m_historyDisplayTime));
			
			UpdatePosition();
		}
	}
	
	public void Display()
	{
		Color prevColor;
		
		prevColor = GUI.color;
		foreach (HistoricalPoint point in m_trailPoints)
		{
			float colorShade = point.remaining / m_historyDisplayTime;
			GUI.color = new Color(0.0f, 0.0f, 0.0f, colorShade * 0.2f);
			GUI.DrawTexture(new Rect(point.point.x - playerSize / 2.0f, point.point.y - playerSize / 2.0f, playerSize, playerSize), m_playerTexture);
		}
		GUI.color = prevColor;
		
//		prevColor = GUI.color;
//		GUI.color = Color.red;
		GUI.DrawTexture(new Rect(Position.x - playerSize / 2.0f, Position.y - playerSize / 2.0f, playerSize, playerSize), m_playerTexture);
//		GUI.color = prevColor;
	}
}

struct HistoricalPoint
{
	public Vector2 point;
	public float remaining;
	
	public HistoricalPoint(Vector2 point, float remaining)
	{
		this.point = point;
		this.remaining = remaining;
	}
}
