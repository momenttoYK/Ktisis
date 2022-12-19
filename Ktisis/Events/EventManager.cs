using Dalamud.Game.ClientState.Keys;

using FFXIVClientStructs.Havok;
using static FFXIVClientStructs.Havok.hkaPose;
using Ktisis.Structs.Actor;
using Ktisis.Structs.Input;
using Ktisis.Interface.Overlay;

namespace Ktisis.Events {
	public static class EventManager {
		public delegate void GPoseChange(bool isInGPose);
		public static GPoseChange? OnGPoseChange = null;

		public delegate void TransformationMatrixChange(bool state);
		public static TransformationMatrixChange? OnTransformationMatrixChange = null;

		public delegate void GizmoChange(bool isEditing);
		public static GizmoChange? OnGizmoChange = null;

		internal delegate bool KeyPressEventDelegate(QueueItem e);
		internal static KeyPressEventDelegate? OnKeyPressed;

		internal delegate void KeyReleaseEventDelegate(VirtualKey key);
		internal static KeyReleaseEventDelegate? OnKeyReleased;

		public static void FireOnGposeChangeEvent(bool isInGPose) {
			Logger.Debug("FireOnGposeChangeEvent {0}", isInGPose ? "ON" : "OFF");
			OnGPoseChange?.Invoke(isInGPose);
		}

		public static unsafe void FireOnTransformationMatrixChangeEvent(bool state) {
			if (OnTransformationMatrixChange == null) return;
			var bone = Skeleton.GetSelectedBone();
			var actor = (Actor*)Ktisis.GPoseTarget!.Address;
			hkQsTransformf* boneTransform = bone is null ? &actor->Model->Transform : bone.AccessModelSpace(PropagateOrNot.DontPropagate);
			OnTransformationMatrixChange(state);
		}

		public static void FireOnGizmoChangeEvent(bool isEditing) {
			OnGizmoChange?.Invoke(isEditing);
		}
	}
}
