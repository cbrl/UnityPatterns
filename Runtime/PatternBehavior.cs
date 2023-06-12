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
        [SerializeField]
        public Pattern pattern;

        // Will reference the owning object's RigidBody, if present.
        [HideInInspector]
        public Rigidbody body;

        protected virtual void Awake()
        {
            body = GetComponent<Rigidbody>();
        }

        protected virtual void Start()
        {
            if (pattern)
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
