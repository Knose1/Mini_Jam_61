namespace Com.Github.Knose1.MiniJam61.Game
{
	public enum GameTeam
	{
		Opponent,
		Player
	}

	[System.Serializable]
	public class TeamData
	{
		public float lifePoint;

		public TeamData(float lifePoint) => this.lifePoint = lifePoint;
	}
}