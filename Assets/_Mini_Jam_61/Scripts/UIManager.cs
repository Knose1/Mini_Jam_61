using Com.Github.Knose1.MiniJam61.Game;
using Com.Github.Knose1.MiniJam61.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Github.Knose1.MiniJam61.UI
{
	public class UIManager : MonoBehaviour
	{
		[SerializeField] private GameObject m_Hud = null;
		[SerializeField] private InfoScreen m_Win = null;
		[SerializeField] private InfoScreen m_Turn = null;

		UIContainer.Layer uiLayer;
		UIContainer.Layer infoLayer;

		private void Awake()
		{
			uiLayer = UIContainer.Instance.Add(m_Hud, true, UIContainer.ActionOnClose.unactivate);
			GameManager.OnPlayerChange += GameManager_OnPlayerChange;
			GameManager.OnEnd += GameManager_OnEnd;
		}

		private void GameManager_OnEnd(GameTeam winner)
		{
			m_Turn.Close();

			UIContainer.Instance.Add(m_Win.gameObject, true, UIContainer.ActionOnClose.unactivate);
			m_Win.SetUser(winner);
		}

		private void GameManager_OnPlayerChange(GameTeam currentPlayerTurn, TeamData teamData)
		{
			UIContainer.Instance.Add(m_Turn.gameObject, true, UIContainer.ActionOnClose.unactivate);
			m_Turn.SetUser(currentPlayerTurn);
		}
	}
}
