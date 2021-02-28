using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private float axisX, axisY; // Axes of x and y vector 

    private float charge = 0, glideCur = 0; // Current charge point and glide time

    private bool onGround; // Is it on ground

    private bool toUp = true; // Is the arrow rotating to up or down

    private bool oneTime = false;  // Checking for one time reset for y vector of player
    private bool airCheck = false; // Is the ball flying

    private Rigidbody2D rb;

    private Vector3 offSetSlider = new Vector3(0, 0.75f, 0); // Offset for charge slider

    [SerializeField] private Slider chargeSlider, gliderSlider;

    [SerializeField] private Canvas chargeSliderCanvas; // Canvas of charge slider

    [SerializeField] private GameObject arrow; // Game object of rotating arrow

    [SerializeField] private float rotationSpeed = 135; // Arrow rotation speed

    [SerializeField] private float maxChargeTime = 1, chargePerSec = 10, glideTime = 1, gravityScale = 0.2f; // Max time for charging, charge point for a second, max time for gliding, gravity scale when gliding.
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        chargeSlider.maxValue = maxChargeTime; // Initializing of charge slider

        gliderSlider.maxValue = glideTime; // Initializing of glider slider
    }

    void Update()
    {
        sliderFollow();

        bool isStop = rb.velocity.magnitude == 0; // Does the ball stop

        // Checking for glide conditions 
        if (Input.GetMouseButton(0) && !onGround && glideCur < glideTime)
        {
            glide();
        }

        else
        {
            gliderSlider.gameObject.SetActive(false);
            rb.gravityScale = 1;
        }

        //Is it on the ground and does it stop
        if (onGround && isStop)
        {
            //Checking for charging conditions
            if (Input.GetMouseButton(0) && charge < maxChargeTime && !airCheck)
            {
                onCharge();
            }

            if (!Input.GetMouseButton(0))
            {
                airCheck = false;
                chargeSlider.gameObject.SetActive(true);
                arrow.SetActive(true);
                aimMove();
            }

            else if (Input.GetMouseButton(0) && airCheck)
            {
                chargeSlider.gameObject.SetActive(true);
                arrow.SetActive(true);
                aimMove();
            }
        }

        else
        {
            // Did player press the left click while flying
            if (Input.GetMouseButton(0))
            {
                airCheck = true;
            }

            arrow.SetActive(false);
            chargeSlider.gameObject.SetActive(false);
        }

        // If player releases the left click and if ball is not in the air, throw the ball
        if (Input.GetMouseButtonUp(0) && !airCheck)
        {
            throwBall();
        }
    }

    // Gliding function
    private void glide()
    {
        // Reset the 'y' vector of player once
        if (!oneTime)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            oneTime = true;
        }

        gliderSlider.gameObject.SetActive(true);

        glideCur += Time.deltaTime;

        gliderSlider.value = glideCur;
        
        // Reduce the gravity
        rb.gravityScale = gravityScale;
    }
    
    // Charging function
    private void onCharge()
    {
        charge += Time.deltaTime;
        chargeSlider.value = charge;

        // Charge the ball proportional to arrow position
        Vector3 arrowVector = (arrow.transform.position - transform.position).normalized;
        axisX += Time.deltaTime * chargePerSec * arrowVector.x;
        axisY += Time.deltaTime * chargePerSec * arrowVector.y;
    }

    // Arrow rotation function
    private void aimMove()
    {
        glideCur = 0;
        charge = 0;
        chargeSlider.value = charge;
        oneTime = false;

        float angleOfArrow = arrow.transform.rotation.eulerAngles.z;

        if (angleOfArrow >= 180 && angleOfArrow <= 270) toUp = false;

        else toUp = true;

        if (toUp) arrow.transform.RotateAround(transform.position, Vector3.forward, rotationSpeed * Time.deltaTime);

        else arrow.transform.RotateAround(transform.position, Vector3.forward, -rotationSpeed * Time.deltaTime);
    }

    // Throwing function
    private void throwBall()
    {
        rb.AddForce(transform.up * axisY, ForceMode2D.Impulse);
        rb.AddForce(transform.right * axisX, ForceMode2D.Impulse);

        axisX = 0;
        axisY = 0;
    }
    
    // Function for slider to follow the player
    private void sliderFollow()
    {
        chargeSliderCanvas.transform.localPosition = transform.position + offSetSlider;
    }

    // Checking for is ball on the ground
    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag("Ground"))
        {
            onGround = true;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Ground"))
        {
            onGround = false;
        }
    }
}
