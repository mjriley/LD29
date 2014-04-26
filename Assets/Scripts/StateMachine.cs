using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StateMachine : MonoBehaviour
{
	public GUIStyle testStyle;
	
	public enum State
	{
		Playing,
		GameOver
	};
	
	Dictionary<State, IState> m_states = new Dictionary<State, IState>();
	
	IState m_currentState;
	
	void Start()
	{
		m_states[State.Playing] = new PlayState(this);
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
