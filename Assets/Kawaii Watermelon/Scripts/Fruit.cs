using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Fruit : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header(" Data ")]
    [SerializeField] private FruitType fruitType;
    private bool hasCollided;
    private bool canBeMerged;

    [Header(" Actions ")]
    public static Action<Fruit, Fruit> onCollisionWithFruit;

    [Header(" Effects ")]
    [SerializeField] private ParticleSystem mergeParticles;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("AllowMerge", .25f);
    }

    private void AllowMerge()
    {
        canBeMerged = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MoveTo(Vector2 targetPosition)
    {
        float clampedX = Mathf.Clamp(targetPosition.x, -2.263969f, 2.263969f);
        transform.position = new Vector2(clampedX, targetPosition.y);
    }


    public void EnablePhysics()
    {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        GetComponent<Collider2D>().enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ManageCollision(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        ManageCollision(collision);
    }

    private void ManageCollision(Collision2D collision)
    {
        hasCollided = true;

        if (!canBeMerged)
            return;



        if (collision.collider.TryGetComponent(out Fruit otherFruit))
        {
            if (otherFruit.GetFruitType() == FruitType.RainBowBall)
            {
                onCollisionWithFruit?.Invoke(this, otherFruit);
            }

            if (otherFruit.GetFruitType() != fruitType)
                return;

            if (!otherFruit.CanBeMerged())
                return;

            onCollisionWithFruit?.Invoke(this, otherFruit);
        }
    }

    public void Merge()
    {
        if (mergeParticles != null)
        {
            mergeParticles.transform.SetParent(null);
            mergeParticles.Play();
        }

        Destroy(gameObject);
    }

    public FruitType GetFruitType()
    {
        return fruitType;
    }

    public Sprite GetSprite()
    {
        return spriteRenderer.sprite;
    }

    public bool HasCollided()
    {
        return hasCollided;
    }

    public bool CanBeMerged()
    {
        return canBeMerged;
    }
}
