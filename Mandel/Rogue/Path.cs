using System.Collections;
using System.Collections.Generic;

namespace Mandel.Rogue
{

	public class Path<Node> : IEnumerable<Node>
	{
		public Node LastStep { get; private set; }

		public Path<Node> PreviousSteps { get; private set; }

		public double TotalCost { get; private set; }

		private Path(Node lastStep, Path<Node> previousSteps, double totalCost)
		{
			this.LastStep = lastStep;
			this.PreviousSteps = previousSteps;
			this.TotalCost = totalCost;
		}

		public Path(Node start)
			: this(start, null, 0)
		{
		}

		public Path<Node> AddStep(Node step, double cost)
		{
			return new Path<Node>(step, this, this.TotalCost + cost);
		}

		public IEnumerator<Node> GetEnumerator()
		{
			for (var p = this; p != null; p = p.PreviousSteps)
			{
				yield return p.LastStep;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}

}