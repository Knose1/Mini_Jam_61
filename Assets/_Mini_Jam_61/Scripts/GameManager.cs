using Com.Github.Knose1.MiniJam61.Game;
using Com.Github.Knose1.MiniJam61.Game.Base;
using Com.Github.Knose1.MiniJam61.Settings;
using Com.Github.Knose1.MiniJam61.UI;
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
		[SerializeField] private Piece m_pyramidePrefab;
		[SerializeField] private Piece m_octahedronPrefab;

		[SerializeField] private PiecePlacingUI m_piecePlacingUi = default;

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

				PiecePlacingUI.PlacingInput allowedInputs =
					PiecePlacingUI.PlacingInput.Cube |
					PiecePlacingUI.PlacingInput.Octahedron |
					PiecePlacingUI.PlacingInput.Pyramide
				;
				m_piecePlacingUi.Show(PiecePlacingUi_ResolveInput, allowedInputs);
			}
		}

		private void PiecePlacingUi_ResolveInput(PiecePlacingUI.PlacingInput obj)
		{

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
