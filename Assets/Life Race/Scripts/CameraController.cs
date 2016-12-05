using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public GameObject playerObject;
    public Rect customCameraBounds;
    public GameObject vrCursor;

    Player.InputMovement playerInput;
    Vector3 offset;

    Rect vrHeadMovementCameraBounds = new Rect(-0.2f, -0.2f, 0.2f, 0.2f);
    Rect mousephysicsMovementCameraBounds = new Rect(-0.2f, -0.2f, 0.2f, 0.2f);
    Rect keyboardphysicsMovementCameraBounds = new Rect(-0.25f, -0.7f, 0.25f, 0.6f);

    // Use this for initialization
    void Start ()
    {
        if(Application.platform == RuntimePlatform.Android)
            vrCursor.SetActive(true);
        else
            vrCursor.SetActive(false);

        if (playerObject == null)
        {
            Debug.LogError("PlayerObject game Object reference missing", this);
        }

        offset = transform.position - playerObject.transform.position;

        playerInput = playerObject.GetComponent<Player>().playerInput;
        switch (playerInput)
        {
            case Player.InputMovement.VRHead:
                {
                    customCameraBounds = vrHeadMovementCameraBounds;
                }
                break;

            case Player.InputMovement.MousePhysics:
                {
                    customCameraBounds = mousephysicsMovementCameraBounds;
                }
                break;

            case Player.InputMovement.KeyboardPhysics:
                {
                    customCameraBounds = keyboardphysicsMovementCameraBounds;
                }
                break;
        }
	}
	
	// Update is called once per frame
	void LateUpdate ()
    {
        Vector3 newPosition = playerObject.transform.position + offset;
        newPosition.y = Mathf.Clamp(newPosition.y, customCameraBounds.y, customCameraBounds.height);
        newPosition.z = Mathf.Clamp(newPosition.z, customCameraBounds.x, customCameraBounds.width);
        transform.position = new Vector3(transform.position.x, newPosition.y, newPosition.z);
    }
}
