using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayState : IState
{
	public float hunterSpeed = 1.0f;

	List<Vector2> m_holes;
	List<Vector2> m_possibleTreats;
	List<IItem> m_treats;
	
	Texture2D m_holeTexture;
	Texture2D m_grassTexture;
	Texture2D m_backgroundTexture;
	
	AudioClip m_backgroundTrack;
	AudioSource m_backingSource;
	
	AudioClip m_carrotEffect;
	AudioClip m_explosionEffect;
	AudioClip m_gotchaEffect;
	AudioSource m_effectSource;
	
	GUIStyle m_scoreStyle = new GUIStyle();
	
	Dictionary<KeyCode, Direction> m_keymap;
	
	Player m_player;		
	Hunter m_hunter;
	
	bool m_enabled;
	StateMachine.State m_nextState;
	
	Vector2 m_selectedHole;
	Rect m_parentRect;
	
	int m_score;
	int m_maxTreats = 3;
	float m_detonatorPercent = 5;
	float m_treatGenerationWindow = 1.0f;
	float m_secondsToNextTreat;
	
	StateMachine m_parent;
	
	public bool Active { get; set; }

	public PlayState(StateMachine parent, AudioSource backingSource, AudioSource effectSource)
	{
		InitKeymap();
		m_parent = parent;
		m_backingSource = backingSource;
		m_effectSource = effectSource;
		m_effectSource.volume = 0.15f;
		
		m_parentRect = new Rect(0.0f, 40.0f, Screen.width, Screen.height - 40.0f);
		
		m_holeTexture = Resources.Load<Texture2D>("Textures/Hole");
		m_grassTexture = Resources.Load<Texture2D>("Textures/Grass");
		m_backgroundTexture = Resources.Load<Texture2D>("Textures/white_square");
		
		m_backgroundTrack = Resources.Load<AudioClip>("Sounds/backing");
		m_carrotEffect = Resources.Load<AudioClip>("Sounds/Pickup");
		m_explosionEffect = Resources.Load<AudioClip>("Sounds/Explosion");
		m_gotchaEffect = Resources.Load<AudioClip>("Sounds/Gotcha");
		
		Reset();
		
		m_scoreStyle = new GUIStyle();
		m_scoreStyle.alignment = TextAnchor.MiddleCenter;
		m_scoreStyle.fontSize = 24;
		m_scoreStyle.normal.textColor = Color.white;
		
		Active = true;
	}
	
	private void InitKeymap()
	{
		m_keymap = new Dictionary<KeyCode, Direction>();
		m_keymap[KeyCode.Q] = Direction.NorthWest;
		m_keymap[KeyCode.W] = Direction.North;
		m_keymap[KeyCode.E] = Direction.NorthEast;
		m_keymap[KeyCode.D] = Direction.East;
		m_keymap[KeyCode.C] = Direction.SouthEast;
		m_keymap[KeyCode.X] = Direction.South;
		m_keymap[KeyCode.Z] = Direction.SouthWest;
		m_keymap[KeyCode.A] = Direction.West;
	}
	
	public void Reset()
	{
		m_enabled = true;
		//m_keymap = new Dictionary<KeyCode, Vector2>();
		
		m_player = new Player();
		m_hunter = new Hunter();
		GenerateHoles();
		InitializeTreats();
		
		m_player.Position = m_holes[0];
		m_player.Destination = m_holes[0];
		
		m_score = 0;
		
		m_player.PoppedUp += HandlePlayerEmergence;
		m_backingSource.clip = m_backgroundTrack;
		m_backingSource.loop = true;
		if (Active)
		{
			m_backingSource.Play();
		}
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
			m_effectSource.clip = m_explosionEffect;
			m_effectSource.Play();
			Disable();
			//m_parent.MoveToState(StateMachine.State.GameOver);
		}
	}
	
	private bool ReadyToExit()
	{
		return (!m_effectSource.isPlaying && m_hunter.AnimationsComplete());
	}
	
	private void GenerateHoles()
	{
		m_holes = new List<Vector2>();
		
		Vector2 point;
		
		// top row
		//m_holes.Add(new Vector2(0.0f + m_holeTexture.width / 2.0f, 0.0f + m_holeTexture.height / 2.0f));
		point = new Vector2(0.0f + m_holeTexture.width / 2.0f, 0.0f + m_holeTexture.height / 2.0f);
		//m_keymap[KeyCode.Q] = point;
		m_holes.Add(point);
		
		point = new Vector2((m_parentRect.width - m_holeTexture.width) / 2.0f + m_holeTexture.width / 2.0f, 0.0f + m_holeTexture.height / 2.0f);
		//m_keymap[KeyCode.W] = point;
		m_holes.Add(point);
		
		point = new Vector2(m_parentRect.width - m_holeTexture.width + m_holeTexture.width / 2.0f, 0.0f + m_holeTexture.height / 2.0f);
		//m_keymap[KeyCode.E] = point;
		m_holes.Add(point);
		
		
		// 2nd row
		float row_height = (m_parentRect.height - m_holeTexture.height) / 2.0f + m_holeTexture.height / 2.0f;
		point = new Vector2(m_parentRect.width / 3.0f - m_holeTexture.width / 2.0f + m_holeTexture.width / 2.0f, row_height);
		//m_keymap[KeyCode.A] = point;
		m_holes.Add(point);
		
		point = new Vector2(m_parentRect.width * 2.0f / 3.0f - m_holeTexture.width / 2.0f + m_holeTexture.width / 2.0f, row_height);
		//m_keymap[KeyCode.D] = point;
		m_holes.Add(point);
		
		// 3rd row
		row_height = m_parentRect.height - m_holeTexture.height + m_holeTexture.height / 2.0f;
		point = new Vector2(0.0f + m_holeTexture.width / 2.0f, row_height);
		//m_keymap[KeyCode.Z] = point;
		m_holes.Add(point);
		
		point = new Vector2((m_parentRect.width - m_holeTexture.width) / 2.0f + m_holeTexture.width / 2.0f, row_height);
		//m_keymap[KeyCode.X] = point;
		m_holes.Add(point);
		
		point = new Vector2(m_parentRect.width - m_holeTexture.width + m_holeTexture.width / 2.0f, row_height);
		//m_keymap[KeyCode.C] = point;
		m_holes.Add(point);
	}
	
	private void InitializeTreats()
	{
		m_treats = new List<IItem>();
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
		IItem item;
		
		int roll = UnityEngine.Random.Range(0, 100);
		if (roll < m_detonatorPercent)
		{
			item = new Detonator();
		}
		else
		{
			item = new Treat();
			//Treat treat = new Treat();
		}
		
		item.Position = m_possibleTreats[index];
		m_treats.Add(item);
		m_possibleTreats.RemoveAt(index);
	}
	
	private void UpdateTreats()
	{
		// remove expiring treats
		List<IItem> deadTreats = new List<IItem>();
		
		foreach (IItem treat in m_treats)
		{
			treat.Update();
			
			if (!treat.Enabled)
			{
				deadTreats.Add(treat);
			}
		}
		
		foreach (IItem item in deadTreats)
		{
			m_treats.Remove(item);
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
		foreach (IItem treat in deadTreats)
		{
			m_treats.Remove(treat);
			m_possibleTreats.Add(treat.Position);
		}
	}
	
	private void HandleKeyboard()
	{
//		foreach (KeyValuePair<KeyCode, Vector2> pair in m_keymap)
//		{
//			if (Input.GetKeyDown(pair.Key))
//			{
//				m_player.Destination = pair.Value;
//			}
//		}

		foreach (KeyValuePair<KeyCode, Direction> pair in m_keymap)
		{
			if (Input.GetKeyDown(pair.Key))
			{
				Vector2 currentPosition = m_player.Position;
				Vector2? closestHole = Hole.GetClosestHole(currentPosition, pair.Value, m_holes);
				
				if (closestHole.HasValue)
				{
					m_player.Destination = closestHole.GetValueOrDefault();
				}
			}
		}
	}
	
	private void Disable()
	{
		m_enabled = false;
		m_hunter.Enabled = false;
	}
	
	private void Cleanup()
	{
		m_backingSource.Stop();
	}
	
	public void Update()
	{
//		if (!Active)
//		{
//			return;
//		}
		
		HandleKeyboard();
		
		// HACK -- this shouldn't be handled regardless, but has to in order to complete animations
		m_hunter.Update();
		
		if (m_enabled)
		{
			m_hunter.RunSpeed = hunterSpeed;
			UpdateTreats();
			
			m_player.Update();
			m_hunter.Update();
			
			if (DidHunterFindPlayer())
			{
				m_effectSource.clip = m_gotchaEffect;
				m_effectSource.Play();
				Disable();
			}
			
			DetectCollisions();
		}
		else if (ReadyToExit())
		{
			Cleanup();
			m_parent.MoveToState(StateMachine.State.GameOver);
		}
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
			Color prevColor = GUI.color;
			GUI.color = new Color(0.0f, 0.0f, 0.0f, 0.5f);
			Rect bounds = new Rect(0.0f, 0.0f, parent.width, parent.height);
			GUI.DrawTexture(bounds, m_backgroundTexture);
			GUI.color = prevColor;
			
			GUI.Label(bounds, "Score: " + m_score, m_scoreStyle);
		GUI.EndGroup();
	}
	
	private void RecycleTreat(IItem item)
	{
		m_treats.Remove(item);
		m_possibleTreats.Add(item.Position);
	}
	
	private void ConsumeTreat()
	{
		IItem item = m_treats.Find(p => p.Position.x == m_player.Position.x && p.Position.y == m_player.Position.y);
		if (item != null)
		{
			if (item is Treat)
			{
				Treat treat = (Treat)item;
				m_effectSource.clip = m_carrotEffect;
				//m_effectSource.volume = 0.15f;
				m_effectSource.Play();
				m_score += treat.Value;
			}
			else if (item is Detonator)
			{
				m_hunter.DetonateMines();
				m_effectSource.clip = m_explosionEffect;
				m_effectSource.Play();
			}
			RecycleTreat(item);
		}
	}
	
	public void Display()
	{
		GUI.DrawTextureWithTexCoords(new Rect(0, 0, Screen.width, Screen.height), m_grassTexture, new Rect(0, 0, 8.0f, 8.0f));
		if (Active)
		{
			DisplayStatusBar();
		}
		
		GUI.BeginGroup(m_parentRect);
		
			foreach (Vector2 hole in m_holes)
			{
				GUI.DrawTexture(new Rect(hole.x - m_holeTexture.width / 2.0f, hole.y - m_holeTexture.height / 2.0f, m_holeTexture.width, m_holeTexture.height), m_holeTexture);
			}
			
			m_player.Display();
			m_hunter.Display();
			
			foreach (IItem item in m_treats)
			{
				item.Display();
			}
		GUI.EndGroup();
	}
}

