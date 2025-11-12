using UnityEngine;

namespace MiniGameCollection.Games2025.Team00
{
    public class Waypoint : MonoBehaviour
    {
        [field: SerializeField] public float SpeedToWaypoint { get; private set; } = 5;
        [field: SerializeField] public bool DelayAtWaypoint { get; private set; } = false;
        [field: SerializeField] public float DelayTimeSeconds { get; private set; } = 1f;
        
        [field: SerializeField]
        [field: ReadOnlyGUI] public bool HasTriggeredDelay { get; private set; } = false;

        public Vector3 Position => transform.position;
        public Quaternion Rotation => transform.rotation;

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        ///     True on success.
        /// </returns>
        public bool ActivateDelay()
        {
            if (!DelayAtWaypoint)
                return false;

            bool success = !HasTriggeredDelay;
            HasTriggeredDelay = true;
            return success;
        }
    }
}
