
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Based on: https://forum.unity3d.com/threads/swipe-in-all-directions-touch-and-mouse.165416/page-2#post-2357031

[RequireComponent(typeof(SwipeManager))]
[RequireComponent(typeof(SpriteRenderer))]
public class InputController : MonoBehaviour
{
    public float maxSpeed = 0.07f;
    public float accelerationConstant = 1f;
    public float decellerationConstant = 3f;
    public float verticalSpeed = 0.0f;
    public float horizontalSpeed = 0.0f;

    private float left, right, top, bottom;
    private float verticalAxis, horizontalAxis;
    private SpriteRenderer spriteRenderer;
    private Vector2 previousTouchPosition;
    private GameController gameController;

    void Start()
    {
        SwipeManager swipeManager = GetComponent<SwipeManager>();
        swipeManager.onSwipe += HandleSwipe;
        swipeManager.onLongPress += HandleLongPress;

        left = Camera.main.ViewportToWorldPoint(Vector3.zero).x;
        right = Camera.main.ViewportToWorldPoint(Vector3.one).x;
        top = Camera.main.ViewportToWorldPoint(Vector3.zero).y;
        bottom = Camera.main.ViewportToWorldPoint(Vector3.one).y;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        previousTouchPosition = Vector2.zero;
        gameController = FindObjectOfType<GameController>();
    }

    void HandleSwipe(SwipeAction swipeAction)
    {
        //Debug.LogFormat("HandleSwipe: {0}", swipeAction);

        if (swipeAction.touchPhase == TouchPhase.Began)
        {
            previousTouchPosition = swipeAction.startPosition;
        }

        var incrementalSwipe = swipeAction.endPosition - previousTouchPosition;
        previousTouchPosition = swipeAction.endPosition;
        incrementalSwipe.Normalize();
        horizontalAxis = incrementalSwipe.x;
        verticalAxis = incrementalSwipe.y;

        if (swipeAction.touchPhase == TouchPhase.Ended)
        {
            verticalAxis = 0;
            horizontalAxis = 0;
        }
    }

    private void Update()
    {
        if (gameController.IsGamePaused)
        {
            return;
        }

        // Use Keyboard/Joystick/Gamepad controls if swipe had no input
        verticalAxis = Input.GetAxis("Vertical") != 0 ? Input.GetAxis("Vertical") : verticalAxis;
        horizontalAxis = Input.GetAxis("Horizontal") != 0 ? Input.GetAxis("Horizontal") : horizontalAxis;

        if (verticalAxis != 0)
        {
            verticalSpeed = Mathf.Lerp(verticalSpeed, verticalAxis, accelerationConstant * Time.deltaTime);
        }
        else
        {
            verticalSpeed = Mathf.Lerp(verticalSpeed, 0, decellerationConstant * Time.deltaTime);
        }

        if (horizontalAxis != 0)
        {
            horizontalSpeed = Mathf.Lerp(horizontalSpeed, horizontalAxis, accelerationConstant * Time.deltaTime);
        }
        else
        {
            horizontalSpeed = Mathf.Lerp(horizontalSpeed, 0, decellerationConstant * Time.deltaTime);
        }

        verticalSpeed = Mathf.Clamp(verticalSpeed, maxSpeed * -1, maxSpeed);
        horizontalSpeed = Mathf.Clamp(horizontalSpeed, maxSpeed * -1, maxSpeed);

        var incrementalMove = new Vector3(horizontalSpeed, verticalSpeed);

        if (transform.position.x + incrementalMove.x <= left + spriteRenderer.bounds.extents.x ||
            transform.position.x + incrementalMove.x >= right - spriteRenderer.bounds.extents.x)
        {
            incrementalMove.x = 0;
        }

        if (transform.position.y + incrementalMove.y <= top + spriteRenderer.bounds.extents.y ||
            transform.position.y + incrementalMove.y >= bottom - spriteRenderer.bounds.extents.y)
        {
            incrementalMove.y = 0;
        }

        transform.position += incrementalMove;

    }

    void HandleLongPress(SwipeAction swipeAction)
    {
        //Debug.LogFormat("HandleLongPress: {0}", swipeAction);
    }
}