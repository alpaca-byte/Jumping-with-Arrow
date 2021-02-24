using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private float axisX, axisY;

    private float charge = 0, glideCur = 0;

    private bool onGround;

    private bool toUp = true;

    private bool oneTime = false;

    private Rigidbody2D rb;

    private Vector3 offSetSlider = new Vector3(0, 0.75f, 0);

    [SerializeField] private Slider chargeSlider, gliderSlider;

    [SerializeField] private Canvas chargeSliderCanvas;

    [SerializeField] private GameObject arrow;

    [SerializeField] private float rotationSpeed = 135; // Arrow rotation speed

    [SerializeField] private float maxChargeTime = 1, chargePerSec = 10, glideTime = 1, gravityScale = 0.2f; // Max time for charging, charge point for a second, max time for gliding, gravity scale when gliding.
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        chargeSlider.maxValue = maxChargeTime;

        gliderSlider.maxValue = glideTime;
    }

    void Update()
    {
        sliderFollow();

        bool isStop = rb.velocity.magnitude == 0;

        if (Input.GetMouseButton(0) && !onGround && glideCur < glideTime)
        {
            glide();
        }

        else
        {
            gliderSlider.gameObject.SetActive(false);
            rb.gravityScale = 1;
        }
        //********************************************************************
        if (Input.GetMouseButton(0) && onGround && charge < maxChargeTime && isStop)
        {
            onCharge();
        }

        if (!Input.GetMouseButton(0) && onGround && isStop)
        {
            chargeSlider.gameObject.SetActive(true);
            arrow.SetActive(true);
            aimMove();
        }

        else if (!(onGround && isStop))
        {
            arrow.SetActive(false);
            chargeSlider.gameObject.SetActive(false);
        }
        //********************************************************************
        if (Input.GetMouseButtonUp(0))
        {
            throwBall();
        }
    }

    private void glide()
    {
        Vector2 speed = rb.velocity;

        if (!oneTime)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            oneTime = true;
        }

        gliderSlider.gameObject.SetActive(true);

        glideCur += Time.deltaTime;

        gliderSlider.value = glideCur;

        rb.gravityScale = gravityScale;
    }
    private void onCharge()
    {
        charge += Time.deltaTime;
        chargeSlider.value = charge;

        Vector3 arrowVector = (arrow.transform.position - transform.position).normalized;
        axisX += Time.deltaTime * chargePerSec * arrowVector.x;
        axisY += Time.deltaTime * chargePerSec * arrowVector.y;
    }

    private void aimMove()
    {
        glideCur = 0;
        charge = 0;
        chargeSlider.value = charge;
        oneTime = false;

        float angleOfArrow = arrow.transform.rotation.eulerAngles.z;

        if (angleOfArrow >= 180 && angleOfArrow <= 270) toUp = false;

        else if (angleOfArrow >= 270 && angleOfArrow <= 360) toUp = true;

        if (toUp) arrow.transform.RotateAround(transform.position, Vector3.forward, rotationSpeed * Time.deltaTime);

        else arrow.transform.RotateAround(transform.position, Vector3.forward, -rotationSpeed * Time.deltaTime);
    }

    private void throwBall()
    {
        rb.AddForce(transform.up * axisY, ForceMode2D.Impulse);
        rb.AddForce(transform.right * axisX, ForceMode2D.Impulse);

        axisX = 0;
        axisY = 0;
    }
    private void sliderFollow()
    {
        chargeSliderCanvas.transform.localPosition = transform.position + offSetSlider;
    }

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
