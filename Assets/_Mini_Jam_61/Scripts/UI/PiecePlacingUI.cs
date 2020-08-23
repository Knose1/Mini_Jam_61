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
		[SerializeField] private ColorSettings m_colorSettings = null;
		[SerializeField] private Image m_background = null;

		private Action<PlacingInput> onInput;

		[Flags]
		public enum PlacingInput
		{
			Nothing		= 1 << 0,
			Cube		= 1 << 1,
			Pyramide    = 1 << 2,
			Octahedron  = 1 << 3
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

			m_cubeBtn.interactable			= (allowedInputs & PlacingInput.Cube) == PlacingInput.Cube;
			m_pyramideBtn.interactable 		= (allowedInputs & PlacingInput.Pyramide) == PlacingInput.Pyramide;
			m_octahedronBtn.interactable 	= (allowedInputs & PlacingInput.Octahedron) == PlacingInput.Octahedron;

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

		public void Hide()
		{
			UIContainer.Instance.Close(gameObject);

			doAction = null;
			onInput = null;

			m_cubeBtn.onClick.RemoveListener(CubeBtn_OnClick);
			m_pyramideBtn.onClick.RemoveListener(PyramideBtn_OnClick);
			m_octahedronBtn.onClick.RemoveListener(OctahedronBtn_OnClick);
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
			var _temp = onInput;

			Hide();

			_temp(inp);
		}


		private void Update()
		{
			doAction?.Invoke();
		}
	}
}
