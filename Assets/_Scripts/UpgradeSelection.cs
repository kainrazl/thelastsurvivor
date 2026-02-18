using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSelection : MonoBehaviour
{
    [SerializeField] List<GameObject> upgradeOptions;
    [SerializeField] GameObject upgradePanel;
    private List<GameObject> availableUpgrades;
    private bool canAdd = false;
    private GameObject selectedUpgrade = null;
    private GameObject gridLayout;

    private void Awake()
    {
        gridLayout = GameObject.Find("UpgradeSelection");
    }

    public void RandomizeUpgrades()
    {
        int index;
        GameObject randomOption = null;

        availableUpgrades = new List<GameObject>();

        for (int i = 0; i < gridLayout.transform.childCount; i++)
        {
            Destroy(gridLayout.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < 3; i++)
        {
            index = Random.Range(0, upgradeOptions.Count);
            randomOption = upgradeOptions[index];

            canAdd = true;

            foreach (GameObject upgrade in availableUpgrades)
            {
                if (randomOption.name == upgrade.name)
                {
                    canAdd = false;
                    break;
                }
            }

            if (canAdd)
            {
                availableUpgrades.Add(randomOption);
            }
            else
            {
                i--;
            }
        }

        foreach (GameObject upgrade in availableUpgrades)
        {
            GameObject selectableUpgrade = Instantiate(upgradePanel);
            selectableUpgrade.name = upgrade.name;

            for (int i = 0; i < selectableUpgrade.transform.childCount; i++)
            {
                if (selectableUpgrade.transform.GetChild(i).name == "Visual")
                {                    
                    GameObject showUpgrade = Instantiate(upgrade);
                    showUpgrade.transform.SetParent(selectableUpgrade.transform.GetChild(i).transform, false);
                }
                else
                {
                    if (selectableUpgrade.transform.GetChild(i).TryGetComponent(out TextMeshProUGUI description))
                    {
                        upgrade.TryGetComponent(out InGamePrefab gamePrefab);

                        if (gamePrefab != null)
                        {
                            GameObject upgradePrefab = gamePrefab.GetPrefab();
                            upgradePrefab.TryGetComponent(out WeaponInstance upgradeInfo);

                            if (upgradeInfo != null)
                            {
                                description.SetText(upgradeInfo.GetProperties().name);
                            }
                        }
                        else
                        {
                            string upgradeName = upgrade.name.Substring(upgrade.name.LastIndexOf('_') + 1).FirstCharacterToUpper();
                            description.SetText(upgradeName);
                        }
                    }
                }
            }

            Button button = selectableUpgrade.GetOrAddComponent<Button>();
            button.transition = Selectable.Transition.ColorTint;
            button.colors = new ColorBlock
            {
                normalColor = new Color(0, 0, 0, 30f),
                highlightedColor = new Color(0, 0, 0, 60f),
                pressedColor = new Color(0, 0, 0, 0f),
                selectedColor = new Color(0, 0, 0, 120f),
                colorMultiplier = 1,
                fadeDuration = 0.1f
            };
            button.onClick.AddListener(() => SetSelectedUpgrade(upgrade));
            selectableUpgrade.transform.localScale = new Vector3(1, 1, 1);
            selectableUpgrade.transform.SetParent(gridLayout.transform, false);
        }
    }

    public void SetSelectedUpgrade(GameObject upgrade)
    {
        selectedUpgrade = null;

        upgrade.TryGetComponent(out InGamePrefab prefab);

        if (prefab != null)
        {
            selectedUpgrade = prefab.GetPrefab();
        }

        Debug.Log("Selected upgrade: " + selectedUpgrade.name);
        Transform weapons = GameObject.FindGameObjectWithTag("EquipedWeapons").transform;
        GameObject equipUpgrade = Instantiate(selectedUpgrade, weapons);
        equipUpgrade.transform.position = weapons.position;

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerExp>().PerkSelected();
        gameObject.SetActive(false);
    }

    public void EquipUpgrade()
    {
        if (selectedUpgrade != null)
        {
            Transform player = GameObject.FindGameObjectWithTag("EquipedWeapons").transform;
            GameObject upgrade = Instantiate(selectedUpgrade, player);
            upgrade.transform.position = player.position;
        }
        else 
        {
            Debug.Log("No upgrade selected");
        }
    }
}
