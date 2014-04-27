using UnityEngine;

public class Animator2D
{
	Texture2D m_texture;
	int m_totalFrames;
	int m_frameDurationMillis;
	
	int m_currentFrame;
	int m_elapsedTime;
	
	Rect m_frameRect;
	
	const int SECS_TO_MILLIS = 1000;
	
	public Animator2D(Texture2D texture, int frameWidth, int frameHeight, int totalFrames, int frameDurationMillis)
	{
		m_texture = texture;
		m_totalFrames = totalFrames;
		m_frameDurationMillis = frameDurationMillis;
		
		m_frameRect = new Rect(0.0f, 0.0f, frameWidth, frameHeight);
		
		Reset();
	}
	
	void Reset()
	{
		m_currentFrame = 0;
		m_elapsedTime = 0;
	}
	
	public void Update()
	{
		if (IsComplete())
		{
			return;
		}
		
		m_elapsedTime += (int)(Time.deltaTime * SECS_TO_MILLIS);
		
		m_currentFrame += m_elapsedTime / m_frameDurationMillis;
		
		m_elapsedTime = m_elapsedTime % m_frameDurationMillis;
	}
	
	public void Display(Rect parent)
	{
		if (IsComplete())
		{
			return;
		}
		
		GUI.BeginGroup(parent);
			GUI.DrawTextureWithTexCoords(m_frameRect, m_texture, new Rect((float)m_currentFrame / (float)m_totalFrames, 0.0f, 1.0f / m_totalFrames, 1.0f));
		GUI.EndGroup();
	}
	
	public bool IsComplete()
	{
		return (m_currentFrame >= m_totalFrames);
	}
}

