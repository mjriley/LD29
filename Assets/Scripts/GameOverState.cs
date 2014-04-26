using UnityEngine;

public class GameOverState : IState
{
	StateMachine m_parent;
	
	public GameOverState(StateMachine parent)
	{
		m_parent = parent;
	}
	
	public void Reset()
	{
	}
	
	public void Update()
	{
	}
	
	public void Display()
	{
		GUI.Label(new Rect(0, 0, 200, 100), "Game Over!");
		
		if (GUI.Button(new Rect(Screen.width / 2, Screen.height / 2, 100, 30), "Start Over?"))
		{
			m_parent.MoveToState(StateMachine.State.Playing);
		}
	}
}

