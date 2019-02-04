using UnityEngine;
using UnityEngine.UI;
using Localisation;
using System.Xml.Linq;
using System.Linq;
using System.IO;
using System.Collections;
using System;

namespace Assets {
    public class UpgradeMenu : MonoBehaviour {

        //Using [SerializeField] to expose private variables to the editor window.
        public GameObject unitInformation, noUnitSelected, controlButtons;
        private GameObject[] _availableUpgrades;        
        private GameObject[] _unitStatModifiers;
        private Upgrade[] _unitUpgradeSlots = new Upgrade[3];
        private Upgrade _selectedUnitUpgrade, _selectedUpgrade;
        private Upgrade[] _allUpgrades = new Upgrade[16];  
        private Unit _selectedUnit;
        public Sprite[] unitSprites = new Sprite[3];           
        public Sprite[] upgradeSprites = new Sprite[16];
        private Image _upgradeItemBG;
        [SerializeField]
        private GameObject[] _unitUpgradeSlotImages = new GameObject[3];//Storing gameobjects as the object also contains additional components and in some instances the object will need to be deactivated and in some instances the sprite will need to be changed.
        [SerializeField]
        private Image unitImage;
        [SerializeField]
        private Text selectedUpgradeName, selectedUpgradeDesc, unitName;

        private void Start() {
            _availableUpgrades = GameObject.FindGameObjectsWithTag("UpgradeMenuItem").OrderBy(x => x.name).ToArray();
            _unitStatModifiers = GameObject.FindGameObjectsWithTag("StatModifier").ToArray();
            _upgradeItemBG = GameObject.Find("Upgrade Items").GetComponent<Image>();           
            LoadUpgrades();
            gameObject.SetActive(false);
        }

        public Unit GetSelectedUnit() {
            return _selectedUnit;
        }

       /* public void TestButtonClick(int i) {
            if (_unitUpgradeSlots[i] == null)
                return;

            _selectedUpgradeSlotUpgrade = i;
        }
        */
        public void SellUpgrade() {

            if (_selectedUnitUpgrade == null)
                return;

            _selectedUnit.RemoveUpgrade(_selectedUnitUpgrade);
            _selectedUnit.Upgrades[_selectedUnit.Upgrades.ToList().IndexOf(_selectedUnitUpgrade)] = null;
            Player.human.AddCredits(Mathf.RoundToInt(_selectedUnitUpgrade.Cost * 0.3f));
            _selectedUnitUpgrade = null;
            _selectedUnit.Upgrades = _selectedUnit.Upgrades.OrderBy(x => x == null).ToArray();
            SelectUnit(_selectedUnit);
        }

        public void SelectUnit(Unit unit) {
            _selectedUnit = unit;

            if (unit == null) {
                Close();
                Open();
                return;
            }

            if (!gameObject.activeSelf)
                return;

            noUnitSelected.SetActive(false);
            unitInformation.SetActive(true);
            unitName.text = _selectedUnit.Name;
            unitImage.sprite = unitSprites[_selectedUnit.GetUnitType()];

            for (int i = 0; i < _unitStatModifiers.Length; i++) {
                Text[] modifiers = _unitStatModifiers[i].GetComponentsInChildren<Text>().ToArray();
                modifiers[0].text = GetStat(i);
                modifiers[1].text = GetModifier(i);
                //Assign upgrades the the unit upgrade slots.               
            }

            for (int i = 0; i < 3; i++) {
                _unitUpgradeSlots[i] = null;//Reset the upgrades stored in the on screen slots (The upgrade selected is kept if the player selects a different unit)
                if (unit.Upgrades[i] == null){//if the slot is empty we'll hide it.
                    Debug.Log("Unit upgrade slot " + i + " is null.");
                    _unitUpgradeSlotImages[i].SetActive(false);
                    _unitUpgradeSlotImages[i].tag = "Untagged";
                } else {//else we'll assign the upgrade to the slot and set the sprite to the upgrades sprite.
                    _unitUpgradeSlotImages[i].SetActive(true);
                    _unitUpgradeSlots[i] = unit.Upgrades[i];
                    _unitUpgradeSlotImages[i].tag = "HasUpgrade";
                    _unitUpgradeSlotImages[i].GetComponent<Image>().sprite = upgradeSprites[(int)unit.Upgrades[i].Id];
                   //_unitUpgradeSlotImages[i].GetComponent<Outline>().enabled = false;// Temporarily disable the item sprites until upgrade sprites are implemented.
                   //_unitUpgradeSlotImages[i].GetComponent<Image>().color = new Color32(0, 0, 0, 0);// ^^^^
                   //   _unitUpgradeSlotImages[i].GetComponentInChildren<Text>().text = _selectedUnit.Upgrades[i].Name;
                }
            }
            Debug.Log("Finished selecting unit - " + unit.Name);
        }

