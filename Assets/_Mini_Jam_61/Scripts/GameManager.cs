using Com.Github.Knose1.MiniJam61.Game;
using Com.Github.Knose1.MiniJam61.Game.Base;
using Com.Github.Knose1.MiniJam61.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Github.Knose1.MiniJam61
{
	public class GameManager : MonoBehaviour
	{
		public delegate bool OnPlayerChangeDelegate(PieceTeam currentPlayerTurn);
		public static event OnPlayerChangeDelegate OnPlayerChange;

		/// <summary>
		/// 
		/// </summary>
		/// <returns>If true, rematch</returns>
		public delegate bool OnEndDelegate(PieceTeam winner);
		public static event OnEndDelegate OnEnd;

		[SerializeField] private PlayerCamera m_playerCamera = null;
		[SerializeField] private Grid m_grid = null;

		[SerializeField] private Piece m_cubePrefab;
		[SerializeField] private Piece m_trianglePrefab;
		[SerializeField] private Piece m_octahedronPrefab;

		Action doAction;
		PieceTeam currentTurn;

		private void Awake()
		{
			doAction = DoTurn;
			m_playerCamera.OnRay += PlayerCamera_OnRay;
		}

		private void PlayerCamera_OnRay(RaycastHit obj)
		{
			Piece piece = obj.transform.GetComponent<Piece>();
			if (piece)
			{
				//Move a piece

				//piece.GetMouvement();
			}
			else
			{
				//Place a piece
				doAction = null;

			}
		}

		private void Start()
		{
			m_playerCamera.SetStateNormal();
		}

		private void Update()
		{
			m_playerCamera.Controller.UpdateControles();
			doAction?.Invoke();
		}

		private void DoTurn()
		{
			m_playerCamera.ManualUpdate();
		}
	}
}
