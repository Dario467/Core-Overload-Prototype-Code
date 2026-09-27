using UnityEngine;

public class Puerta : MonoBehaviour
{
    [Header("Estado de la Puerta")]
    public bool isOpen = false;

    private void Awake()
    {
        ActualizarPuerta(); 
    }

    public void CambiarEstado(bool nuevoEstado)
    {
        isOpen = nuevoEstado;
        ActualizarPuerta();
    }

    private void ActualizarPuerta()
    {
   
        gameObject.SetActive(!isOpen); 
    }
}