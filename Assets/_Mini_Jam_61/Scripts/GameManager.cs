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
		[SerializeField] private PieceSettings m_pieceSetting = default;

		[SerializeField] private ParticleSystem m_particleAvailableMovePrefab = default;
		[SerializeField] private ParticleSystem m_particleCurrentTile = default;

		List<ParticleSystem> particleAvailableMoveList = new List<ParticleSystem>();

		Action doAction;
		GameTeam _currentTurn = GameTeam.Player;
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

		public TeamData CurrentTeam => _currentTurn == GameTeam.Player ? playerTeam : oponentTeam;

		List<Vector2Int> moves;
		Piece currentSelectedPiece;

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

		/*///////////////////////////////*/
		/*                               */
		/*       Raycast Do Action       */
		/*                               */
		/*///////////////////////////////*/

		private void PlayerCamera_OnRay(RaycastHit obj)
		{
			Piece piece = obj.transform.parent.GetComponent<Piece>();

			Vector3 pos = default;
			Vector2Int posGrid = m_grid.WorldToGrid(obj.point);


			if (!piece) piece = m_grid.GetPieceAt(m_grid.WorldToGrid(obj.point));

			if (piece) posGrid = m_grid.WorldToGrid(piece.transform);

			m_piecePlacingUi.Hide();

			if (moves != null && moves.Contains(posGrid))
			{
				Piece pieceAtPose = m_grid.GetPieceAt(posGrid);
				if (pieceAtPose)
				{
					pieceAtPose.Kill(m_grid, ref playerTeam, ref oponentTeam, false);
				}

				currentSelectedPiece.transform.position = m_grid.GridToWorld(posGrid);
				currentSelectedPiece = null;

				UnSelectPiece();
				UnsetSelectedTile();
				SetNextTurn();
			}
			else if (piece && piece.Team == CurrentTurn)
			{
				UnSelectPiece();

				//Select a piece
				pos = SelectPiece(piece);
				SetSelectedTile(pos);
			}
			else if (!piece)
			{
				UnSelectPiece();

				//Place a piece
				pos = ModeAddAPiece(obj);
				SetSelectedTile(pos);
			}
		}

		
		private void PiecePlacingUi_ResolveInput(PiecePlacingUI.PlacingInput inp, Vector2Int grid)
		{
			Piece piece = null;
			switch (inp)
			{
				case PiecePlacingUI.PlacingInput.Cube:
					piece = Instantiate(m_cubePrefab);
					CurrentTeam.lifePoint -= m_pieceSetting.CubeCost;
					break;
				case PiecePlacingUI.PlacingInput.Pyramide:
					piece = Instantiate(m_pyramidePrefab);
					CurrentTeam.lifePoint -= m_pieceSetting.TriangleCost;
					break;
				case PiecePlacingUI.PlacingInput.Octahedron:
					piece = Instantiate(m_octahedronPrefab);
					CurrentTeam.lifePoint -= m_pieceSetting.OctahedronCost;
					break;
			}

			if (piece) 
			{
				m_grid.PlacePiece(piece, grid);
				piece.Team = CurrentTurn;
			}

			if (inp != PiecePlacingUI.PlacingInput.Nothing) SetNextTurn();

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
		/*             Modes             */
		/*                               */
		/*///////////////////////////////*/

		private Vector3 ModeAddAPiece(RaycastHit obj)
		{
			Vector3 pos = m_grid.GridToWorld(m_grid.WorldToGrid(obj.point));
			//doAction = null;

			PiecePlacingUI.PlacingInput allowedInputs = PiecePlacingUI.PlacingInput.Nothing;

			if (CurrentTeam.lifePoint - m_pieceSetting.CubeCost > 0)
			{
				allowedInputs = (allowedInputs == PiecePlacingUI.PlacingInput.Nothing) ? 
					PiecePlacingUI.PlacingInput.Cube : 
					allowedInputs | PiecePlacingUI.PlacingInput.Cube;
			}

			if (CurrentTeam.lifePoint - m_pieceSetting.OctahedronCost > 0)
			{
				allowedInputs = (allowedInputs == PiecePlacingUI.PlacingInput.Nothing) ?
					PiecePlacingUI.PlacingInput.Octahedron :
					allowedInputs | PiecePlacingUI.PlacingInput.Octahedron;
			}

			if (CurrentTeam.lifePoint - m_pieceSetting.TriangleCost > 0)
			{
				allowedInputs = (allowedInputs == PiecePlacingUI.PlacingInput.Nothing) ?
					PiecePlacingUI.PlacingInput.Pyramide :
					allowedInputs | PiecePlacingUI.PlacingInput.Pyramide;
			}

			m_piecePlacingUi.Show((PiecePlacingUI.PlacingInput inp) =>
			{
				PiecePlacingUi_ResolveInput(inp, m_grid.WorldToGrid(obj.point));
			}, allowedInputs, CurrentTurn);
			return pos;
		}

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
