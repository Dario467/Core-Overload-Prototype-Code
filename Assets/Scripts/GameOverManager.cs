using System.Collections; 
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance{ get; private set; }
    [Header("Elementos Visuales")]
    public GameObject panelNegro;     
    public GameObject objetoAnimacion; 

    [Header("Audio")]
    public AudioSource audioExplosion;
    
    [Header("Tiempos de la Secuencia")]
    [Tooltip("Segundos que la pantalla se queda 100% en negro")]
    public float tiempoPantallaNegra = 1f; 
    [Tooltip("Segundos que dura la animación antes de mostrar los botones")]
    public float tiempoDuracionAnimacion = 1.5f; 

    void Awake()
    {
       if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            objetoAnimacion.SetActive(false);
            panelNegro.SetActive(false);
            SceneManager.sceneLoaded += DisableGameOverMenu;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void ActivarMenuMuerte()
    {
        StartCoroutine(SecuenciaCinematografica());
    }

    private IEnumerator SecuenciaCinematografica()
    {
        // Congelamos el juego
        Time.timeScale = 0f;

        panelNegro.SetActive(true);

        // --- CORRECCIÓN: La explosión suena AQUÍ, junto con la pantalla negra ---
        if (audioExplosion != null) audioExplosion.Play();

        // PASO 2: Esperamos en la oscuridad
        yield return new WaitForSecondsRealtime(tiempoPantallaNegra);

        // PASO 3: Arranca la animación
        if (objetoAnimacion != null) 
        {
            objetoAnimacion.SetActive(true);
        }
    }

    public void ReiniciarNivel()
    {
        Time.timeScale = 1;
        GameManager.Instance.RestartLevel();
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

    private void DisableGameOverMenu(Scene scene, LoadSceneMode mode)
    {
        objetoAnimacion.SetActive(false);
        panelNegro.SetActive(false);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= DisableGameOverMenu;
    }
}