using UnityEngine;
using System.Collections;

public class PipeItem : MonoBehaviour
{
    Transform rotator;
    public enum OType
    {
        Egg,
        None
    }

    public OType type = OType.None;

	// Use this for initialization
	void Awake ()
    {
        rotator = transform.GetChild(0);
	}
	
	// Update is called once per frame
	public void Position (Pipe pipe, float curve_rotation, Vector3 ring_rotation)
    {
        transform.SetParent(pipe.transform, false);
        transform.localRotation = Quaternion.Euler(0f, 0f, -curve_rotation);
        rotator.localPosition = new Vector3(0f, pipe.CurveRadius);
        rotator.localRotation = Quaternion.Euler(ring_rotation);
	}
}
