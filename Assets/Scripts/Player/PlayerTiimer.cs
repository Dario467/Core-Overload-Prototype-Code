using System;
using UnityEngine;
using UnityEngine.InputSystem; 
using UnityEngine.UI;
using System.Collections; 

public class PlayerTimer : MonoBehaviour 

{
    [Header("Configuración de Tiempo")]
    public float tiempoMaximoVida = 20f; 
    public float tiempoADescontar = 1f;
    private float tiempoTranscurrido;
    [SerializeField] private float dangerZonePercentage = 0.8f; 
    public static event Action<float, float> UpdateUITimer;

    [Header("Estado de la Zona")]
    public bool estaEnZonaDebilitante = false; 
    public bool puedeMoverse = true; 

    [Header("Referencias de UI")]
    public Slider slider; 
    public Animator dangerAnimator;
    
    private float dangerZone;

    public void Start()
    {
        tiempoTranscurrido = 0f;
        dangerZone = tiempoMaximoVida * dangerZonePercentage; 

        if (slider != null)
        {
            slider.minValue = 0f;
            slider.maxValue = tiempoMaximoVida;
            slider.value = tiempoTranscurrido;
        }
        ActualizarUI();
    }
    
    public void Update()
    {
        if (!estaEnZonaDebilitante && tiempoTranscurrido < tiempoMaximoVida)
        {
            tiempoTranscurrido += Time.deltaTime;
            ActualizarUI();
        }

        if (tiempoTranscurrido >= tiempoMaximoVida)
        {
            tiempoTranscurrido = tiempoMaximoVida; 
            ActualizarUI(); 
            Morir();
        }
    }

    public void RestarTiempo(float cantidad)
    {
        if (tiempoTranscurrido > 0) 
        {
            tiempoTranscurrido -= cantidad;
            
            if (tiempoTranscurrido <= 0)
            {
                tiempoTranscurrido = 0;
                StartCoroutine(MomentoDebilidad()); 
            }
            ActualizarUI(); 
        }
    }

    public void ResetTime()
    {
        tiempoTranscurrido = 0;
        ActualizarUI(); 
    }

    IEnumerator MomentoDebilidad()
    {
        puedeMoverse = false;
        
        yield return new WaitForSeconds(0.8f); 

        puedeMoverse = true;
    }


   

    private void ActualizarUI()
    {
        UpdateUITimer?.Invoke(tiempoTranscurrido, tiempoMaximoVida);
        if (slider != null) slider.value = tiempoTranscurrido;
        if (dangerAnimator != null)
        {
            bool isDanger = tiempoTranscurrido >= dangerZone;
            dangerAnimator.SetBool("Danger", isDanger);
        }
    }


    public void VaciarTiempoTotalmente()
    {
        tiempoTranscurrido = 0f; 
        ActualizarUI();
        
        StartCoroutine(MomentoDebilidad());
    }

 
    private void Morir() 
{ 

    GameOverManager.Instance.ActivarMenuMuerte();

    gameObject.SetActive(false); 
}
}