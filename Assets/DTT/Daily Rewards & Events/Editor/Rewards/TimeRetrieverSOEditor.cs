using DTT.PublishingTools;
using DTT.Utils.EditorUtilities;
using DTT.Utils.Extensions;
using UnityEditor;

namespace DTT.DailyRewards.Editor
{
    /// <summary>
    /// Cache for editor inspector properties to make access easier.
    /// </summary>
    public class TimeRetrieverSOEditorCache : SerializedPropertyCache
    {
        public TimeRetrieverSOEditorCache(SerializedObject serializedObject) : base(serializedObject)
        {

        }

        /// <summary>
        /// The time retriever serialized property. (enum)
        /// </summary>
        public SerializedProperty TimeRetrieverSetup => base["timeRetrieverSetup"];

        /// <summary>
        /// The custom API url serialized property. (string)
        /// </summary>
        public SerializedProperty CustomAPIUrl => base["customAPIUrl"];

        /// <summary>
        /// The Is JSON serialized property. (boolean)
        /// </summary>
        public SerializedProperty IsJson => base["isJson"];

        /// <summary>
        /// The JSON key serialized property. (string)
        /// </summary>
        public SerializedProperty JsonKey => base["jsonKey"];

        /// <summary>
        /// The fallback policy serialized property. (enum)
        /// </summary>
        public SerializedProperty FallbackPolicy => base["fallbackPolicy"];

        /// <summary>
        /// The cache remote call serialized property. (enum)
        /// </summary>
        public SerializedProperty CacheRemoteCall => base["cacheRemoteCall"];

        /// <summary>
        /// The cache timeout in seconds serialized property. (int)
        /// </summary>
        public SerializedProperty CacheTimeoutInSeconds => base["cacheTimeoutInSeconds"];
    }

    /// <summary>
    /// Custom editor to simplify and help understand which options
    /// matter and when.
    /// </summary>
    [CustomEditor(typeof(TimeRetrieverSO))]
    [DTTHeader("dtt.daily-rewards", "Time Retriever")]
    public class TimeRetrieverSOEditor : DTTInspector
    {
        /// <summary>
        /// A cache for easier reference of serialized properties.
        /// </summary>
        private TimeRetrieverSOEditorCache _cache;

        /// <summary>
        /// Setup the cache for easier reference.
        /// </summary>
        protected override void OnEnable()
        {
            _cache = new TimeRetrieverSOEditorCache(serializedObject);
            base.OnEnable();
        }

        /// <summary>
        /// Draw content dependent on what flags are set.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_cache.TimeRetrieverSetup);

            if (_cache.TimeRetrieverSetup.intValue == TimeRetrieverSetup.REMOTE.ToInt())
            {
                EditorGUILayout.PropertyField(_cache.CustomAPIUrl);
                EditorGUILayout.PropertyField(_cache.FallbackPolicy);
                EditorGUILayout.PropertyField(_cache.IsJson);

                if (_cache.IsJson.boolValue)
                    EditorGUILayout.PropertyField(_cache.JsonKey);

                EditorGUILayout.PropertyField(_cache.CacheRemoteCall);
                if (_cache.CacheRemoteCall.boolValue)
                    EditorGUILayout.PropertyField(_cache.CacheTimeoutInSeconds);
            }

            string[] customParams =
            {
                _cache.TimeRetrieverSetup.name,
                _cache.CustomAPIUrl.name,
                _cache.IsJson.name,
                _cache.JsonKey.name,
                _cache.FallbackPolicy.name,
                _cache.CacheRemoteCall.name,
                _cache.CacheTimeoutInSeconds.name,

            };
            DrawPropertiesExcluding(serializedObject, customParams);

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            //base.OnInspectorGUI();
        }
    }
}