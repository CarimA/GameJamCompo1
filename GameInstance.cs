using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameJamCompo1;
public class GameInstance : Game
{
	private GraphicsDeviceManager _graphics;
	private SpriteBatch _spriteBatch;
	private readonly RenderTarget2D _backbuffer;

	private Player _player;
	private Map _map;

	public static Texture2D Pixel;

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

		Pixel = new Texture2D(GraphicsDevice, 1, 1);
		Pixel.SetData(new[] { Color.White });

		Window.AllowUserResizing = true;
		IsMouseVisible = true;

		_backbuffer = new RenderTarget2D(GraphicsDevice, Constants.BufferWidth, Constants.BufferHeight);

		_map = new Map();
		_player = new Player
		{
			Position = new Vector2(22, 12),
			Direction = new Vector2(-1, 0),
			CameraPlane = new Vector2(0, 0.66f)
		};
	}

	protected override void Initialize()
	{
		_spriteBatch = new SpriteBatch(GraphicsDevice);
		base.Initialize();
	}

	protected override void Update(GameTime gameTime)
	{
		base.Update(gameTime);

		var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

		var x = 0f;
		var y = 0f;
		var ks = Keyboard.GetState();

		var moveSpeed = 2f * dt;
		var rotSpeed = 1f * dt;

		if (ks.IsKeyDown(Keys.D))
		{
			var oldDirX = _player.Direction.X;
			var oldPlaneX = _player.CameraPlane.X;

			_player.Direction = new Vector2(
				(float)(_player.Direction.X * Math.Cos(-rotSpeed) - _player.Direction.Y * Math.Sin(-rotSpeed)),
				(float)(oldDirX * Math.Sin(-rotSpeed) + _player.Direction.Y * Math.Cos(-rotSpeed)));

			_player.CameraPlane = new Vector2(
				(float)(_player.CameraPlane.X * Math.Cos(-rotSpeed) - _player.CameraPlane.Y * Math.Sin(-rotSpeed)),
				(float)(oldPlaneX * Math.Sin(-rotSpeed) + _player.CameraPlane.Y * Math.Cos(-rotSpeed)));
		}

		if (ks.IsKeyDown(Keys.A))
		{
			var oldDirX = _player.Direction.X;
			var oldPlaneX = _player.CameraPlane.X;

			_player.Direction = new Vector2(
				(float)(_player.Direction.X * Math.Cos(rotSpeed) - _player.Direction.Y * Math.Sin(rotSpeed)),
				(float)(oldDirX * Math.Sin(rotSpeed) + _player.Direction.Y * Math.Cos(rotSpeed)));

			_player.CameraPlane = new Vector2(
				(float)(_player.CameraPlane.X * Math.Cos(rotSpeed) - _player.CameraPlane.Y * Math.Sin(rotSpeed)),
				(float)(oldPlaneX * Math.Sin(rotSpeed) + _player.CameraPlane.Y * Math.Cos(rotSpeed)));
		}

		if (ks.IsKeyDown(Keys.W))
		{
			x += _player.Direction.X;
			y += _player.Direction.Y;
		}

		if (ks.IsKeyDown(Keys.S))
		{
			x -= _player.Direction.X;
			y -= _player.Direction.Y;
		}

		/*if (ks.IsKeyDown(Keys.A))
		{
			x -= _player.Direction.X;
			y += _player.Direction.Y;
		}

		if (ks.IsKeyDown(Keys.D))
		{
			x += _player.Direction.X;
			y -= _player.Direction.Y;
		}*/

		var axis = new Vector2(x, y);
		var movement = axis * moveSpeed;

		_player.Position += movement;

		//_player.Position += movement;
	}

	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.SetRenderTarget(_backbuffer);
		GraphicsDevice.Clear(Color.CornflowerBlue);

		_spriteBatch.Begin();
		_map.Draw(_spriteBatch, _player);
		_spriteBatch.End();

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
		var width = Constants.BufferWidth;
		var height = Constants.BufferHeight;
		var widthScale = displayWidth / (double)Constants.BufferWidth;
		var heightScale = displayHeight / (double)Constants.BufferHeight;
		var smallest = (int)Math.Min(widthScale, heightScale);

		width *= smallest;
		height *= smallest;

		var x = displayWidth / 2 - width / 2;
		var y = displayHeight / 2 - height / 2;

		return new Rectangle(x, y, width, height);
	}
}