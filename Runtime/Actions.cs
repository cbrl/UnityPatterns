using System.Collections;
using UnityEngine;


namespace Patterns.Actions
{
    [System.Serializable]
    public class RunPatternAction : Action
    {
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
        public bool repeatForever = true;

        [Tooltip("Number of times to repeat. Ignored if repeatForever is true.")]
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
        public bool repeatForever = true;

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
        public float waitSeconds = 0;

        public override IEnumerator Run(PatternBehavior behavior)
        {
            yield return new WaitForSeconds(waitSeconds);
        }
    }


    [System.Serializable]
    public class SpawnAction : Action
    {
        public GameObject prefab;

        [Tooltip("An optional pattern to give the newly spawned object")]
        public Pattern pattern = null;

        [Tooltip("The offset of the spawned object relative to the object this pattern is attached to.")]
        public Vector3 offset = Vector3.zero;

        [Tooltip("The amount by which the offset will increase on each iteration, if this action is part of a loop.")]
        public Vector3 offsetIncrement = Vector3.zero;

        [Tooltip("The maximum offset of the spawned object.")]
        public Vector3 offsetMax = Vector3.positiveInfinity;

        [Tooltip("The rotation of the spawned object relative to the object this pattern is attached to.")]
        public Vector3 rotation = Vector3.zero;

        [Tooltip("The amount by which the rotation will increase on each iteration, if this action is part of a loop.")]
        public Vector3 rotationIncrement = Vector3.zero;

        [Tooltip("The maximum rotation of the spawned object.")]
        public Vector3 rotationMax = Vector3.positiveInfinity;

        private int loopCount = 0;

        public override IEnumerator Run(PatternBehavior behavior)
        {

            var obj = Object.Instantiate(prefab);

            var newOffset = Vector3.Min(offset + (offsetIncrement * loopCount), offsetMax);
            var newRotation = Vector3.Min(rotation + (rotationIncrement * loopCount), rotationMax);

            obj.transform.SetPositionAndRotation(
                behavior.transform.position + newOffset,
                behavior.transform.rotation * Quaternion.Euler(newRotation)
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

            loopCount += 1;

            yield return null;
        }
    }


    [System.Serializable]
    public class SetVelocityAction : Action
    {
        public float velocity = 0.0f;

        [Tooltip("The time it takes for the velocity to change. Set to <= 0 for instant change.")]
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
                    var initTime = Time.realtimeSinceStartup;
                    var currentTime = initTime;
                    var initVelocity = Vector3.Dot(behavior.transform.forward, behavior.body.velocity);

                    while ((currentTime - initTime) < timeToChange)
                    {
                        var forward = Vector3.Normalize(behavior.transform.forward);
                        var lerpPercent = System.Math.Min((currentTime - initTime) * timeFactor, 1.0f);

                        behavior.body.velocity = forward * Mathf.Lerp(initVelocity, velocity, lerpPercent);

                        currentTime = Time.realtimeSinceStartup;

                        yield return null;
                    }

                    // Set final velocity. While loop above can skip last increment.
                    behavior.body.velocity = Vector3.Normalize(behavior.transform.forward) * velocity;
                }
            }

            yield return null;
        }
    }


    [System.Serializable]
    public class TargetPositionAction : Action
    {
        public Vector3 target = Vector3.zero;

        [Tooltip("The maximum rate at which the object can turn towards the target")]
        public float maxDegreesPerSecond = 0.0f;

        public override IEnumerator Run(PatternBehavior behavior)
        {
            var toTargetVector = target - behavior.transform.position;
            var toTargetQuat = Quaternion.LookRotation(toTargetVector);

            behavior.transform.rotation = Quaternion.RotateTowards(behavior.transform.rotation, toTargetQuat, maxDegreesPerSecond * Time.deltaTime);

            yield return null;
        }
    }


    [System.Serializable]
    public class TargetObjectAction : Action
    {
        public GameObject target;

        // Maximum rate at which the object can turn towards the target
        public float maxDegreesPerSecond = 0.0f;

        public override IEnumerator Run(PatternBehavior behavior)
        {
            var toTargetVector = target.transform.position - behavior.transform.position;
            var toTargetQuat = Quaternion.LookRotation(toTargetVector);

            behavior.transform.rotation = Quaternion.RotateTowards(behavior.transform.rotation, toTargetQuat, maxDegreesPerSecond * Time.deltaTime);

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
