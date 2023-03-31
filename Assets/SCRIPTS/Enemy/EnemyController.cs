using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace NOX
{
    public class EnemyController : MonoBehaviour
    {
        // ////////////////////////////////////// VARIABLES
        [Header("States and Stats")]
        [SerializeField] int enemyID;
        [SerializeField] float rotSpeed;
        [SerializeField] float moveSpeed;
        [SerializeField] float waitIndex;
        [SerializeField] float detectionIndex;

        [SerializeField] bool isMoving;
        [SerializeField] bool isGameOver;
        [SerializeField] bool isDetectColor;
        [SerializeField] public bool isDead;

        // Caches
        [SerializeField] int wayPointID;
        [SerializeField] PathWay pathWay;
        [SerializeField] Transform currentWayPoint;
        [SerializeField] Transform destinationWayPoint;
        [SerializeField] Animator anim;
        [SerializeField] FOVDrawer fovDrawer;
        [SerializeField] Animator fovAnim;
        [SerializeField] PlayerController player;
        [SerializeField] Animator playerAnim;
        [SerializeField] Animation fovColorAnim;
        [SerializeField] Rigidbody playerRB;
        [SerializeField] Rigidbody rb;
        // ////////////////////////////////////// VARIABLES

        // ////////////////////////////////////// EVENTS
        private void Start()
        {
            AssignPathWay();
            AssignWayPoints();
            IdentifyCaches();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<WayPoint>())
            {
              
                if(wayPointID < pathWay.wayPoints.Count -1)
                {
                    waitIndex = other.GetComponent<WayPoint>().waitTime;
                    wayPointID++;
                    currentWayPoint = pathWay.wayPoints[wayPointID - 1];
                    destinationWayPoint = pathWay.wayPoints[wayPointID];
                }
                else
                {
                    currentWayPoint = pathWay.wayPoints[wayPointID];
                    wayPointID = 0;
                    destinationWayPoint = pathWay.wayPoints[wayPointID];
                }
            }
        }

        private void Update()
        {
            EnemyMovement();
            EnemyAnimations();
            WaitAndDetectionTimer();
            DetectPlayer();
            Death();
        }



        // ////////////////////////////////////// EVENTS

        // ////////////////////////////////////// METHODS
        private void IdentifyCaches()
        {
            anim = GetComponent<Animator>();
            fovDrawer = GetComponentInChildren<FOVDrawer>();
            fovAnim = fovDrawer.GetComponent<Animator>();
            player = FindObjectOfType<PlayerController>();
            playerAnim = player.GetComponentInChildren<Animator>();
            playerRB = player.GetComponent<Rigidbody>();
            rb = GetComponent<Rigidbody>();
        }

        private void AssignPathWay()
        {
            var allPathWays = FindObjectsOfType<PathWay>();

            pathWay = allPathWays.FirstOrDefault(o => o.pathWayID == enemyID);
        }

        private void AssignWayPoints()
        {
            wayPointID = 0;
            currentWayPoint = pathWay.wayPoints[wayPointID];
            transform.position = currentWayPoint.position;
            destinationWayPoint = pathWay.wayPoints[wayPointID + 1];
        }

        private void EnemyMovement()
        {
            if (waitIndex == 0)
            {
                if (transform.position.x != destinationWayPoint.transform.position.x && transform.position.z != destinationWayPoint.transform.position.z)
                {
                    var destinationDir = ((transform.position - destinationWayPoint.transform.position).normalized) * -1;
                    Quaternion rotDir = Quaternion.LookRotation(destinationDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotDir, rotSpeed * Time.deltaTime);

                    Vector3 eulerAngles = transform.rotation.eulerAngles;
                    eulerAngles = new Vector3(0, eulerAngles.y, 0);
                    transform.rotation = Quaternion.Euler(eulerAngles);

                    transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

                    isMoving = true;
                }
            }
                
            else
            {
                isMoving = false;
            }
        }

        private void EnemyAnimations()
        {
            if(isMoving == true && waitIndex == 0)
            {
                anim.SetBool("isWalking", true);
            }

            if(waitIndex > 0)
            {
                anim.SetBool("isWalking", false);
            }
        }

        private void WaitAndDetectionTimer()
        {
            if (fovDrawer.isDetecting)
            {
                waitIndex = 1;
                detectionIndex = detectionIndex + 1 * Time.deltaTime;

                
                fovColorAnim["FovColor"].speed = 0;
                fovColorAnim["FovColor"].time = detectionIndex;
                fovColorAnim.Play("FovColor");


                fovAnim.speed = 0;
                fovColorAnim.Play("FovColor");
                isDetectColor = true;
            }

            else if (waitIndex > 0 && fovDrawer.isDetecting == false && !isGameOver)
            {
                waitIndex = waitIndex - 1 * Time.deltaTime;
                if (waitIndex < 0)
                {
                    waitIndex = 0;
                }
            }

            else
            {
                detectionIndex = detectionIndex - 1 * Time.deltaTime;
                if (detectionIndex < 0)
                {
                    detectionIndex = 0;
                    isDetectColor = false;
                }
                fovAnim.speed = 1;

                
                if (isDetectColor)
                {
                    fovColorAnim["FovColor"].speed = 0;
                    fovColorAnim["FovColor"].time = detectionIndex;
                    fovColorAnim.Play("FovColor");
                }
                

            }
        }

        private void DetectPlayer()
        {
            if (player.isCrouching)
            {
                if (detectionIndex > 4)
                {
                    player.isGameOver = true;
                    playerRB.velocity = new Vector3(0, 0, 0); // stop the velocity of the player after detected
                    playerAnim.Play("idle"); // stop further player action animations after detection
                }
            }
            else
            {
                if (detectionIndex > 3)
                {
                    player.isGameOver = true;
                    playerRB.velocity = new Vector3(0, 0, 0); // stop the velocity of the player after detected
                    playerAnim.Play("idle"); // stop further player action animations after detection
                }
            }
        }

        private void Death()
        {
            if (isDead)
            {
                StartCoroutine(SetDeadBool());
            }
        }
        // ////////////////////////////////////// METHODS

        IEnumerator SetDeadBool()
        {
            yield return new WaitForSeconds(0.7f);
            player.commandText.SetActive(false);
            anim.SetBool("isDead", true);
            fovDrawer.gameObject.SetActive(false);
            rb.isKinematic = true;
            gameObject.GetComponent<SphereCollider>().enabled = false;
            this.enabled = false;
        }
    }
}