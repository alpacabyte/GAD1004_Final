using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private SpriteRenderer rendererFeet;
    [SerializeField] private Sprite[] feetWalk;
    [SerializeField] private float deadzoneMagnitude = 0.075f;
    [SerializeField] private TMP_Text staminaCounter;
    [SerializeField] private float maxStamina = 100;
    [SerializeField] private float staminaDecreaseAmount = 3;
    [SerializeField] private float staminaIncreaseAmount = 3;
    [SerializeField] private Transform _lookingPivot;
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _runSpeed = 3f;
    [SerializeField] private Image staminaBarFill;
    private int feetAnimCounter;
    private float feetAnimTimeCounter;
    private GameObject staminaBar;
    private float currentStamina;
    private Camera _cam;
    private Rigidbody2D _charRigidbody;
    private Vector2 _movementVector;
    void Start()
    {
        _charRigidbody = GetComponent<Rigidbody2D>();
        staminaBar = staminaBarFill.transform.parent.gameObject;
        _cam = Camera.main;
        currentStamina = maxStamina;
    }

    void Update()
    {
        _movementVector.x = Input.GetAxisRaw("Horizontal");
        _movementVector.y = Input.GetAxisRaw("Vertical");

        LookToMouse();

        SetStaminaBar();
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.LeftShift) && currentStamina > 0)
        {
            Run();
        }

        else
        {
            Walk();
        }
    }
    private void Walk()
    {
        Vector2 moveVector = _movementVector.normalized * _moveSpeed * Time.fixedDeltaTime;
        _charRigidbody.MovePosition(_charRigidbody.position + moveVector);

        AnimateFeet(moveVector, 0.075f);

        if (currentStamina < maxStamina)
        {
            currentStamina += staminaIncreaseAmount * Time.fixedDeltaTime;
        }
    }
    private void AnimateFeet(Vector2 moveVector, float feetAnimTime)
    {
        if (moveVector.magnitude != 0)
        {
            feetAnimTimeCounter -= Time.deltaTime;
            if (feetAnimTimeCounter < 0)
            {
                rendererFeet.sprite = feetWalk[feetAnimCounter++];

                if (feetAnimCounter == feetWalk.Length)
                {
                    feetAnimCounter = 0;
                }

                feetAnimTimeCounter = feetAnimTime;
            }
        }
        else
        {
            rendererFeet.sprite = feetWalk[5];
            feetAnimCounter = 5;
        }
    }
    private void Run()
    {
        Vector2 moveVector = _movementVector.normalized * _runSpeed * Time.fixedDeltaTime;
        _charRigidbody.MovePosition(_charRigidbody.position + moveVector);

        AnimateFeet(moveVector, 0.025f);

        currentStamina -= staminaDecreaseAmount * Time.fixedDeltaTime;
    }
    void LookToMouse()
    {
        Vector3 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDir = mousePos - _lookingPivot.position;

        if (lookDir.magnitude > deadzoneMagnitude)
        {
            float nextAngle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
            _charRigidbody.MoveRotation(nextAngle);
        }
    }
    private void SetStaminaBar()
    {
        staminaCounter.text = (int)((currentStamina / maxStamina) * 100) + "/" + maxStamina;
        float fillAmount = currentStamina / maxStamina;
        staminaBarFill.fillAmount = fillAmount;

        if (currentStamina >= maxStamina)
        {
            staminaBar.SetActive(false);
        }

        else
        {
            staminaBar.SetActive(true);
        }
    }
    public void BuyStamina()
    {
        maxStamina += 10;
    }
    public void SetLookingPivot(Transform newLookingPivot)
    {
        _lookingPivot = newLookingPivot;
    }
}
