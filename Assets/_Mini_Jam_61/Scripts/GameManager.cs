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

		[SerializeField] private float startLifePoint = 10f;

		[SerializeField] private PlayerCamera m_playerCamera = null;
		[SerializeField] private Grid m_grid = null;

		[SerializeField] private Piece m_cubePrefab;
		[SerializeField] private Piece m_pyramidePrefab;
		[SerializeField] private Piece m_octahedronPrefab;

		[SerializeField] private PiecePlacingUI m_piecePlacingUi = default;

		[SerializeField] private ParticleSystem m_particleAvailableMovePrefab = default;
		[SerializeField] private ParticleSystem m_particleCurrentTile = default;

		List<ParticleSystem> particleAvailableMoveList = new List<ParticleSystem>();

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

		TeamData playerTeam;
		TeamData oponentTeam;

		private void Awake()
		{
			doAction = DoTurn;
			m_playerCamera.OnRay += PlayerCamera_OnRay;
			m_particleCurrentTile.gameObject.SetActive(false);
		}

		private void Start()
		{
			m_playerCamera.SetStateNormal();
			playerTeam = new TeamData(startLifePoint);
			oponentTeam = new TeamData(startLifePoint);
		}

		private void PlayerCamera_OnRay(RaycastHit obj)
		{
			Piece piece = obj.transform.GetComponent<Piece>();

			Vector3 pos;

			if (piece)
			{
				//Move a piece
				pos = m_grid.GridToWord(m_grid.WorldToGrid(obj.transform.position));

				List<Vector2Int> moves = piece.GetMouvement(m_grid);
				m_playerCamera.SetStatePlacePiece();

				for (int i = moves.Count - 1; i >= 0; i--)
				{
					ParticleSystem item = Instantiate(m_particleAvailableMovePrefab);
					item.transform.position = m_grid.GridToWord(moves[i]);
					item.Play();
					particleAvailableMoveList.Add(item); ;
				}
			}
			else
			{
				//Place a piece
				pos = m_grid.GridToWord(m_grid.WorldToGrid(obj.point));

				doAction = null;

				PiecePlacingUI.PlacingInput allowedInputs =
					PiecePlacingUI.PlacingInput.Cube |
					PiecePlacingUI.PlacingInput.Octahedron |
					PiecePlacingUI.PlacingInput.Pyramide
				;
				m_piecePlacingUi.Show(PiecePlacingUi_ResolveInput, allowedInputs, CurrentTurn);
			}

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
