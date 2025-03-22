using System;
using Gamekit2D;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    private bool _facingLeft = false;
    public bool FacingLeft { get { return _facingLeft; } set { _facingLeft = value; } }

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float dashSpeed = 4f;
    [SerializeField] private float dashCD = .25f;
    [SerializeField] private TrailRenderer myTrailRenderer;
    private bool isDashing = false;

    [SerializeField] private int behindAmbienceSortingOrder = -6;
    [SerializeField] private int playerSortingOrder = 5;

    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator myAnimator;
    private SpriteRenderer mySpriteRenderer;

    private GameObject[] objects;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        playerControls.Combat.Dash.performed += _ => Dash();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    void Update()
    {
        objects = GameObject.FindGameObjectsWithTag("Objects");

        if (objects != null)
        {
            GameObject closestObject = FindClosestObjectBelowPlayer(4f);

            if (closestObject != null && transform.position.y > closestObject.transform.position.y)
            {
                mySpriteRenderer.sortingOrder = behindAmbienceSortingOrder;
            }
            else
            {
                mySpriteRenderer.sortingOrder = playerSortingOrder;
            }
        }

        PlayerInput();
    }

    private GameObject FindClosestObjectBelowPlayer(float maxDistance)
    {
        List<GameObject> closestObjects = objects
            .Where(ob => Vector3.Distance(transform.position, ob.transform.position) <= maxDistance)
            .OrderBy(ob => Vector3.Distance(transform.position, ob.transform.position))
            .Take(7)
            .ToList();

        GameObject lowestObj = null;

        foreach (GameObject obj in closestObjects)
        {
            if (obj.CompareTag("Objects") && obj.transform.position.y < transform.position.y)
            {
                lowestObj = obj;
            }
        }

        return lowestObj != null ? lowestObj : closestObjects.FirstOrDefault();
    }

    private void FixedUpdate()
    {
        AdjustPlayerFacingDirection();
        Move();
    }

    private void Move()
    {
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
    }

    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();

        myAnimator.SetFloat("moveX", movement.x);
        myAnimator.SetFloat("moveY", movement.y);
    }

    private void AdjustPlayerFacingDirection()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

        if (mousePos.x < playerScreenPoint.x)
        {
            mySpriteRenderer.flipX = true;
            FacingLeft = true;
        }
        else
        {
            mySpriteRenderer.flipX = false;
            FacingLeft = false;
        }
    }
    
    private void Dash()
    {
        if (!isDashing)
        {
            isDashing = true;
            moveSpeed *= dashSpeed;
            myTrailRenderer.emitting = true;
            StartCoroutine(EndDashRoutine());
        }
    }

    private IEnumerator EndDashRoutine()
    {
        float dashTime = .2f;
        yield return new WaitForSeconds(dashTime);
        moveSpeed /= dashSpeed;
        myTrailRenderer.emitting = false;
        yield return new WaitForSeconds(dashCD);
        isDashing = false;
    }

}
