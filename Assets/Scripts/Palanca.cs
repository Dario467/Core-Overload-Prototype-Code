using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Palanca : MonoBehaviour
{
    [Header("Configuración")]
    public bool isActive = false;
    
    [Tooltip("Arrastra aquí el objeto de la puerta que esta palanca va a abrir")]
    public Puerta puertaAsignada; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerHitbox"))
        {
            GolpearPalanca();
        }
    }

    private void GolpearPalanca()
    {
        isActive = !isActive;

        if (puertaAsignada != null)
        {
            puertaAsignada.CambiarEstado(isActive);
        }
        else
        {
            Debug.LogWarning("A esta palanca le falta una puerta asignada");
        }
    }
}