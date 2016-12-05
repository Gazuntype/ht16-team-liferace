using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Text distanceLabel, velocityLabel;

    // Update is called once per frame
    public void SetValues(float distance_traveled, float velocity)
    {
        distanceLabel.text = ((int)(distance_traveled)).ToString();
        velocityLabel.text = ((int)(velocity * 10f)).ToString();
    }
}
