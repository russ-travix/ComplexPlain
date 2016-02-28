using System;
using System.Collections.Generic;
using System.Numerics;
using Mandel.Fractals;

namespace Mandel.ComplexNumbers
{
	public static class ComplexFunctions
	{
		public static double Mag(this Complex c)
		{
			return (c.Real * c.Real) + (c.Imaginary * c.Imaginary);
		}

		public static Complex Iterate(this Complex z, Complex c)
		{
			return z*z + c;
		}

		public static Complex IterateThree(this Complex z, Complex c)
		{
			return z*z*z + c;
		}

		public static Complex ComplexFromCartesian(this Complex c, int x, int y,double scaleX, double scaleY)
		{
			var real = c.Real + x*scaleX;
			var imag = c.Imaginary + y*scaleY;

			return new Complex(real, imag);
		}

		public static List<ComplexFunction> Functions { get; set; }

		private const int MaxIterations = 128;

		private const int MaxCollatzIterations = 128;

		static ComplexFunctions()
		{
			Functions = new List<ComplexFunction>();

			Functions.AddRange(new[] {
				new ComplexFunction
				{
					Label = "Collatz (HSV)",
					Function = (c, px, py, p) =>
							{
								var inset = true;

								var iteration = 1;

								var z = c;

								if (z.Mag() > 16384)
								{
									inset = false;
								}

								while (inset && iteration < MaxCollatzIterations)
								{
									var cosR = Math.Cos(Math.PI * z.Real) * Math.Cosh(Math.PI * z.Imaginary);
									var cosI = -( Math.Sin(Math.PI * z.Real) ) * Math.Sinh(Math.PI * z.Imaginary);

									var tmpR = z.Real * cosR - z.Imaginary * cosI;
									var tmpI = z.Real * cosI + z.Imaginary * cosR;

									z = new Complex(
										(2 + 7 * z.Real - 2 * cosR - 5 * tmpR) / 4,
										(7 * z.Imaginary - 2 * cosI - 5 * tmpI) / 4
										);

									++iteration;

									if (z.Mag() > 16384)
									{
										inset = false;
									}
								}

								return new ComplexResult(z) { Dwell = inset ? (255 / MaxCollatzIterations) * iteration  : 0 };
							},
						//UseDwell = true,
						AllowJulia = false,
						Min = new Complex(-2, -1.21),
						Max = new Complex(0.6, 1.21)
						
				},
				new ComplexFunction
				{
					Label = "Collatz (Dwell)",
					Function = (c, px, py, p) =>
							{
								var inset = true;

								var iteration = 1;

								var z = c;

								if (z.Mag() > 16384)
								{
									inset = false;
								}

								while (inset && iteration < MaxCollatzIterations)
								{
									var cosR = Math.Cos(Math.PI * z.Real) * Math.Cosh(Math.PI * z.Imaginary);
									var cosI = -( Math.Sin(Math.PI * z.Real) ) * Math.Sinh(Math.PI * z.Imaginary);

									var tmpR = z.Real * cosR - z.Imaginary * cosI;
									var tmpI = z.Real * cosI + z.Imaginary * cosR;

									z = new Complex(
										(2 + 7 * z.Real - 2 * cosR - 5 * tmpR) / 4,
										(7 * z.Imaginary - 2 * cosI - 5 * tmpI) / 4
										);

									++iteration;

									if (z.Mag() > 16384)
									{
										inset = false;
									}
								}

								return new ComplexResult(z) { Dwell = inset ? (255 / MaxCollatzIterations) * iteration : 0 };
							},
						UseDwell = true,
						AllowJulia = false,
						Min = new Complex(-2, -1.21),
						Max = new Complex(0.6, 1.21)
				},
				new ComplexFunction
				{
					Label = "Collatz chain experiment",
					Function = (c, px, py, p) =>
						{
							var iteration = 1;
							
							var root = p;

							while (root != 1 && iteration++ < MaxIterations)
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
							var result = new ComplexResult(Complex.Zero) { Dwell = iteration < MaxIterations ? iteration : 0 };
							return result;
						},
						UseDwell = true,
						AllowJulia = false,
						Min = new Complex(-2, -1.21),
						Max = new Complex(0.6, 1.2),
						Smooth = false
				},
				new ComplexFunction
				{
					Label = "Pure Count",
					Function = (c, px, py, p) => new ComplexResult(Complex.Zero) { Dwell = p },
						UseDwell = true,
						AllowJulia = false,
						Min = new Complex(-2, -1.21),
						Max = new Complex(0.6, 1.2),
						Smooth = false
				},
				new ComplexFunction
					{
						Label = "Mandelbrot (Escape time)",
						Function = (c, px, py, p) =>
							{
								int iteration;
								var z = new Complex();
								for (iteration = 0; iteration < MaxIterations; iteration++)
								{
									z = z.Iterate(c);
									if (z.Magnitude >= 4.0)
										break;
								}

								var result = new ComplexResult(z) { Dwell = iteration < MaxIterations ? iteration : 0 };
								return result;
							},
						UseDwell = true,
						AllowJulia = true,
						Min = new Complex(-2, -1.21),
						Max = new Complex(0.6, 1.2)
					},
				new ComplexFunction
					{
						Label = "Mandelbrot (Escape time with Periodicity)",
						Function = (c, px, py, p) =>
							{
								int iteration;
								var z = new Complex();
								var oz = new Complex();

								for (iteration = 0; iteration < MaxIterations; iteration++)
								{
									z = z.Iterate(c);
									if (z.Magnitude >= 4.0)
										break;
									
									if (oz == z)
									{
										iteration = MaxIterations;
										break;
									}

									oz = z;
								}

								var result = new ComplexResult(z) { Dwell = iteration < MaxIterations ? iteration : 0 };
								return result;
							},
						UseDwell = true,
						AllowJulia = true,
						Min = new Complex(-2, -1.21),
						Max = new Complex(0.6, 1.2)
					},
				new ComplexFunction
					{
						Label = "Mandelbrot (Escape time with Periodicity+Smooth)",
						Function = (c, px, py, p) =>
							{
								int iteration;
								var z = new Complex();
								var oz = new Complex();

								for (iteration = 0; iteration < MaxIterations; iteration++)
								{
									z = z.Iterate(c);
									if (z.Magnitude >= 4.0)
										break;
									
									if (oz == z)
									{
										iteration = MaxIterations;
										break;
									}

									oz = z;
								}

								var result = new ComplexResult(z) { Dwell = iteration < MaxIterations ? iteration : 0 };
								return result;
							},
						UseDwell = true,
						AllowJulia = true,
						Smooth = true,
						Min = new Complex(-2, -1.21),
						Max = new Complex(0.6, 1.2)
					},
				new ComplexFunction
					{
						Label = "Mandelbrot (Phase)",
						Function = (c, px, py, p) =>
							{
								int iteration;
								var z = new Complex();
								for (iteration = 0; iteration < MaxIterations; iteration++)
								{
									z = z.Iterate(c);
									if (z.Magnitude >= 4.0)
										break;
								}

								//ComplexResult result = new ComplexResult(z) { Dwell = iteration < MaxIterations ? (int)Math.Tan(c.Magnitude + z.Magnitude): 0 };
								//return result;
								return new Complex(c.Phase, z.Phase);
							},
						Min = new Complex(-2, -1.21),
						Max = new Complex(0.6, 1.2)
					},
				new ComplexFunction
					{
						Label = "Mandelbrot (Phase or Unit)",
						Function = (c, px, py, p) =>
							{
								int iteration;
								var z = new Complex();
								for (iteration = 0; iteration < MaxIterations; iteration++)
								{
									z = z.Iterate(c);
									if (z.Magnitude >= 4.0)
										break;
								}

								return iteration < MaxIterations ? new Complex(c.Phase, z.Phase) : new Complex();
							},
						Min = new Complex(-2, -1.21),
						Max = new Complex(0.6, 1.2)
					},
				new ComplexFunction
					{
						Label = "Mandelbrot (Psycadelic)",
						Function = (c, px, py, p) =>
							{
								int iteration;
								var z = new Complex();
								for (iteration = 0; iteration < MaxIterations; iteration++)
								{
									z = z.Iterate(c);

									if (z.Magnitude >= 4.0)
										break;
								}

								return z;
							},
						AllowJulia = true
					},
				new ComplexFunction
					{
						Label = "Burning ship",
						Min = new Complex(-2.5, -2),
						Max = new Complex(2, 1),
						Function = (c, px, py, p) =>
							{
								int iteration;
								var z = new Complex();
								for (iteration = 0; iteration < MaxIterations; iteration++)
								{
									z = z.AbsSquare() + c;

									if (z.Magnitude > 10.0)
										break;
								}

								var result = new ComplexResult(z) { Dwell = iteration < MaxIterations ? iteration : 0 };
								return result;
							},
						UseDwell = true,
						AllowJulia = true
					},
				new ComplexFunction
					{
						Label = "Burning ship (HSV)",
						Min = new Complex(-2.5, -2),
						Max = new Complex(2, 1),
						Function = (c, px, py, p) =>
							{
								int iteration;
								var z = new Complex();
								for (iteration = 0; iteration < MaxIterations; iteration++)
								{
									z = z.AbsSquare() + c;

									if (z.Magnitude > 10.0)
										break;
								}

								return z;
							},
						AllowJulia = true
					},
				new ComplexFunction
					{
						Label = "Newton ^3",
						Function = (c, px, py, p) =>
								{
									const int numRoots = 5;
								
									var z = c;
									Complex epsilon;

									var iter = 0;
									do
									{
										if (++iter > MaxIterations)
										{
											break;
										}

										epsilon = Complex.Pow(z, numRoots - 1) / numRoots * Complex.Pow(z, numRoots - 1);
										z += epsilon;

									} while (epsilon.Magnitude > 0.0001);

									return new ComplexResult(z) { Dwell = iter > MaxIterations ? 0 : iter};
								},

							UseDwell = true,
							Min = new Complex(-1.5, -1),
							Max = new Complex(1.5, 1),
					},
				new ComplexFunction
					{
						Label = "Newton-test",
						Function = (c, px, py, p) =>
								{
									double xold = 0.0, yold = 0.0;

									var x = c.Real;
									var y = c.Imaginary;

									var flag = false;

									int i;
									for (i = 0; i < 512 && !flag; i++)
									{
										var xsqr = x * x;
										var ysqr = y * y;

										var denom = 3.0 * ( ( xsqr - ysqr ) * ( xsqr - ysqr ) + 4.0 * xsqr * ysqr );

										if (denom == 0)
										{
											denom = 0.00000001;
										}

										x = 0.6 * x + ( xsqr - ysqr ) / denom;
										y = 0.6 * y - 2.0 * x * y / denom;

										if (xold == x && yold == y)
										{
											flag = true;
										}
										xold = x;
										yold = y;
									}

									return new ComplexResult(c) { Dwell = flag ? (i * 16) - 1 : 0 };
								},
						UseDwell = true
					}, 
				new ComplexFunction
					{
						Label = "Newton-test HSV",
						Function = (c, px, py, p) =>
								{
									double xold = 0.0, yold = 0.0;

									var x = c.Real;
									var y = c.Imaginary;

									var flag = false;

									int i;
									for (i = 0; i < 512 && !flag; i++)
									{
										var xsqr = x * x;
										var ysqr = y * y;

										var denom = 3.0 * ( ( xsqr - ysqr ) * ( xsqr - ysqr ) + 4.0 * xsqr * ysqr );

										if (denom == 0)
										{
											denom = 0.00000001;
										}

										x = 0.6 * x + ( xsqr - ysqr ) / denom;
										y = 0.6 * y - 2.0 * x * y / denom;

										if (xold == x && yold == y)
										{
											flag = true;
										}
										xold = x;
										yold = y;
									}

									return new Complex(x, y);
								}
					}, 
				new ComplexFunction
					{
						Label = "Tri-Mandelbrot (Escape time)",
						Function = (c, px, py, p) =>
							{
								int iteration;
								var z = new Complex();
								for (iteration = 0; iteration < MaxIterations; iteration++)
								{
									z = z.IterateThree(c);
									if (z.Magnitude >= 4.0)
										break;
								}

								var result = new ComplexResult(z) { Dwell = iteration < MaxIterations ? iteration : 0 };
								return result;
							},
						UseDwell = true,
						AllowJulia = true
					},
				new ComplexFunction
					{
						Label = "Mandelbrot (Level Set Method)",
						Function = (c, px, py, p) =>
							{
								int iteration;
								var z = new Complex();
								for (iteration = 0; iteration < MaxIterations; iteration++)
								{
									z = z * z + c;
									if (z.Magnitude >= 4.0)
										break;
								}

								var result = new ComplexResult(z) { Dwell = iteration < MaxIterations ? iteration % 2 == 0 ? 0 : 255 : 0 };
								return result;
							},
						UseDwell = true,
						AllowJulia = true
					},
				new ComplexFunction
					{
						Label = "Mandelbrot - Parameter plane",
						Function = (c, px, py, p) =>
							{
								var z = new Complex();
								for (var i = 0; i < 7; i++)
								{
									z = z * z + c;
								}

								var ret = new ComplexResult(z);

								if (z.Real > 0 && z.Imaginary > 0) ret.Dwell = 50;
								if (z.Real < 0 && z.Imaginary < 0) ret.Dwell = 150;
								if (z.Real < 0 && z.Imaginary > 0) ret.Dwell = 100;
								if (z.Real > 0 && z.Imaginary < 0) ret.Dwell = 200;

								return ret;
							},
						UseDwell = true,
						AllowJulia = true
					},
				new ComplexFunction
					{
						Label = "Simple Z",
						Function = (c, px, py, p) => c,
						Min = new Complex(-4, -4),
						Max = new Complex(4, 4)
					},
				new ComplexFunction
					{
						Label = "Z ^ 2 - 1",
						Function = (c, px, py, p) => ((c-1.0) * (c+1.0)),
						Min = new Complex(-4, -4),
						Max = new Complex(4, 4)
					},
				new ComplexFunction
					{
						Label = "Z ^ 3 + 1",
						Function = (c, px, py, p) => c * c * c + 1.0,
						Min = new Complex(-4, -4),
						Max = new Complex(4, 4)
					},
				new ComplexFunction
					{
						Label = "1 / Z",
						Function = (c, px, py, p) => 1.0 / c,
						Min = new Complex(-4, -4),
						Max = new Complex(4, 4)
					},
				new ComplexFunction
					{
						Label	= "(z^2-1)(z-2-i)^2/(z^2+2+2i)",
						Function = (c, px, py, p) =>
							{
								var zs = (c - 2.0) - Complex.ImaginaryOne;
								return ( ( c * c - 1.0 ) * zs * zs / ( c * c + 2.0 + 2.0 * Complex.ImaginaryOne ) );
							},
						Min = new Complex(-4, -4),
						Max = new Complex(4, 4)
					},
				new ComplexFunction
					{
						Label	= "sqrt(z)",
						Function = (c, px, py, p) => Complex.Sqrt(c),
						Min = new Complex(-4, -4),
						Max = new Complex(4, 4)
					},
				new ComplexFunction
					{
						Label	= "exp(z)",
						Function = (c, px, py, p) => Complex.Exp(c),
						Min = new Complex(-4, -4),
						Max = new Complex(4, 4)
					},
				//The fifth iterate f(5)( z ) of f ( z ) = ( 1 + i ) sin z. Corners at ± 3 ± 3 i.
				new ComplexFunction
					{
						Label	= "f(5)(z) - 1+i * sin z",
						Function = (c, px, py, p) =>
								{
									for (var i = 0; i < 5; i++)
									{
										c = ( 1 + c.Imaginary ) * Complex.Sin(c);
									}

									return c;
								},
						Min = new Complex(-3, -4),
						Max = new Complex(3, 4)
					},
				new ComplexFunction
					{
						Label	= "z * z - constant c (Julia)",
						Function = (c, px, py, p) =>
								{
									var complex = new Complex(0.75, 0.2);

									for (var i = 0; i < 50; i++)
									{
										complex = complex * complex - complex;

										if (complex.Magnitude >= 4)
										{
											complex = Complex.ImaginaryOne;
											break;
										}
									}

									return complex;
								},
						Min = new Complex(-1.6, -1.1),
						Max = new Complex(1.6, 1.1)
					},
				new ComplexFunction
					{
						Label	= "Log(z)",
						Function = (c, px, py, p) => Complex.Log(c),
						Min = new Complex(-4, -4),
						Max = new Complex(4, 4)
					},
				new ComplexFunction
					{
						Label	= "reciprocal(z)",
						Function = (c, px, py, p) => Complex.Reciprocal(c),
						Min = new Complex(-4, -4),
						Max = new Complex(4, 4)
					},
				new ComplexFunction
					{
						Label	= "Tan(z)",
						Function = (c, px, py, p) => Complex.Tan(c),
						Min = new Complex(-4, -4),
						Max = new Complex(4, 4)
					},
				new ComplexFunction
					{
						Label	= "Atan(z)",
						Function = (c, px, py, p) => Complex.Atan(c),
						Min = new Complex(-4, -4),
						Max = new Complex(4, 4)
					},
				new ComplexFunction
					{
						Label	= "Tanh(z)",
						Function = (c, px, py, p) => Complex.Tanh(c),
						Min = new Complex(-4, -4),
						Max = new Complex(4, 4)
					},
				new ComplexFunction
					{
						Label	= "Sin(1/z)",
						Function = (c, px, py, p) => Complex.Sin(1/c),
						Min = new Complex(-4, -4),
						Max = new Complex(4, 4)
					},
				new ComplexFunction
					{
						Label	= "Sin(z)",
						Function = (c, px, py, p) => Complex.Sin(c),
						Min = new Complex(-(Math.PI * 2), 1.5),
						Max = new Complex(Math.PI * 2, -1.5)
					},
				new ComplexFunction
					{
						Label	= "r^i - i^r",
						Function = (c, px, py, p) => new Complex(Math.Pow(c.Real, c.Imaginary), Math.Pow(c.Imaginary, c.Real)),
						Min = new Complex(-4, -4),
						Max = new Complex(4, 4)
					},
			});
		}
	}
}