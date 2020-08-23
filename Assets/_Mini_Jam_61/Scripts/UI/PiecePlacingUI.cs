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
		}

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
			onInput(inp);
		}


		private void Update()
		{
			doAction?.Invoke();
		}
	}
}
