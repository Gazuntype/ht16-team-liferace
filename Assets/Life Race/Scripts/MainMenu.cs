using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;

    public GameObject world;
    public GameObject hud;
    public Player player;
    public Text scoreLabel;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        InputTracking.Recenter();
        gameObject.SetActive(true);
        hud.SetActive(false);
        world.SetActive(false);
        AudioController.Instace.PlayMenuSound();
    }
    // Use this for initialization
    public void PlayGame()
    {
        gameObject.SetActive(false);
        world.SetActive(true);
        player.gameObject.SetActive(true);
        hud.SetActive(true);
        player.StartGame(0);
        AudioController.Instace.PlayGameSound();
    }

    public void EndGame(float distance_traveled)
    {
        InputTracking.Recenter();
        scoreLabel.text = ((int)(distance_traveled)).ToString();
        gameObject.SetActive(true);
        world.SetActive(false);
        hud.SetActive(false);
        AudioController.Instace.PlayMenuSound();
    }
}
