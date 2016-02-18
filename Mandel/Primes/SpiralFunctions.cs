using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Mandel.Primes
{
	public static class SpiralFunctions
	{
		public static List<SpiralFunc> Functions;

		private static readonly Dictionary<Tuple<int, int>, int> memo;

		static SpiralFunctions()
		{
			memo = new Dictionary<Tuple<int, int>, int>();

			Functions = new List<SpiralFunc>();

			Functions.AddRange(new[]
				{
					new SpiralFunc
						{
							Label = "Ullam",
							Function = (i,x,y) => MathFunctions.IsPrime(i) ? 255 : 0
						},
					new SpiralFunc
						{
							Label = "Harshad",
							Function = (i, x, y) =>
								{
									if (i == 0)
									{
										return 0;
									}

									int n = i;

									int sum = 0;
									do
									{
										sum += i % 10;
										i /= 10;
									}
									while (i > 0);

									if (n % sum == 0)
									{
										return 255;
									}

									return 0;
								}
						},
					new SpiralFunc
						{
							Label = "Harshad (Dwell)",
							BackgroundColor = Colors.White,
							Function = (i, x, y) =>
								{
									if (i == 0)
									{
										return 0;
									}

									int n = i;

									int sum = 0;
									int count = 0;
									do
									{
										sum += i % 10;
										i /= 10;
										count++;
									}
									while (i > 0);

									if (n % sum == 0)
									{
										return 255 / count;
									}

									return 0;
								}
						},
					new SpiralFunc
						{
							Label = "Color Modulus",
							Function = (i,x,y) =>
								{ 
									if (i % 2 == 0)
									{
										return 40;
									}
									
									if (i % 3 == 0)
									{
										return 60;
									}

									if (i % 5 == 0)
									{
										return 100;
									}

									if (i % 7 == 0)
									{
										return 140;
									}

									return 0;
								}
						},
					new SpiralFunc
						{
							Label = "Collatz",
							Function = (i,x,y) =>
								{
									int iteration = 1;
							
									int root = i;

									while (root != 1 && iteration++ < 256)
									{
										if (root % 2 == 0)
										{
											root /= 2;
										}
										else
										{
											root = root * 3 + 1;
										}
									}

									return iteration;
								}
						},
					new SpiralFunc
						{
							Label = "Collatz (*2)",
							Function = (i,x,y) =>
								{
									int iteration = 1;
							
									int root = i;

									while (root != 1 && iteration++ < 128)
									{
										if (root % 2 == 0)
										{
											root /= 2;
										}
										else
										{
											root = root * 2;
										}
									}

									return iteration;
								}
						},
					new SpiralFunc
						{
							Label = "Linear",
							Function = (i,x,y) => i
						},
					new SpiralFunc
						{
							Label = "Divisors",
							Function = (i,x,y) => MathFunctions.NumOfDivisors(i)
						},
					new SpiralFunc
						{
							Label = "Josephus",
							Function = (i,x,y) =>
								{
									int r = 0;

									for (int j = 1; j <= i; j++ )
									{
										int rindex = r + 2;

										r = rindex % j;
									}

									return r;
								}
						},
					new SpiralFunc
						{
							Label = "Eulers Totiant",
							Function = (i, x, y) => MathFunctions.EulerTotientFast(i)
						},
					new SpiralFunc
						{
							Label = "Happy!",
							Function = (i, x, y) =>
								{
									if (i > 999)
									{
										i -= 999;
									}

									int iterations;
									return MathFunctions.IsHappy(i, out iterations) ? 255 : 0;
								}
						}

				});
		}
	}
}