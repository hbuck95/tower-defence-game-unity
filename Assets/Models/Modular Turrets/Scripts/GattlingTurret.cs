using UnityEngine;
using System.Collections;

namespace Assets
{
    public class GattlingTurret : GunTurret
    {
        public Renderer[] muzzlesFires;

        override protected void Awake()
        {
            base.Awake();
            foreach (Renderer r in muzzlesFires)
            {
               // r.enabled = false;
            }
        }

        /*override protected void OnTriggerExit(Collider t)
        {
            base.OnTriggerExit(t);
            if (target == null)
            {
                foreach (Renderer r in muzzlesFires)
                    r.enabled = false;
            }
        }

        override protected void Update()
        {
            base.Update();
            if (target == null)
            {
                foreach (Renderer r in muzzlesFires)
                {
                    r.enabled = false;
                }
            }
        }*/
        // Update is called once per frame

        /* override protected void FireProjectile()
         {
             foreach (Renderer r in muzzlesFires)
             {
                 r.enabled = true;
             }
             //nextFireTime = Time.time + reloadTime;
             nextMoveTime = Time.time + firePauseTime;

             foreach (Transform m in muzzlePositions)
             {
                 Instantiate(projectile, m.position, m.rotation);
             }

         }*/

        /*override protected IEnumerator FireProjectile() {
            //nextFireTime = Time.time + reloadTime;
            nextMoveTime = Time.time + firePauseTime;
            building.Attack(targets[0]);
            Debug.Log("Fire Projectile");

            foreach (Transform t in muzzlePositions) {
                Instantiate(projectile, t.position, t.rotation);
                Instantiate(muzzleFire[0], t.position, t.rotation);               
            }

            yield return new WaitForEndOfFrame();
        }*/
    }
}