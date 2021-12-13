using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchLabel : MonoBehaviour
{
    public Image proficiencyBar;
    public SpriteRenderer champIcon;
    public Text winLoseLabel;
    public Text gameModeLabel;
    public SpriteRenderer spell1Icon;
    public SpriteRenderer spell2Icon;



    public void Init(Sprite champ, bool win, string gameMode, Sprite spell1, Sprite spell2)
    {
        champIcon.sprite = champ;
        winLoseLabel.text = win ? "WYGRANA" : "PRZEGRANA";
        gameModeLabel.text = gameMode;
        spell1Icon.sprite = spell1;
        spell2Icon.sprite = spell2;
    }
    public void SetProficiency(float proficiency)
    {
        proficiencyBar.fillAmount = proficiency;
    }
}
