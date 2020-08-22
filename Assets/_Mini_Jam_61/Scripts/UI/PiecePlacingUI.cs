using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Com.Github.Knose1.MiniJam61.UI
{
	public class PiecePlacingUI : MonoBehaviour
	{
		private Action doAction;

		public void WaitForInput()
		{

		}

		private void Update()
		{
			doAction?.Invoke();
		}
	}
}
