using System.Collections;
using UnityEngine;


// Packages that make [SerializeReference] usable
//https://github.com/mackysoft/Unity-SerializeReferenceExtensions
//https://github.com/TextusGames/UnitySerializedReferenceUI


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

        [Tooltip("If set, the pattern will automatically run when the associated function is called.")]
        public StartTrigger trigger = StartTrigger.Start;

        // Will reference the owning object's RigidBody, if present.
        [HideInInspector]
        public Rigidbody body;

        void Awake()
        {
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
            // By default, all scripts that refer to the same Pattern asset will actually refer to the same instance
            // of a Pattern object. This is because a ScriptableObject asset is actually a reference to a single global
            // instance of the object. This means that if Actions inside that pattern modify their state, then that
            // modified state will be visible in every script which uses that Pattern asset. By cloning the pattern, it
            // can be freely modified without affecting other objects using the same asset.
            StartCoroutine(pattern.Clone().Run(this));
        }

    }
} //namespace Patterns
