using UnityEngine;

public class Collectable : MonoBehaviour
{
    [Header("Collectable Settings")]
    public float rotationSpeed = 50f;
    public float bobSpeed = 2f;
    public float bobHeight = 0.5f;
    
    private Vector3 startPosition;
    private GameManager gameManager;
    
    void Start()
    {
        startPosition = transform.position;
        gameManager = FindObjectOfType<GameManager>();
        
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found! Make sure there's a GameManager in the scene.");
        }
    }
    
    void Update()
    {
        // Rotate the collectable
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        
        // Bob up and down
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollectItem();
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CollectItem();
        }
    }
    
    void CollectItem()
    {
        if (gameManager != null)
        {
            gameManager.CollectItem();
        }
        
        // Destroy the collectable
        Destroy(gameObject);
    }
}