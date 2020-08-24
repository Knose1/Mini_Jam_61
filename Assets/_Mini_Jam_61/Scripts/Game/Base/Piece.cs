using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Github.Knose1.MiniJam61.Settings;
using System;
using DG.Tweening;

namespace Com.Github.Knose1.MiniJam61.Game.Base
{

	[ExecuteInEditMode]
	public abstract class Piece : MonoBehaviour
	{
		
		[SerializeField] protected List<string> m_colors = new List<string>();
		[SerializeField] protected Material m_material = default;
		[SerializeField] protected GameTeam m_team = default;
		public GameTeam Team 
		{
			get => m_team;
			set => m_team = value;
		}

		[SerializeField] protected PieceSettings m_piecesSettings = default;
		[SerializeField] protected ColorSettings m_settings = default;
		[SerializeField] protected MeshRenderer m_renderer = null;

		[SerializeField] protected Transform m_innerObject = null;

		[SerializeField] protected float m_normalKillDuration = 0.3f;
		[SerializeField] protected float m_suicideDuration = 1;

		[SerializeField] protected float m_normalKillEndPos = -40;
		[SerializeField] protected float m_suicideEndPos = 2;

		protected bool isKilled = false;
		protected Material materialClone = null;

		protected float turnsSinceCreated = 0;

		private void OnValidate()
		{
			Start();
		}

		private void Start()
		{
			if (m_material != null)
				materialClone = new Material(m_material);

			if (m_team == GameTeam.Player)		transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
			if (m_team == GameTeam.Opponent)	transform.rotation = Quaternion.AngleAxis(0, Vector3.up);

			turnsSinceCreated = 0;
		}

		public void OnPlayerChange(GameTeam currentPlayerTurn)
		{
			//If it's different, then we've passed a turn
			if (currentPlayerTurn != Team) turnsSinceCreated += 1;
		}

		public void TwinPlace(float interval = 0)
		{
			Vector3 pos = m_innerObject.transform.localPosition;
			Vector3 posRotation = m_innerObject.transform.rotation.eulerAngles;
			Vector3 scale = m_innerObject.transform.localScale;

			m_innerObject.transform.rotation = Quaternion.Euler(new Vector3(0, 264, 0) + posRotation);
			m_innerObject.transform.localPosition = pos + new Vector3(0, m_suicideEndPos, 0);
			m_innerObject.transform.localScale = Vector3.zero;

			Sequence secBack = DOTween.Sequence();
			secBack.AppendInterval(interval);
			secBack.Append(m_innerObject.DOScale(scale, m_suicideDuration));
			secBack.Insert(interval + m_suicideDuration / 2, m_innerObject.DOLocalRotate(new Vector3(0, -264, 0), m_suicideDuration, RotateMode.LocalAxisAdd));
			secBack.Append(m_innerObject.DOLocalMoveY(pos.y, m_suicideDuration));
		}

		private void Update()
		{
			if (isKilled) return;

			if (materialClone == null) return;

			materialClone.CopyPropertiesFromMaterial(m_material);

			Color c = default;
			switch (m_team)
			{
				case GameTeam.Opponent:
					c = m_settings.OpponentColor;
					break;
				case GameTeam.Player:
					c = m_settings.PlayerColor;
					break;
			}

			for (int i = 0; i < m_colors.Count; i++)
			{
				materialClone.SetColor(m_colors[i], c);
			}

			m_renderer.material = materialClone;
		}

		public abstract List<Vector2Int> GetMouvement(Grid grid);
		
		public abstract int CalculatePieceMultiple();

		protected virtual void KillEffect(Grid grid, ref TeamData playerTeam, ref TeamData oponentTeam)
		{
			TeamData myTeam = null;
			switch (Team)
			{
				case GameTeam.Opponent:
					myTeam = oponentTeam;
					break;
				case GameTeam.Player:
					myTeam = playerTeam;
					break;
			}

			myTeam.lifePoint += CalculatePieceMultiple();
		}

		public void Kill(Grid grid, ref TeamData playerTeam, ref TeamData oponentTeam, bool doEffect = false)
		{
			isKilled = true;

			grid.RemovePiece(this);
			if (doEffect) KillEffect(grid, ref playerTeam, ref oponentTeam);

			Sequence sec = DOTween.Sequence();
			if (!doEffect)
			{
				sec.AppendInterval(GameManager.Instance.MoveTwinDuration - 0.1f);
			}

			sec.Append(m_innerObject.DOLocalMoveY(m_suicideEndPos, m_suicideDuration));
			sec.Join(m_innerObject.DOLocalRotate(new Vector3(0, 264, 0), m_suicideDuration, RotateMode.LocalAxisAdd));
			sec.Join(m_innerObject.DOShakeScale(m_suicideDuration / 2));
			sec.Insert(m_suicideDuration / 2, m_innerObject.DOScale(Vector3.zero, m_suicideDuration));
			sec.onComplete += KillInstant;
		}

		public void KillInstant()
		{
			Destroy(gameObject);
		}
	}
}