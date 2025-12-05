using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class IgnoreCollisionWithLayers : MonoBehaviour
{
    [Header("Configurações de Colisão")]
    [SerializeField] private LayerMask collisionLayersToIgnore = ~0;

    private Collider2D objectCollider;

    private void Awake()
    {
        objectCollider = GetComponent<Collider2D>();

        IgnoreCollisionsWithLayers(true);
    }

    private void IgnoreCollisionsWithLayers(bool ignore)
    {
        for (int i = 0; i < 32; i++)
        {
            if ((collisionLayersToIgnore.value & (1 << i)) != 0)
            {
                Physics2D.IgnoreLayerCollision(gameObject.layer, i, ignore);
            }
        }
    }

    public void SetIgnoreCollisions(bool ignore)
    {
        IgnoreCollisionsWithLayers(ignore);
    }
}