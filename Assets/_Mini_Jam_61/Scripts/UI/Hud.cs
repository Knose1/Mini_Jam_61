using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Com.Github.Knose1.MiniJam61.Settings;
using Com.Github.Knose1.MiniJam61.Game;
using System;

namespace Com.Github.Knose1.MiniJam61.UI
{
	public class Hud : MonoBehaviour
	{
		[SerializeField] private List<Image> m_border = new List<Image>();
		[SerializeField] private ColorSettings m_colors = null;

		[SerializeField] private Text m_life = null;
		[SerializeField] private Text m_costGiveInfo = null;
		[SerializeField] private Button m_destroyBtn = null;
		[SerializeField] private GameObject m_destroyBtnContainer = null;

		private void Awake()
		{
			GameManager.OnPlayerChange += GameManager_OnPlayerChange;
			GameManager.OnSelectedPieceChange += GameManager_OnSelectedPieceChange;
			m_destroyBtn.onClick.AddListener(DestroyBtn_OnClick);
		}

		private void GameManager_OnSelectedPieceChange(Game.Base.Piece currentSelectedPiece)
		{
			if (currentSelectedPiece)
				m_costGiveInfo.text = currentSelectedPiece.CalculatePieceMultiple().ToString();

			m_destroyBtnContainer.SetActive(currentSelectedPiece);
		}

		private void DestroyBtn_OnClick()
		{
			GameManager.Instance.KillCurrentSelectedPieceWithEffects();
		}

		private void GameManager_OnPlayerChange(Game.GameTeam currentPlayerTurn, TeamData teamData)
		{
			Color c = default;
			switch (currentPlayerTurn)
			{
				case Game.GameTeam.Opponent:
					c = m_colors.OpponentColor;
					break;
				case Game.GameTeam.Player:
					c = m_colors.PlayerColor;
					break;
			}

			foreach (Image item in m_border)
			{
				item.color = c;
			}

			m_life.text = teamData.lifePoint.ToString();
		}
	}
}
