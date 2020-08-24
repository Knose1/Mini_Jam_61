using Com.Github.Knose1.MiniJam61.Game.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Github.Knose1.MiniJam61.Game.Pieces
{
	public class Cube : Piece
	{
		public override int CalculatePieceMultiple()
		{
			return Mathf.FloorToInt(Mathf.Min(m_piecesSettings.CubeGivesMultiple * turnsSinceCreated, m_piecesSettings.CubeGivesMax));
		}

		public override List<Vector2Int> GetMouvement(Grid grid)
		{
			List<Vector2Int> moves = new List<Vector2Int>();

			Vector2Int pos = grid.WorldToGrid(transform);

			for (int x = -1; x <= 1; x++)
			{
				for (int y = -1; y <= 1; y++)
				{
					if (x == y && x == 0) continue;
					if (Mathf.Abs(x) == Mathf.Abs(y)) continue;
					Vector2Int vector = new Vector2Int(x, y);
					int i = 1;

					Vector2Int finalPos = vector * i + pos;
					while (grid.IsPosInsideGrid(finalPos))
					{
						Piece piece = grid.GetPieceAt(finalPos);
						if (piece)
						{
							if (piece.Team != Team) moves.Add(finalPos);
							break;
						}

						moves.Add(finalPos);
						i += 1;
						finalPos = vector * i + pos;
					}
				}
			}

			return moves;
		}
	}
}
