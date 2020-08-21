using UnityEngine;

namespace Com.Github.Knose1.MiniJam61.Settings
{
	[CreateAssetMenu(fileName=nameof(ColorSettings), menuName=nameof(MiniJam61)+"/"+nameof(ColorSettings))]
	public class ColorSettings : ScriptableObject
	{
		[SerializeField] protected Color m_playerColor = default;
		public Color PlayerColor => m_playerColor;
		
		[SerializeField] protected Color m_opponentColor = default;
		public Color OpponentColor => m_opponentColor;
	}
}