using Com.Github.Knose1.MiniJam61.Game;
using Com.Github.Knose1.MiniJam61.Game.Base;
using Com.Github.Knose1.MiniJam61.Settings;
using Com.Github.Knose1.MiniJam61.UI;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Github.Knose1.MiniJam61
{
	public class GameManager : MonoBehaviour
	{
		private static GameManager _instance = null;
		public static GameManager Instance => _instance = _instance ?? FindObjectOfType<GameManager>();

		public delegate void OnPlayerChangeDelegate(GameTeam currentPlayerTurn, TeamData teamData);
		public static event OnPlayerChangeDelegate OnPlayerChange;

		public delegate void OnSelectedPieceChangeDelegate(Piece currentSelectedPiece);
		public static event OnSelectedPieceChangeDelegate OnSelectedPieceChange;

		/// <summary>
		/// 
		/// </summary>
		/// <returns>If true, rematch</returns>
		public delegate bool OnEndDelegate(GameTeam winner);

		public static event OnEndDelegate OnEnd;

		[SerializeField] private float startLifePoint = 10f;

		[SerializeField] private Vector2Int m_startGridSize = new Vector2Int(5,5);
		[SerializeField] private Vector2Int m_expandSize = new Vector2Int(2,2);
		[SerializeField] private List<int> m_piecesRequiredRequiredForExpand = new List<int>() { 7, 9, 12, 15, 20, 32, 45 };
		[SerializeField] private float m_resizeTwinDuration = 0.25f;
		[SerializeField] private float m_moveTwinDuration = 0.5f;
		public float MoveTwinDuration => m_moveTwinDuration;

		[SerializeField] private Controller m_controller = null;
		[SerializeField] private PlayerCamera m_playerCamera = null;
		[SerializeField] private Grid m_grid = null;

		[SerializeField] private Piece m_cubePrefab = null;
		[SerializeField] private Piece m_pyramidePrefab = null;
		[SerializeField] private Piece m_octahedronPrefab = null;

		[SerializeField] private PiecePlacingUI m_piecePlacingUi = default;
		[SerializeField] private PieceSettings m_pieceSetting = default;

		[SerializeField] private ParticleSystem m_particleAvailableMovePrefab = default;
		[SerializeField] private ParticleSystem m_particleCurrentTile = default;

		[SerializeField] private List<Piece> m_pieces = new List<Piece>();

		List<ParticleSystem> particleAvailableMoveList = new List<ParticleSystem>();

		GameTeam _currentTurn = GameTeam.Player;
		GameTeam CurrentTurn
		{
			get => _currentTurn; 
			set
			{
				_currentTurn = value;
				OnPlayerChange?.Invoke(_currentTurn, CurrentTeam);

				for (int i = m_grid.PiecesCount - 1; i >= 0; i--)
				{
					m_grid.Pieces[i].OnPlayerChange(_currentTurn);
				}
			}
		}

		TeamData playerTeam;
		TeamData oponentTeam;
		TeamData CurrentTeam => _currentTurn == GameTeam.Player ? playerTeam : oponentTeam;

		List<Vector2Int> moves;
		Piece _currentSelectedPiece;
		Piece CurrentSelectedPiece
		{
			get => _currentSelectedPiece;
			set
			{
				_currentSelectedPiece = value;
				OnSelectedPieceChange?.Invoke(_currentSelectedPiece);
			}
		}

		int nextExpantion = 0;
		

		private void Awake()
		{
			m_playerCamera.OnRay += PlayerCamera_OnRay;
			m_particleCurrentTile.gameObject.SetActive(false);

			for (int i = m_pieces.Count - 1; i >= 0; i--)
			{
				m_pieces[i].gameObject.SetActive(false);
			}
		}

		private void Start() => StartGame();
		private void StartGame()
		{
			m_grid.Size = m_startGridSize;
			m_playerCamera.SetStateNormal();
			playerTeam = new TeamData(startLifePoint);
			oponentTeam = new TeamData(startLifePoint);
			nextExpantion = 0;

			CurrentTurn = GameTeam.Player;

			for (int i = m_pieces.Count - 1; i >= 0; i--)
			{
				Piece original = m_pieces[i];
				Piece piece = Instantiate(original);
				piece.gameObject.SetActive(true);
				piece.TwinPlace(i);
				m_grid.PlacePiece(piece, m_grid.WorldToGrid(original.transform));
			}

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


			if (moves != null && moves.Contains(posGrid))
			{
				m_piecePlacingUi.Hide();
				//Move a piece
				Piece pieceAtPose = m_grid.GetPieceAt(posGrid);
				if (pieceAtPose)
				{
					pieceAtPose.Kill(m_grid, ref playerTeam, ref oponentTeam, false);
				}

				CurrentSelectedPiece.transform.DOMove(m_grid.GridToWorld(posGrid), m_moveTwinDuration);
				CurrentSelectedPiece = null;

				CurrentTeam.lifePoint -= 1;

				UnSelectPiece();
				UnsetSelectedTile();
				SetNextTurn();
			}
			else if (piece && piece.Team == CurrentTurn)
			{
				m_piecePlacingUi.Hide();
				UnSelectPiece();

				//Select a piece
				pos = SelectPiece(piece);
				SetSelectedTile(pos);
			}
			else if (!piece)
			{
				m_piecePlacingUi.Hide();
				UnSelectPiece();

				//Select a tile for placing
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
					piece.TwinPlace(0);
					CurrentTeam.lifePoint -= m_pieceSetting.CubeCost;
					break;
				case PiecePlacingUI.PlacingInput.Pyramide:
					piece = Instantiate(m_pyramidePrefab);
					piece.TwinPlace(0);
					CurrentTeam.lifePoint -= m_pieceSetting.TriangleCost;
					break;
				case PiecePlacingUI.PlacingInput.Octahedron:
					piece = Instantiate(m_octahedronPrefab);
					piece.TwinPlace(0);
					CurrentTeam.lifePoint -= m_pieceSetting.OctahedronCost;
					break;
			}

			if (piece) 
			{
				m_grid.PlacePiece(piece, grid);
				piece.Team = CurrentTurn;
			}

			if (inp != PiecePlacingUI.PlacingInput.Nothing) SetNextTurn();

			UnsetSelectedTile();

			if (nextExpantion < m_piecesRequiredRequiredForExpand.Count && m_grid.PiecesCount >= m_piecesRequiredRequiredForExpand[nextExpantion])
			{
				m_playerCamera.SetStateVoid();
				
				DOTween.To(() => m_grid.Size, (Vector2 v) => m_grid.Size = v, m_grid.Size + m_expandSize, m_resizeTwinDuration).onComplete += () => {

					m_playerCamera.SetStateNormal();
				};
				nextExpantion += 1;
				return;
			}
		}

		/*///////////////////////////////*/
		/*                               */
		/*           Do Action           */
		/*                               */
		/*///////////////////////////////*/

		private void Update()
		{
			m_playerCamera.Controller.UpdateControles();
			m_playerCamera.ManualUpdate();
			if (m_controller.EscapeDown)
			{
				UnSelectPiece();
				UnsetSelectedTile();
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

		public void KillCurrentSelectedPieceWithEffects()
		{
			CurrentSelectedPiece.Kill(m_grid, ref playerTeam, ref oponentTeam, true);
			UnSelectPiece();
			UnsetSelectedTile();
			SetNextTurn();
		}

		private void UnSelectPiece()
		{
			for (int i = particleAvailableMoveList.Count - 1; i >= 0; i--)
			{
				Destroy(particleAvailableMoveList[i].gameObject);
				particleAvailableMoveList.RemoveAt(i);
			}
			CurrentSelectedPiece = null;
			moves = null;
		}

		private Vector3 SelectPiece(Piece piece)
		{
			UnSelectPiece();
			Vector3 pos = m_grid.GridToWorld(m_grid.WorldToGrid(piece.transform.position));
			CurrentSelectedPiece = piece;

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
