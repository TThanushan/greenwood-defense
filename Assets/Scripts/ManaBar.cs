using TMPro;
using UnityEngine;

public class ManaBar : MonoBehaviour
{
    public static ManaBar instance;

    public float maxMana;
    public float currentMana;
    public float regenerationSpeed;

    private GameObject manaBar;
    private TextMeshProUGUI currentManaText;

    private void Awake()
    {
        if (!instance)
            instance = this;
        if (!manaBar)
            manaBar = transform.Find("ManaBar/Bar").gameObject;
        if (!currentManaText)
            currentManaText = transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        UpdateManaBarLength();
        UpdateCurrentManaText();
        RegenerateMana();
    }
    private void Start()
    {
        regenerationSpeed += 0.1f * int.Parse(PoolObject.instance.GetStageNumber());
    }
    private void UpdateCurrentManaText()
    {
        currentManaText.text = Mathf.FloorToInt(currentMana).ToString();
    }

    private void RegenerateMana()
    {
        if (currentMana < maxMana)
            currentMana += regenerationSpeed * Time.deltaTime;
        else if (currentMana > maxMana)
            currentMana = maxMana;
    }

    public void UseMana(float amount)
    {
        currentMana -= amount;
    }

    private void UpdateManaBarLength()
    {
        manaBar.transform.localScale = new Vector3(GetNewBarLength(), manaBar.transform.localScale.y, manaBar.transform.localScale.z);
    }

    private float GetNewBarLength()
    {
        float barLenght = currentMana / maxMana;
        return barLenght;
    }
}
