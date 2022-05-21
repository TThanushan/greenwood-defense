using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBar : MonoBehaviour
{
    public Vector2 spawnPosition;
    public UnitButton[] UnitButtons;

    private void Start()
    {
        SetUnitButtons();
        OrderChildButtonsByCost();
    }

    private void Update()
    {
        UpdateUnitButtons();
    }

    private Transform GetCorrespondingChild(string name)
    {
        foreach (Transform child in transform)
        {
            if (child.name.Contains(name))
                return child;
        }
        return null;
    }

    private void SetUnitButtons()
    {
        foreach (UnitButton unitButton in UnitButtons)
        {
            Transform child;
            if (!(child = GetCorrespondingChild(unitButton.Name))) continue;

            unitButton.reloadBar = child.Find("ReloadBar/Bar");
            child.Find("CostText").GetComponent<TMPro.TextMeshProUGUI>().text = unitButton.cost.ToString();

            unitButton.enoughManaShade = child.Find("EnoughManaShade").gameObject;

            // Set image.
            
        }
    }

    public void SpawnUnit(string name)
    {
        foreach (UnitButton unitButton in UnitButtons)
        {
            if (unitButton.Name == name && unitButton.ReadyToSpawn() && unitButton.HasEnoughMana())
            {
                SpawnUnit(unitButton.prefab);
                unitButton.ResetCurrentReloadTime();
                ManaBar.instance.UseMana(unitButton.cost);
            }
        }
    }

    private void SpawnUnit(GameObject prefab)
    {
        GameObject newUnit = PoolObject.instance.GetPoolObject(prefab);
        float randomYPos = spawnPosition.y + Random.Range(-0.1f, 0.3f);
        newUnit.transform.position = new Vector2(spawnPosition.x, randomYPos);
    }

    private void UpdateUnitButtons()
    {
        foreach (UnitButton unitButton in UnitButtons)
        {
            unitButton.Update();
        }
    }

    
    public void OrderChildButtonsByCost()
    {
        float lowest = Mathf.Infinity;
        foreach (UnitButton unitButton in UnitButtons)
        {
            if (unitButton.cost <= lowest)
            {
                lowest = unitButton.cost;
                GetCorrespondingChild(unitButton.Name).SetAsFirstSibling();
            }
        }
    }


    [System.Serializable]
    public class UnitButton
    {
        public string Name { get => prefab.name;}
        public GameObject prefab;
        public float cost;
        public float reloadTime;
        public Transform reloadBar;
        public GameObject enoughManaShade;

        private string name;
        private float currentReloadTime = 0f;

        public void Update()
        {
            UpdateCurrentReloadTime();
            UpdateReloadBarLength();
            UpdateEnoughManaShade();
        }

        public bool ReadyToSpawn()
        {
            return currentReloadTime <= 0f;
        }

        public void ResetCurrentReloadTime()
        {
            currentReloadTime = reloadTime;
        }

        public bool HasEnoughMana()
        {
            return ManaBar.instance.currentMana >= cost;
        }

        private void UpdateEnoughManaShade()
        {
            if (!enoughManaShade) return;
            if (HasEnoughMana() && enoughManaShade.activeSelf)
                enoughManaShade.SetActive(false);
            else if (!HasEnoughMana() && !enoughManaShade.activeSelf)
                enoughManaShade.SetActive(true);
        }

        private void UpdateCurrentReloadTime()
        {
            if (currentReloadTime > 0f)
                currentReloadTime -= Time.deltaTime;
            else
                currentReloadTime = 0f;
        }

        private void UpdateReloadBarLength()
        {
            if (!reloadBar) return;
            reloadBar.localScale = new Vector2(1f, currentReloadTime / reloadTime);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(spawnPosition, 0.25f);
    }
}
