using System;
using System.Collections.Generic;

namespace Noesis.MonoGame;

/// <summary>
/// Keeps MonoGame graphics resources used by NoesisGUI strongly referenced, so they are never left to the finalizer.
/// Finalizing a MonoGame texture off the main thread blocks, which deadlocks NoesisGUI shutdown
/// (it waits for pending finalizers). Resources are instead disposed on the main thread once all
/// their Noesis wrappers are gone (<see cref="DisposeReleased"/>) or on shutdown (<see cref="DisposeAll"/>).
/// </summary>
internal static class NoesisResourceTracker
{
    private sealed class Entry
    {
        public readonly List<WeakReference<NoesisTexture>> Owners = new();
    }

    private static readonly object _lock = new();
    private static readonly Dictionary<Microsoft.Xna.Framework.Graphics.GraphicsResource, Entry> _entries = new(ReferenceEqualityComparer.Instance);
    private static readonly List<Microsoft.Xna.Framework.Graphics.GraphicsResource> _released = new();

    /// <summary>
    /// Tracks a resource which is not owned by a Noesis wrapper yet, it is kept alive until a wrapper releases it or until shutdown.
    /// </summary>
    public static void Track(Microsoft.Xna.Framework.Graphics.GraphicsResource resource)
    {
        lock (_lock)
        {
            GetEntry(resource);
        }
    }

    public static void Track(Microsoft.Xna.Framework.Graphics.GraphicsResource resource, NoesisTexture owner)
    {
        lock (_lock)
        {
            GetEntry(resource).Owners.Add(new WeakReference<NoesisTexture>(owner));
        }
    }

    /// <summary>
    /// Disposes resources whose Noesis wrappers were all released. Must be called from the main thread.
    /// </summary>
    public static void DisposeReleased()
    {
        lock (_lock)
        {
            foreach (KeyValuePair<Microsoft.Xna.Framework.Graphics.GraphicsResource, Entry> entry in _entries)
            {
                List<WeakReference<NoesisTexture>> owners = entry.Value.Owners;

                if (owners.Count == 0)
                {
                    continue;
                }

                owners.RemoveAll(owner => !owner.TryGetTarget(out NoesisTexture? texture) || texture.IsDisposed);

                if (owners.Count == 0)
                {
                    _released.Add(entry.Key);
                }
            }

            foreach (Microsoft.Xna.Framework.Graphics.GraphicsResource resource in _released)
            {
                _entries.Remove(resource);
                resource.Dispose();
            }

            _released.Clear();
        }
    }

    /// <summary>
    /// Disposes all tracked resources. Must be called from the main thread after the Noesis renderer is shut down.
    /// </summary>
    public static void DisposeAll()
    {
        lock (_lock)
        {
            foreach (Microsoft.Xna.Framework.Graphics.GraphicsResource resource in _entries.Keys)
            {
                resource.Dispose();
            }

            _entries.Clear();
        }
    }

    private static Entry GetEntry(Microsoft.Xna.Framework.Graphics.GraphicsResource resource)
    {
        if (!_entries.TryGetValue(resource, out Entry? entry))
        {
            entry = new Entry();
            _entries.Add(resource, entry);
        }

        return entry;
    }
}
