//This class is to allow for easier management of the units individual weapons.
//This class is needed as the unit class and unitagent class are created and added to the unit at runtime.

//Originally the plan was to fully animate the weapons along with the unit, however a lot of issues occured when importing the unit models into 3ds MAX
//Now the gun models are attached to the unit model via hierarchy, and are attached the main bone used for the attacking animation.

using UnityEngine;

public class UnitWeapon : MonoBehaviour {
    public GameObject weapon;

    public void Show() {
        weapon.SetActive(true);
    }

    public void Hide() {
        weapon.SetActive(false);
    }
}
