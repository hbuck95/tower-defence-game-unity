//@Script - Handles the player selecting units and buildings on the battlefield. One method, called once, rather than in each units update.

using UnityEngine;

namespace Assets {

    public class SelectionHandler : MonoBehaviour {
        private Unit _selectedUnit;
        private Building _selectedBuilding;
        private BuildingFactory _buildingFactory;
        public int chosenBuilding;
        public bool selectingBuildingLocation;
        public UpgradeMenu upgradeMenu;
        private int _collisionLayer;

        private void Start() {
            _buildingFactory = GameObject.Find("ScriptHolder").GetComponent<BuildingFactory>();
            _collisionLayer = 1 << LayerMask.NameToLayer("Default");
        }

        private void DirectUnit(Ray ray) {
            Plane plane = new Plane(Vector3.up, 0);
            UnityEngine.AI.NavMeshHit hit;
            float dist = Camera.main.transform.position.y;
            plane.Raycast(ray, out dist);
            UnityEngine.AI.NavMesh.SamplePosition(ray.GetPoint(dist), out hit, 1, UnityEngine.AI.NavMesh.AllAreas);

            if (hit.hit) {
                _selectedUnit.GetComponent<UnitAgent>().SetDestination(hit.position);
                Debug.Log(string.Format("Directing {1} to {0}", hit.position, _selectedUnit.Name));              
            } else {
                Debug.Log("Invalid Position");
            }
            _selectedUnit = null;
        }


        private void Update() {
            if (Input.GetMouseButtonDown(0)) {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, _collisionLayer)) {
                    var selection = hit.transform.gameObject;

                    //Placing a building in the environment
                    if (selectingBuildingLocation) {                      
                        Plane plane = new Plane(Vector3.up, 0);
                        float dist = Camera.main.transform.position.y;
                        if (plane.Raycast(ray, out dist)) {
                            _buildingFactory.CreateBuilding(chosenBuilding, true, ray.GetPoint(dist), BuildingFactory.BuildingType.Defense);
                            selectingBuildingLocation = false;
                            return;
                        }
                    }                
                    
                    if (selection.name.Contains("Unit")) {
                        TargetUnit(selection.GetComponent<Unit>());
                        return;
                    }

                    if(selection.name.Contains("Building")) {
                        TargetBuilding(selection.GetComponent<Building>());
                        return;
                    }
            
                    if (selection.name.Contains("Tower")) {
                        TargetBuilding(selection.GetComponent<Tower>());
                        return;
                    }

                    if (_selectedUnit != null && _selectedUnit.tag == "Friendly") {
                        DirectUnit(ray);
                        return;
                    }
                }
            }

            if(Input.GetMouseButtonDown(1)) {
                if (selectingBuildingLocation) {
                    selectingBuildingLocation = false;
                    Debug.Log("Cancelled building placement.");
                }
            }
        }

        private void TargetUnit(Unit u) {
            if(u.tag == "Friendly") {
                _selectedUnit = u;
                _selectedBuilding = null;
                // upgradeMenu.gameObject.SetActive(true);
                upgradeMenu.SelectUnit(u);
            } else if (u.tag == "Enemy" && _selectedUnit != null) {
                UnitAgent agent = _selectedUnit.GetComponent<UnitAgent>();
                agent.SetDestination(u.transform.position);
                agent.targetUnit = u;
                Debug.Log("Targetted enemy unit is a " + u.GetComponent<Unit>().Name);
                _selectedUnit = null;
                agent.targetBuilding = null;
            }        
        }

        private void TargetBuilding(Building b) {
            if (b.tag.Contains("Friendly")) {
                _selectedBuilding = b;
                _selectedUnit = null;
            } else if (b.tag.Contains("Enemy") && _selectedUnit != null) {
                UnitAgent agent = _selectedUnit.GetComponent<UnitAgent>();
                agent.SetDestination(b.GetPosition());
                agent.targetBuilding = b;
                _selectedUnit = null;
                agent.targetUnit = null;
            }
        }
    }
}