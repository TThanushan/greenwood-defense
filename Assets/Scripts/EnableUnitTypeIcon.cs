using UnityEngine;

public class EnableUnitTypeIcon : MonoBehaviour
{


    void Start()
    {
        if (ContainsMultipleStrings("Chicken", "Toucan"))
            transform.Find("Dps").gameObject.SetActive(true);
        else if (ContainsMultipleStrings("Rock", "Duck", "Slime", "Rhinoceros"))
            transform.Find("Tank").gameObject.SetActive(true);
        else if (ContainsMultipleStrings("Trunk", "Plant", "FireSkull"))
            transform.Find("Range").gameObject.SetActive(true);
        else
            transform.Find("Specialist").gameObject.SetActive(true);
    }


    bool ContainsMultipleStrings(params string[] substrings)
    {
        string name = transform.parent.gameObject.name;

        foreach (string substring in substrings)
        {
            if (name.Contains(substring))
                return true;
        }
        return false;
    }
}
