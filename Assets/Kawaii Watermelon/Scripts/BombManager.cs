using UnityEngine;

public class BombManager : MonoBehaviour
{
    private float explosionRadius = 1.2f;
    [SerializeField] private LayerMask fruitLayer;
    [SerializeField] private ParticleSystem explosionEffect;

    private bool hasExploded = false;


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Fruit") && !hasExploded)
        {
            Explode();
        }
    }

    private void Explode()
    {
        // Detect fruits within the explosion radius
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, fruitLayer);

        // Destroy fruits within the explosion radius
        foreach (Collider2D collider in colliders)
        {
            Fruit fruit = collider.GetComponent<Fruit>();
            if (fruit != null)
            {
                Destroy(fruit.gameObject);
            }
        }

        // Optionally, play explosion effect or animation
        if (explosionEffect != null)
        {
            GameObject.Find("AudioSourceBombPowerUpBlast").GetComponent<AudioSource>().Play();
            explosionEffect.Play();
            //Debug.Log("Nice");

            // Detach the particle effect object from its parent
            explosionEffect.transform.SetParent(null);

        }

        // Destroy the bomb object

        hasExploded = true;
        FindObjectOfType<FruitManager>().isPowerUp2Active = false;
        Debug.Log("Sooo?");
        Destroy(gameObject, 0.3f);
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

    // Optionally, visualize the explosion radius in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
