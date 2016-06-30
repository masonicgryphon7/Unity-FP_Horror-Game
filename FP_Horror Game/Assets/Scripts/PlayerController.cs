using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public float inputX, inputY;
    private CharacterController controller;

    [Header("Player References")]
    public GameObject cameraGO;
    public float crouchSpeed = 2f;
    public float walkSpeed = 4f;
    public float runSpeed = 6f;
    public float jumpSpeed = 6f;
    public float gravity = 24f;

    [Header("Player Visual Configuration")]
    public Vector3 moveDirection, currentPosition;
    public int state; //0 = stand   1 = crouch
    public float normalHeight = -.2f, crouchHeight = -1f, crouchProneSpeed = 2f;
    public float speed, velMagnitude;
    public bool grounded, isRunning, isCrouching, isFalling;
    [HideInInspector]
    public bool canMove = true, canJump = true, canCrouch = true;

    private Vector3 lastPosition;
    private float rayDistance, slideLimit, fallDistance, highestPoint, distanceToObstacle;
    private bool didJump, sliding;

    void Start()
    {
        controller = this.GetComponent<CharacterController>();
        rayDistance = controller.height / 2 + 1.1f;
        slideLimit = controller.slopeLimit - .2f;
        speed = walkSpeed;
    }

    void Update()
    {
        velMagnitude = controller.velocity.magnitude;

        moveDirection = transform.TransformDirection(moveDirection);

        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");
        float inputModifyFactor = (inputX != 0f && inputY != 0f) ? .7071f : 1f;

        if (grounded) //IF GROUNDED
        {
            didJump = false;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, -Vector3.up, out hit, rayDistance))
            {
                float hitangle = Vector3.Angle(hit.normal, Vector3.up);
                if (hitangle > slideLimit)
                    sliding = true;
                else
                    sliding = false;
            }

            if (cameraGO.transform.localPosition.y > normalHeight - .1f && grounded)
            {
                if (Input.GetButton("Run") && inputY > 0)
                    isRunning = true;
                else
                    isRunning = false;
            }

            if (isFalling)
            {
                isFalling = false;
                fallDistance = highestPoint - currentPosition.y;
            }

            if (sliding)
            {
                Vector3 hitNormal = hit.normal;
                moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                Vector3.OrthoNormalize(ref hitNormal, ref moveDirection);
                moveDirection *= 12f;
            }
            else
            {
                if (state == 0)
                {
                    isCrouching = false;
                    if (isRunning)
                        speed = runSpeed;
                    else
                        speed = walkSpeed;
                }
                else if (state == 1)
                {
                    speed = crouchSpeed;
                    isRunning = false;
                    isCrouching = true;
                }

                if (Cursor.lockState == CursorLockMode.Locked && canMove) moveDirection = new Vector3(inputX * inputModifyFactor, -.75f, inputY * inputModifyFactor);
                else moveDirection = new Vector3(0, -.75f, 0);
                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection *= speed;

                if (canMove && gravity > 0)
                    if (Input.GetButtonDown("Jump"))
                    {
                        if (state == 0)
                            if (canJump)
                                moveDirection.y = jumpSpeed;

                        if (state > 0)
                        {
                            CheckDistance();
                            if (distanceToObstacle > 1.6f)
                                state = 0;
                        }
                    }
            }
        }
        else //ELSE IF I'M NOT GROUNDED
        {
            //Calculate highest point when we jump
            currentPosition = transform.position;

            if (currentPosition.y > lastPosition.y)
            {
                highestPoint = transform.position.y;
                isFalling = true;
            }

            //Save fall start point
            if (!isFalling)
            {
                highestPoint = transform.position.y;
                isFalling = true;
            }

            if (gravity > 0)
            {
                if (inputX != 0 || inputY != 0)
                {
                    moveDirection.x = inputX;
                    moveDirection.z = inputY * speed;
                    moveDirection = transform.TransformDirection(moveDirection);
                }
            }
        }
        //END IF NOT GROUNDED

        if (Input.GetButtonDown("Crouch") && canCrouch)
        {
            CheckDistance();

            if (state == 0)
                state = 1;
            else if (state == 1)
            {
                if (distanceToObstacle > 1.6f)
                    state = 0;
            }
        }

        if (state == 0 && gravity > 10f) //Standing Position
        {
            controller.height = 2.5f;
            controller.center = new Vector3(0, 0.07f, 0);

            if (cameraGO.transform.localPosition.y > normalHeight)
                cameraGO.transform.localPosition = new Vector3(cameraGO.transform.localPosition.x, normalHeight, cameraGO.transform.localPosition.z);//cameraGO.transform.localPosition.y = normalHeight;
            else if (cameraGO.transform.localPosition.y < normalHeight)
            {
                float yP = cameraGO.transform.localPosition.y;
                yP += Time.deltaTime * crouchProneSpeed;
                cameraGO.transform.localPosition = new Vector3(cameraGO.transform.localPosition.x, yP, cameraGO.transform.localPosition.z);
            }

        }
        else if (state == 1) //Crouch Position
        {
            controller.height = 1.3f;
            controller.center = new Vector3(0, -0.5f, 0);

            if (cameraGO.transform.localPosition.y != crouchHeight)
            {
                if (cameraGO.transform.localPosition.y > crouchHeight)
                {
                    float yP = cameraGO.transform.localPosition.y;
                    yP -= Time.deltaTime * crouchProneSpeed;
                    cameraGO.transform.localPosition = new Vector3(cameraGO.transform.localPosition.x, yP, cameraGO.transform.localPosition.z);
                }
                if (cameraGO.transform.localPosition.y < crouchHeight)
                {
                    float yP = cameraGO.transform.localPosition.y;
                    yP += Time.deltaTime * crouchProneSpeed;
                    cameraGO.transform.localPosition = new Vector3(cameraGO.transform.localPosition.x, yP, cameraGO.transform.localPosition.z);
                }
            }
        }

        // Apply gravity
        if (gravity > 0)
            moveDirection.y -= gravity * Time.deltaTime;

        grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
    }

    //So we don't stand up where there's no floor up
    void CheckDistance()
    {
        Vector3 pos = transform.position + controller.center - new Vector3(0, controller.height / 2, 0);
        RaycastHit hit;
        if (Physics.SphereCast(pos, controller.radius, transform.up, out hit, 10))
        {
            distanceToObstacle = hit.distance;
            Debug.DrawLine(pos, hit.point, Color.red, 2.0f);
        }
        else
            distanceToObstacle = 3;
    }
}
