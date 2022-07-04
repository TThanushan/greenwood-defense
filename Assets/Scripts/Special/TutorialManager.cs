using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    public int tutorialIndex = 0;
    GameObject mainCanvas;

    private void Start()
    {
        mainCanvas = GameObject.Find("InGameMainCanvas");
        DontDestroyOnLoad(gameObject);
    }
    void HideUI(bool value = false)
    {
        mainCanvas.transform.Find("TopGroup").gameObject.SetActive(value);
        mainCanvas.transform.Find("LeftGroup").gameObject.SetActive(value);
        mainCanvas.transform.Find("RightGroup").gameObject.SetActive(value);
    }
    void EndTutorial()
    {
        SaveManager.instance.SetTutorialDone();
        Destroy(gameObject);
    }


    void UpdateTutorial()
    {
        if (tutorialIndex == 0)
        {
            // Hide UI element.
            HideUI();

            // Highlight the ally captain, with msg "Protect the king"

            // Tell player to click on the right of the screen (until its all the way on the right)

            // Highlight the enemy captain, with msg "Defeat the frog captain to win"

            // Make two melee frog spawn, with msg "Carefull, the enemy is sending two soldiers."

            // Tell player to click on the left of the screen (until its all the way on the left).

            // Set player current mana to 30.

            // {Show mana bar UI.

            // {Show duck unit button.

            // Highligth the duck unit button, with msg "Quick defend your king by summoning a duck!".

            // Highligth duck unit button mana cost with msg "Summoning a duck cost 20 mana.".

            // msg "You passively regen mana, killing enemy units also regen mana.".

            // Wait until collision between units.

            // Wait a bit until duck has fight a bit.

            // Highligth the chicken unit buttom, with msg "You soldier need help, summon a chicken.".


        }
    }
}