        private string GetStat(int stat) {
            switch (stat) {
                case 0:
                    return _selectedUnit.MovementSpeed.ToString();
                case 1:
                    return _selectedUnit.AttackSpeed.ToString();
                case 2:
                    return _selectedUnit.MaxDamage.ToString();
                case 3:
                    return _selectedUnit.Resistance.ToString();
                case 4:
                    return _selectedUnit.BaseMaxHealth.ToString();
                case 5:
                    return _selectedUnit.Accuracy.ToString();
                default:
                    return null;            
            }
        }

       private string GetModifier(int modifier) {
            switch (modifier) {
                case 0:
                    return _selectedUnit.MovementSpeedModifier >= 0 ?
                        "+" + _selectedUnit.MovementSpeedModifier.ToString() :
                        "-" + _selectedUnit.MovementSpeedModifier.ToString();
                case 1:
                    return _selectedUnit.AttackSpeedModifier >= 0 ?
                        "+" + _selectedUnit.AttackSpeedModifier.ToString() :
                        "-" + _selectedUnit.AttackSpeedModifier.ToString();
                case 2:
                    return _selectedUnit.DamageModifier-1 >= 0 ?//Damage modifier is a multiplier so we will always show 0 to the user in its default state.
                        "+" + (_selectedUnit.DamageModifier-1).ToString() :
                        "-" + (_selectedUnit.DamageModifier-1).ToString();
                case 3:
                    return _selectedUnit.ResistanceModifier >= 0 ?
                        "+" + _selectedUnit.ResistanceModifier.ToString() :
                        "-" + _selectedUnit.ResistanceModifier.ToString();
                case 4:
                    return _selectedUnit.HealthModifier >= 0 ?
                        "+" + (_selectedUnit.HealthModifier).ToString() :
                        "-" + (_selectedUnit.HealthModifier).ToString();
                case 5:
                    return _selectedUnit.AccuracyModifier >= 0 ?
                        "+" + _selectedUnit.AccuracyModifier.ToString() :
                        "-" + _selectedUnit.AccuracyModifier.ToString();
                default:
                    return null;
            }
        }

        /// <summary>
        /// - Used to refresh the scroll pane content to make it visible as it is not visible after being enabled.
        /// - Potential Bug with UI elements in 5.3+
        /// - Forcing a canvas update does not solve the issue, neither does registering the component in the update registry.
        /// </summary>
        private IEnumerator RefreshScrollPane() {
            _upgradeItemBG.enabled = false;
            yield return new WaitForEndOfFrame();
            _upgradeItemBG.enabled = true;
        }

        public void Open() {
            gameObject.SetActive(true);
            StartCoroutine(RefreshScrollPane());
          

            if (_selectedUpgrade == null)
                controlButtons.SetActive(false);


            if (_selectedUnit == null) {
                unitInformation.SetActive(false);
                noUnitSelected.SetActive(true);
            } else {
                noUnitSelected.SetActive(false);
                unitInformation.SetActive(true);
                unitName.text = _selectedUnit.Name;
            }
        } 

        public void Close() {
            _selectedUnit = null;
            _selectedUpgrade = null;
            gameObject.SetActive(false);
        }

