using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

namespace NOX
{
    public class PlayerController : MonoBehaviour
    {
        // ////////////////////////////////////// VARIABLES
        [Header("States and Stats")]
        [SerializeField] bool isMoving;
        [SerializeField] public bool isCrouching;
        [SerializeField] bool isRunning;
        [SerializeField] bool isAttacking;
        [SerializeField] bool isMenuOpen;
        [SerializeField] float movementSpeed;
        [SerializeField] float speed;
        [SerializeField] float rotateSpeed;
        [SerializeField] bool velocityResetSwitch = true;
        [SerializeField] public bool isGameOver;
        [SerializeField] bool canAttack;

        [SerializeField] public float currentPower = 5;
        [SerializeField] public float currentStamina;
        [SerializeField] public float currentAgility;

        [SerializeField] float gravityMultiplier;

        [Space]
        [SerializeField] Compass currentDir;
        [SerializeField] Compass rotateDir;
        [SerializeField] bool isCamRotating;
        // Caches
        Rigidbody rb;
        [Header("Assigned Caches")]
        [SerializeField] Animator rakeAnim;
        [SerializeField] Transform rakeTransform;
        [SerializeField] UICamera uiCamera;
        [SerializeField] public HUD hud;
        [SerializeField] public GameObject gameOverScreen;
        [SerializeField] public GameObject winScreen;
        [SerializeField] public GameObject commandText;
        [SerializeField] EnemyController enemy;
        [SerializeField] EndPoint endPoint;

        [SerializeField] Transform staminaBarUI;
        [SerializeField] Transform staminaBar;

        [SerializeField] public float staminaTest;

        PowerTMPro powerTMPro;
        StaminaTMPro staminaTMPro;
        AgilityTMPro agilityTMPro;

        //Chronometer
        float camRotIndex;


        // ////////////////////////////////////// VARIABLES


        // ////////////////////////////////////// EVENTS
        void Start()
        {
            IdentifyCaches();
            ShowStats();
        }

        void FixedUpdate()
        {
            LimitMovement();
            PlayerInputs();
            FixGravity();
        }

        void Update()
        {
            Movement();
            SetRotateDirection();
            ControlCamera();
            OpenCloseMenu();
            GameOver();
            Attack();
            PlayerStamina();

            staminaTest = SaveData.Instance().maxStaminaNumber;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Enemy")
            {
                enemy = other.GetComponent<EnemyController>();
                canAttack = true;
                commandText.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            enemy = null;
            canAttack = false;
            commandText.SetActive(false);
        }

        // ////////////////////////////////////// EVENTS


        // ////////////////////////////////////// METHODS

        // Initials
        private void IdentifyCaches()
        {
            rb = GetComponent<Rigidbody>();
            rotateDir = currentDir;

            uiCamera = FindObjectOfType<UICamera>();
            hud = FindObjectOfType<HUD>();


            powerTMPro = FindObjectOfType<PowerTMPro>();
            staminaTMPro = FindObjectOfType<StaminaTMPro>();
            agilityTMPro = FindObjectOfType<AgilityTMPro>();

            staminaBarUI = FindObjectOfType<StaminaBarUI>().transform;
            staminaBar = FindObjectOfType<StaminaBar>().transform;

            endPoint = FindObjectOfType<EndPoint>();
        }

        // Movement Inputs
        private void PlayerInputs()
        {
            if (!isCamRotating && !isAttacking)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    rb.AddRelativeForce(0, 0, speed);
                }

                if (Input.GetKey(KeyCode.S))
                {
                    rb.AddRelativeForce(0, 0, -speed);
                }

                if (Input.GetKey(KeyCode.A))
                {
                    rb.AddRelativeForce(-speed, 0, 0);
                }

                if (Input.GetKey(KeyCode.D))
                {
                    rb.AddRelativeForce(speed, 0, 0);
                }
            }
            
        }

        // Movement Phsyics

