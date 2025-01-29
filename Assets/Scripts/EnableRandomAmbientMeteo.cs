using UnityEngine;

public class EnableRandomAmbientMeteo : MonoBehaviour
{
    // array with the following elements: "VastForest" "SnowyCanyon" "CrystalCave" "TreasureCave" "ChurchInAutumn" "SunnyBeach" "Desert" "SunshineForest" "PoisonCave" "Purgatory".
    private GameObject ambientMeteo; // Parent GameObject containing all ambient conditions

    private int currentStage;

    void Start()
    {
        ambientMeteo = gameObject;
        // Get the current stage number
        currentStage = StageInfosManager.instance.GetCurrentStageNumber();

        // Enable a random ambient condition based on the current stage
        EnableRandomAmbient();
    }

    void EnableRandomAmbient()
    {
        // Add a 75% chance to return.
        if (Random.Range(0, 100) <= 60) return;

        // Disable all ambient conditions first
        foreach (Transform child in ambientMeteo.transform)
        {
            child.gameObject.SetActive(false);
        }

        // Determine which ambient conditions are possible for the current stage
        GameObject[] possibleAmbients = GetPossibleAmbients();

        // If there are possible ambients, enable a random one
        if (possibleAmbients.Length > 0)
        {
            int randomIndex = Random.Range(0, possibleAmbients.Length);
            possibleAmbients[randomIndex].SetActive(true);
        }
    }

    GameObject[] GetPossibleAmbients()
    {
        // Define which ambient conditions are possible based on the current stage
        if (currentStage < 10)
        {
            return new GameObject[]
            {
                ambientMeteo.transform.Find("Sunny Day Time").gameObject,
                ambientMeteo.transform.Find("Heavy Rain").gameObject,

            };
        }
        else if (currentStage < 20)
        {
            return new GameObject[]
            {
                ambientMeteo.transform.Find("Snow").gameObject,
                ambientMeteo.transform.Find("Heavy Rain").gameObject,
                ambientMeteo.transform.Find("Sunny Day Time").gameObject,
            };
        }
        else if (currentStage < 30)
        {
            return new GameObject[]
            {
                ambientMeteo.transform.Find("Sunny Day Time").gameObject,
                ambientMeteo.transform.Find("Heavy Rain").gameObject,
                ambientMeteo.transform.Find("Cavern SunBeam Dust").gameObject,

            };
        }
        else if (currentStage < 40)
        {
            return new GameObject[]
            {
                ambientMeteo.transform.Find("Sunny Day Time").gameObject,
                ambientMeteo.transform.Find("Heavy Rain").gameObject,
                ambientMeteo.transform.Find("Cavern SunBeam Dust").gameObject,

            };
        }
        else if (currentStage < 50)
        {
            return new GameObject[]
            {
                ambientMeteo.transform.Find("Sunny Day Time").gameObject,
                ambientMeteo.transform.Find("Heavy Rain").gameObject,
                ambientMeteo.transform.Find("Cavern SunBeam Dust").gameObject,

            };
        }
        else if (currentStage < 60)
        {
            return new GameObject[]
            {
                ambientMeteo.transform.Find("Sunny Day Time").gameObject,
                ambientMeteo.transform.Find("Heavy Rain").gameObject,
                ambientMeteo.transform.Find("Cavern SunBeam Dust").gameObject,

            };
        }
        else if (currentStage < 70)
        {
            return new GameObject[]
            {
                ambientMeteo.transform.Find("Sunny Day Time").gameObject,
                ambientMeteo.transform.Find("Heavy Rain").gameObject,
                ambientMeteo.transform.Find("Cavern SunBeam Dust").gameObject,

            };
        }
        else if (currentStage < 80)
        {
            return new GameObject[]
            {
                ambientMeteo.transform.Find("Sunny Day Time").gameObject,
                ambientMeteo.transform.Find("Heavy Rain").gameObject,
                ambientMeteo.transform.Find("Cavern SunBeam Dust").gameObject,

            };
        }
        else if (currentStage < 90)
        {
            return new GameObject[]
            {
                ambientMeteo.transform.Find("Sunny Day Time").gameObject,
                ambientMeteo.transform.Find("Heavy Rain").gameObject,
                ambientMeteo.transform.Find("Cavern SunBeam Dust").gameObject,

            };
        }
        else if (currentStage <= 100)
        {
            return new GameObject[]
            {
                ambientMeteo.transform.Find("Sunny Day Time").gameObject,
                ambientMeteo.transform.Find("Heavy Rain").gameObject,
                ambientMeteo.transform.Find("Cavern SunBeam Dust").gameObject,

            };
        }
        else
        {
            // Return an empty array if no ambient conditions are possible
            return new GameObject[0];
        }
    }
}