        private void LoadUpgrades() {
            var doc = XDocument.Load(Path.Combine(Application.dataPath, "LanguageLocalisation/Upgrades.xml"));
            string identifier, path; 

            var nodes = doc.Root.Elements(Localise.GetLang()).Elements();

            for (int i = 0; i <= 13; i++) {
                identifier = string.Format("upgrade{0}_", i + 1);
                var attributes = nodes.Where(e => e.FirstAttribute.ToString().Contains(identifier));
                    _allUpgrades[i] = new Upgrade {
                        Id = i,
                        Name = attributes.First(x => x.FirstAttribute.ToString().Contains("name")).Value,
                        Description = attributes.First(x => x.FirstAttribute.ToString().Contains("desc")).Value,
                        Cost = int.Parse(attributes.First(x => x.FirstAttribute.ToString().Contains("cost")).Value),
                        LevelReq = int.Parse(attributes.First(x => x.FirstAttribute.ToString().Contains("level")).Value),
                        DamageModifier = float.Parse(attributes.First(x => x.FirstAttribute.ToString().Contains("damage")).Value),
                        HealthModifier = int.Parse(attributes.First(x => x.FirstAttribute.ToString().Contains("health")).Value),
                        SpeedModifier = float.Parse(attributes.First(x => x.FirstAttribute.ToString().Contains("speedModifier")).Value),
                        ResistanceModifier = float.Parse(attributes.First(x => x.FirstAttribute.ToString().Contains("resist")).Value),
                        AccuracyModifier = float.Parse(attributes.First(x => x.FirstAttribute.ToString().Contains("accuracy")).Value),
                        AttackSpeedModifier = float.Parse(attributes.First(x => x.FirstAttribute.ToString().Contains("attack")).Value),
                        Unlocked = false,
                        type = (Upgrade.Type)int.Parse(attributes.First(x => x.FirstAttribute.ToString().Contains("type")).Value)
                    };
            }

            for (int i = 0; i < _availableUpgrades.Length; i++) {
                try {
                    path = string.Format("Canvas/Upgrade Interface/Available Upgrades/ScrollView/Upgrade Items/{0}/Upgrade Info/", _availableUpgrades[i].name);
                    GameObject.Find(path + "Upgrade Name").GetComponent<Text>().text = "<b>" + _allUpgrades[i].Name + "</b>";
                    GameObject.Find(path + "Description").GetComponent<Text>().text = _allUpgrades[i].Description;
                    GameObject.Find(path + "CostValue").GetComponent<Text>().text = _allUpgrades[i].Cost.ToString();
                    GameObject.Find(path + "LevelReq").GetComponent<Text>().text = _allUpgrades[i].LevelReq.ToString();
                    GameObject.Find(path + "Damage Icon").GetComponentInChildren<Text>().text = Format(_allUpgrades[i].DamageModifier,"%");
                    GameObject.Find(path + "Health Icon").GetComponentInChildren<Text>().text = Format(_allUpgrades[i].HealthModifier,"HP");
                    GameObject.Find(path + "Movement Speed Icon").GetComponentInChildren<Text>().text = Format(_allUpgrades[i].SpeedModifier,"%");
                    GameObject.Find(path + "Resistance Icon").GetComponentInChildren<Text>().text = Format(_allUpgrades[i].ResistanceModifier,"%");
                    GameObject.Find(path + "Attack Speed Icon").GetComponentInChildren<Text>().text = Format(_allUpgrades[i].AttackSpeedModifier,"s");
                    //GameObject.Find(path + "Accuracy Icon").GetComponentInChildren<Text>().text = Format(_allUpgrades[i].AccuracyModifier,"%");
                } catch (NullReferenceException nrex) {
                    Debug.LogException(nrex);
                    Debug.Log("NullReferenceException - This has most likely occured due to being unable to locate a component due to it being either inactive or incorrectly named.");
                }
            }
        }

        /// <summary>
        /// Formats a string to show the upgrade modifier, whether or not it is a positive, and its type e.g. %, second, or health based.
        /// </summary>
        /// <param name="value">The input numeric float value. Also casting input ints to float.</param>
        /// <param name="append">The characters to be appended onto the end of the string to show its type. E.g. "%"</param>
        /// <returns>Returns the formatted string.</returns>
        private string Format(float value, string append) {
            return string.Format("{0}{1}{2}", value >= 0 ? "+" : "", value, append);
        }

        public GameObject[] GetUpgradeItems() {
            return _availableUpgrades;
        }