        private void LimitMovement()
        {
            if (!isMoving)
            {
                rb.velocity = new Vector3 (0, rb.velocity.y, 0);
            }

            if (rb.velocity.x >= movementSpeed)
            {
                rb.velocity = new Vector3(movementSpeed, rb.velocity.y, rb.velocity.z);
            }

            if (rb.velocity.x <= -movementSpeed)
            {
                rb.velocity = new Vector3(-movementSpeed, rb.velocity.y, rb.velocity.z);
            }

            if (rb.velocity.z >= movementSpeed)
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, movementSpeed);
            }

            if (rb.velocity.z <= -movementSpeed)
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, -movementSpeed);
            }
        }

        private void Movement()
        {
            if (!isCamRotating)
            {
                if (isCrouching)
                {
                    if (Input.GetKey(KeyCode.W))
                    {
                        isMoving = true;
                        rakeAnim.SetBool("isCrouchWalking", true);
                        StopAllCoroutines();
                        StartCoroutine(Rotate0());
                    }

                    if (Input.GetKey(KeyCode.A))
                    {
                        isMoving = true;
                        rakeAnim.SetBool("isCrouchWalking", true);
                        StopAllCoroutines();
                        StartCoroutine(RotateMinus90());
                    }

                    if (Input.GetKey(KeyCode.S))
                    {
                        isMoving = true;
                        rakeAnim.SetBool("isCrouchWalking", true);
                        StopAllCoroutines();
                        StartCoroutine(Rotate180());
                    }

                    if (Input.GetKey(KeyCode.D))
                    {
                        isMoving = true;
                        rakeAnim.SetBool("isCrouchWalking", true);
                        StopAllCoroutines();
                        StartCoroutine(RotatePlus90());
                    }

                    if (Input.GetKeyDown(KeyCode.C) || SaveData.Instance().currentStaminaNumber <= 0)
                    {
                        speed = 300;
                        isCrouching = false;
                        isMoving = false;
                        rakeAnim.SetBool("isCrouching", false);
                        rakeAnim.SetBool("isCrouchWalking", false);
                    }


                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        isCamRotating = true;
                        camRotIndex = 1.5f;
                        rakeAnim.SetBool("isWalking", false);
                        rakeAnim.SetBool("isCrouchWalking", false);
                        rakeAnim.SetTrigger("isCrouchAttacking");
                    }

                }
                else
                {
                    if (Input.GetKey(KeyCode.W))
                    {
                        isMoving = true;
                        rakeAnim.SetBool("isWalking", true);
                        StopAllCoroutines();
                        StartCoroutine(Rotate0());
                    }

                    if (Input.GetKey(KeyCode.A))
                    {
                        isMoving = true;
                        rakeAnim.SetBool("isWalking", true);
                        StopAllCoroutines();
                        StartCoroutine(RotateMinus90());
                    }

                    if (Input.GetKey(KeyCode.S))
                    {
                        isMoving = true;
                        rakeAnim.SetBool("isWalking", true);
                        StopAllCoroutines();
                        StartCoroutine(Rotate180());
                    }

                    if (Input.GetKey(KeyCode.D))
                    {
                        isMoving = true;
                        rakeAnim.SetBool("isWalking", true);
                        StopAllCoroutines();
                        StartCoroutine(RotatePlus90());
                    }


                    if (Input.GetKeyDown(KeyCode.C) && SaveData.Instance().currentStaminaNumber > 0)
                    {
                        speed = 200;
                        isCrouching = true;
                        rakeAnim.SetBool("isCrouching", true);
                        rakeAnim.SetBool("isWalking", false);
                        StopAllCoroutines();
                    }

                    if (Input.GetKey(KeyCode.LeftShift) && SaveData.Instance().currentStaminaNumber >= 0)
                    {
                        speed = 600;
                        isRunning = true;
                        isCrouching = false;
                        isMoving = true;
                        rakeAnim.SetBool("isRunning", true);
                    }

                    if (Input.GetKeyUp(KeyCode.LeftShift) || SaveData.Instance().currentStaminaNumber <= 0)
                    {
                        speed = 300;
                        isRunning = false;
                        isCrouching = false;
                        isMoving = false;
                        rakeAnim.SetBool("isRunning", false);
                    }

                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        isCamRotating = true;
                        camRotIndex = 1.5f;
                        rakeAnim.SetBool("isWalking", false);
                        rakeAnim.SetBool("isCrouchWalking", false);
                        rakeAnim.SetTrigger("isAttacking");
                    }

                }

                
            }


            if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D))
            {
                isMoving = false;
                rakeAnim.SetBool("isWalking", false);
                rakeAnim.SetBool("isCrouchWalking", false);
                //rakeAnim.SetBool("isRunning", false);
            }

        }


        // Camera Controls

        private void ControlCamera()
        {
            
            if(isCamRotating && camRotIndex > 0)
            {
                camRotIndex = camRotIndex - 1 * Time.deltaTime;
            }

            if(isCamRotating && camRotIndex < 0)
            {
                camRotIndex = 0;
                isCamRotating = false;
                currentDir = rotateDir;
                
            }

            if (Input.GetKeyDown(KeyCode.E) && !isCamRotating)
            {
                isCamRotating = true;
                camRotIndex = 1f;
                
                
                if (currentDir == Compass.N)
                    {
                        if (isCamRotating)
                        {
                            StopAllCoroutines();
                            StartCoroutine(RotateCamNE());
                        }
                    }

                if (currentDir == Compass.NE)
                    {
                        if (isCamRotating)
                        {
                            StopAllCoroutines();
                            StartCoroutine(RotateCamE());
                        }
                    }

                if (currentDir == Compass.E)
                    {
                        if (isCamRotating)
                        {
                            StopAllCoroutines();
                            StartCoroutine(RotateCamSE());
                        }
                    }

                if (currentDir == Compass.SE)
                    {
                        if (isCamRotating)
                        {
                            StopAllCoroutines();
                            StartCoroutine(RotateCamS());
                        }
                    }

                if (currentDir == Compass.S)
                    {
                        if (isCamRotating)
                        {
                            StopAllCoroutines();
                            StartCoroutine(RotateCamSW());
                        }
                    }

                if (currentDir == Compass.SW)
                    {
                        if (isCamRotating)
                        {
                            StopAllCoroutines();
                            StartCoroutine(RotateCamW());
                        }
                    }

                if (currentDir == Compass.W)
                    {
                        if (isCamRotating)
                        {
                            StopAllCoroutines();
                            StartCoroutine(RotateCamNW());
                        }
                    }

                if (currentDir == Compass.NW)
                    {
                        if (isCamRotating)
                        {
                            StopAllCoroutines();
                            StartCoroutine(RotateCamN());
                        }
                    }

                speed = 300;
                isMoving = false;
                isRunning = false;
                rakeAnim.SetBool("isWalking", false);
                rakeAnim.SetBool("isCrouchWalking", false);
                rakeAnim.SetBool("isRunning", false);


            }

            if (Input.GetKeyDown(KeyCode.Q) && !isCamRotating)
            {
                isCamRotating = true;
                camRotIndex = 1f;



                if (currentDir == Compass.N)
                {
                    if (isCamRotating)
                    {
                        StopAllCoroutines();
                        StartCoroutine(RotateCamNW());
                    }
                }

                if (currentDir == Compass.NW)
                {
                    if (isCamRotating)
                    {
                        StopAllCoroutines();
                        StartCoroutine(RotateCamW());
                    }
                }

                if (currentDir == Compass.W)
                {
                    if (isCamRotating)
                    {
                        StopAllCoroutines();
                        StartCoroutine(RotateCamSW());
                    }
                }

                if (currentDir == Compass.SW)
                {
                    if (isCamRotating)
                    {
                        StopAllCoroutines();
                        StartCoroutine(RotateCamS());
                    }
                }

                if (currentDir == Compass.S)
                {
                    if (isCamRotating)
                    {
                        StopAllCoroutines();
                        StartCoroutine(RotateCamSE());
                    }
                }

                if (currentDir == Compass.SE)
                {
                    if (isCamRotating)
                    {
                        StopAllCoroutines();
                        StartCoroutine(RotateCamE());
                    }
                }

                if (currentDir == Compass.E)
                {
                    if (isCamRotating)
                    {
                        StopAllCoroutines();
                        StartCoroutine(RotateCamNE());
                    }
                }

                if (currentDir == Compass.NE)
                {
                    if (isCamRotating)
                    {
                        StopAllCoroutines();
                        StartCoroutine(RotateCamN());
                    }
                }

                speed = 300;
                isMoving = false;
                isRunning = false;
                rakeAnim.SetBool("isWalking", false);
                rakeAnim.SetBool("isCrouchWalking", false);
                rakeAnim.SetBool("isRunning", false);

            }
        }

        private void SetRotateDirection()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!isCamRotating)
                {
                    if (currentDir == Compass.N)
                    {
                        rotateDir = Compass.NE;
                    }

                    if (currentDir == Compass.NE)
                    {
                        rotateDir = Compass.E;
                    }

                    if (currentDir == Compass.E)
                    {
                        rotateDir = Compass.SE;
                    }

                    if (currentDir == Compass.SE)
                    {
                        rotateDir = Compass.S;
                    }

                    if (currentDir == Compass.S)
                    {
                        rotateDir = Compass.SW;
                    }

                    if (currentDir == Compass.SW)
                    {
                        rotateDir = Compass.W;
                    }

                    if (currentDir == Compass.W)
                    {
                        rotateDir = Compass.NW;
                    }

                    if (currentDir == Compass.NW)
                    {
                        rotateDir = Compass.N;
                    }
                }
                

            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (!isCamRotating)
                {
                    if (currentDir == Compass.N)
                    {
                        rotateDir = Compass.NW;
                    }

                    if (currentDir == Compass.NW)
                    {
                        rotateDir = Compass.W;
                    }

                    if (currentDir == Compass.W)
                    {
                        rotateDir = Compass.SW;
                    }

                    if (currentDir == Compass.SW)
                    {
                        rotateDir = Compass.S;
                    }

                    if (currentDir == Compass.S)
                    {
                        rotateDir = Compass.SE;
                    }

                    if (currentDir == Compass.SE)
                    {
                        rotateDir = Compass.E;
                    }

                    if (currentDir == Compass.E)
                    {
                        rotateDir = Compass.NE;
                    }

                    if (currentDir == Compass.NE)
                    {
                        rotateDir = Compass.N;
                    }
                }

            }

        }

        // Menu

        private void OpenCloseMenu()
        {
            if (!isMenuOpen)
            {
                if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape))
                {
                    uiCamera.gameObject.GetComponent<Camera>().enabled = true;
                    hud.gameObject.GetComponent<Canvas>().enabled = false;
                    isMenuOpen = true;
                }
            }

            else if (isMenuOpen)
            {
                if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape))
                {
                    uiCamera.gameObject.GetComponent<Camera>().enabled = false;
                    hud.gameObject.GetComponent<Canvas>().enabled = true;
                    isMenuOpen = false;
                }
            }
        }

        private void GameOver()
        {
            if (isGameOver == true)
            {
                hud.gameObject.GetComponent<Canvas>().enabled = true;
                hud.GetComponentInChildren<Animator>().Play("FadeOut");
                gameOverScreen.SetActive(true);
                this.enabled = false;
            }

            if (endPoint.winState == true)
            {
                hud.gameObject.GetComponent<Canvas>().enabled = true;
                hud.GetComponentInChildren<Animator>().Play("FadeOut");
                winScreen.SetActive(true);
                this.enabled = false;
            }
        }

        private void Attack()
        {
            if (canAttack && Input.GetKeyDown(KeyCode.F))
            {
                enemy.isDead = true;
            }
        }

        public void ShowStats()
        {
            powerTMPro.GetComponent<TextMeshProUGUI>().text = "Power: \n" + currentPower;
            staminaTMPro.GetComponent<TextMeshProUGUI>().text = "Stamina: \n" + currentStamina;
            agilityTMPro.GetComponent<TextMeshProUGUI>().text = "Agility: \n" + currentAgility;

            SaveData.Instance().maxStaminaNumber = 100 + currentStamina;
        }

        private void PlayerStamina()
        {
            if (isCrouching || isRunning)
            {
                if(SaveData.Instance().currentStaminaNumber > 0)
                {
                    SaveData.Instance().currentStaminaNumber -= 6 * Time.deltaTime;
                }
                else
                {
                    SaveData.Instance().currentStaminaNumber = 0;
                }
            }
            else
            {
                if(SaveData.Instance().currentStaminaNumber < SaveData.Instance().maxStaminaNumber)
                {
                    SaveData.Instance().currentStaminaNumber += 3 * Time.deltaTime;
                }
                else
                {
                    SaveData.Instance().currentStaminaNumber = SaveData.Instance().maxStaminaNumber;
                }
            }

            var staminaPercent = SaveData.Instance().currentStaminaNumber / SaveData.Instance().maxStaminaNumber;

            staminaBarUI.localScale = new Vector3(staminaPercent, staminaBarUI.localScale.y, staminaBarUI.localScale.z);
            staminaBar.localScale = new Vector3(staminaPercent, staminaBar.localScale.y, staminaBar.localScale.z);

        }

        private void FixGravity()
        {
            rb.AddForce(rb.velocity.x, gravityMultiplier, rb.velocity.z);
        }




        // ////////////////////////////////////// METHODS

        // ////////////////////////////////////// COROUTINES


        // Rotate Cam

        IEnumerator RotateCamNE()
        {
            while (transform.rotation.y > 45 || transform.rotation.y < 45)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 45, 0), 10 * Time.deltaTime);
                yield return null;
            }
        }

        IEnumerator RotateCamE()
        {
            while (transform.rotation.y > 90 || transform.rotation.y < 90)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 90, 0), 10 * Time.deltaTime);
                yield return null;
            }
        }

        IEnumerator RotateCamSE()
        {
            while (transform.rotation.y > 135 || transform.rotation.y < 135)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 135, 0), 10 * Time.deltaTime);
                yield return null;
            }
        }

        IEnumerator RotateCamS()
        {
            while (transform.rotation.y > 180 || transform.rotation.y < 180)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 180, 0), 10 * Time.deltaTime);
                yield return null;
            }
        }

        IEnumerator RotateCamSW()
        {
            while (transform.rotation.y > 225 || transform.rotation.y < 225)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 225, 0), 10 * Time.deltaTime);
                yield return null;
            }
        }

        IEnumerator RotateCamW()
        {
            while (transform.rotation.y > 270 || transform.rotation.y < 270)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 270, 0), 10 * Time.deltaTime);
                yield return null;
            }
        }

        IEnumerator RotateCamNW()
        {
            while (transform.rotation.y > 315 || transform.rotation.y < 315)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 315, 0), 10 * Time.deltaTime);
                yield return null;
            }
        }

        IEnumerator RotateCamN()
        {
            while (transform.rotation.y > 0 || transform.rotation.y < 0)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 0), 10 * Time.deltaTime);
                yield return null;
            }
        }


        // Rotate Char

        IEnumerator RotatePlus90()
        {
            while (rakeTransform.localRotation.y > 90 || rakeTransform.localRotation.y < 90)
            {
                rakeTransform.localRotation = Quaternion.Slerp(rakeTransform.localRotation, Quaternion.Euler(0, 90, 0), rotateSpeed * Time.deltaTime);
                yield return null;
            }
        }

        IEnumerator RotateMinus90()
        {
            while (rakeTransform.localRotation.y > -90 || rakeTransform.localRotation.y < -90)
            {
                rakeTransform.localRotation = Quaternion.Slerp(rakeTransform.localRotation, Quaternion.Euler(0, -90, 0), rotateSpeed * Time.deltaTime);
                yield return null;
            }
        }

        IEnumerator Rotate180()
        {

            while (rakeTransform.localRotation.y > 180 || rakeTransform.localRotation.y < 180)
            {
                rakeTransform.localRotation = Quaternion.Slerp(rakeTransform.localRotation, Quaternion.Euler(0, 180, 0), rotateSpeed * Time.deltaTime);
                yield return null;
            }
        }

        IEnumerator Rotate0()
        {

            while (rakeTransform.localRotation.y > 0 || rakeTransform.localRotation.y < 0)
            {
                rakeTransform.localRotation = Quaternion.Slerp(rakeTransform.localRotation, Quaternion.Euler(0, 0, 0), rotateSpeed * Time.deltaTime);
                yield return null;
            }
        }



        // ////////////////////////////////////// COROUTINES
    }
}

