using UnityEngine;
using System.Linq;
using System.Reflection;

namespace Assets {
    public class BuildingFactory : MonoBehaviour {
        //string name, string desc, int maxHealth, int levelReq, int cost, int maxDamage, float attackRange, float attackSpeed
        private static readonly Building GunTurret = new Building("Gun Turret", "A gun turret.", 300, 0, 150, 18, 30, 1.3f, 1);
        private static readonly Building LaserTurret = new Building("Laser Turret", "A laser turret.", 500, 5, 250, 25, 40, 1.6f, 2);
        private static readonly Building MissileTurret = new Building("Missile Turret", "A devistatingly powerful missile launching turret.", 1000, 15, 500, 65, 25, 2.5f, 3);
        private static readonly Building EnergyShield = new Building("Energy Barrier", "An energy barrier.", 500, 10, 350, 7, 35, -1, 4);
        public GameObject[] friendlyprefabs = new GameObject[4];
        public GameObject[] enemyPrefabs;
        public AudioClip[] clips;
        public AudioClip explode;
        private int _collisionLayer;
        private Vector3 _friendlyTowerTurretLoc = new Vector3(37.25f, 28.5f, 140.25f);
        private Vector3 _towerTurrentRot = new Vector3(270f, 270f, 0f);
        private Vector3 _enemyTowerTurretLoc = new Vector3(307.5f, 28.5f, 137.3f);
        public Transform enemyBuildingTest;

        private void Start() {
            _collisionLayer = 1 << LayerMask.NameToLayer("BuildingPlacement");
            //CreateBuilding(2, false, enemyBuildingTest.position, BuildingType.Defense);
        }

        private Building GetBuilding(int type) {
            switch (type) {
                case 1:
                    return GunTurret;
                case 2:
                    return LaserTurret;
                case 3:
                    return MissileTurret;
                case 4:
                    return EnergyShield;
                default:
                    return GunTurret;
            }
        }

        public enum BuildingType {
            TowerTurret, Defense
        }

        private GameObject GetObject(int type, bool friendly) {
            return friendly ? friendlyprefabs[type - 1] : enemyPrefabs[type - 1];
        }

        private bool LegalPosition(Vector3 v) {
            return Physics.OverlapSphere(v, 4, LayerMask.GetMask("CollisionCheck")).Length == 0 && Physics.OverlapSphere(v, 4, _collisionLayer).Length != 0;
        }

        public void CreateTowerTurrent(bool friendly) {
            Vector3 spawn = friendly ? _friendlyTowerTurretLoc : _enemyTowerTurretLoc;
            Player player = friendly ? Player.human : Player.cpu;
            CreateBuilding(1, friendly, spawn, BuildingType.TowerTurret);
            player.DestroyBuilding(1);//tower buildings do not count towards the players building limit.            
        }

        private AudioClip GetAttackSFX(int type) {
            switch (type) {
                case 1:
                    return clips[0];
                case 2:
                    return clips[1];
                case 3:
                    return clips[2];
                default:
                    return clips[0];
            }

        }

        public void CreateBuilding(int type, bool friendly, Vector3 spawn, BuildingType buildingType) {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            Building building = GetBuilding(type);
            Player player = friendly ? Player.human : Player.cpu;

            if (player.PlayerId == 1) {

                if (player.Credits - building.Cost < 0) {
                    Debug.Log("You don't have enough credits to purchase this unit!");
                    return;
                }

                if (player.DeployedBuildings >= player.MaxBuildings) {
                    Debug.Log(string.Format("The maximum amount of buildings you can deploy is {0}.\nYou currently have {1} buildings deployed and cannot spawn any more.", player.MaxBuildings, player.DeployedBuildings));
                    return;
                }

                if (type != 4) {
                    if (!LegalPosition(spawn)) {
                        Debug.Log("Illegal Position. This spot conflicts with another placed building.");
                        return;
                    }
                }
            }

            GameObject obj = Instantiate(GetObject(type, friendly), spawn, Quaternion.Euler(new Vector3(-90f,0,0))) as GameObject;
            obj.tag = friendly ? "Friendly" : "Enemy";
            obj.AddComponent<Building>();          
            obj.GetComponentInChildren<Light>().color = friendly ? OptionsMenu.FriendlyUnitColour : OptionsMenu.EnemyUnitColour;
            Building destBuilding = obj.GetComponent<Building>();
            FieldInfo[] fieldInfo = building.GetType().GetFields(flags);
            FieldInfo[] dest = destBuilding.GetType().GetFields(flags);

            foreach (FieldInfo info in fieldInfo) {
                FieldInfo destField = dest.FirstOrDefault(field => field.Name == info.Name);
                destField.SetValue(destBuilding, info.GetValue(building));
            }
       
            //Configure attacking radius' for each building
            if (type == 4) {//Energy Barrier                
                obj.transform.localScale = new Vector3(destBuilding.AttackRange, destBuilding.AttackRange, destBuilding.AttackRange);
                obj.transform.position.Set(transform.position.x, 0, transform.position.y);
            } else {
                obj.GetComponent<SphereCollider>().radius = destBuilding.AttackRange;
            }

            destBuilding.explodeSFX = explode;
            destBuilding.attackSFX = GetAttackSFX(type);

            switch (type) {
                case 1:
                    player.gunTurretCount++;
                    break;
                case 2:
                    player.laserTurretCount++;
                    break;
                case 3:
                    player.missileLauncherCount++;
                    break;
                case 4:
                    player.energyBarrierCount++;           
                    break;
            }
            
            player.DeployBuilding();
            player.SpendCredits(building.Cost);
        }

    }
}
