using UnityEngine;
using System.Collections;

public class UIController : MonoBehaviour {
    public GameObject Raycaster;
    RaycastHit HitInfo;
    public GameObject LookBar;
    float dTime;
    bool looking;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        dTime = Time.deltaTime/100;
        Debug.DrawLine(Raycaster.transform.position, transform.forward, Color.red, 1000f);
        if (Physics.Raycast(Raycaster.transform.position, transform.forward, out HitInfo, 500f))
        {
           
            if (HitInfo.transform.name.Contains("Play"))
            {
                print("PlayGame");
                if (!looking)
                {
                    looking = true;
                    StartCoroutine(PlayGame());
                  
                }
                
            }
            else
            {
                print("Now False");
                looking = false;
            }
        }
	}
    IEnumerator PlayGame()
    {
        LookBar.GetComponent<Animator>().SetTrigger("Scale");
        yield return new WaitForSeconds(2f);
        if (looking)
        {
            MainMenu.Instance.PlayGame();
        }
    }
}
