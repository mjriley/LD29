using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayState : IState
{
	public float hunterSpeed = 1.0f;

	List<Vector2> m_holes;
	List<Vector2> m_possibleTreats;
	List<Treat> m_treats;
	
	Texture2D m_holeTexture;
	GUIStyle m_emptyStyle = new GUIStyle();
	
	Dictionary<KeyCode, Vector2> m_keymap;
	
	Player m_player;		
	Hunter m_hunter;
	
	Vector2 m_selectedHole;
	Rect m_parentRect;
	
	int m_score;
	int m_maxTreats = 3;
	float m_treatGenerationWindow = 1.0f;
	float m_secondsToNextTreat;
	
	StateMachine m_parent;

	public PlayState(StateMachine parent)
	{
		m_parent = parent;
		
		m_parentRect = new Rect(0.0f, 40.0f, Screen.width, Screen.height - 40.0f);
		
		m_holeTexture = Resources.Load<Texture2D>("Textures/white_circle");
		
		Reset();
	}
	
	public void Reset()
	{
		m_keymap = new Dictionary<KeyCode, Vector2>();
		m_player = new Player();
		m_hunter = new Hunter();
		GenerateHoles();
		InitializeTreats();
		
		m_player.Position = m_holes[0];
		m_player.Destination = m_holes[0];
		
		m_score = 0;
		
		m_player.PoppedUp += HandlePlayerEmergence;
	}
	
	private void HandlePlayerEmergence(object player, EventArgs args)
	{
		m_hunter.Target = m_player.Position;
		ConsumeTreat();
	}
	
	private void DetectCollisions()
	{
		if (m_hunter.HandleCollisions(m_player.bounds))
		{
			m_parent.MoveToState(StateMachine.State.GameOver);
		}
	}
	
	private void GenerateHoles()
	{
		m_holes = new List<Vector2>();
		
		Vector2 point;
		
		// top row
		//m_holes.Add(new Vector2(0.0f + m_holeTexture.width / 2.0f, 0.0f + m_holeTexture.height / 2.0f));
		point = new Vector2(0.0f + m_holeTexture.width / 2.0f, 0.0f + m_holeTexture.height / 2.0f);
		m_keymap[KeyCode.Q] = point;
		m_holes.Add(point);
		
		point = new Vector2((m_parentRect.width - m_holeTexture.width) / 2.0f + m_holeTexture.width / 2.0f, 0.0f + m_holeTexture.height / 2.0f);
		m_keymap[KeyCode.W] = point;
		m_holes.Add(point);
		
		point = new Vector2(m_parentRect.width - m_holeTexture.width + m_holeTexture.width / 2.0f, 0.0f + m_holeTexture.height / 2.0f);
		m_keymap[KeyCode.E] = point;
		m_holes.Add(point);
		
		
		// 2nd row
		float row_height = (m_parentRect.height - m_holeTexture.height) / 2.0f + m_holeTexture.height / 2.0f;
		point = new Vector2(m_parentRect.width / 3.0f - m_holeTexture.width / 2.0f + m_holeTexture.width / 2.0f, row_height);
		m_keymap[KeyCode.A] = point;
		m_holes.Add(point);
		
		point = new Vector2(m_parentRect.width * 2.0f / 3.0f - m_holeTexture.width / 2.0f + m_holeTexture.width / 2.0f, row_height);
		m_keymap[KeyCode.D] = point;
		m_holes.Add(point);
		
		// 3rd row
		row_height = m_parentRect.height - m_holeTexture.height + m_holeTexture.height / 2.0f;
		point = new Vector2(0.0f + m_holeTexture.width / 2.0f, row_height);
		m_keymap[KeyCode.Z] = point;
		m_holes.Add(point);
		
		point = new Vector2((m_parentRect.width - m_holeTexture.width) / 2.0f + m_holeTexture.width / 2.0f, row_height);
		m_keymap[KeyCode.X] = point;
		m_holes.Add(point);
		
		point = new Vector2(m_parentRect.width - m_holeTexture.width + m_holeTexture.width / 2.0f, row_height);
		m_keymap[KeyCode.C] = point;
		m_holes.Add(point);
	}
	
	private void InitializeTreats()
	{
		m_treats = new List<Treat>();
		m_possibleTreats = new List<Vector2>();
		foreach (Vector2 hole in m_holes)
		{
			m_possibleTreats.Add(hole);
		}
		
		m_secondsToNextTreat = m_treatGenerationWindow;
		
		GenerateNewTreat();
	}
	
	private void GenerateNewTreat()
	{
		int index = UnityEngine.Random.Range(0, m_possibleTreats.Count);
		Treat treat = new Treat();
		treat.Position = m_possibleTreats[index];
		m_treats.Add(treat);
		m_possibleTreats.RemoveAt(index);
	}
	
	private void UpdateTreats()
	{
		
		// remove expiring treats
		List<Treat> deadTreats = new List<Treat>();
		
		foreach (Treat treat in m_treats)
		{
			treat.Update();
			
			if (!treat.Enabled)
			{
				deadTreats.Add(treat);
			}
		}
		
		foreach (Treat treat in deadTreats)
		{
			m_treats.Remove(treat);
		}
		
		// generate a new, different treat (if one expired on this frame)
		m_secondsToNextTreat -= Time.deltaTime;
		if (m_secondsToNextTreat <= 0)
		{
			if (m_treats.Count < m_maxTreats)
			{
				GenerateNewTreat();
			}
			m_secondsToNextTreat = m_treatGenerationWindow;
		}
		
		// add the expiring treats back to the pool
		foreach (Treat treat in deadTreats)
		{
			m_treats.Remove(treat);
			m_possibleTreats.Add(treat.Position);
		}
	}
	
	private void HandleKeyboard()
	{
		foreach (KeyValuePair<KeyCode, Vector2> pair in m_keymap)
		{
			if (Input.GetKeyDown(pair.Key))
			{
				m_player.Destination = pair.Value;
			}
		}
	}
	
	public void Update()
	{
		m_hunter.RunSpeed = hunterSpeed;
		UpdateTreats();
		
		m_player.Update();
		m_hunter.Update();
		
		if (DidHunterFindPlayer())
		{
			m_parent.MoveToState(StateMachine.State.GameOver);
		}
		
		DetectCollisions();
		
		HandleKeyboard();
	}
	
	private bool DidHunterFindPlayer()
	{
		if (Mathf.Abs(m_hunter.Position.x - m_player.Position.x) <= float.Epsilon && Mathf.Abs(m_hunter.Position.y - m_player.Position.y) <= float.Epsilon)
		{
			return true;
		}
		
		return false;
	}
	
	private void DisplayStatusBar()
	{
		Rect parent = new Rect(0.0f, 0.0f, Screen.width, 40.0f);
		GUI.BeginGroup(parent);
			GUI.Box(new Rect(0.0f, 0.0f, parent.width, parent.height), "Score: " + m_score);
		GUI.EndGroup();
	}
	
	private void RecycleTreat(Treat treat)
	{
		m_treats.Remove(treat);
		m_possibleTreats.Add(treat.Position);
	}
	
	private void ConsumeTreat()
	{
		Treat treat = m_treats.Find(p => p.Position.x == m_player.Position.x && p.Position.y == m_player.Position.y);
		if (treat != null)
		{
			m_score += treat.Value;
			RecycleTreat(treat);
		}
		
	}
	
	public void Display()
	{
		DisplayStatusBar();
		
		GUI.BeginGroup(m_parentRect);
		
			foreach (Vector2 hole in m_holes)
			{
				if (GUI.Button(new Rect(hole.x - m_holeTexture.width / 2.0f, hole.y - m_holeTexture.height / 2.0f, m_holeTexture.width, m_holeTexture.height), m_holeTexture, m_emptyStyle))
				{
					Vector2 holePosition = new Vector2(hole.x, hole.y);
					m_player.Destination = holePosition;
				}
			}
			
			m_player.Display();
			m_hunter.Display();
			
			foreach (Treat treat in m_treats)
			{
				treat.Display();
			}
		GUI.EndGroup();
	}
}

