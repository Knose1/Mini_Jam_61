using Com.Github.Knose1.MiniJam61.Game.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Github.Knose1.MiniJam61
{
	public class Grid : MonoBehaviour
	{
		[SerializeField] protected Vector2Int m_size;
		public Vector2Int Size {
			get => m_size;
			set => m_size = value;
		}
		
		[SerializeField] protected Transform m_board;
		[SerializeField] protected Transform m_piece_container;

		[SerializeField] protected List<Piece> m_pieces = new List<Piece>();

		private void OnValidate()
		{
			if (!m_board) return;

			m_board.localScale = new Vector3(m_size.x, (m_size.x + m_size.y) / 2f, m_size.y);
			m_board.position = new Vector3(m_size.x - 1f, -1f, m_size.y - 1f) / 2;
		}

		public void PlaceObj(Transform piece, Vector2Int pos)
		{
			piece.SetParent(m_piece_container);
			piece.transform.localPosition = new Vector3(-pos.x, 0, -pos.y);
		}
		public void PlacePiece(Piece piece, Vector2Int pos) 
		{
			m_pieces.Add(piece);
			PlaceObj(piece.transform, pos);
		}

		public void RemovePiece(Piece piece)
		{
			m_pieces.Remove(piece);
			piece.transform.SetParent(null);
		}

		public bool IsPosInsideGrid(Vector2Int pos)
		{
			return
				pos.x >= 0 && pos.x < m_size.x
				&&
				pos.y >= 0 && pos.y < m_size.y
			;
		}

		public Vector2Int WorldToGrid(Transform transform)
		{
			return WorldToGrid(transform.position);
		}

		public Vector2Int WorldToGrid(Vector3 pos)
		{
			return Vector2Int.RoundToInt(new Vector2(pos.x, pos.z));
		}

		public Vector3 GridToWorld(Vector2Int pos)
		{
			return new Vector3(pos.x, 0, pos.y);
		}

		public Piece GetPieceAt(Vector2Int pos)
		{
			for (int i = m_pieces.Count - 1; i >= 0; i--)
			{
				Piece p = m_pieces[i];
				Vector2Int vector2Int = WorldToGrid(p.transform);
				if (vector2Int == pos) return p;
			}

			return null;
		}
	}
}
