using Il2CppInterop.Runtime.Injection;
using MelonLoader;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace BloomEngine.Utilities;

/// <summary>
/// Static class that provides extensions for some Il2Cpp and Unity types.
/// </summary>
public static class Il2CppHelper
{
    // Listener extension methods for UnityEvents
    extension(UnityEvent unityEvent)
    {
        public void AddListener(Action action) => unityEvent.AddListener(action);
        public void RemoveListener(Action action) => unityEvent.RemoveListener(action);
        public void SetListeners(List<Action> actions)
        {
            unityEvent.RemoveAllListeners();

            foreach (Action action in actions)
                unityEvent.AddListener(action);
        }
    }

    // Listener extension methods for UnityEvents with parameters
    extension<T>(UnityEvent<T> unityEvent)
    {
        public void AddListener(Action<T> action) => unityEvent.AddListener(action);
        public void RemoveListener(Action<T> action) => unityEvent.RemoveListener(action);
        public void SetListeners(List<Action<T>> actions)
        {
            unityEvent.RemoveAllListeners();

            foreach (Action<T> action in actions)
                unityEvent.AddListener(action);
        }
    }

    extension<T>(IEnumerable<T> collection)
    {
        public Il2CppSystem.Collections.Generic.List<T> ToIl2CppList()
        {
            var result = new Il2CppSystem.Collections.Generic.List<T>();

            foreach (var item in collection)
                result.Add(item);

            return result;
        }
    }


    /// <summary>
    /// Searches for all <see cref="MonoBehaviour"/> types in the given assembly and registers them in Il2Cpp.
    /// </summary>
    /// <param name="assembly">Assembly to register types from. Use <see cref="MelonBase.MelonAssembly"/> to get the assembly of your mod.</param>
    public static void RegisterAllMonoBehaviours(Assembly assembly)
    {
        var monoBehaviourTypes = assembly.GetTypes()
            .Where(type => type.IsSubclassOf(typeof(MonoBehaviour)) && !type.IsAbstract)
            .OrderBy(type => type.Name);

        foreach (var type in monoBehaviourTypes)
        {
            try
            {
                ClassInjector.RegisterTypeInIl2Cpp(type);
            }
            catch (Exception e)
            {
                MelonLogger.Error($"Failed to register MonoBehaviour: {type.FullName}\n{e}");
            }
        }
    }
}