using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StateMachine : MonoBehaviour
{
	public GUIStyle testStyle;
	
	private AudioSource m_backingSource;
	private AudioSource m_effectSource;
	private AudioSource m_effect1;
	
	public enum State
	{
		Playing,
		GameOver
	};
	
	Dictionary<State, IState> m_states = new Dictionary<State, IState>();
	
	IState m_currentState;
	
	void Start()
	{
		AudioSource[] sources = GetComponents<AudioSource>();
		m_backingSource = sources[0];
		m_effectSource = sources[1];
		m_states[State.Playing] = new PlayState(this, m_backingSource, m_effectSource);
		m_states[State.GameOver] = new GameOverState(this, testStyle);
		
		MoveToState(State.Playing);
	}
	
	void Update()
	{
		if (m_currentState != null)
		{
			m_currentState.Update();
		}
	}
	
	void OnGUI()
	{
		if (m_currentState != null)
		{
			m_currentState.Display();
		}
	}
	
	public void MoveToState(State state)
	{
		m_currentState = m_states[state];
		m_currentState.Reset();
	}
}
