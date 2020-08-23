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

		[SerializeField] private Controller m_controller = null;
		[SerializeField] private PlayerCamera m_playerCamera = null;
		[SerializeField] private Grid m_grid = null;

		[SerializeField] private Piece m_cubePrefab;
		[SerializeField] private Piece m_pyramidePrefab;
		[SerializeField] private Piece m_octahedronPrefab;

		[SerializeField] private PiecePlacingUI m_piecePlacingUi = default;

		[SerializeField] private ParticleSystem m_particleAvailableMovePrefab = default;
		[SerializeField] private ParticleSystem m_particleCurrentTile = default;

		List<ParticleSystem> particleAvailableMoveList = new List<ParticleSystem>();

		Action<RaycastHit> doActionOnRay;
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

		List<Vector2Int> moves;
		Piece currentSelectedPiece;

		private void Awake()
		{
			doAction = DoTurn;
			doActionOnRay = DoMoveOrPlace;
			m_playerCamera.OnRay += PlayerCamera_OnRay;
			m_particleCurrentTile.gameObject.SetActive(false);
		}

		private void Start()
		{
			m_playerCamera.SetStateNormal();
			playerTeam = new TeamData(startLifePoint);
			oponentTeam = new TeamData(startLifePoint);
		}

		/*///////////////////////////////*/
		/*                               */
		/*       Raycast Do Action       */
		/*                               */
		/*///////////////////////////////*/

		private void PlayerCamera_OnRay(RaycastHit obj)
		{
			doActionOnRay?.Invoke(obj);
		}

		private void DoCheckForMove(RaycastHit obj)
		{
			Piece raycastPiece = obj.transform.parent.GetComponent<Piece>();

			if (!raycastPiece) raycastPiece = m_grid.GetPieceAt(m_grid.WorldToGrid(obj.point));

			Vector2Int pos = m_grid.WorldToGrid(obj.point);

			if (raycastPiece && raycastPiece.Team == CurrentTurn)
			{
				pos = m_grid.WorldToGrid(raycastPiece.transform.position);

				//When select another piece
				SelectPiece(raycastPiece);
				SetSelectedTile(m_grid.GridToWorld(pos));
				return;
			}

			if (!moves.Contains(pos)) return;

			Piece pieceAtPose = m_grid.GetPieceAt(pos);
			if (pieceAtPose) 
			{
				pieceAtPose.Kill(m_grid, ref playerTeam, ref oponentTeam, false);
			}

			currentSelectedPiece.transform.position = m_grid.GridToWorld(pos);
			currentSelectedPiece = null;
			doActionOnRay = DoMoveOrPlace;

			UnSelectPiece();
			UnsetSelectedTile();
			SetNextTurn();
		}

		/// <summary>
		/// If a square is selected, open piece add UI
		/// 
		/// If a piece is selected, select and set mode DoCheckForMove
		/// </summary>
		/// <param name="obj"></param>
		private void DoMoveOrPlace(RaycastHit obj)
		{
			Piece piece = obj.transform.parent.GetComponent<Piece>();

			Vector3 pos;

			if (!piece) piece = m_grid.GetPieceAt(m_grid.WorldToGrid(obj.point));

			if (piece && piece.Team != CurrentTurn) return;

			UnSelectPiece();

			if (piece)
			{
				//Select a piece
				pos = SelectPiece(piece);
			}
			else
			{
				//Place a piece
				pos = m_grid.GridToWorld(m_grid.WorldToGrid(obj.point));

				doAction = null;

				PiecePlacingUI.PlacingInput allowedInputs =
					PiecePlacingUI.PlacingInput.Cube |
					PiecePlacingUI.PlacingInput.Octahedron |
					PiecePlacingUI.PlacingInput.Pyramide
				;
				m_piecePlacingUi.Show(PiecePlacingUi_ResolveInput, allowedInputs, CurrentTurn);
			}

			SetSelectedTile(pos);
		}

		private void PiecePlacingUi_ResolveInput(PiecePlacingUI.PlacingInput obj)
		{
			if (obj != PiecePlacingUI.PlacingInput.Nothing) SetNextTurn();
			doAction = DoTurn;
			UnsetSelectedTile();
		}

		/*///////////////////////////////*/
		/*                               */
		/*           Do Action           */
		/*                               */
		/*///////////////////////////////*/

		private void Update()
		{
			m_playerCamera.Controller.UpdateControles();
			doAction?.Invoke();
		}

		private void DoTurn()
		{
			m_playerCamera.ManualUpdate();
		}
		private void DoTurnWithSelection()
		{
			DoTurn();
			if (m_controller.EscapeDown)
			{
				UnSelectPiece();
				UnsetSelectedTile();

				doAction = DoTurn;
				doActionOnRay = DoMoveOrPlace;
			}
		}

		/*///////////////////////////////*/
		/*                               */
		/*           Next Turn           */
		/*                               */
		/*///////////////////////////////*/

		private void SetNextTurn() => CurrentTurn = (GameTeam)(((int)CurrentTurn + 1) % 2);


		/*///////////////////////////////*/
		/*                               */
		/*           Selection           */
		/*                               */
		/*///////////////////////////////*/

		private void UnSelectPiece()
		{
			for (int i = particleAvailableMoveList.Count - 1; i >= 0; i--)
			{
				Destroy(particleAvailableMoveList[i].gameObject);
				particleAvailableMoveList.RemoveAt(i);
			}
			currentSelectedPiece = null;
			moves = null;
		}

		private Vector3 SelectPiece(Piece piece)
		{
			UnSelectPiece();
			Vector3 pos = m_grid.GridToWorld(m_grid.WorldToGrid(piece.transform.position));
			currentSelectedPiece = piece;
			doActionOnRay = DoCheckForMove;

			doAction = DoTurnWithSelection;

			moves = piece.GetMouvement(m_grid);
			for (int i = moves.Count - 1; i >= 0; i--)
			{
				m_particleAvailableMovePrefab.GetComponent<TileShow>().Team = CurrentTurn;

				ParticleSystem item = Instantiate(m_particleAvailableMovePrefab);
				item.transform.position = m_grid.GridToWorld(moves[i]);
				item.Play();
				
				particleAvailableMoveList.Add(item);
			}

			return pos;
		}

		private void UnsetSelectedTile()
		{
			m_particleCurrentTile.Stop();
			m_particleCurrentTile.gameObject.SetActive(false);
		}

		private void SetSelectedTile(Vector3 pos)
		{
			m_particleCurrentTile.GetComponent<TileShow>().Team = CurrentTurn;
			m_particleCurrentTile.gameObject.SetActive(true);
			m_particleCurrentTile.Play();

			m_particleCurrentTile.transform.position = pos;
		}
	}
}
