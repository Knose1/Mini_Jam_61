using Com.Github.Knose1.MiniJam61.Game;
using Com.Github.Knose1.MiniJam61.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Github.Knose1.MiniJam61.UI
{
	public class InfoScreen : MonoBehaviour
	{
		[SerializeField] private ColorSettings m_colorSettings = null;
		[SerializeField] private Text m_textInfo = null;
		[SerializeField] private List<Image> m_color = null;
		[SerializeField] private List<Image> m_oppositeColor = null;
		[SerializeField] private string m_textFormat = "Player {0} has Win";

		public void SetUser(GameTeam user)
		{
			m_textInfo.text = string.Format(m_textFormat, ((int)user) + 1);

			Color c = default;
			Color cO = default;
			switch (user)
			{
				case Game.GameTeam.Opponent:
					c = m_colorSettings.OpponentColor;
					cO = m_colorSettings.PlayerColor;
					break;
				case Game.GameTeam.Player:
					c = m_colorSettings.PlayerColor;
					cO = m_colorSettings.OpponentColor;
					break;
			}

			foreach (Image item in m_color)
			{
				item.color = c;
			}

			foreach (Image item in m_oppositeColor)
			{
				item.color = cO;
			}
		}

		public void Close()
		{
			UIContainer.Instance.Close(gameObject);
		}
	}
}
