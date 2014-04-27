using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
	[TestFixture]
	public class HoleTests
	{
		[Test]
		public void CardinalDirections()
		{
			Vector2 northPoint = new Vector2(0, -1);	
			Vector2 northEastPoint = new Vector2(1, -1);
			Vector2 northWestPoint = new Vector2(-1, -1);
			Vector2 southPoint = new Vector2(0, 1);
			Vector2 southEastPoint = new Vector2(1, 1);
			Vector2 southWestPoint = new Vector2(-1, 1);
			Vector2 eastPoint = new Vector2(1, 0);
			Vector2 westPoint = new Vector2(-1, 0);
			
			List<Vector2> points = new List<Vector2>()
			{
				northPoint, northEastPoint, northWestPoint,
				southPoint, southEastPoint, southWestPoint,
				eastPoint, westPoint
			};
			
			
			
			Vector2 initialPoint = new Vector2(0, 0);
			
			Vector2? actualPoint = Hole.GetClosestHole(initialPoint, Direction.South, points);
			
			Assert.IsTrue(actualPoint.HasValue);
			Assert.AreEqual(southPoint, actualPoint.GetValueOrDefault());
		}
		
		[Test]
		public void DifferentiatesBetweenDistance()
		{
			Vector2 initialPoint = new Vector2(0, 0);
			
			List<Vector2> points = new List<Vector2>()
			{
				new Vector2(2, 0),
				new Vector2(1, 0),
				new Vector2(3, 0)
			};
			
			Vector2 actualPoint = Hole.GetClosestHole(initialPoint, Direction.East, points).GetValueOrDefault();
			Assert.AreEqual(new Vector2(1, 0), actualPoint);
		}
		
		[Test]
		public void NoViablePoints()
		{
			Vector2 initialPoint = new Vector2(0, 0);
			
			List<Vector2> points = new List<Vector2>()
			{
				new Vector2(2, 0),
				new Vector2(1, 0),
				new Vector2(3, 0)
			};
			
			Vector2? actualPoint = Hole.GetClosestHole(initialPoint, Direction.NorthEast, points);
			Assert.IsFalse(actualPoint.HasValue);
		}
		
		[Test]
		public void WraparoundAngle()
		{
			Vector2 initialPoint = new Vector2(0, 0);
			
			List<Vector2> points = new List<Vector2>()
			{
				new Vector2(2, 0),
				new Vector2(1, -0.2f)
			};
			
			Vector2 actualPoint = Hole.GetClosestHole(initialPoint, Direction.East, points).GetValueOrDefault();
			Assert.AreEqual(new Vector2(1, -0.2f), actualPoint);
		}
		
		[Test]
		public void NormalizeNegativeAngle()
		{
			Assert.AreEqual(270, Hole.NormalizeAngle(-90));
		}
	}
}

