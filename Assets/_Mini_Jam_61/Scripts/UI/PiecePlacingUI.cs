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

		public void Show(Action<PlacingInput> onInput, PlacingInput allowedInputs)
		{
			this.onInput = onInput;
			doAction = DoActionCheckForInput;

			m_cubeBtn.onClick.AddListener(CubeBtn_OnClick);
			m_pyramideBtn.onClick.AddListener(PyramideBtn_OnClick);
			m_octahedronBtn.onClick.AddListener(OctahedronBtn_OnClick);
		}


		private void CubeBtn_OnClick() => SendOnInputAndSleep(PlacingInput.Cube);
		private void PyramideBtn_OnClick() => SendOnInputAndSleep(PlacingInput.Pyramide);
		private void OctahedronBtn_OnClick() => SendOnInputAndSleep(PlacingInput.Octahedron);
		private void DoActionCheckForInput()
		{
			if (m_controller.EscapeDown)
			{
				SendOnInputAndSleep(PlacingInput.Nothing);
			}
		}

		private void SendOnInputAndSleep(PlacingInput inp)
		{
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
