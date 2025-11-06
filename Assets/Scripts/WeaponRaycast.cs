using UnityEngine;

public class WeaponRaycast : MonoBehaviour
{
    public float fireRate = 50f;
    public float damage = 10f;
    public float range = 60f;
    public LayerMask hitMask = ~0;

    float nextFireTime;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0) && Time.time >= nextFireTime){
            nextFireTime = Time.time + 1f / fireRate;
            FireOnce();
        }
        
    }

    void FireOnce()
    {
        // Ray from screen center
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore))
        {
            // Apply damage if object has Health
            if (hit.collider.TryGetComponent<Health>(out var health))
                health.TakeDamage(damage);

            // Quick hit feedback
            Debug.DrawLine(ray.origin, hit.point, Color.red, 0.1f);
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * range, Color.gray, 0.1f);
        }
    }
    
}
