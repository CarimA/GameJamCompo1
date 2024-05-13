using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameJamCompo1;

public class Map
{
	public int[,] Cells { get; }

	public Map()
	{
		Cells = new[,]
		{
			{ 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
			{ 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{ 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{ 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{ 1,0,0,0,0,0,2,2,2,2,2,0,0,0,0,3,0,3,0,3,0,0,0,1},
			{ 1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{ 1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,3,0,0,0,3,0,0,0,1},
			{ 1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{ 1,0,0,0,0,0,2,2,0,2,2,0,0,0,0,3,0,3,0,3,0,0,0,1},
			{ 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{ 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{ 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{ 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{ 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{ 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{ 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{ 1,4,4,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{ 1,4,0,4,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{ 1,4,0,0,0,0,5,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{ 1,4,0,4,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{ 1,4,0,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{ 1,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{ 1,4,4,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			{ 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
		};
	}

	public int GetCell(Point point)
	{
		var x = point.X;
		var y = point.Y;

		if (x < 0 || x >= Cells.GetLength(0) || y < 0 || y >= Cells.GetLength(1))
			return 0;

		return Cells[x, y];
	}

	public void Draw(SpriteBatch spriteBatch, Player player)
	{
		for (var x = 0; x < Constants.BufferWidth; x++)
		{
			var cameraX = 2f * (x / (float)Constants.BufferWidth) - 1f;
			var rayDirection = player.Direction + player.CameraPlane * cameraX;

			var cell = new Point((int)player.Position.X, (int)player.Position.Y);
			var sideDistance = Vector2.Zero;
			var deltaDistance = new Vector2(
				Math.Abs(1f / rayDirection.X),
				Math.Abs(1f / rayDirection.Y));
			var step = Point.Zero;

			(step.X, sideDistance.X) = rayDirection.X switch
			{
				< 0 => (-1, (player.Position.X - cell.X) * deltaDistance.X),
				> 0 => (1, (cell.X + 1f - player.Position.X) * deltaDistance.X),
				_ => (0, 0)
			};

			(step.Y, sideDistance.Y) = rayDirection.Y switch
			{
				< 0 => (-1, (player.Position.Y - cell.Y) * deltaDistance.Y),
				> 0 => (1, (cell.Y + 1f - player.Position.Y) * deltaDistance.Y),
				_ => (0, 0)
			};

			var hit = false;
			var side = 0;
			var sanityCheck = 0;
			while (!hit)
			{
				if (sideDistance.X < sideDistance.Y)
				{
					sideDistance.X += deltaDistance.X;
					cell.X += step.X;
					side = 0;
				}
				else // if (sideDistance.Y < sideDistance.X)
				{
					sideDistance.Y += deltaDistance.Y;
					cell.Y += step.Y;
					side = 1;
				}

				sanityCheck++;
				if (GetCell(cell) > 0 || sanityCheck >= 100)
					hit = true;
			}

			if (sanityCheck >= 100)
				continue;

			var wallDistance = side == 0
				? sideDistance.X - deltaDistance.X
				: sideDistance.Y - deltaDistance.Y;

			var lineHeight = (int)(Constants.BufferHeight / wallDistance);
			var drawStart = Math.Max(0, -lineHeight / 2 + Constants.BufferHeight / 2);
			var drawEnd = Math.Min(Constants.BufferHeight, lineHeight / 2 + Constants.BufferHeight / 2);

			var color = GetCell(cell) switch
			{
				1 => Color.Red * ((side + 1) / 2f),
				2 => Color.Green * ((side + 1) / 2f),
				3 => Color.Blue * ((side + 1) / 2f),
				4 => Color.White * ((side + 1) / 2f),
				_ => Color.Yellow * ((side + 1) / 2f)
			};
			color.A = Byte.MaxValue;

			DrawLine(spriteBatch, x, drawStart, drawEnd, color);
		}
	}

	private void DrawLine(SpriteBatch spriteBatch, int x, int yStart, int yEnd, Color color)
	{
		spriteBatch.Draw(GameInstance.Pixel, new Rectangle(x, yStart, 1, yEnd - yStart), color);
	}
}