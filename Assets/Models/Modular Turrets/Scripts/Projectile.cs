using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{

  public float speed = 50;
  public float range = 50;
  public float damage = 1;
  private float currentDistance = 0;
    public string tagCheck;
  // Use this for initialization
  void Start ()
  {
	
  }
	
  // Update is called once per frame
  void Update ()
  {

    transform.Translate (Vector3.forward * Time.deltaTime * speed);
    currentDistance += Time.deltaTime * speed;
   
    if (currentDistance >= range) {
      if (transform.parent) {
        Destroy (transform.parent.gameObject);
      }
      Destroy (gameObject);
     
    }
    
  }
  
  void OnTriggerEnter (Collider c)
  {
    if (c.gameObject.tag == tagCheck) {
      Destroy (gameObject);
      //c.gameObject.SendMessage ("Hit", damage, SendMessageOptions.DontRequireReceiver);
    }
    
  }
}
