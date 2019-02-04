using UnityEngine;
using System.Reflection;
using System.Linq;

namespace Assets {
    public class TowerFactory : MonoBehaviour {
        private readonly Tower _default = new Tower("Tower", "A tower", 30000, 0, 0, 40, 20, 5, -1);//normally 5k health;
        public GameObject friendlyTower, enemyTower;
        public BuildingFactory buildingFactory;
        public TowerInfo tInfo;
        
        private void Start() {
            CreateTower(true,1);
            CreateTower(false,1);
        }

        public void CreateTower(bool friendly, int type) {//1 main tower, 2 mini tower
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            GameObject towerObject = friendly ? friendlyTower : enemyTower;
            towerObject.AddComponent<Tower>();          
            Tower destTower = towerObject.GetComponent<Tower>();

           /*if (type == 1) {//setup the towers gun turret
                buildingFactory.CreateTowerTurrent(friendly);

                destTower.GetComponent<SphereCollider>().enabled = false;
                destTower.towerGun = friendly ? GameObject.FindGameObjectWithTag("FriendlyTowerGun") : GameObject.FindGameObjectWithTag("EnemyTowerGun");
                destTower.towerGun.tag = friendly ? "Friendly" : "Enemy";
                destTower.towerGun.AddComponent<Building>();
                var gun = destTower.towerGun.GetComponent<Building>();
                gun.SetValues("Gun Turret", "A gun turret.", 300, 0, 150, 18, 30, 1.3f, 1);
                destTower.GetComponent<SphereCollider>().enabled = true;
            }*/

            FieldInfo[] fieldInfo = _default.GetType().BaseType.GetFields(flags);
            FieldInfo[] dest = destTower.GetType().BaseType.GetFields(flags);

            foreach (FieldInfo info in fieldInfo) {
                FieldInfo destField = dest.FirstOrDefault(field => field.Name == info.Name);
                destField.SetValue(destTower, info.GetValue(_default));
            }

            towerObject.AddComponent<TowerAgent>();

            if (friendly) {
                tInfo.friendlyTower = destTower;
            } else {
                tInfo.enemyTower = destTower;

            }

        }

    }
}