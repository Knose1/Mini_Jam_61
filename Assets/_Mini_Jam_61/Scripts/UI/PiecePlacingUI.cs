using Com.Github.Knose1.MiniJam61.Game;
using Com.Github.Knose1.MiniJam61.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Github.Knose1.MiniJam61.UI
{
	public class PiecePlacingUI : MonoBehaviour
	{
		[SerializeField] private Button m_cubeBtn = null;
		[SerializeField] private Button m_pyramideBtn = null;
		[SerializeField] private Button m_octahedronBtn = null;

		[SerializeField] private Controller m_controller = null;
		[SerializeField] private ColorSettings m_colorSettings;
		[SerializeField] private Image m_background = null;

		private Action<PlacingInput> onInput;

		[Flags]
		public enum PlacingInput
		{
			Nothing		= 0 >> 0,
			Cube		= 0 >> 1,
			Pyramide    = 0 >> 2,
			Octahedron  = 0 >> 3
		}

		private Action doAction;

		public void Show(Action<PlacingInput> onInput, PlacingInput allowedInputs, GameTeam currentPlayer)
		{
			UIContainer.Instance.Add(gameObject, true, UIContainer.ActionOnClose.unactivate);

			this.onInput = onInput;
			doAction = DoActionCheckForInput;

			m_cubeBtn.onClick.AddListener(CubeBtn_OnClick);
			m_pyramideBtn.onClick.AddListener(PyramideBtn_OnClick);
			m_octahedronBtn.onClick.AddListener(OctahedronBtn_OnClick);

			Color c = default;
			switch (currentPlayer)
			{
				case GameTeam.Opponent:
					c = m_colorSettings.OpponentColor;
					break;
				case GameTeam.Player:
					c = m_colorSettings.PlayerColor;
					break;
			}
			
			m_background.color = c;
		}


		private void CubeBtn_OnClick() => SendOnInputAndSleep(PlacingInput.Cube);
		private void PyramideBtn_OnClick() => SendOnInputAndSleep(PlacingInput.Pyramide);
		private void OctahedronBtn_OnClick() => SendOnInputAndSleep(PlacingInput.Octahedron);
		private void DoActionCheckForInput()
		{
			if (m_controller.EscapeDown || m_controller.MouseRightDown)
			{
				SendOnInputAndSleep(PlacingInput.Nothing);
			}
		}

		private void SendOnInputAndSleep(PlacingInput inp)
		{
			UIContainer.Instance.Close(gameObject);

			doAction = null;

			m_cubeBtn.onClick.RemoveListener(CubeBtn_OnClick);
			m_pyramideBtn.onClick.RemoveListener(PyramideBtn_OnClick);
			m_octahedronBtn.onClick.RemoveListener(OctahedronBtn_OnClick);

			onInput(inp);
		}


		private void Update()
		{
			doAction?.Invoke();
		}
	}
}
