using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using Dalamud.Logging;

namespace Ktisis.Interop.Resolvers;

// Temporary workaround as loading unmanaged DLLs in Dalamud is currently bugged.
// See the following issue: https://github.com/goatcorp/Dalamud/issues/1238
// Thanks to Minoost for providing this method.

internal class DllResolver {
	private AssemblyLoadContext? Context;

	private readonly List<nint> Handles = new();

	internal void Init() {
		PluginLog.Debug("Creating DLL resolver for unmanaged DLLs");

		Context = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly());
		if (Context != null)
			Context.ResolvingUnmanagedDll += ResolveUnmanaged;
	}

	internal void Dispose() {
		PluginLog.Debug("Disposing DLL resolver for unmanaged DLLs");

		if (Context != null)
			Context.ResolvingUnmanagedDll -= ResolveUnmanaged;
		Context = null;

		if (Handles.Count > 0)
			Handles.ForEach(FreeHandle);
		Handles.Clear();
	}

	private nint ResolveUnmanaged(Assembly assembly, string library) {
		var loc = Path.GetDirectoryName(assembly.Location);
		if (loc == null) return nint.Zero;

		var path = Path.Combine(loc, library);
		PluginLog.Debug($"Resolving native assembly path: {path}");

		if (NativeLibrary.TryLoad(path, out var handle) && handle != nint.Zero) {
			Handles.Add(handle);
			PluginLog.Debug($"Success, resolved library handle: {handle:X}");
		} else {
			PluginLog.Warning($"Failed to resolve native assembly path: {path}");
		}

		return handle;
	}

	private void FreeHandle(nint handle) {
		PluginLog.Debug($"Freeing library handle: {handle:X}");
		NativeLibrary.Free(handle);
	}
}