using UnityEngine;
using UnityEngine.VR;
using System.Collections;

public class Player : MonoBehaviour
{
    private const float k_Damping = 0.5f;            // The amount of damping applied to the movement of the ship.
    private const float k_ExpDampingCoef = -20f;                // The coefficient used to damp the movement of the flyer.
    private const float k_BankingCoef = 3f;                     // How much the ship banks when it moves.

    public enum InputMovement
    {
        None,
        VRHead,
        MousePhysics,
        KeyboardPhysics
    }

    public PipeSystem pipeSystem;
    public float startVelocity = 2.5f;
    public float maxVelocity = 2.5f;
    public float maneuverSpeed = .6f;
    public float deathCountdown = -1f;
    public float[] accelerations;

    public MainMenu mainMenu;
    public HUD hud;
    public Transform targetMarker;
    public Transform cameraHUD;
    public VRMouseLook mouseLook;

    public InputMovement playerInput = InputMovement.None;

    Pipe currentPipe;
    PlayerAvatar avatar;
    Rigidbody body;
    Transform world;
    Transform rotater;
    float distanceTraveled;
    float deltaRotation;
    float systemRotation;
    float worldRotation;
    float avatarRotation;

    float distanceFromCamera = .65f;

    Vector3 physicsVelocity;
    float savedVelocity;
    float velocity;
    float acceleration;
    bool isDead = false;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        world = pipeSystem.transform.parent;
        rotater = transform.GetChild(0);
        avatar = rotater.GetChild(0).GetComponent<PlayerAvatar>();
        gameObject.SetActive(false);
    }

    // Use this for initialization
    public void StartGame(int acceleration_mode)
    {
        isDead = false;
        acceleration = accelerations[acceleration_mode];
        velocity = startVelocity;
        distanceTraveled = 0;
        systemRotation = 0;
        worldRotation = 0;
        avatarRotation = 0;

        hud.SetValues(distanceTraveled, velocity);

        currentPipe = pipeSystem.SetupFirstPipe();
        SetupCurrentPipe();

        avatar.sperm.SetActive(true);
        gameObject.SetActive(true);
    }

    void SetupCurrentPipe()
    {
        deltaRotation = 360f / (2f * Mathf.PI * currentPipe.CurveRadius);
        worldRotation += currentPipe.RelativeRotation; ;

        if (worldRotation < 0f)
        {
            worldRotation += 360f;
        }
        else if (worldRotation >= 360f)
        {
            worldRotation -= 360f;
        }

        world.localRotation = Quaternion.Euler(worldRotation, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        MouseVRMovement();
        UpdateProperties();
        UpdateWorldRotation();
        UpdatePhysicsAndAvatarProperties();
    }

    void UpdateProperties()
    {
        if (!isDead)
        {
            velocity += acceleration * Time.deltaTime;
            velocity = Mathf.Clamp(velocity, startVelocity, maxVelocity);
        }

        hud.SetValues(distanceTraveled, velocity);
    }

    void UpdateWorldRotation()
    {
        float delta = velocity * Time.deltaTime;
        distanceTraveled += delta * 10;
        systemRotation += delta * deltaRotation;

        if (systemRotation >= currentPipe.CurveAngle)
        {
            delta = (systemRotation - currentPipe.CurveAngle) / deltaRotation;
            currentPipe = pipeSystem.SetupNextPipe(distanceTraveled > pipeSystem.eggDistance);
            SetupCurrentPipe();
            systemRotation = delta * deltaRotation;
        }

        pipeSystem.transform.localRotation = Quaternion.Euler(0f, 0f, systemRotation);
    }

    void UpdatePhysicsAndAvatarProperties()
    {
        if (isDead && deathCountdown >= 0f)
        {
            deathCountdown -= Time.deltaTime;
            Vector3 v = new Vector3((deathCountdown * physicsVelocity.x) / 3f, physicsVelocity.y, physicsVelocity.z);
            body.velocity = v;

            velocity = (deathCountdown * savedVelocity) / 3f;

            if (deathCountdown < 0f)
            {
                deathCountdown = -1f;
                body.velocity = v;
                velocity = savedVelocity;
                avatar.StopParticles();
                GameOver();
            }
        }
    }

    void MouseVRMovement()
    {
        if (playerInput == InputMovement.VRHead)
        {
            Quaternion mouseRotation;
            //Vector3 rot = Vector3.zero;
            #if UNITY_STANDALONE

                    mouseRotation = mouseLook.GetVRHeadRotation();
                    distanceFromCamera = .75f * 2;

            #elif UNITY_EDITOR

            mouseRotation = mouseLook.GetVRHeadRotation();
                    distanceFromCamera = .75f * 2;
            //rot = mouseLook.GetVRHeadRotation().eulerAngles;
            #else
                    mouseRotation = InputTracking.GetLocalRotation(VRNode.Head);
                    //rot = InputTracking.GetLocalRotation(VRNode.Head).eulerAngles;
                    Quaternion q = mouseRotation;
                    mouseRotation.x = q.z;
                    mouseRotation.z = -q.x;             
                    distanceFromCamera = .75f;       
                    //mouseRotation = Quaternion.Euler(q.y, 0f, q.x);
            #endif

            //Debug.Log("Rotation: " + rot);
            targetMarker.position = Camera.main.transform.position + (mouseRotation * Vector3.right) * distanceFromCamera;
            //cameraHUD.position = Camera.main.transform.position + (mouseRotation * Vector3.right) * .5f;

            // Move the flyer towards the target marker.
            transform.position = Vector3.Lerp(transform.position, targetMarker.position,
                k_Damping * (1f - Mathf.Exp(k_ExpDampingCoef * Time.deltaTime)));

            // Calculate the vector from the target marker to the flyer.
            Vector3 dist = transform.position - targetMarker.position;

            // Base the target markers pitch (x rotation) on the distance in the y axis and it's roll (z rotation) on the distance in the x axis.
            //targetMarker.eulerAngles = new Vector3(0f, 0f, 0f) * k_BankingCoef;
            targetMarker.eulerAngles = new Vector3(dist.y, 0f, dist.x) * k_BankingCoef;

            // Make the flyer bank towards the marker.
            transform.rotation = Quaternion.Lerp(transform.rotation, targetMarker.rotation,
                k_Damping * (1f - Mathf.Exp(k_ExpDampingCoef * Time.deltaTime)));
        }
    }

    void FixedUpdate()
    {
        if (!isDead)
        {
            UpdateMovement();
        }
    }

    void UpdateMovement()
    {
        //KeyboardPhysicsMovement ();
        //MousePhysicsMovement();
    }

    void KeyboardPhysicsMovement()
	{
        if (playerInput == InputMovement.KeyboardPhysics)
        {
            float moveHorizontal = Time.deltaTime * -Input.GetAxis("Horizontal");
            float moveVertical = Time.deltaTime * Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(0f, moveVertical, moveHorizontal);

            float force = maneuverSpeed * (50 * body.mass);
            //body.MovePosition(movement * maneuverSpeed);
            //body.position = movement * maneuverSpeed;
            body.AddForce(force * movement);
            //transform.Translate(movement * maneuverSpeed);
        }
    }

	void MousePhysicsMovement()
	{
        if (playerInput == InputMovement.MousePhysics)
        {
            //Quaternion mouseRotation = InputTracking.GetLocalRotation(VRNode.Head);
            Quaternion mouseRotation = mouseLook.GetRotation();
            targetMarker.position = Camera.main.transform.position + (mouseRotation * Vector3.right);

            // Calculate the vector from the target marker to the flyer.
            Vector3 dist = transform.position - targetMarker.position;

            // Base the target markers pitch (x rotation) on the distance in the y axis and it's roll (z rotation) on the distance in the x axis.
            targetMarker.eulerAngles = new Vector3(dist.y, 0f, dist.x) * k_BankingCoef;

            float headRotation = Time.deltaTime * -CheckForGazeRotation();
            float headTilt = Time.deltaTime * CheckForHeadTilt();

            Vector3 movement = new Vector3(0f, headTilt, headRotation);

            float force = maneuverSpeed * (50 * body.mass);
            body.AddForce(force * movement);
        }   
	}

    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("On Collision Enter - " + collision.transform.name);
    }

    void OnCollitionStay(Collision collision)
    {
        Debug.Log("On Collision Stay - " + collision.transform.name);
    }

    void OnCollisionExit(Collision collision)
    {
        //Debug.Log("On Collision Exit - " + collision.transform.name);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (!isDead && deathCountdown < 0f)
        {
            isDead = true;
            avatar.EmittBurst();
            //burst.Play();            

            if (collider.transform.parent.parent.GetComponent<PipeItem>().type == PipeItem.OType.Egg)
            {
                AudioController.Instace.PlaySoundEffect();
                deathCountdown = 0f;
            }
            else
            {
                deathCountdown = 3f;
            }

            physicsVelocity = body.velocity;
            savedVelocity = velocity;
        }
    }

    public void GameOver()
    {
        gameObject.SetActive(false);
        mainMenu.EndGame(distanceTraveled);
    }

    // Move the paddle by rotating your head (around Y axis)
    private float CheckForGazeRotation()
    {
        // Just turning your head left or right from the origin will make the paddle move,
        // so we have a deadzone in the middle to make it stop.
        // TODO: Could check how far we've rotated to scale the speed of movement...
        float yRot = Camera.main.transform.rotation.eulerAngles.y;
        yRot -= 90f;

        if (yRot < 0f)
        {
            yRot += 360f;
        }
        else if (yRot >= 360f)
        {
            yRot -= 360f;
        }

        if (yRot > 5.0f && yRot < 180.0f)
            return 1.0f;
        else if (yRot > 180.0f && yRot < 355.0f)
            return -1.0f;

        return 0.0f;
    }

    // Move the paddle by tilting your head (around Z axis.) Note that you get some Z rotation during Y rotation.
    private float CheckForHeadTilt()
    {
        // Just tilting your head left or right will make the paddle move,
        // so we have a deadzone in the middle to require a more deliberate movement.
        // TODO: Could check how far we've rotated to scale the speed of movement...
        float xRot = Camera.main.transform.rotation.eulerAngles.x;

        if (xRot > 5.0f && xRot < 180.0f)
            return -1.0f;
        else if (xRot > 180.0f && xRot < 355.0f)
            return 1.0f;

        return 0.0f;
    }
}
