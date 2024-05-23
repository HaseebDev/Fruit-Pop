using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowBallManager : MonoBehaviour
{
    private bool isActivated = false; // Flag to indicate if the rainbow ball power-up is activated

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isActivated)
        {
            Fruit fruit = collision.gameObject.GetComponent<Fruit>();
            if (fruit != null)
            {
                // If the rainbow ball collides with a fruit, merge them
                //MergeFruits(fruit);
            }
        }
    }

    //private void MergeFruits(Fruit fruit)
    //{
    //    // Get the position of the fruit to merge with the rainbow ball
    //    Vector2 mergePosition = fruit.transform.position;

    //    // Trigger the merge process
    //    MergeManager.Instance.ProcessMerge(transform.position, mergePosition);
    //}

    // Method to activate the rainbow ball power-up
    public void ActivateRainbowBall()
    {
        isActivated = true;
        // You can add visual feedback or any other effects to indicate the power-up mode is active
        Debug.Log("Rainbow Ball activated!");
    }

    // Method to deactivate the rainbow ball power-up
    public void DeactivateRainbowBall()
    {
        isActivated = false;
        Debug.Log("Rainbow Ball deactivated!");
    }


    public void MoveTo(Vector2 targetPosition)
    {
        transform.position = targetPosition;
    }

    public void EnablePhysics()
    {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        GetComponent<Collider2D>().enabled = true;
    }
}
