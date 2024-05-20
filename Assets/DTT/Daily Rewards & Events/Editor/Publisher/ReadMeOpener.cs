#if UNITY_EDITOR

using DTT.PublishingTools;
using UnityEditor;

namespace DTT.DailyRewards.Editor.Publisher
{
    /// <summary>
    ///     Class that handles opening the editor window for the package template package.
    /// </summary>
    internal static class ReadMeOpener
    {
        /// <summary>
        /// Opens the readme for this package.
        /// </summary>
        [MenuItem("Tools/DTT/Daily Rewards & Events/ReadMe")]
        private static void OpenReadMe()
        {
            DTTReadMeEditorWindow.Open(DTTEditorConfig.GetAssetJson("dtt.daily-rewards"));
        }
    }
}
#endif