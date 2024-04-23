using Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScriptableObjectExtension
{
    /// <summary>
    /// Creates and returns a clone of any given scriptable object.
    /// </summary>
    public static T Clone<T>(this T scriptableObject) where T : ScriptableObject
    {
        if (scriptableObject == null)
        {
            Debug.LogError($"ScriptableObject was null. Returning default {typeof(T)} object.");
            return (T)ScriptableObject.CreateInstance(typeof(T));
        }

        T instance = Object.Instantiate(scriptableObject);
        instance.name = scriptableObject.name; // remove (Clone) from name
        return instance;
    }
}


namespace Patterns
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Pattern", order = 1)]
    public class Pattern : ScriptableObject
    {
        [SerializeReference, SubclassSelector]
        public Action[] actions = new Action[0];

        public IEnumerator Run(PatternBehavior behavior)
        {
            foreach (var action in actions)
            {
                if (!action.enabled)
                {
                    continue;
                }

                if (action.waitForCompletion)
                {
                    yield return action.Run(behavior);
                }
                else
                {
                    behavior.StartCoroutine(action.Run(behavior));
                }
            }
        }
    }


    [System.Serializable]
    public abstract class Action
    {
        public bool enabled = true;

        [SerializeField]
        [Tooltip("Wait for the action to complete before running the next action. Actions will run concurrently if false.")]
        public bool waitForCompletion = true;

        public abstract IEnumerator Run(PatternBehavior behavior);
    }
} //namespace Patterns