        public void SelectUpgradeSlot(int slot) {
            _selectedUnitUpgrade = _unitUpgradeSlots[slot];
            selectedUpgradeName.text = _selectedUnitUpgrade.Name;
            selectedUpgradeDesc.text = _selectedUnitUpgrade.Description;
        }

        public void SelectUpgrade(int upgradeId) {
            _selectedUpgrade = _allUpgrades[upgradeId];
            _allUpgrades[upgradeId].Unlock();
            controlButtons.SetActive(true);

            try {

                if ((int)_selectedUpgrade.type == 1 || (int)_selectedUpgrade.type == 3) {
                    Debug.Log("Upgrade no longer exists.");
                    return;
                }

                if (!_selectedUpgrade.Unlocked) {
                    Debug.Log(string.Format("Can't select {0} as you have not unlocked it Yet.\nIt will unlock at level {1}.", _selectedUpgrade.Name, _selectedUpgrade.LevelReq));
                    return;
                }

                if (_selectedUnit.Upgrades.Contains(_selectedUpgrade) || _unitUpgradeSlots.Contains(_selectedUpgrade)) {
                    Debug.Log("Can't select the same upgrade twice!");
                    return;
                }             

            } catch (NullReferenceException) {
                //Suppressing NullReferenceException.
                //Exception occurs when checking to see if the selected unit contains upgrades when the unit is null.
                //This is being allowed to occur as the script needs to continue to add upgrades to the player.
            }

            if(_selectedUnit != null) {
                var freeSlot = _unitUpgradeSlotImages.FirstOrDefault(x => x.tag == "Untagged");
                var freeUpgradeSlot = _unitUpgradeSlots.FirstOrDefault(x => x == null);

                if(freeSlot == null) {
                    Debug.Log("No free slots.");
                    return;
                }

                freeUpgradeSlot = _selectedUpgrade;
                freeSlot.SetActive(true);
                freeSlot.GetComponent<Image>().sprite = upgradeSprites[_selectedUpgrade.Id];
            }

            selectedUpgradeName.text = _selectedUpgrade.Name;
            selectedUpgradeDesc.text = _selectedUpgrade.Description;

            /*  for (int i = 0; i < _unitUpgradeSlots.Length; i++) {
                  if(_unitUpgradeSlots[i] == null) {
                      _unitUpgradeSlots[i] = _selectedUpgrade;
                      _unitUpgradeSlotImages[i].SetActive(true);
                      _unitUpgradeSlotImages[i].GetComponent<Image>().sprite = upgradeSprites[_selectedUpgrade.Id];
                      //_unitUpgradeSlotImages[i].GetComponent<Outline>().enabled = false;// Temporarily disable the item sprites until upgrade sprites are implemented.
                     // _unitUpgradeSlotImages[i].GetComponent<Image>().color = new Color32(0, 0, 0, 0);// ^^^^
                      //_unitUpgradeSlotImages[i].GetComponentInChildren<Text>().text = selectedUpgrade.Name;
                      selectedUpgradeName.text = _selectedUpgrade.Name;
                      selectedUpgradeDesc.text = _selectedUpgrade.Description;
                      break;
                  }
              }*/
        }

