using DTT.PublishingTools;
using DTT.Utils.EditorUtilities;
using UnityEditor;

namespace DTT.DailyRewards.Editor
{
    /// <summary>
    /// Editor Cache for easier object access.
    /// </summary>
    public class TimePatterEditorCache : SerializedPropertyCache
    {
        /// <summary>
        /// The base constructor.
        /// </summary>
        /// <param name="serializedObject">The object that has serialized fields to cache.</param>
        public TimePatterEditorCache(SerializedObject serializedObject) : base(serializedObject)
        {

        }

        /// <summary>
        /// The use custom pattern serialized property (boolean).
        /// </summary>
        public SerializedProperty UseCustomPattern => base["useCustomPattern"];

        /// <summary>
        /// The custom pattern serialized property (int array).
        /// </summary>
        public SerializedProperty CustomPattern => base["customPattern"];

        /// <summary>
        /// The units between reward serialized property (enum).
        /// </summary>
        public SerializedProperty UnitsBetweenReward => base["unitsBetweenReward"];
    }

    /// <summary>
    /// Custom editor to simplify and help understand which options
    /// matter and when
    /// </summary>
    [CustomEditor(typeof(TimePatternSO))]
    [DTTHeader("dtt.daily-rewards", "Time Pattern")]
    public class TimePatterEditor : DTTInspector
    {
        /// <summary>
        /// A cache for easier reference of serialized properties.
        /// </summary>
        private TimePatterEditorCache _cache;

        /// <summary>
        /// Setup the cache for easier reference.
        /// </summary>
        protected override void OnEnable()
        {
            _cache = new TimePatterEditorCache(serializedObject);
            base.OnEnable();
        }

        /// <summary>
        /// Draw content dependent on what flags are set.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(_cache.UseCustomPattern);

            EditorGUILayout.PropertyField(_cache.UseCustomPattern.boolValue ? _cache.CustomPattern : _cache.UnitsBetweenReward);

            string[] customParams =
            {
                _cache.UseCustomPattern.name,
                _cache.CustomPattern.name,
                _cache.UnitsBetweenReward.name,
            };
            DrawPropertiesExcluding(serializedObject, customParams);

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

        }
    }
}