using ComponentKit.Model;
using Jitter.LinearMath;

namespace Calcifer.Engine.Physics
{
	public class CrateComponent: DependencyComponent
	{
		[RequireComponent] private PhysicsComponent physics;

		protected override void OnAdded(ComponentStateEventArgs registrationArgs)
		{
			base.OnAdded(registrationArgs);
			physics.Body.SetMassProperties(JMatrix.Zero, 1.0f / 100.0f, true);
		}
	}
}
