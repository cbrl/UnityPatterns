using System.Collections;
using UnityEngine;


namespace Patterns.Actions
{
    [System.Serializable]
    public class RunPatternAction : Action
    {
        [SerializeField]
        public Pattern pattern;

        public override IEnumerator Run(PatternBehavior behavior)
        {
            yield return pattern.Clone().Run(behavior);
        }
    }


    [System.Serializable]
    public class ActionList : Action
    {
        [SerializeReference]
        [SerializeReferenceButton]
        public Action[] actions = new Action[0];

        public override IEnumerator Run(PatternBehavior behavior)
        {
            foreach (var action in actions)
            {
                if (!action.enabled)
                {
                    continue;
                }

                yield return action.Run(behavior);
            }
        }
    }


    [System.Serializable]
    public class RepeatPattern : RunPatternAction
    {
        [SerializeField]
        public bool repeatForever = true;

        // Number of times to repeat. Ignored if repeatForever is true.
        [SerializeField]
        public int repeatCount = 1;

        public override IEnumerator Run(PatternBehavior behavior)
        {
            if (repeatForever)
            {
                while (true)
                {
                    yield return pattern.Run(behavior);
                }
            }
            else
            {
                for (int i = 0; i < repeatCount; i++)
                {
                    yield return pattern.Run(behavior);
                }
            }
        }
    }


    [System.Serializable]
    public class RepeatActions : ActionList
    {
        [SerializeField]
        public bool repeatForever = true;

        [SerializeField]
        [Tooltip("Number of times to repeat. Ignored if repeatForever is true.")]
        public int repeatCount = 1;

        public override IEnumerator Run(PatternBehavior behavior)
        {
            for (int i = 0; (i < repeatCount) || repeatForever; i++)
            {
                yield return base.Run(behavior);
            }
        }
    }


    [System.Serializable]
    public class WaitAction : Action
    {
        [SerializeField]
        public float waitSeconds = 0;

        public override IEnumerator Run(PatternBehavior behavior)
        {
            yield return new WaitForSeconds(waitSeconds);
        }
    }


    [System.Serializable]
    public class SpawnAction : Action
    {
        [SerializeField]
        public GameObject prefab;

        // An optional pattern to give the newly spawned object
        [SerializeField]
        public Pattern pattern = null;

        [SerializeField]
        public Vector3 offset = new();

        [SerializeField]
        public Vector3 rotation = new();

        public override IEnumerator Run(PatternBehavior behavior)
		{
            var obj = Object.Instantiate(prefab);

            obj.transform.SetPositionAndRotation(
                behavior.transform.position + offset,
                behavior.transform.rotation * Quaternion.Euler(rotation)
            );

            if (pattern != null)
            {
                obj.AddComponent<PatternBehavior>().pattern = pattern.Clone();
            }

            // The PatternBehavior will not automatically run the pattern since it was null when the component was created
            if (obj.TryGetComponent(out PatternBehavior spawnedBehavior))
            {
                spawnedBehavior.Run();
            }

            yield return null;
		}
	}


    [System.Serializable]
    public class SetVelocityAction : Action
    {
        [SerializeField]
        public float velocity = 0.0f;

        // The time it takes for the velocity to change. Set to <= 0 for instant change.
        [SerializeField]
        public float timeToChange = 0.0f;

        public override IEnumerator Run(PatternBehavior behavior)
        {
            if (behavior.body != null)
            {
                var timeFactor = 1.0f / System.Math.Max(0.0f, timeToChange);
                
                // If infinite timeFactor (instant change), just set the new velocity.
                if (float.IsInfinity(timeFactor))
                {
                    behavior.body.velocity = Vector3.Normalize(behavior.transform.forward) * velocity;
                }
                else // If non-instant change is desired, then lerp between the start/end rotation over time
                {
                    var elapsedTime = 0.0f;
                    var initVelocity = Vector3.Dot(behavior.transform.forward, behavior.body.velocity);

                    while (elapsedTime < timeToChange)
                    {
                        var forward = Vector3.Normalize(behavior.transform.forward);
                        behavior.body.velocity = forward * Mathf.Lerp(initVelocity, velocity, elapsedTime * timeFactor);
                        elapsedTime += Time.deltaTime;
                        yield return new WaitForFixedUpdate();
                    }
                }
            }
            yield return null;
        }
    }


    [System.Serializable]
    public class TargetObjectAction : Action
    {
        [SerializeField]
        public GameObject target;

        // Maximum rate at which the object can turn towards the target
        [SerializeField]
        public float maxDegreesPerSecond = 0.0f;

        public override IEnumerator Run(PatternBehavior behavior)
        {
            var toTargetVector = target.transform.position - behavior.transform.position;
            var toTargetQuat = Quaternion.LookRotation(toTargetVector);

            behavior.transform.rotation = Quaternion.RotateTowards(behavior.transform.rotation, toTargetQuat, maxDegreesPerSecond * Time.deltaTime);

            behavior.body.velocity = Vector3.Normalize(behavior.transform.forward) * 10;

            yield return null;
        }
    }


    [System.Serializable]
    public class DestroyAction : Action
    {
        public override IEnumerator Run(PatternBehavior behavior)
        {
            Object.Destroy(behavior.gameObject);
            yield return null;
        }
    }
} //namespace Patterns.Actions
