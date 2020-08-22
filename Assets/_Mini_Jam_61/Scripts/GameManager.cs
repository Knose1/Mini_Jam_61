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

		[SerializeField] private Grid m_grid;
		Action doAction;
		PieceTeam currentTurn;

		private void Awake()
		{
			doAction = DoActionVoid;
		}

		private void Start()
		{
			
		}

		private void Update()
		{
			doAction();
		}

		private void DoActionVoid(){}
		private void DoTurn(){}
	}
}
