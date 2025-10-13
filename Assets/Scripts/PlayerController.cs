using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    private GameObject focalPoint;
    public GameObject powerupIndicator;

    private float powerUpStrenght = 15.0f;
    public float speed = 5.0f;
    public bool hasPowerup = false;
    public bool isGrounded = true;
    public Vector3 jump = new Vector3(0,100,0);
    public float jumpForce = 1000.0f;
    int jumpCount = 0;
    public int maxJumpCount = 2;
    public bool isSmashing = true;

    // Start is called before the first frame update
    void Start()
    {
        focalPoint = GameObject.Find("FocalPoint");
        playerRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumpCount)
        {
            Debug.Log("Jump");
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpCount++;
            if (jumpCount == maxJumpCount)
            {
                playerRb.AddForce(Vector3.up * 2 * jumpForce, ForceMode.Impulse);
            }
            
        }
        if (Input.GetKeyDown(KeyCode.S) && !isGrounded)
        {
            isSmashing = true;
            Debug.Log("Smashed");
            playerRb.velocity = Vector3.zero;
            playerRb.AddForce(Vector3.down * 50f, ForceMode.Impulse);
        }

        float forwardInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * speed * forwardInput);
        powerupIndicator.transform.position = transform.position + new Vector3(0,-0.5f,0);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
            hasPowerup = true;
            powerupIndicator.SetActive(true);
            Destroy(other.gameObject);
            StartCoroutine(PowerupCounterdownRoutine());
        }
    }
    
    
    IEnumerator PowerupCounterdownRoutine()
    {
        yield return new WaitForSeconds(5);
        hasPowerup = false;
        powerupIndicator.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && hasPowerup) 
        { 
            Rigidbody enemyRb = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayForce = (collision.gameObject.transform.position - transform.position);
            Debug.Log("Collided with" + collision.gameObject.name+"with powerup set to" + hasPowerup);
            enemyRb.AddForce(awayForce * powerUpStrenght, ForceMode.Impulse);
        }
        //Under development for double jump with a smash
        if (collision.gameObject.CompareTag("Island"))
        {
            jumpCount = 0;
            isGrounded = true;
        }
        if (isSmashing)
        {
            Debug.Log("Ground Smash Impact!");
            isSmashing = false;
        }

    }
}
