﻿using Com.Github.Knose1.MiniJam61.Game.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Com.Github.Knose1.MiniJam61.Game.Pieces
{
	public class Octaedron : Piece
	{
		public override int CalculatePieceMultiple()
		{
			return Mathf.FloorToInt(Mathf.Min(m_piecesSettings.OctahedronGivesMultiple * turnsSinceCreated, m_piecesSettings.OctahedronGivesMax));
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
					//if (Mathf.Abs(x) == Mathf.Abs(y)) continue;
					
					Vector2Int finalPos = new Vector2Int(x, y) + pos;
					if (grid.IsPosInsideGrid(finalPos))
					{
						Piece piece = grid.GetPieceAt(finalPos);
						if (piece)
						{
							if (piece.Team != Team) moves.Add(finalPos);
							continue;
						}
						moves.Add(finalPos);
					}
				}
			}

			return moves;
		}
	}
}
