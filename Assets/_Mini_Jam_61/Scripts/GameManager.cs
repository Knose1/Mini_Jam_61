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
		public delegate bool OnPlayerChangeDelegate(GameTeam currentPlayerTurn);
		public static event OnPlayerChangeDelegate OnPlayerChange;

		/// <summary>
		/// 
		/// </summary>
		/// <returns>If true, rematch</returns>
		public delegate bool OnEndDelegate(GameTeam winner);
		public static event OnEndDelegate OnEnd;

		[SerializeField] private PlayerCamera m_playerCamera = null;
		[SerializeField] private Grid m_grid = null;

		[SerializeField] private Piece m_cubePrefab;
		[SerializeField] private Piece m_pyramidePrefab;
		[SerializeField] private Piece m_octahedronPrefab;

		[SerializeField] private PiecePlacingUI m_piecePlacingUi = default;

		[SerializeField] private ParticleSystem m_particleAvailableMovePrefab = default;
		[SerializeField] private ParticleSystem m_particleCurrentTile = default;

		Action doAction;
		GameTeam _currentTurn;
		GameTeam CurrentTurn
		{
			get => _currentTurn; 
			set
			{
				_currentTurn = value;
				OnPlayerChange?.Invoke(_currentTurn);
			}
		}

		private void Awake()
		{
			doAction = DoTurn;
			m_playerCamera.OnRay += PlayerCamera_OnRay;
			m_particleCurrentTile.gameObject.SetActive(false);
		}

		private void Start()
		{
			m_playerCamera.SetStateNormal();
		}

		private void PlayerCamera_OnRay(RaycastHit obj)
		{
			Piece piece = obj.transform.GetComponent<Piece>();

			Vector3Int pos;

			if (piece)
			{
				//Move a piece
				pos = Vector3Int.RoundToInt(obj.transform.position);

				//piece.GetMouvement();
			}
			else
			{
				//Place a piece
				pos = Vector3Int.RoundToInt(obj.point);

				doAction = null;

				PiecePlacingUI.PlacingInput allowedInputs =
					PiecePlacingUI.PlacingInput.Cube |
					PiecePlacingUI.PlacingInput.Octahedron |
					PiecePlacingUI.PlacingInput.Pyramide
				;
				m_piecePlacingUi.Show(PiecePlacingUi_ResolveInput, allowedInputs, CurrentTurn);
			}

			pos.y = 0;

			m_particleCurrentTile.gameObject.SetActive(true);
			m_particleCurrentTile.Play();

			m_particleCurrentTile.transform.position = pos;
		}

		private void PiecePlacingUi_ResolveInput(PiecePlacingUI.PlacingInput obj)
		{
			if (obj != PiecePlacingUI.PlacingInput.Nothing) SetNextTurn();
			doAction = DoTurn;
			m_particleCurrentTile.Stop();
			m_particleCurrentTile.gameObject.SetActive(false);
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

		private void SetNextTurn() => CurrentTurn = (GameTeam)(((int)CurrentTurn + 1) % 2);
	}
}
