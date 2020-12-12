namespace Monofoxe.Spriter.Monofoxe
{
	/// <summary>
	/// Allows for adding custom logic before animation update.
	/// </summary>
	public abstract class AnimationDriver
  {
		/// <summary>
		/// Called when driver is added to the animator.
		/// </summary>
		public abstract void Bind(FoxeAnimator animator);

		/// <summary>
		/// Called every animation update.
		/// </summary>
		public abstract void Update(FoxeAnimator animator);

		public abstract AnimationDriver Clone();
  }
}
