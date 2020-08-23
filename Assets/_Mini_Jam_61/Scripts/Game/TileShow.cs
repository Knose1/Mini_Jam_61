using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Github.Knose1.MiniJam61.Settings;

namespace Com.Github.Knose1.MiniJam61.Game.Base
{
	public class TileShow : MonoBehaviour
	{
		
		[SerializeField] protected List<ParticleSystem> m_particleSystems = new List<ParticleSystem>();
		[SerializeField] protected GameTeam m_team = default;
		public GameTeam Team 
		{
			get => m_team;
			set => m_team = value;
		}

		[SerializeField] protected ColorSettings m_settings = default;
		

		protected Material materialClone = null;
		new protected MeshRenderer renderer = null;

		private void OnValidate()
		{
			if (Application.isPlaying) SetColor();
		}

		private void Start()
		{
			
		}

		private void SetColor()
		{
			Color c = default;
			switch (m_team)
			{
				case GameTeam.Opponent:
					c = m_settings.OpponentColor;
					break;
				case GameTeam.Player:
					c = m_settings.PlayerColor;
					break;
			}

			for (int i = 0; i < m_particleSystems.Count; i++)
			{
				ParticleSystem lParticleSystem = m_particleSystems[i];
				lParticleSystem.Stop();
				ParticleSystem.MainModule lMain = lParticleSystem.main;
				lMain.startColor = c;

				lParticleSystem.Play();
			}

		}
	}
}