using UnityEngine;

namespace Assets {
    public class Tower : Building {
        public GameObject towerGun;
        public Tower(string name, string desc, int maxHealth, int levelReq, int cost, int maxDamage, float attackRange, float attackSpeed, int buildingType) : base(name, desc, maxHealth, levelReq, cost, maxDamage, attackRange, attackSpeed, buildingType) {
            Name = name;
            Description = desc;
            MaxHealth = MaxHealth;
            LevelRequirement = levelReq;
            Cost = cost;
            MaxDamage = maxDamage;
            AttackRange = attackRange;
            state = State.New;
            buildingType = -1;
        }

        public bool Destroyed { get; set; }
        private State state;


        private enum State {
            Destroyed,
            HeavilyDamaged,
            ModeratelyDamaged,
            LightlyDamaged,
            New
        }

        private string getState() {
            return state.ToString();
        }

        public override void TakeDamage(int damage) {
            base.TakeDamage(damage);
            checkState();
        }

        public override void Attack(Unit target) {
            base.Attack(target);
        }
        
        private void checkState() {
            if(CurrentHealth > 0 && CurrentHealth < MaxHealth * 0.25) {
                state = State.HeavilyDamaged;
            } else if(CurrentHealth < MaxHealth * 0.5) {
                state = State.ModeratelyDamaged;
            } else if (CurrentHealth < MaxHealth * 0.75) {
                state = State.LightlyDamaged;
            } else {
                state = State.New;
            }
        }














        /*
        private int currentHealth, maxHealth;   
        private int state;
        private GameObject towerObject;
        private GameObject[] stateObjects = new GameObject[4];
        private Vector3 location;
        private bool destroyed;

        public oTower(int health, GameObject towerObject) {
            this.currentHealth = health;
            this.maxHealth = health;
            this.towerObject = towerObject;
            destroyed = false;
            state = (int)States.New;
            location = towerObject.transform.position;
            if (towerObject.name == "EnemyTower") {
                GameObject.Find("EnemyT").GetComponent<Text>().text = "Enemy Tower:  Health: " + currentHealth + "/" + maxHealth + "  State: " + (States)state;
            } else {
                GameObject.Find("FriendlyT").GetComponent<Text>().text = "Friendly Tower:  Health: " + currentHealth + "/" + maxHealth + "  State: " + (States)state;
            }
        }

        public enum States {
            Destroyed = 5,
            HeavilyDamaged = 4,
            ModeratelyDamaged = 3,
            LightlyDamaged = 2,
            New = 1
        }

        public int CurrentHealth {
            get { return currentHealth; }
            set { currentHealth = value; }
        }

        public int State {
            get { return state; }
            set { state = value;  }
        }

        public GameObject TowerObject {
            get { return towerObject; }
        }

        public Vector3 Location {
            get { return location; }
        }

        public bool Destroyed {
            get { return destroyed; }
        }

        public void ApplyDamage(int damage) {
            currentHealth -= damage;
            Debug.Log("Applying " + damage + " damage to " + towerObject.name + ".");
            Debug.Log("Health Before: " + (currentHealth + damage) + "  Health Now: " + currentHealth);

            if (currentHealth < 0) {
                currentHealth = 0;
                destroyed = true;
                state = (int)States.Destroyed;
            } else if (currentHealth <= maxHealth * 0.25){
                state = (int)States.HeavilyDamaged;
            } else if (currentHealth <= maxHealth * 0.50) {
                state = (int)States.ModeratelyDamaged;
            } else if (currentHealth <= maxHealth * 0.75) {
                state = (int)States.LightlyDamaged;
            } else {
                state = (int)States.New;
            }
            Debug.Log(towerObject.name+" state is " + (States)state);
            if (towerObject.name == "EnemyTower")
            {
                GameObject.Find("EnemyT").GetComponent<Text>().text = "Enemy Tower:  Health: " + currentHealth + "/" + maxHealth + "  State: " + (States)state;
            }
            else
            {
                GameObject.Find("FriendlyT").GetComponent<Text>().text = "Friendly Tower:  Health: " + currentHealth + "/" + maxHealth + "  State: " + (States)state;
            }

        }*/

    }
}