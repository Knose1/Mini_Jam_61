using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Github.Knose1.MiniJam61
{
	public class Grid : MonoBehaviour
	{
		[SerializeField] protected Vector2Int m_size;
		[SerializeField] protected Transform m_board;

		private void OnValidate()
		{
			if (!m_board) return;

			m_board.localScale = new Vector3(m_size.x, (m_size.x + m_size.y)/2f, m_size.y);
			m_board.position = new Vector3(m_size.x - 1f, 1f, m_size.y - 1f) / -2;
		}
	}
}
