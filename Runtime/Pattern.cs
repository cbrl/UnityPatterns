using Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Patterns
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Pattern", order = 1)]
    public class Pattern : ScriptableObject
    {
        [SerializeReference]
        [SerializeReferenceButton]
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
        [SerializeField]
        public bool enabled = true;

        // Wait for the action to complete before running the next action
        [SerializeField]
        public bool waitForCompletion = true;

        public abstract IEnumerator Run(PatternBehavior behavior);
    }
} //namespace Patterns
