using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drug : MonoBehaviour
{
    public float boost = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision other)// Picked up a drug
    {
        if (!other.transform.name.Equals("Player")) return;
        other.transform.GetComponent<PlayerMovement>().health = Mathf.Min(other.transform.GetComponent<PlayerMovement>().maxHealth, other.transform.GetComponent<PlayerMovement>().health + boost);
        Debug.Log("Player picked up a drug");
        Destroy(gameObject);
    }
}
