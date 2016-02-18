using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Mandel.Mapbuilder
{

	/// <summary>
	/// This class demonstrates a simple map builder for a roguelike game. For a detailed
	/// look at using it, go here http://www.evilscience.co.uk/?p=553
	/// </summary>
	internal class MapBuilder
	{
		private const int emptyCell = 0;

		private const int filledCell = 1;

		public int[,] Map;

		private readonly Point[] DirectionsStraight = new[]
			{
				new Point(0, -1), //n
				new Point(0, 1), //s
				new Point(1, 0), //w
				new Point(-1, 0) //e
			};

		private readonly Random rnd = new Random();

		/// <summary>
		/// Built corridors stored here
		/// </summary>
		private List<Point> BuiltCorridors;

		/// <summary>
		/// Built rooms stored here
		/// </summary>
		private List<Rectangle> BuiltRooms;

		/// <summary>
		/// Room to be built stored here
		/// </summary>
		private Rectangle CurrentRoom;

		/// <summary>
		/// Corridor to be built stored here
		/// </summary>
		private List<Point> PotentialCorridors;

		public MapBuilder(int x, int y)
		{
			this.MapSize = new Size(x, y);
			this.Map = new int[x, y];
			this.CorridorMaxTurns = 5;
			this.RoomMin = new Size(15, 15);
			this.RoomMax = new Size(50, 50);
			this.CorridorMin = 3;
			this.CorridorMax = 15;
			this.MaxRooms = 15;
			//this.MapSize = new Size(150, 150);

			this.RoomDistance = 5;
			this.CorridorDistance = 2;
			this.CorridorSpace = 2;

			this.BuildProb = 50;
			this.BreakOut = 250;
		}

		public delegate void moveDelegate();

		public event moveDelegate playerMoved;

		/// <summary>
		/// describes the outcome of the corridor building operation
		/// </summary>
		private enum CorridorItemHit
		{
			Invalid, //invalid point generated
			Self, //corridor hit self
			ExistingCorridor, //hit a built corridor
			OriginRoom, //corridor hit origin room 
			ExistingRoom, //corridor hit existing room
			Completed, //corridor built without problem    
			TooClose, //
			OK //point OK
		}

		public int BreakOut { get; set; }

		public int BuildProb { get; set; }

		public int CorridorDistance { get; set; }

		public int CorridorMax { get; set; }

		public int CorridorMaxTurns { get; set; }

		public int CorridorMin { get; set; }

		public int CorridorSpace { get; set; }

		public int MaxRooms { get; set; }

		public int RoomDistance { get; set; }

		public Size MapSize { get; set; }

		public Size RoomMax { get; set; }

		public Size RoomMin { get; set; }


		/// <summary>
		/// Randomly choose a room and attempt to build a corridor terminated by
		/// a room off it, and repeat until MaxRooms has been reached. The map
		/// is started of by placing two rooms on opposite sides of the map and joins
		/// them with a long corridor, using the method PlaceStartRooms()
		/// </summary>
		/// <returns>
		/// Bool indicating if the map was built, i.e. the property BreakOut was not
		/// exceed
		/// </returns>
		public bool BuildConnectedStartRooms()
		{
			int loopctr = 0;

			this.Clear();

			this.PlaceStartRooms();

			// Attempt to build the required number of rooms
			while (this.BuiltRooms.Count() < this.MaxRooms)
			{
				// Bail out if this value is exceeded
				if (loopctr++ > this.BreakOut)
				{
					return false;
				}

				Point location;
				Point direction;
				if (this.CorridorGetStart(out location, out direction))
				{
					CorridorItemHit CorBuildOutcome = this.CorridorMakeStraight(
						ref location, ref direction, this.rnd.Next(1, this.CorridorMaxTurns), this.rnd.Next(0, 100) > 50);

					switch (CorBuildOutcome)
					{
						case CorridorItemHit.ExistingRoom:
						case CorridorItemHit.ExistingCorridor:
						case CorridorItemHit.Self:
							this.CorridorBuild();
							break;

						case CorridorItemHit.Completed:
							if (this.RoomAttemptBuildOnCorridor(direction))
							{
								this.CorridorBuild();
								this.RoomBuild();
							}
							break;
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Randomly choose a room and attempt to build a corridor terminated by
		/// a room off it, and repeat until MaxRooms has been reached. The map
		/// is started of by placing a room in approximately the centre of the map
		/// using the method PlaceStartRoom()
		/// </summary>
		/// <returns>
		/// Bool indicating if the map was built, i.e. the property BreakOut was not exceed
		/// </returns>
		public bool BuildOneStartRoom()
		{
			int loopctr = 0;

			this.Clear();

			this.PlaceStartRoom();

			// Attempt to build the required number of rooms
			while (this.BuiltRooms.Count() < this.MaxRooms)
			{
				if (loopctr++ > this.BreakOut)
				{
					// Bail out if this value is exceeded
					return false;
				}

				Point location;
				Point direction;

				if (this.CorridorGetStart(out location, out direction))
				{
					CorridorItemHit outcome = this.CorridorMakeStraight(
						ref location, ref direction, this.rnd.Next(1, this.CorridorMaxTurns), this.rnd.Next(0, 100) > 50);

					switch (outcome)
					{
						case CorridorItemHit.ExistingRoom:
						case CorridorItemHit.ExistingCorridor:
						case CorridorItemHit.Self:
							this.CorridorBuild();
							break;

						case CorridorItemHit.Completed:
							if (this.RoomAttemptBuildOnCorridor(direction))
							{
								this.CorridorBuild();
								this.RoomBuild();
							}
							break;
					}
				}
			}

			return true;
		}

		#region Methods

		protected virtual void OnPlayerMoved()
		{
			moveDelegate handler = this.playerMoved;

			if (handler != null)
			{
				handler();
			}
		}

		/// <summary>
		/// Initialise everything
		/// </summary>
		private void Clear()
		{
			this.PotentialCorridors = new List<Point>();
			this.BuiltRooms = new List<Rectangle>();
			this.BuiltCorridors = new List<Point>();
			this.Map = new int[this.MapSize.Width,this.MapSize.Height];

			for (int x = 0; x < this.MapSize.Width; x++)
			{
				for (int y = 0; y < this.MapSize.Height; y++)
				{
					this.Map[x, y] = filledCell;
				}
			}
		}

		/// <summary>
		/// Build the contents of PotentialCorridor, adding it's points to the builtCorridors
		/// list then empty
		/// </summary>
		private void CorridorBuild()
		{
			foreach (Point p in this.PotentialCorridors)
			{
				this.SetPoint(p.X, p.Y, emptyCell);
				this.BuiltCorridors.Add(p);
			}

			this.PotentialCorridors.Clear();
		}

		/// <summary>
		/// Randomly get a point on an existing corridor
		/// </summary>
		/// <param name="location">Out: Location of point on room edge</param>
		/// <param name="direction">Out: Direction of point</param>
		/// <returns>Bool indicating success</returns>
		private void CorridorGetEdge(out Point location, out Point direction)
		{
			var validdirections = new List<Point>();

			do
			{
				//the modifiers below prevent the first of last point being chosen
				location = this.BuiltCorridors[this.rnd.Next(1, this.BuiltCorridors.Count - 1)];

				//attempt to locate all the empy map points around the location
				//using the directions to offset the randomly chosen point
				foreach (Point p in this.DirectionsStraight)
				{
					if (this.IsPointValid(location.X + p.X, location.Y + p.Y))
					{
						if (this.GetPoint(location.X + p.X, location.Y + p.Y) == filledCell)
						{
							validdirections.Add(p);
						}
					}
				}
			}
			while (validdirections.Count == 0);

			direction = validdirections[this.rnd.Next(0, validdirections.Count)];
			location.Offset(direction);
		}

		/// <summary>
		/// Get a starting point for a corridor, randomly choosing between a room and a corridor.
		/// </summary>
		/// <param name="location">Out: pLocation of point</param>
		/// <param name="direction">Out: pDirection of point</param>
		/// <returns>Bool indicating if location found is OK</returns>
		private bool CorridorGetStart(out Point location, out Point direction)
		{
			this.CurrentRoom = new Rectangle();
			this.PotentialCorridors = new List<Point>();

			if (this.BuiltCorridors.Count > 0)
			{
				if (this.rnd.Next(0, 100) >= this.BuildProb)
				{
					this.RoomGetEdge(out location, out direction);
				}
				else
				{
					this.CorridorGetEdge(out location, out direction);
				}
			}
			else
			{
				// No corridors present, so build off a room
				this.RoomGetEdge(out location, out direction);
			}

			// Finally check the point we've found
			return this.CorridorPointTest(location, direction) == CorridorItemHit.OK;
		}

		/// <summary>
		/// Attempt to make a corridor, storing it in the lPotentialCorridor list
		/// </summary>
		/// <param name="start">Start point of corridor</param>
		/// <param name="direction"></param>
		/// <param name="turns">Number of turns to make</param>
		/// <param name="preventBackTracking"></param>
		private CorridorItemHit CorridorMakeStraight(ref Point start, ref Point direction, int turns, bool preventBackTracking)
		{
			this.PotentialCorridors = new List<Point> { start };

			int corridorlength;
			var startdirection = new Point(direction.X, direction.Y);

			while (turns > 0)
			{
				turns--;

				corridorlength = this.rnd.Next(this.CorridorMin, this.CorridorMax);

				// Build corridor
				while (corridorlength > 0)
				{
					corridorlength--;

					// Make a point and offset it
					start.Offset(direction);

					CorridorItemHit outcome = this.CorridorPointTest(start, direction);

					if (outcome != CorridorItemHit.OK)
					{
						return outcome;
					}

					this.PotentialCorridors.Add(start);
				}

				if (turns > 1)
				{
					direction = !preventBackTracking
								? this.GetRandomDirection(direction)
								: this.GetRandomDirection(direction, startdirection);
				}
			}

			return CorridorItemHit.Completed;
		}

		/// <summary>
		/// Test the provided point to see if it has empty cells on either side
		/// of it. This is to stop corridors being built adjacent to a room.
		/// </summary>
		/// <param name="point">Point to test</param>
		/// <param name="direction">Direction it is moving in</param>
		/// <returns></returns>
		private CorridorItemHit CorridorPointTest(Point point, Point direction)
		{
			// Invalid point hit, exit
			if (!this.IsPointValid(point.X, point.Y))
			{
				return CorridorItemHit.Invalid;
			}

			// In an existing corridor
			if (this.BuiltCorridors.Contains(point))
			{
				return CorridorItemHit.ExistingCorridor;
			}

			// Hit self
			if (this.PotentialCorridors.Contains(point))
			{
				return CorridorItemHit.Self;
			}

			// The corridors origin room has been reached, exit
			if (this.CurrentRoom.Contains(point))
			{
				return CorridorItemHit.OriginRoom;
			}

			// Is point in a room
			if (this.BuiltRooms.Any(r => r.Contains(point)))
			{
				return CorridorItemHit.ExistingRoom;
			}

			// Using the property corridor space, check that number of cells on
			// either side of the point are empty
			foreach (int r in Enumerable.Range(-this.CorridorSpace, 2 * this.CorridorSpace + 1).ToList())
			{
				// North or south
				if (direction.X == 0)
				{
					if (this.IsPointValid(point.X + r, point.Y))
					{
						if (this.GetPoint(point.X + r, point.Y) != filledCell)
						{
							return CorridorItemHit.TooClose;
						}
					}
				}
					// East or west
				else if (direction.Y == 0)
				{
					if (this.IsPointValid(point.X, point.Y + r))
					{
						if (this.GetPoint(point.X, point.Y + r) != filledCell)
						{
							return CorridorItemHit.TooClose;
						}
					}
				}
			}

			return CorridorItemHit.OK;
		}

		private Point DirectionReverse(Point direction)
		{
			return new Point(-direction.X, -direction.Y);
		}

		private int GetPoint(int x, int y)
		{
			return this.Map[x, y];
		}

		/// <summary>
		/// Get a random direction, excluding the opposite of the provided direction to
		/// prevent a corridor going back on it's Build
		/// </summary>
		/// <param name="dir">Current direction</param>
		/// <returns></returns>
		private Point GetRandomDirection(Point dir)
		{
			int len = this.DirectionsStraight.GetLength(0);

			Point newDirection = this.DirectionsStraight[this.rnd.Next(0, len)];

			while (this.DirectionReverse(newDirection) == dir)
			{
				newDirection = this.DirectionsStraight[this.rnd.Next(0, len)];
			}

			return newDirection;
		}

		/// <summary>
		/// Get a random direction, excluding the provided directions and the opposite of
		/// the provided direction to prevent a corridor going back on it's self.
		/// The parameter pDirExclude is the first direction chosen for a corridor, and
		/// to prevent it from being used will prevent a corridor from going back on
		/// it'self
		/// </summary>
		/// <param name="dir">Current direction</param>
		/// <param name="dirExclude">Direction to exclude</param>
		/// <returns></returns>
		private Point GetRandomDirection(Point dir, Point dirExclude)
		{
			int len = this.DirectionsStraight.GetLength(0);

			Point newDirection = this.DirectionsStraight[this.rnd.Next(0, len)];

			while (this.DirectionReverse(newDirection) == dir | this.DirectionReverse(newDirection) == dirExclude)
			{
				newDirection = this.DirectionsStraight[this.rnd.Next(0, len)];
			}

			return newDirection;
		}

		private Boolean IsPointValid(int x, int y)
		{
			return x >= 0 & x < this.Map.GetLength(0) & y >= 0 & y < this.Map.GetLength(1);
		}

		/// <summary>
		/// Place a random sized room in the middle of the map
		/// </summary>
		private void PlaceStartRoom()
		{
			this.CurrentRoom = new Rectangle
							{
								Width = this.rnd.Next(this.RoomMin.Width, this.RoomMax.Width),
								Height = this.rnd.Next(this.RoomMin.Height, this.RoomMax.Height),
								X = this.MapSize.Width / 2,
								Y = this.MapSize.Height / 2
							};
			this.RoomBuild();
		}

		/// <summary>
		/// Place a start room anywhere on the map
		/// </summary>
		private void PlaceStartRooms()
		{
			bool connection = false;

			while (!connection)
			{
				this.Clear();
				Point startdirection = this.GetRandomDirection(new Point());

				//place a room on the top and bottom
				if (startdirection.X == 0)
				{
					//room at the top of the map
					this.CurrentRoom = new Rectangle
									{
										Width = this.rnd.Next(this.RoomMin.Width, this.RoomMax.Width),
										Height = this.rnd.Next(this.RoomMin.Height, this.RoomMax.Height),
										X = this.rnd.Next(0, this.MapSize.Width - this.CurrentRoom.Width),
										Y = 1
									};
					this.RoomBuild();

					//at the bottom of the map
					this.CurrentRoom = new Rectangle
									{
										Width = this.rnd.Next(this.RoomMin.Width, this.RoomMax.Width),
										Height = this.rnd.Next(this.RoomMin.Height, this.RoomMax.Height),
										X = this.rnd.Next(0, this.MapSize.Width - this.CurrentRoom.Width),
										Y = this.MapSize.Height - this.CurrentRoom.Height - 1
									};
					this.RoomBuild();
				}
				else //place a room on the east and west side
				{
					//west side of room
					this.CurrentRoom = new Rectangle
									{
										Width = this.rnd.Next(this.RoomMin.Width, this.RoomMax.Width),
										Height = this.rnd.Next(this.RoomMin.Height, this.RoomMax.Height),
										Y = this.rnd.Next(0, this.MapSize.Height - this.CurrentRoom.Height),
										X = 1
									};

					this.RoomBuild();

					this.CurrentRoom = new Rectangle
									{
										Width = this.rnd.Next(this.RoomMin.Width, this.RoomMax.Width),
										Height = this.rnd.Next(this.RoomMin.Height, this.RoomMax.Height),
										Y = this.rnd.Next(0, this.MapSize.Height - this.CurrentRoom.Height),
										X = this.MapSize.Width - this.CurrentRoom.Width - 2
									};

					this.RoomBuild();
				}

				Point location;
				Point direction;

				if (this.CorridorGetStart(out location, out direction))
				{
					switch (this.CorridorMakeStraight(ref location, ref direction, 100, true))
					{
						case CorridorItemHit.ExistingRoom:
							this.CorridorBuild();
							connection = true;
							break;
					}
				}
			}
		}

		/// <summary>
		/// Make a room off the last point of Corridor, using
		/// CorridorDirection as an indicator of how to offset the room.
		/// The potential room is stored in Room.
		/// </summary>
		private bool RoomAttemptBuildOnCorridor(Point direction)
		{
			this.CurrentRoom = new Rectangle
							{
								Width = this.rnd.Next(this.RoomMin.Width, this.RoomMax.Width),
								Height = this.rnd.Next(this.RoomMin.Height, this.RoomMax.Height)
							};

			// Startbuilding room from this point
			Point lc = this.PotentialCorridors.Last();

			// North/south direction
			if (direction.X == 0)
			{
				this.CurrentRoom.X = this.rnd.Next(lc.X - this.CurrentRoom.Width + 1, lc.X);

				if (direction.Y == 1)
				{
					// South
					this.CurrentRoom.Y = lc.Y + 1;
				}
				else
				{
					// North
					this.CurrentRoom.Y = lc.Y - this.CurrentRoom.Height - 1;
				}
			}
				// East / west direction
			else if (direction.Y == 0)
			{
				this.CurrentRoom.Y = this.rnd.Next(lc.Y - this.CurrentRoom.Height + 1, lc.Y);

				if (direction.X == -1)
				{
					// West
					this.CurrentRoom.X = lc.X - this.CurrentRoom.Width;
				}
				else
				{
					// East
					this.CurrentRoom.X = lc.X + 1;
				}
			}

			return this.RoomVerify();
		}

		/// <summary>
		/// Add the global Room to the rooms collection and draw it on the map
		/// </summary>
		private void RoomBuild()
		{
			this.BuiltRooms.Add(this.CurrentRoom);

			for (int x = this.CurrentRoom.Left; x <= this.CurrentRoom.Right; x++)
			{
				for (int y = this.CurrentRoom.Top; y <= this.CurrentRoom.Bottom; y++)
				{
					if (x < Map.GetUpperBound(0) && y < Map.GetUpperBound(1))
					{
						this.Map[x, y] = emptyCell;
					}
				}
			}
		}

		/// <summary>
		/// Randomly get a point on the edge of a randomly selected room
		/// </summary>
		/// <param name="location">Out: Location of point on room edge</param>
		/// <param name="direction">Out: Direction of point</param>
		/// <returns>If Location is legal</returns>
		private void RoomGetEdge(out Point location, out Point direction)
		{
			this.CurrentRoom = this.BuiltRooms[this.rnd.Next(0, this.BuiltRooms.Count())];

			//pick a random point within a room
			//the +1 / -1 on the values are to stop a corner from being chosen
			location = new Point(
				this.rnd.Next(this.CurrentRoom.Left + 1, this.CurrentRoom.Right - 1),
				this.rnd.Next(this.CurrentRoom.Top + 1, this.CurrentRoom.Bottom - 1));

			//get a random direction
			direction = this.DirectionsStraight[this.rnd.Next(0, this.DirectionsStraight.GetLength(0))];

			do
			{
				//move in that direction
				location.Offset(direction);

				if (!this.IsPointValid(location.X, location.Y))
				{
					return;
				}

				//until we meet an empty cell
			}
			while (this.GetPoint(location.X, location.Y) != filledCell);
		}

		/// <summary>
		/// Check if CurrentRoom can be built
		/// </summary>
		/// <returns>Bool indicating success</returns>
		private bool RoomVerify()
		{
			// Make it one bigger to ensure that testing gives it a border
			this.CurrentRoom.Inflate(this.RoomDistance, this.RoomDistance);

			// Check it occupies legal, empty coordinates
			for (int x = this.CurrentRoom.Left; x <= this.CurrentRoom.Right; x++)
			{
				for (int y = this.CurrentRoom.Top; y <= this.CurrentRoom.Bottom; y++)
				{
					if (!this.IsPointValid(x, y) || this.GetPoint(x, y) != filledCell)
					{
						return false;
					}
				}
			}

			// Check it doesn't encroach onto existing rooms
			if (this.BuiltRooms.Any(r => r.IntersectsWith(this.CurrentRoom)))
			{
				return false;
			}

			this.CurrentRoom.Inflate(-this.RoomDistance, -this.RoomDistance);

			// Check the room is the specified distance away from corridors
			this.CurrentRoom.Inflate(this.CorridorDistance, this.CorridorDistance);

			if (this.BuiltCorridors.Any(p => this.CurrentRoom.Contains(p)))
			{
				return false;
			}

			this.CurrentRoom.Inflate(-this.CorridorDistance, -this.CorridorDistance);

			return true;
		}

		private void SetPoint(int x, int y, int val)
		{
			this.Map[x, y] = val;
		}

		#endregion
	}

}