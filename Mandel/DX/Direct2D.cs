using System.Drawing;
using System.Windows.Forms;

using SlimDX;
using SlimDX.DXGI;
using SlimDX.Direct3D11;
using SlimDX.Direct2D;
using SlimDX.Windows;

using Device = SlimDX.Direct3D11.Device;
using FactoryD2D = SlimDX.Direct2D.Factory;
using FactoryDXGI = SlimDX.DXGI.Factory;

namespace Mandel.DX
{
	public class Direct2D
	{
		private readonly RenderTarget renderTarget;

		private readonly SwapChain swapChain;

		private readonly Device device;

		private readonly RenderForm form;

		public Direct2D()
		{
			form = new RenderForm("Direct2D Demo in C#");

			var swapChainDesc = new SwapChainDescription
				{
					BufferCount = 2,
					Usage = Usage.RenderTargetOutput,
					OutputHandle = form.Handle,
					IsWindowed = true,
					ModeDescription = new ModeDescription(0, 0, new Rational(60, 1), Format.R8G8B8A8_UNorm),
					SampleDescription = new SampleDescription(1, 0),
					Flags = SwapChainFlags.AllowModeSwitch,
					SwapEffect = SwapEffect.Discard
				};

			Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.BgraSupport, swapChainDesc, out device, out swapChain);

			Surface backBuffer = Surface.FromSwapChain(swapChain, 0);

			using (var factory = new FactoryD2D())
			{
				var dpi = factory.DesktopDpi;

				renderTarget = RenderTarget.FromDXGI(
						factory,
						backBuffer,
						new RenderTargetProperties
						{
							HorizontalDpi = dpi.Width,
							VerticalDpi = dpi.Height,
							MinimumFeatureLevel = SlimDX.Direct2D.FeatureLevel.Default,
							PixelFormat = new PixelFormat(Format.Unknown, AlphaMode.Ignore),
							Type = RenderTargetType.Default,
							Usage = RenderTargetUsage.None
						}
					);
			}

			//Disable automatic ALT+ENTER processing because it doesn't work properly with WinForms.
			using (var factory = swapChain.GetParent<FactoryDXGI>())
				{
					factory.SetWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAltEnter);
				}

			form.KeyDown += (o, e) =>
				{
					if (e.Alt && e.KeyCode == Keys.Enter)
					{
						swapChain.IsFullScreen = !swapChain.IsFullScreen;
					}
				};

			
		}

		~Direct2D()
		{
			renderTarget.Dispose();
			swapChain.Dispose();
			device.Dispose();
		}

		public void StartRender()
		{
			MessagePump.Run(form, RenderLoop);
		}

		private void RenderLoop()
		{
			renderTarget.BeginDraw();
			renderTarget.Transform = Matrix3x2.Identity;
			renderTarget.Clear(Color.White);

			// Here be drawing code ...
			using (var brush = new SolidColorBrush(renderTarget, new Color4(Color.LightSlateGray)))
			{
				for (int x = 0; x < renderTarget.Size.Width; x += 10)
				{
					renderTarget.DrawLine(brush, x, 0, x, renderTarget.Size.Height, 0.5f);
				}

				for (int y = 0; y < renderTarget.Size.Height; y += 10)
				{
					renderTarget.DrawLine(brush, 0, y, renderTarget.Size.Width, y, 0.5f);
				}

				renderTarget.FillRectangle(brush, new RectangleF(renderTarget.Size.Width / 2 - 50, renderTarget.Size.Height / 2 - 50, 100, 100));
			}

			renderTarget.DrawRectangle(new SolidColorBrush(renderTarget, new Color4(Color.CornflowerBlue)),
				new RectangleF(renderTarget.Size.Width / 2 - 100, renderTarget.Size.Height / 2 - 100, 200, 200));

			renderTarget.EndDraw();
			swapChain.Present(0, PresentFlags.None);
		}
	}
}
