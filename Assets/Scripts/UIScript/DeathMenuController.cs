using UnityEngine;

public class DeathMenuController : MonoBehaviour
{
    public GameObject DeathMenu;
    void Awake()
    {
        DeathMenu.SetActive(false);
    }

    void OnEnable()
    {
        DeathMenu.SetActive(false);
    }
    
    public void ShowMenu()
    {
        DeathMenu.SetActive(true);
    }

    void OnDisable()
    {
        DeathMenu.SetActive(false);
    }
}
