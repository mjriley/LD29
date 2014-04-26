using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Display : MonoBehaviour
{
	public float hunterSpeed = 5.0f;

	List<Vector2> m_holes = new List<Vector2>();
	List<Vector2> m_possibleTreats = new List<Vector2>();
	List<Treat> m_treats = new List<Treat>();
	
	Texture2D m_holeTexture;
	GUIStyle m_emptyStyle = new GUIStyle();
	
	Player m_player;		
	Hunter m_hunter;
	
	Vector2 m_selectedHole;
	Rect m_parentRect;
	
	int m_score;
	int m_maxTreats = 3;
	float m_treatGenerationWindow = 1.0f;
	float m_secondsToNextTreat;

	void Start()
	{
		m_parentRect = new Rect(0.0f, 40.0f, Screen.width, Screen.height - 40.0f);
		
		m_holeTexture = Resources.Load<Texture2D>("Textures/white_circle");
		m_player = new Player();
		m_hunter = new Hunter();
		
		GenerateHoles();
		
		foreach (Vector2 hole in m_holes)
		{
			m_possibleTreats.Add(hole);
		}
		
		m_score = 0;
		m_secondsToNextTreat = m_treatGenerationWindow;
		
		GenerateNewTreat();
	}
	
	private void GenerateHoles()
	{
		m_holes.Add(new Vector2(0.0f + m_holeTexture.width / 2.0f, 0.0f + m_holeTexture.height / 2.0f));
		m_holes.Add(new Vector2((m_parentRect.width - m_holeTexture.width) / 2.0f + m_holeTexture.width / 2.0f, 0.0f + m_holeTexture.height / 2.0f));
		m_holes.Add(new Vector2(m_parentRect.width - m_holeTexture.width + m_holeTexture.width / 2.0f, 0.0f + m_holeTexture.height / 2.0f));
		
		float row_height = (m_parentRect.height - m_holeTexture.height) / 2.0f + m_holeTexture.height / 2.0f;
		m_holes.Add(new Vector2(m_parentRect.width / 3.0f - m_holeTexture.width / 2.0f + m_holeTexture.width / 2.0f, row_height));
		m_holes.Add(new Vector2(m_parentRect.width * 2.0f / 3.0f - m_holeTexture.width / 2.0f + m_holeTexture.width / 2.0f, row_height));
		
		row_height = m_parentRect.height - m_holeTexture.height + m_holeTexture.height / 2.0f;
		m_holes.Add(new Vector2(0.0f + m_holeTexture.width / 2.0f, row_height));
		m_holes.Add(new Vector2((m_parentRect.width - m_holeTexture.width) / 2.0f + m_holeTexture.width / 2.0f, row_height));
		m_holes.Add(new Vector2(m_parentRect.width - m_holeTexture.width + m_holeTexture.width / 2.0f, row_height));
	}
	
	private void GenerateNewTreat()
	{
		int index = Random.Range(0, m_possibleTreats.Count);
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
	
	void Update()
	{
		m_hunter.RunSpeed = hunterSpeed;
		UpdateTreats();
		
		m_player.Update();
		m_hunter.Update();
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
		Treat treat = m_treats.Find(p => p.Position.x == m_selectedHole.x && p.Position.y == m_selectedHole.y);
		if (treat != null)
		{
			m_score += treat.Value;
			RecycleTreat(treat);
		}
		
	}
	
	void OnGUI()
	{
		DisplayStatusBar();
		
		GUI.BeginGroup(m_parentRect);
		
			foreach (Vector2 hole in m_holes)
			{
				if (GUI.Button(new Rect(hole.x - m_holeTexture.width / 2.0f, hole.y - m_holeTexture.height / 2.0f, m_holeTexture.width, m_holeTexture.height), m_holeTexture, m_emptyStyle))
				{
					m_player.IsUp = true;
					Vector2 holePosition = new Vector2(hole.x, hole.y);
					m_player.Position = holePosition;
					m_hunter.Target = holePosition;
					m_selectedHole = holePosition;
					ConsumeTreat();
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
