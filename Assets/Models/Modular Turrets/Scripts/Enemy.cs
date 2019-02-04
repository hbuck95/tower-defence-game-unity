using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
  float speed = 15f;
  public float health = 100;
  public GameObject explosion;

  // Use this for initialization
  void Start ()
  {
    transform.GetComponent<Rigidbody>().velocity = transform.TransformDirection (0, -speed, 0);
  }
	
  // Update is called once per frame
  void Update ()
  {

  }
  
  void Hit (float damage)
  {
    health -= damage;
    if (health <= 0) {
      Instantiate (explosion, transform.position, transform.rotation);
      Destroy (gameObject);
    }
  }
}
