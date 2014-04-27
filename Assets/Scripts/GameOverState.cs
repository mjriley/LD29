using UnityEngine;

public class GameOverState : IState
{
	StateMachine m_parent;
	Texture2D m_deadGopherTexture;
	
	GUIStyle m_style;
	
	int padding = 50;
	
	Camera m_camera;
	
	public GameOverState(StateMachine parent, GUIStyle style)
	{
		m_parent = parent;
		m_deadGopherTexture = Resources.Load<Texture2D>("Textures/DeadGopher");
		m_style = style;
	}
	
	public void Reset()
	{
		m_camera = GameObject.Find("Main Camera").GetComponent<Camera>();
	}
	
	public void Update()
	{
		if (m_camera != null)
		{
			m_camera.backgroundColor = Color.black;
		}
	}
	
	public void Display()
	{
		int texSizeX = m_deadGopherTexture.width * 4;
		int texSizeY = m_deadGopherTexture.height * 4;
		
		float gopherX = (Screen.width - texSizeX) / 2.0f;
		float gopherY = Screen.height / 3.0f - texSizeY / 2.0f;
	
		GUI.DrawTexture(new Rect((Screen.width - texSizeX) / 2.0f, Screen.height / 3.0f - texSizeY / 2.0f, texSizeX, texSizeY), m_deadGopherTexture);
		
		GUIContent gameText = new GUIContent("GAME");
		Vector2 textCoords = m_style.CalcSize(gameText);
		GUI.Label(new Rect(gopherX - textCoords.x - padding, gopherY + texSizeY / 2.0f - textCoords.y / 2.0f, textCoords.x, textCoords.y), gameText, m_style);
		
		GUIContent overText = new GUIContent("OVER");
		textCoords = m_style.CalcSize(overText);
		GUI.Label(new Rect(gopherX + texSizeX + padding, gopherY + texSizeY / 2.0f - textCoords.y / 2.0f, textCoords.x, textCoords.y), overText, m_style);
		
		
		if (GUI.Button(new Rect(Screen.width / 2 - 50, gopherY + texSizeY + padding, 100, 30), "Play Again?"))
		{
			m_parent.MoveToState(StateMachine.State.Playing);
		}
	}
}

