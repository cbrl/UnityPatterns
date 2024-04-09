using System.Collections;
using UnityEngine;


// Packages that make [SerializeReference] usable
//https://github.com/TextusGames/UnitySerializedReferenceUI
//https://github.com/mackysoft/Unity-SerializeReferenceExtensions


namespace Patterns
{
    [System.Serializable]
    public class PatternBehavior : MonoBehaviour
    {
        public enum StartTrigger
        {
            None,
            Awake,
            Start,
            OnEnable,
        }

        public Pattern pattern;

        public StartTrigger trigger = StartTrigger.Start;

        // Will reference the owning object's RigidBody, if present.
        [HideInInspector]
        public Rigidbody body;

        void Awake()
        {
            if (pattern)
            {
                // By default, all scripts that refer to the same Pattern asset will actually refer to the same instance
                // of a Pattern object. This is due to how ScriptableObjects work. All references to the same
                // ScriptableObject asset will refer to a single instance of the object. This means that if any Actions
                // inside that pattern modify their state, then those modifications will be visible in every script that
                // uses that Pattern asset. By cloning the pattern, it can be freely changed without affecting other
                // objects which use the same pattern.
                pattern = pattern.Clone();
            }

            body = GetComponent<Rigidbody>();

            if (trigger == StartTrigger.Awake && pattern)
            {
                Run();
            }
        }

        void Start()
        {
            if (trigger == StartTrigger.Start && pattern)
            {
                Run();
            }
        }

        void OnEnable()
        {
            if (trigger == StartTrigger.OnEnable && pattern)
            {
                Run();
            }
        }

        public void Run()
        {
            StartCoroutine(pattern.Run(this));
        }

    }
} //namespace Patterns
