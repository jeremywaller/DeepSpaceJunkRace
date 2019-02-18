using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float MaxHealth = 100f;
    [Tooltip("Inner Shield")]
    public float MaxShield1 = 300f;
    [Tooltip("Middle Shield")]
    public float MaxShield2 = 200f;
    [Tooltip("Outer Shield")]
    public float MaxShield3 = 100f;

    [HideInInspector]
    public float CurrentHealth;
    [HideInInspector]
    public float CurrentShield1;
    [HideInInspector]
    public float CurrentShield2;
    [HideInInspector]
    public float CurrentShield3;

    private SpriteRenderer spriteRenderer;
    private Slider healthBar;
    private Slider shield1Bar;
    private Slider shield2Bar;
    private Slider shield3Bar;
    private ParticleSystem shield1;
    private ParticleSystem shield2;
    private ParticleSystem shield3;
    private GameController gameController;

    // Use this for initialization
    public void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        CurrentHealth = MaxHealth;
        CurrentShield1 = MaxShield1;
        CurrentShield2 = MaxShield2;
        CurrentShield3 = MaxShield3;
        healthBar = GameObject.Find("PlayerHealth").GetComponent<Slider>();
        shield1Bar = GameObject.Find("PlayerShield1").GetComponent<Slider>();
        shield2Bar = GameObject.Find("PlayerShield2").GetComponent<Slider>();
        shield3Bar = GameObject.Find("PlayerShield3").GetComponent<Slider>();
        
        var shieldz = GetComponentsInChildren<ParticleSystem>();
        shield1 = shieldz.FirstOrDefault(s => s.name == "Shield");
        shield2 = shieldz.FirstOrDefault(s => s.name == "Shield II");
        shield3 = shieldz.FirstOrDefault(s => s.name == "Shield III");
        gameController = GameController.Instance;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (gameController.IsGamePaused)
        {
            //game is paused so get outta here
            return;
        }

        //var isAtBackOfScreen = transform.position.x < left + spriteRenderer.bounds.extents.x;

        //var isAtYLimit = transform.position.y < top + spriteRenderer.bounds.extents.y ||
        //                 transform.position.y > bottom - spriteRenderer.bounds.extents.y;

        

        //if (Input.touchCount > 0)
        //{
        //    // Touch controls - ship chases finger.
        //    var touchPosition = Input.GetTouch(0).position;
        //    var moveToPosition = Camera.main.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y));
        //    transform.position = Vector2.Lerp(transform.position, moveToPosition, accelerationConstant * Time.deltaTime);

        //}
        //else
        //{
            //// Keyboard Controls
            //verticalAxis = Input.GetAxis("Vertical");
            //horizontalAxis = Input.GetAxis("Horizontal");

            //if (isAtBackOfScreen)
            //{
            //    horizontalAxis = Mathf.Clamp(horizontalAxis, 0, 1);
            //}

            //if (verticalAxis != 0)
            //{
            //    verticalSpeed = Mathf.Lerp(verticalSpeed, verticalAxis, accelerationConstant * Time.deltaTime);
            //}
            //else
            //{
            //    verticalSpeed = Mathf.Lerp(verticalSpeed, 0, decellerationConstant * Time.deltaTime);
            //}

            //if (horizontalAxis != 0)
            //{
            //    horizontalSpeed = Mathf.Lerp(horizontalSpeed, horizontalAxis, accelerationConstant * Time.deltaTime);
            //}
            //else
            //{
            //    horizontalSpeed = Mathf.Lerp(horizontalSpeed, 0, decellerationConstant * Time.deltaTime);
            //}

            //verticalSpeed = Mathf.Clamp(verticalSpeed, maxSpeed * -1, maxSpeed);
            //horizontalSpeed = Mathf.Clamp(horizontalSpeed, maxSpeed * -1, maxSpeed);

            //var incrementalMove = new Vector3(horizontalSpeed, verticalSpeed);

            //if (transform.position.x + incrementalMove.x <= left + spriteRenderer.bounds.extents.x ||
            //    transform.position.x + incrementalMove.x >= right - spriteRenderer.bounds.extents.x)
            //{
            //    incrementalMove.x = 0;
            //}

            //if (transform.position.y + incrementalMove.y <= top + spriteRenderer.bounds.extents.y ||
            //    transform.position.y + incrementalMove.y >= bottom - spriteRenderer.bounds.extents.y)
            //{
            //    incrementalMove.y = 0;
            //}

            //transform.position += incrementalMove;
        //}

	}

    private void LateUpdate()
    {
        if (gameController.IsGamePaused)
        {
            return;
        }

        if (tag != "Player")
        {
            return;
        }

        // Update health bar
        healthBar.value = CurrentHealth / MaxHealth;

        // Update shield bars
        shield1Bar.value = CurrentShield1 / MaxShield1;
        shield2Bar.value = CurrentShield2 / MaxShield2;
        shield3Bar.value = CurrentShield3 / MaxShield3;

        if (shield1 && CurrentShield1 <= 0 && shield1.isPlaying)
        {
            ShowShield(1, false);
        }
        else if (shield1 && CurrentShield1 > 0 && shield1.isStopped)
        {
            ShowShield(1, true);
        }

        if (shield2 && CurrentShield2 <= 0 && shield2.isPlaying)
        {
            ShowShield(2, false);
        }
        else if (shield2 && CurrentShield2 > 0 && shield2.isStopped)
        {
            ShowShield(2, true);
        }

        if (shield3 && CurrentShield3 <= 0 && shield3.isPlaying)
        {
            ShowShield(3, false);
        }
        else if (shield3 && CurrentShield3 > 0 && shield3.isStopped)
        {
            ShowShield(3, true);
        }
    }
        
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var damageScript = collision.gameObject.GetComponent<Damage>();

        // If this is an object the player can damage, then the player takes damage if it collides with the object.
        if (damageScript)
        {
            TakeDamage(damageScript.CollissionDamage);
        }
    }

    public void TakeDamage(float amount)
    {
        Handheld.Vibrate();

        var leftOverDamage = Shield3TakeDamage(amount);
        leftOverDamage = leftOverDamage > 0 ? Shield2TakeDamage(leftOverDamage) : 0;
        leftOverDamage = leftOverDamage > 0 ? Shield1TakeDamage(leftOverDamage) : 0;
        leftOverDamage = leftOverDamage > 0 ? HullTakeDamage(leftOverDamage) : 0;

        if (CurrentHealth <= 0)
        {
            Death();
        }
    }

    public float Shield3TakeDamage(float amount)
    {
        var leftOverDamage = amount - CurrentShield3;
        CurrentShield3 = Mathf.Max(CurrentShield3 - amount, 0);
        
        return leftOverDamage;
    }

    public float Shield2TakeDamage(float amount)
    {
        var leftOverDamage = amount - CurrentShield2;
        CurrentShield2 = Mathf.Max(CurrentShield2 - amount, 0);
        
        return leftOverDamage;
    }

    public float Shield1TakeDamage(float amount)
    {
        var leftOverDamage = amount - CurrentShield1;
        CurrentShield1 = Mathf.Max(CurrentShield1 - amount, 0);
        
        return leftOverDamage;
    }

    public void AddToShields(float amount)
    {
        //add to shield1, then 2, then 3
        var shieldAdd = (MaxShield1 - CurrentShield1) >= amount ? amount : (MaxShield1 - CurrentShield1);
        CurrentShield1 += shieldAdd;
        amount -= shieldAdd;

        shieldAdd = (MaxShield2 - CurrentShield2) >= amount ? amount : (MaxShield2 - CurrentShield2);
        CurrentShield2 += shieldAdd;
        amount -= shieldAdd;

        shieldAdd = (MaxShield3 - CurrentShield3) >= amount ? amount : (MaxShield3 - CurrentShield3);
        CurrentShield3 += shieldAdd;
        amount -= shieldAdd;

        //Debug.Log(string.Format("Just got back {0} shields!", amount));
    }

    public void ShowShield(int shield, bool isShown)
    {
        switch (shield)
        {
            case 1:
                if (isShown == true)
                {
                    shield1.Play();
                }
                else
                {
                    shield1.Stop();
                }
                break;
            case 2:
                if (isShown == true)
                {
                    shield2.Play();
                }
                else
                {
                    shield2.Stop();
                }
                break;
            case 3:
                if (isShown == true)
                {
                    shield3.Play();
                }
                else
                {
                    shield3.Stop();
                }
                break;
        }
    }

    public float HullTakeDamage(float amount)
    {
        var leftOverDamage = amount - CurrentHealth;
        CurrentHealth = Mathf.Max(CurrentHealth - amount, 0);

        return leftOverDamage;
    }

    public void AddToHull(float amount)
    {
        CurrentHealth = ((CurrentHealth += amount) > MaxHealth) ? MaxHealth : CurrentHealth;

        //Debug.Log(string.Format("Just got back {0} hull!", amount));
    }

    private void Death()
    {
        healthBar.value = 0;

        // TODO: Show total enemies killed of each type, etc.
        gameController.GameOver();
    }
}
