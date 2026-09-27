using System.Collections;
using UnityEngine;

public class EntityFlash : MonoBehaviour
{
    [SerializeField]private Material impactMaterial;
    [SerializeField]private float impactTime;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();    
    }

    private IEnumerator ImpactFlash()
    {
        var originalMaterial = spriteRenderer.material;
        spriteRenderer.material = impactMaterial;
        yield return new WaitForSeconds(impactTime);
        spriteRenderer.material = originalMaterial;
    }

    public void ExecuteImpact()
    {
        StartCoroutine(ImpactFlash());
    }
}
