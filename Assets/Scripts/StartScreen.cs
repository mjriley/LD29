using UnityEngine;

public class StartScreen : MonoBehaviour
{
	enum State
	{
		Default,
		HowToPlay,
		Keybindings
	};
	
	State m_currentState;
	Texture2D m_boxBackground;
	Texture2D m_gameBackground;
	string m_instructions;
	string m_keybinds;
	
	public GUIStyle m_style;
	public GUIStyle m_keybindStyle;
	public GUIStyle m_instructionStyle;
	
	public GameObject mainGame;
	
	public void Start()
	{
		m_currentState = State.Default;
		m_boxBackground = Resources.Load<Texture2D>("Textures/white_square");
		m_gameBackground = Resources.Load<Texture2D>("Textures/GameShot");
		
		m_instructions = 
			@"
The groundskeeper is out to kill you! 
As the gopher, burrow from hole to hole, eating carrots to increase your score. 
Do not let the groundskeeper catch you unburrowed
(although you can move through him under ground).
Be careful not to run into the groundskeeper's landmines.
Look out for a rare detonator to clear all landmines.

Movement: 
	Directional movement will steer you to the closest hole in the desired direction.
	Note that you can change direction while moving between holes.
			";
			
		m_keybinds =
			@"
Q: Move to nearest Northwest hole
W: Move to nearest North hole
E: Move to nearest Northeast hole
A: Move to nearest West hole
D: Move to nearest East hole
Z: Move to nearest Southwest hole
X: Move to nearest South hole
C: Move to nearest Southeast hole
			";
			
		m_instructionStyle = new GUIStyle();
		m_instructionStyle.normal.textColor = Color.white;
		m_instructionStyle.fontSize = 14;
			
		m_keybindStyle = new GUIStyle();
		m_keybindStyle.normal.textColor = Color.white;
		m_keybindStyle.fontSize = 18;
	}
	
	public void OnGUI()
	{
		Color prevColor = GUI.color;
		GUI.color = new Color(0.4f, 0.4f, 0.4f);
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), m_gameBackground);
		GUI.color = prevColor;
		
		GUIContent title = new GUIContent("GOPHER'S REVENGE");
		Vector2 bounds = m_style.CalcSize(title);
		GUI.Label(new Rect(Screen.width / 2.0f - bounds.x / 2.0f, Screen.height / 4.0f - bounds.y / 2.0f, bounds.x, bounds.y), title, m_style);
		
		if (GUI.Button(new Rect(Screen.width / 2.0f - 50, Screen.height / 4.0f + bounds.y / 2.0f + 50, 100, 30), "Start Game"))
		{
			mainGame.SetActive(true);
			gameObject.SetActive(false);
		}
		
		if (GUI.Button(new Rect(Screen.width / 3.0f - 50, Screen.height	 / 4.0f * 3.0f + bounds.y / 2.0f, 100, 30), "How to Play"))
		{
			m_currentState = State.HowToPlay;
		}
		
		if (GUI.Button(new Rect(Screen.width / 3.0f * 2.0f - 50, Screen.height / 4.0f * 3.0f + bounds.y / 2.0f, 100, 30), "Keybindings"))
		{
			m_currentState = State.Keybindings;
		}
		
		if (m_currentState == State.HowToPlay)
		{
			float boxWidth = 5.0f * Screen.width / 6.0f;
			float boxHeight = 5.0f * Screen.height / 6.0f;
			Rect box = new Rect(Screen.width / 2.0f - boxWidth / 2.0f, Screen.height / 2.0f - boxHeight / 2.0f, boxWidth, boxHeight);
			
			GUI.BeginGroup(box);
				prevColor = GUI.color;
				GUI.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
				GUI.DrawTexture(new Rect(0.0f, 0.0f, box.width, box.height), m_boxBackground);
				GUI.color = prevColor;
				
				GUIContent content = new GUIContent(m_instructions);
				GUIStyle style = m_instructionStyle;
				Vector2 sizes = style.CalcSize(content);
				float height = style.CalcHeight(content, sizes.x);
				GUI.Label(new Rect(box.width / 2.0f - sizes.x / 2.0f, box.height / 2.0f - height / 2.0f, sizes.x, height), content, style);
				
				int backButtonWidth = 100;
				int backButtonHeight = 30;
				int verticalPadding = 10;
				if (GUI.Button(new Rect(box.width / 2.0f - backButtonWidth / 2.0f, box.height - backButtonHeight - verticalPadding, backButtonWidth, backButtonHeight), "OK!"))
				{
					m_currentState = State.Default;
				}
			GUI.EndGroup();
		}
		else if (m_currentState == State.Keybindings)
		{
			float boxWidth = 5.0f * Screen.width / 6.0f;
			float boxHeight = 5.0f * Screen.height / 6.0f;
			Rect box = new Rect(Screen.width / 2.0f - boxWidth / 2.0f, Screen.height / 2.0f - boxHeight / 2.0f, boxWidth, boxHeight);
			
			GUI.BeginGroup(box);
				prevColor = GUI.color;
				GUI.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
				GUI.DrawTexture(new Rect(0.0f, 0.0f, box.width, box.height), m_boxBackground);
				GUI.color = prevColor;
				
				GUIContent content = new GUIContent(m_keybinds);
				Vector2 sizes = m_keybindStyle.CalcSize(content);
				float height = m_keybindStyle.CalcHeight(content, sizes.x);
				GUI.Label(new Rect(box.width / 2.0f - sizes.x / 2.0f, box.height / 2.0f - height / 2.0f, sizes.x, height), content, m_keybindStyle);
				
				int backButtonWidth = 100;
				int backButtonHeight = 30;
				int verticalPadding = 10;
				if (GUI.Button(new Rect(box.width / 2.0f - backButtonWidth / 2.0f, box.height - backButtonHeight - verticalPadding, backButtonWidth, backButtonHeight), "OK!"))
				{
					m_currentState = State.Default;
				}
			GUI.EndGroup();
		}
	}
}
