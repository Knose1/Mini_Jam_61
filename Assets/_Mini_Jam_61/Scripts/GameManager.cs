using Com.Github.Knose1.MiniJam61.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Github.Knose1.MiniJam61
{
	public class GameManager : MonoBehaviour
	{
		public delegate bool OnPlayerChangeDelegate(PieceTeam currentPlayerTurn);
		public static event OnPlayerChangeDelegate OnPlayerChange;

		/// <summary>
		/// 
		/// </summary>
		/// <returns>If true, rematch</returns>
		public delegate bool OnEndDelegate(PieceTeam winner);
		public static event OnEndDelegate OnEnd;

		[SerializeField] private Controller m_controller = null;
		[SerializeField] private PlayerCamera m_playerCamera = null;
		[SerializeField] private Grid m_grid = null;
		Action doAction;
		PieceTeam currentTurn;

		private void Awake()
		{
			doAction = DoActionVoid;
		}

		private void Start()
		{
			m_playerCamera.SetStateNormal();
		}

		private void Update()
		{
			m_playerCamera.Controller.UpdateControles();
			doAction();
		}

		private void DoActionVoid(){}
		private void DoTurn()
		{
			m_playerCamera.ManualUpdate();
		}
	}
}
