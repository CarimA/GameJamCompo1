using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GameJamCompo1;
public class GameInstance : Game
{
	private const int BufferHeight = 144;
	private const int BufferWidth = 224;

	private GraphicsDeviceManager _graphics;
	private SpriteBatch _spriteBatch;
	private readonly RenderTarget2D _backbuffer;

	public GameInstance()
	{
		_graphics = new GraphicsDeviceManager(this)
		{
			GraphicsProfile = GraphicsProfile.HiDef,
			IsFullScreen = false,
			HardwareModeSwitch = false,
			PreferHalfPixelOffset = false,
			PreferMultiSampling = false,
			PreferredBackBufferFormat = SurfaceFormat.Color,
			PreferredBackBufferHeight = 720,
			PreferredBackBufferWidth = 1280,
			PreferredDepthStencilFormat = DepthFormat.None,
			SynchronizeWithVerticalRetrace = true,
		};
		_graphics.ApplyChanges();

		Window.AllowUserResizing = true;
		IsMouseVisible = true;

		_backbuffer = new RenderTarget2D(GraphicsDevice, BufferWidth, BufferHeight);
	}

	protected override void Initialize()
	{
		_spriteBatch = new SpriteBatch(GraphicsDevice);
		base.Initialize();
	}
	
	protected override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.SetRenderTarget(_backbuffer);
		GraphicsDevice.Clear(Color.CornflowerBlue);



		GraphicsDevice.SetRenderTarget(null);
		GraphicsDevice.Clear(Color.Black);

		_spriteBatch.Begin(samplerState: SamplerState.PointClamp);
		_spriteBatch.Draw(_backbuffer, ScaledBackbuffer(), Color.White);
		_spriteBatch.End();

		base.Draw(gameTime);
	}

	private Rectangle ScaledBackbuffer()
	{
		var displayWidth = Window.ClientBounds.Width;
		var displayHeight = Window.ClientBounds.Height;
		var width = BufferWidth;
		var height = BufferHeight;
		var widthScale = displayWidth / (double)BufferWidth;
		var heightScale = displayHeight / (double)BufferHeight;
		var smallest = (int)Math.Min(widthScale, heightScale);

		width *= smallest;
		height *= smallest;

		var x = displayWidth / 2 - width / 2;
		var y = displayHeight / 2 - height / 2;

		return new Rectangle(x, y, width, height);
	}
}
