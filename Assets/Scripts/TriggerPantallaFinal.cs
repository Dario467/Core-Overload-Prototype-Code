using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerPantallaFinal : MonoBehaviour
{
    [Header("Interfaz Visual")]
    [Tooltip("Arrastra aquí tu GameObject 'ImagenFinal' o el Panel que contiene la imagen y el botón")]
    public GameObject imagenYBotones; 
    
    [Tooltip("Arrastra aquí el GameObject 'SlideryDash' para desactivarlo cuando el jugador toque el collider")]
    public GameObject slideryDashUI;

    [Header("Configuración de Colisión")]
    [Tooltip("El Tag del objeto que activará el trigger (usualmente 'Player')")]
    public string tagJugador = "Player";

    private void Start()
    {
        if (imagenYBotones != null)
        {
            imagenYBotones.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(tagJugador))
        {
            MostrarPantallaFinal();
        }
    }

    private void MostrarPantallaFinal()
    {
        Time.timeScale = 0f;

        if (slideryDashUI != null)
        {
            slideryDashUI.SetActive(false);
        }

        if (imagenYBotones != null) 
        {
            imagenYBotones.SetActive(true);
        }
    }

    public void VolverAlMenu()
    {
        Time.timeScale = 1f;

        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene("MenuPrincipal");
        }
        else
        {
            SceneManager.LoadScene("MenuPrincipal");
        }
    }
}