using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JunkBase : Damage
{
    [Header("Junk Values")]
    [Tooltip("Pixels moved per time delta")]
    public float MovementSpeed;
    [Tooltip("The initial total health")]
    public int StartingHealth;

    [Header("Junk Fracture Values")]
    [Tooltip("The health value when pieces fracture off of it")]
    public int FracturePercentage;
    [Tooltip("The number of pieces that spawn during a fracture")]
    public int FracturePieces;
    [Tooltip("The speed of the fractured off pieces")]
    public float FragmentSpeed;
    [Tooltip("The prefab to use for the fracture pieces")]
    public GameObject JunkFragments;

    private bool hasFractured = false;
    private ParticleSystem particle;
    

    private void OnEnable ()
    {
        Health = StartingHealth;
        hasFractured = false;
        var parent = transform.parent;
        if (parent != null)
        {
            particle = parent.GetComponentInChildren<ParticleSystem>();
        }
        
        if (particle != null)
        {
            particle.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (gameController.IsGamePaused)
        {
            return;
        }

        if (Health < FracturePercentage && !hasFractured)
        {
            hasFractured = true;
            Fracture();
        }
    }

    private void FixedUpdate()
    {
        if (gameController.IsGamePaused)
        {
            return;
        }

        if (transform.parent != null)
        {
            transform.parent.position = new Vector3(transform.parent.position.x - MovementSpeed * Time.deltaTime, transform.parent.position.y, transform.parent.position.z);
            transform.parent.Rotate(0, 0, 1);
        } 
        else
        {
            transform.position = new Vector3(transform.position.x - MovementSpeed * Time.deltaTime, transform.position.y, transform.position.z);
            transform.Rotate(0, 0, 1);
        }
        
    }

    private void OnDisable()
    {
        if (Health <= 0)
        {
            if (particle != null)
            {
                particle.gameObject.SetActive(true);
            }
            ScorePoints();
        }
    }

    private void Fracture()
    {
        for (int i = 0; i < FracturePieces; i++)
        {
            var piece = SimplePool.Spawn(JunkFragments, gameObject.transform.position, gameObject.transform.rotation);
            var pieceRB = piece.GetComponent<Rigidbody2D>();
            pieceRB.velocity = Random.onUnitSphere * FragmentSpeed;
        }
    }
}
