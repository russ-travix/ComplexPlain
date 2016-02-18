using System;
using System.Collections.Generic;

namespace Mandel.Primes
{
	public static class MathFunctions
	{
		public static bool IsHappy(int n, out int iterations)
		{
			iterations = 0;

			List<int> cache = new List<int>();

			int sum = 0;
			while (n != 1)
			{
				if (cache.Contains(n))
				{
					return false;
				}
				cache.Add(n);

				while (n != 0)
				{
					int digit = n % 10;
					sum += digit * digit;
					n /= 10;

					iterations++;
				}

				n = sum;
				sum = 0;
			}

			return true;
		}

		public static bool[] PrimeSieve(long nth)
		{
			long sieveSize = nth + 1;

			bool[] sieve = new bool[sieveSize];

			Array.Clear(sieve, 0, (int)sieveSize);

			sieve[0] = sieve[1] = true;

			long p, max = (long)Math.Sqrt(sieveSize) + 1;

			for (long i = 2; i <= max; i++)
			{
				if (sieve[i])
				{
					continue;
				}

				p = i + i;

				while (p < sieveSize)
				{
					sieve[p] = true;
					p += i;
				}
			}

			return sieve;
		}

		public static long[] PrimesToNth(long nth)
		{
			if (nth < 2)
			{
				return null;
			}

			bool[] sieve = PrimeSieve(nth);
			long[] primes = new long[nth + 1];

			long index = 0;
			for (long i = 2; i <= nth; i++)
			{
				if (!sieve[i])
				{
					primes[index++] = i;
				}
			}

			Array.Resize(ref primes, (int)index);
			return primes;
		}

		public static long[] CompositesToNth(long nth)
		{
			if (nth < 2)
			{
				return null;
			}

			bool[] sieve = PrimeSieve(nth);
			long[] composites = new long[nth + 1];

			long index = 0;
			for (long i = 2; i <= nth; i++)
			{
				if (sieve[i])
				{
					composites[index++] = i;
				}
			}

			Array.Resize(ref composites, (int)index);
			return composites;
		}

		public static bool IsPrime(long n)
		{
			long max = (long)Math.Sqrt(n);
			for (int i = 2; i <= max; i++)
			{
				if (n % i == 0)
				{
					return false;
				}
			}

			return true;
		}

		public static long GreatestCommonDivisor(long a, long b)
		{
			while (b != 0)
			{
				long temp = b;
				b = a % b;
				a = temp;
			}

			return a;
		}

		public static int NumOfDivisors(int n)
		{
			int limit = n;
			int numberOfDivisors = 0;

			for (int i = 1; i < limit; ++i)
			{
				if (n % i == 0)
				{
					limit = n / i;

					if (limit != i)
					{
						numberOfDivisors++;
					}

					numberOfDivisors++;
				}
			}

			return numberOfDivisors;
		}

		public static ulong Gcd2(ulong a, ulong b)
		{
			int shift;
			if (a == 0 || b == 0) return a | b;

			for (shift = 0; ((a | b) & 1) == 0; ++shift) { a >>= 1; b >>= 1; }
			while ((a & 1) == 0) a >>= 1;

			do
			{
				while ((b & 1) == 0) b >>= 1;

				if (a < b)
				{
					b -= a;
				}
				else
				{
					ulong temp = a - b;
					a = b;
					b = temp;
				}
				b >>= 1;
			} while (b != 0);
			return a << shift;
		}

		public static long LeastCommonMultiplier(long a, long b)
		{
			long lcm = 0;
			long tmp = GreatestCommonDivisor(a, b);

			if (tmp != 0)
			{
				if (b > a)
				{
					lcm = (b / tmp) * a;
				}
				else
				{
					lcm = (a / tmp) * b;
				}
			}

			return lcm;
		}

		public static long EulerTotient(long n)
		{
			long sum = 0;
			for (long i = 1; i <= n; i++)
			{
				if (GreatestCommonDivisor(i, n) == 1) sum++;
			}

			return sum;
		}

		private static readonly long[] primes;

		public static int EulerTotientFast(int n)
		{
			int totient = n;
			int currentNum = n;
			int p;
			int prevP = 0;

			long max = (long)Math.Sqrt(n + 1) + 1;

			for (int i = 0; i < max; i++)
			{
				p = (int)primes[i];
			
				if (p > currentNum)
				{
					break;
				}

				int temp = currentNum / p;
				if (temp * p == currentNum)
				{
					currentNum = temp;
					i--;
					if (prevP != p)
					{
						prevP = p; 
						totient -= (totient / p);
					}
				}
			}

			return totient;
		}

		static MathFunctions()
		{
			primes = PrimesToNth(640000);
		}
	}
}
