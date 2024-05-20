using DTT.PublishingTools;
using DTT.Utils.EditorUtilities;
using UnityEditor;

namespace DTT.DailyRewards.Editor
{
    /// <summary>
    /// Cache for editor inspector properties to make access easier.
    /// </summary>
    public class DailyRewardPropertyCache : SerializedPropertyCache
    {
        /// <summary>
        /// The base constructor for a serialized object.
        /// </summary>
        /// <param name="serializedObject">The serialized object.</param>
        public DailyRewardPropertyCache(SerializedObject serializedObject) : base(serializedObject)
        {

        }

        /// <summary>
        /// The is finite serialized value (boolean).
        /// </summary>
        public SerializedProperty IsFinite => base["isFinite"];

        /// <summary>
        /// The Finite reward amount serialized value (int).
        /// </summary>
        public SerializedProperty FiniteRewardAmount => base["finiteRewardAmount"];
    }

    /// <summary>
    /// Custom editor to simplify and help understand which options
    /// matter and when.
    /// </summary>
    [CustomEditor(typeof(DailyRewardSO))]
    [DTTHeader("dtt.daily-rewards", "Daily Reward")]
    public class DailyRewardEditor : DTTInspector
    {
        /// <summary>
        /// A cache for easier reference of serialized properties.
        /// </summary>
        private DailyRewardPropertyCache _cache;

        /// <summary>
        /// Setup the cache for easier reference.
        /// </summary>
        protected override void OnEnable()
        {
            _cache = new DailyRewardPropertyCache(serializedObject);
            base.OnEnable();
        }

        /// <summary>
        /// Draw content dependent on what flags are set.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(_cache.IsFinite);

            if (_cache.IsFinite.boolValue)
                EditorGUILayout.PropertyField(_cache.FiniteRewardAmount);

            DrawPropertiesExcluding(serializedObject, _cache.IsFinite.name, _cache.FiniteRewardAmount.name);

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}