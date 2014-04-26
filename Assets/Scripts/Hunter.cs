using UnityEngine;
using System.Collections.Generic;

public class Hunter : ICollidable
{
	Texture2D m_hunterTexture;
	public int hunterSize = 32;
	
	public Vector2 Position { get; set; }
	public float RunSpeed { get; set; }
	
	private Vector2 m_target;
	public Vector2 Target { get { return m_target; } set { m_target = value; m_hasTarget = true; } }
	private bool m_hasTarget = false;
	
	private List<Landmine> m_landmines;
	float mineCheckFrequency = 5.0f;
	float checkTimeRemaining;
	//float mineLiklihood = 0.5f;
	
	public Bounds bounds { get { return new Bounds(Position, new Vector2(hunterSize, hunterSize)); } } 
	
	public Hunter()
	{
		m_hunterTexture = Resources.Load<Texture2D>("Textures/white_circle");
		RunSpeed = 1.0f;
		Position = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);
		
		m_landmines = new List<Landmine>();
		checkTimeRemaining = mineCheckFrequency;
	}
	
	private bool ReachedTarget()
	{
		return ((Mathf.Abs(Target.x - Position.x) <= float.Epsilon) && (Mathf.Abs(Target.y - Position.y) <= float.Epsilon));
	}
	
	public bool HandleCollisions(Bounds bounds)
	{
		foreach (Landmine mine in m_landmines)
		{
			if (mine.HandleCollisions(bounds))
			{
				return true;
			}
		}
		
		return false;
	}
	
	public List<Landmine> GetLandmines()
	{
		return m_landmines;
	} 
	
	private void PlantMines()
	{
		checkTimeRemaining -= Time.deltaTime;
		
		if (checkTimeRemaining < float.Epsilon)
		{
			Landmine mine = new Landmine();
			mine.Position = Position;
			
			m_landmines.Add(mine);
			
			checkTimeRemaining = mineCheckFrequency;
		}
	}
	
	private void UpdatePosition()
	{
		if (m_hasTarget && !ReachedTarget())
		{
			float xdiff = Target.x - Position.x;
			float ydiff = Target.y - Position.y;
			
			float distance = Mathf.Sqrt(xdiff * xdiff + ydiff * ydiff);
			float cos = ydiff / distance;
			float sin = xdiff / distance;
			
			float xdistance = RunSpeed * sin;
			float ydistance = RunSpeed * cos;
			
			float newX;
			if (xdistance > 0)
			{
				newX = Mathf.Min(Target.x, Position.x + xdistance);
			}
			else
			{
				newX = Mathf.Max(Target.x, Position.x + xdistance);
			}
			
			float newY;
			if (ydistance > 0)
			{
				newY = Mathf.Min(Target.y, Position.y + ydistance);
			}
			else
			{
				newY = Mathf.Max(Target.y, Position.y + ydistance);
			}
			
			
			Vector2 newPosition = new Vector2(newX, newY);
			
			Position = newPosition;
		}
	}
	
	public void Update()
	{
		UpdatePosition();
		PlantMines();
	}
	
	public void Display()
	{
		foreach (Landmine mine in m_landmines)
		{
			mine.Display();
		}
		
		Color prevColor = GUI.color;
		GUI.color = Color.blue;
		GUI.DrawTexture(new Rect(Position.x - hunterSize / 2.0f, Position.y - hunterSize / 2.0f, hunterSize, hunterSize), m_hunterTexture);
		GUI.color = prevColor;
	}
}
