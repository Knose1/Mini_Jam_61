namespace Com.Github.Knose1.MiniJam61.Game
{
	public enum GameTeam
	{
		Opponent,
		Player
	}

	public struct TeamData
	{
		float lifePoint;

		public TeamData(float lifePoint):this() => this.lifePoint = lifePoint;
	}
}