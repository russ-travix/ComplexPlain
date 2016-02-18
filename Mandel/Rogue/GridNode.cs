using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mandel.Rogue
{
	public enum GridNeighbors
	{
		North,
		NorthEast,
		East,
		SouthEast,
		South,
		SouthWest,
		West,
		NorthWest,
		None
	}

	public enum GridTerrain
	{
		Grass,
		Gravel,
		Water,
		Marsh,
		Road
	}

	class GridNode
	{
		public int X { get; set; }

		public int Y { get; set; }

		public bool Walkable { get; set; }

		public float Difficulty { get; set; }

		public GridNeighbors Neighbors = GridNeighbors.None;

		public GridTerrain Terrain = GridTerrain.Road;
	}
}
