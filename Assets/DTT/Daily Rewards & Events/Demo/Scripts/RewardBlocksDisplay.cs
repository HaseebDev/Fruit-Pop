using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.DailyRewards.Demo
{
    /// <summary>
    /// Displays a list of rewards with blocks.
    /// </summary>
    public class RewardBlocksDisplay : MonoBehaviour
    {

        /// <summary>
        /// The block prefab of the reward.
        /// </summary>
        [SerializeField]
        private RewardBlock blockPrefab;

        /// <summary>
        /// All currently made blocks, in order.
        /// </summary>
        [SerializeField]
        private List<RewardBlock> alreadyMadeBlocks;

        /// <summary>
        /// Where the blocks should be instantiated.
        /// </summary>
        [SerializeField]
        private Transform blockHolder;


        /// <summary>
        /// The scroll rect of the reward displayer, moved to the end
        /// when new blocks are added.
        /// </summary>
        [SerializeField]
        private ScrollRect scrollRect;


        /// <summary>
        /// Given a list of rewards, display them in the scroll view using blocks.
        /// </summary>
        /// <param name="previousRewards">All previously claimed rewards.</param>
        /// <param name="nextAvailableReward">The next available reward (may be invalid/null/unavailable).</param>
        public void UpdateBlocks(Reward[] previousRewards, RewardCallback nextAvailableReward)
        {

            previousRewards = previousRewards.Concat(new[] { nextAvailableReward.Reward }).ToArray();


            for (int i = 0; i < previousRewards.Length; i++)
            {
                Reward nextReward = previousRewards[i];
                if (alreadyMadeBlocks.Count <= i)
                {
                    AppendNewBlock(nextReward);
                    continue;
                }
                alreadyMadeBlocks[i].UpdateReward(nextReward);
            }

        }


        /// <summary>
        /// Add a new block to the scroll view.
        /// </summary>
        /// <param name="newReward">The new reward.</param>
        private void AppendNewBlock(Reward newReward)
        {
            RewardBlock newBlock = Instantiate(blockPrefab, blockHolder);
            alreadyMadeBlocks.Add(newBlock);
            newBlock.UpdateReward(newReward);
            StartCoroutine(ScrollToEnd());
        }

        /// <summary>
        /// Scrolls to the end of the scroll view.
        /// </summary>
        private IEnumerator ScrollToEnd()
        {
            yield return new WaitForEndOfFrame();
            Canvas.ForceUpdateCanvases();
            scrollRect.gameObject.SetActive(true);
            scrollRect.horizontalNormalizedPosition = 1f;
            Canvas.ForceUpdateCanvases();

        }
    }


}