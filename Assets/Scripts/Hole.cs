using UnityEngine;
using System.Collections.Generic;

public enum Direction : int
{
	East = 0,
	NorthEast,
	North,
	NorthWest,
	West,
	SouthWest,
	South,
	SouthEast
};

public class Hole
{
	public static int TranslateDirectionToAngle(Direction direction)
	{
		return (int)direction * 45;
	}
	
	const float TOLERANCE = 22.5f;
	
	private static bool IsWithinAngleTolerance(int desiredAngle, float angle)
	{
		if (desiredAngle == 0)
		{
			// handle the edge case
			return ((angle < TOLERANCE) || (angle > 360 - TOLERANCE));
		}
		
		return ((angle <= desiredAngle + TOLERANCE) && (angle >= desiredAngle - TOLERANCE));
	}
	
	public static float NormalizeAngle(float angle)
	{
		if (angle >= 360)
		{
			return angle % 360;
		}
		
		if (angle < 0)
		{
			angle = Mathf.Abs(angle);
			angle %= 360;
			angle = 360 - angle;
		}
		
		return angle;
	}
	
	public static Vector2? GetClosestHole(Vector2 currentHole, Direction direction, List<Vector2> holes)
	{
		int desiredAngle = TranslateDirectionToAngle(direction);
		
		float shortestDistance = float.MaxValue;
		Vector2? closestHole = null;
		
		foreach (Vector2 hole in holes)
		{
			if (hole == currentHole)
			{
				continue;
			}
			
			float diffX = (hole.x - currentHole.x);
			float diffY = (hole.y - currentHole.y);
			// invert the sign on the y differential due to the drawing coordinates being inverted
			diffY *= -1.0f;
			
			float angle = Mathf.Atan2(diffY, diffX) * (180 / Mathf.PI);
			angle = NormalizeAngle(angle);
			
			if (IsWithinAngleTolerance(desiredAngle, angle))
			{
				float distance = Mathf.Sqrt(diffX * diffX + diffY * diffY);
				if (distance < shortestDistance)
				{
					closestHole = hole;
					shortestDistance = distance;
				}
			}
		}
		
		return closestHole;
	}
}