        public void BuyUpgrades() {
            int count = 0;
            
            if (Player.human.Credits - _selectedUpgrade.Cost < 0) {
                Debug.Log(string.Format("You don't have enough credits to purchase the {0} upgrade!", _selectedUpgrade.Name));
                _unitUpgradeSlotImages[count].SetActive(false);
                _unitUpgradeSlots[count] = null;
                return;
            }

            if (_selectedUpgrade.type == 0) {//Unit upgrades       

                if (_selectedUnit == null) {
                    Debug.Log("Unable to buy upgrade as there is no unit selected!");
                    return;
                }

                if (_selectedUnit.Upgrades.Contains(_selectedUpgrade)) {
                    Debug.Log("Can't buy the same upgrade multiple times!");
                    return;
                }

                _selectedUnit.AddUpgrade(_selectedUpgrade);
                SelectUnit(_selectedUnit);
            }

            if ((int)_selectedUpgrade.type == 2) {//Gameplay upgrades
                if (!Player.human.HasUpgrade(_selectedUpgrade)) {
                    Player.human.AddUpgrade(_selectedUpgrade);
                } else {
                    Debug.Log("You already have this upgrade!");
                    return;
                }
            }

            Player.human.SpendCredits(_selectedUpgrade.Cost);

            /*  foreach (var upgrade in _unitUpgradeSlots.Where(x => x != null)) {

                  if (Player.human.Credits - upgrade.Cost < 0) {
                      Debug.Log("You don't have enough credits to purchase this upgrade!");
                      _unitUpgradeSlotImages[count].SetActive(false);
                      _unitUpgradeSlots[count] = null;
                      return;
                  }

                  if ((int)upgrade.type == 0) {//Unit upgrades       

                      if (_selectedUnit == null) {
                          Debug.Log("Unable to buy upgrade as there is no unit selected!");
                          Debug.Log(upgrade.Name);
                          return;
                      }           
                      _selectedUnit.AddUpgrade(upgrade);
                      Player.human.SpendCredits(upgrade.Cost);
                      playerCredits.text = Player.human.Credits.ToString();
                  } else if ((int)upgrade.type == 1) {//Building upgrades                   
                      //Redacted due to time constraints.
                  } else if((int)upgrade.type == 2) {//Gameplay upgrades
                      if (!Player.human.HasUpgrade(upgrade)) {
                          if(Player.human.Credits - upgrade.Cost < 0) {
                              Debug.Log("You don't have enough credits to purchase this upgrade!");
                              return;
                          }
                          Player.human.AddUpgrade(upgrade);
                      } else
                      {
                          Debug.Log("You already have this upgrade!");
                      }
                  } else if ((int)upgrade.type == 3) {//Multipurpose upgrades (e.g. for both units and buildings)  
                      //Redacted due to time constraints.
                  }
                  count++;
              }*/

            /*
                        foreach (Upgrade upgrade in _unitUpgradeSlots) {
                            if (upgrade != null) {
                                if (!_selectedUnit.Upgrades.Contains(upgrade)) {
                                    if (Player.human.Credits - upgrade.Cost < 0) {
                                        Debug.Log("You don't have enough credits to purchase this upgrade!");
                                        _unitUpgradeSlotImages[count].SetActive(false);
                                        _unitUpgradeSlots[count] = null;
                                        return;
                                    }
                                    _selectedUnit.AddUpgrade(upgrade);
                                    Player.human.SpendCredits(upgrade.Cost);
                                    playerLevel.text = Player.human.Level.ToString();
                                    playerCredits.text = Player.human.Credits.ToString();
                                }
                            }
                            count++;
                        }
                        */
            //gameObject.SetActive(false);
            //gameObject.SetActive(true);

        }

        public static void LocaliseLanguage() {
            Localise.LoadDoc("UpgradeInterface.xml");
            GameObject.Find("Upgrade Title").GetComponent<Text>().text = Localise.GetString("upgrade_title");
           // GameObject.Find("Filter Upgrades Title").GetComponent<Text>().text = Localise.GetString("filter_title");
           // GameObject.Find("Filter Unit").GetComponent<Text>().text = Localise.GetString("filter_unit");
           // GameObject.Find("Filter Building").GetComponent<Text>().text = Localise.GetString("filter_building");          
          //  GameObject.Find("PlaceholderSearchText").GetComponent<Text>().text = Localise.GetString("search_text");
            GameObject.Find("Cancel Upgrade Button").GetComponentInChildren<Text>().text = Localise.GetString("cancel");
          //  GameObject.Find("Sell/Buy Button").GetComponentInChildren<Text>().text = Localise.GetString("buy");
            foreach (var g in GameObject.FindGameObjectsWithTag("Filter Health")) {
                g.GetComponent<Text>().text = Localise.GetString("filter_health");
            }
            foreach (var g in GameObject.FindGameObjectsWithTag("Filter Damage")) {
                g.GetComponent<Text>().text = Localise.GetString("filter_damage");
            }
            foreach (var g in GameObject.FindGameObjectsWithTag("Filter Defence")) {
                g.GetComponent<Text>().text = Localise.GetString("filter_defence");
            }
            foreach (var g in GameObject.FindGameObjectsWithTag("Filter Speed")) {
                g.GetComponent<Text>().text = Localise.GetString("filter_speed");
            }
            Localise.UnloadDoc();

        }
    }
}
