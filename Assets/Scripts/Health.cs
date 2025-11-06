using UnityEngine;

public class Health : MonoBehaviour
{

    public float maxHP = 50f;
    float hp;

    void Awake() => hp = maxHP;

    public void TakeDamage(float amount)
    {
        hp -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. HP left: {hp}");
        if(hp <= 0){
            Debug.Log($"{gameObject.name} destroyed!");
            Destroy(gameObject);
        } 

    }

    
}
