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

		UIContainer.Layer uiLayer;

		private void Awake()
		{
			uiLayer = UIContainer.Instance.Add(m_Hud, true, UIContainer.ActionOnClose.unactivate);
			GameManager.OnPlayerChange += GameManager_OnPlayerChange;
			GameManager.OnEnd += GameManager_OnEnd;
		}

		private bool GameManager_OnEnd(GameTeam winner)
		{
			throw new System.NotImplementedException();
		}

		private void GameManager_OnPlayerChange(GameTeam currentPlayerTurn, TeamData teamData)
		{
			
		}
	}
}
