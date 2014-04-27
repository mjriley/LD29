using UnityEngine;

public class StartScreen : MonoBehaviour
{
	public GUIStyle m_style;
	
	public void Start()
	{
	}
	
	public void OnGUI()
	{
		GUIContent title = new GUIContent("GOPHER'S REVENGE");
		Vector2 bounds = m_style.CalcSize(title);
		GUI.Label(new Rect(Screen.width / 2.0f - bounds.x / 2.0f, Screen.height / 4.0f - bounds.y / 2.0f, bounds.x, bounds.y), title, m_style);
		
		GUI.Button(new Rect(Screen.width / 2.0f - 50, Screen.height / 4.0f + bounds.y / 2.0f + 50, 100, 30), "Start Game");
	}
}
