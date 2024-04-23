using System.Collections;
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
        [Tooltip("Reset the state of all actions between calls to Run()")]
        public bool resetOnRun = true;

        [SerializeReference, SubclassSelector]
        public Action[] actions = new Action[0];

        public IEnumerator Run(PatternBehavior behavior)
        {
            Action[] runningActions;

            if (resetOnRun)
            {
                runningActions = new Action[actions.Length];

                for (int i = 0; i < actions.Length; i++)
                {
                    runningActions[i] = actions[i].Clone();
                }
            }
            else
            {
                runningActions = actions;
            }

            foreach (var action in runningActions)
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

        [Tooltip("Wait for the action to complete before running the next action. Actions will run concurrently if false.")]
        public bool waitForCompletion = true;

        public abstract IEnumerator Run(PatternBehavior behavior);

        public Action Clone()
        {
            return (Action)JsonUtility.FromJson(JsonUtility.ToJson(this), GetType());
        }
    }
} //namespace Patterns
