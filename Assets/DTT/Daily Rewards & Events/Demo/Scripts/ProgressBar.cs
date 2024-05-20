using System;
using UnityEngine;

namespace DTT.DailyRewards.Demo
{
    /// <summary>
    /// A simple progress bar implementation, with smoothed movement.
    /// </summary>
    public class ProgressBar : MonoBehaviour
    {

        /// <summary>
        /// The fill of the bar.
        /// </summary>
        [SerializeField]
        private RectTransform barFill;

        /// <summary>
        /// The background of the bar.
        /// </summary>
        [SerializeField]
        private RectTransform barBackground;

        /// <summary>
        /// The  desired fill of the progress bar
        /// </summary>
        [SerializeField]
        [Range(0f, 1f)]
        private float desiredFill;

        /// <summary>
        /// How fast to lerp the progress bar.
        /// </summary>
        [SerializeField]
        [Range(0.1f, 10f)]
        private float lerpSpeed = 1;



        /// <summary>
        /// The current fill amount of progress bar.
        /// </summary>
        private float _currentFill;


        /// <summary>
        /// Set the current fill to the desired fill.
        /// </summary>
        private void Start()
        {
            _currentFill = desiredFill;
        }


        /// <summary>
        /// </summary>
        private void FixedUpdate()
        {

            if (Math.Abs(desiredFill - _currentFill) > 0.01f)
            {
                float toChange = (desiredFill - _currentFill) * lerpSpeed * Time.fixedDeltaTime;
                float minChange = 0.002f;
                if (Math.Abs(toChange) < minChange)
                    toChange = Math.Sign(toChange) * minChange;

                //cap change amount to not overshoot
                if (Math.Abs(toChange) > Math.Abs(desiredFill - _currentFill))
                    _currentFill = desiredFill;
                else
                    _currentFill += toChange;
            }
            else
            {
                _currentFill = desiredFill;
            }
            UpdateBarFill(_currentFill);
        }

        /// <summary>
        /// Set new progress amount.
        /// </summary>
        /// <param name="fillValue">Amount to fill to (value clamped between 0 and 1).</param>
        /// <param name="moveInstant">If the bar should move instantly to the new value or lerp.</param>
        public void SetNewFill(float fillValue, bool moveInstant = false)
        {
            fillValue = Mathf.Clamp(fillValue, 0, 1);
            desiredFill = fillValue;
            if (moveInstant)
            {
                _currentFill = fillValue;
            }
        }

        /// <summary>
        /// Update the current fill of the bar.
        /// </summary>
        /// <param name="newFill">The new fill amount of the bar.</param>
        private void UpdateBarFill(float newFill)
        {
            float maxWidth = barBackground.rect.width;
            float newWidth = maxWidth * newFill;
            barFill.sizeDelta = new Vector2(newWidth - maxWidth, barFill.sizeDelta.y);
        }
    }
}