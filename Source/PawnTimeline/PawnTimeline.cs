namespace PawnTimeline {
	[Verse.StaticConstructorOnStartup]
	public static class PawnTimeline {
		static PawnTimeline() {
			Verse.Log.Message("Hello, World!");
		}
	}
}
