using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchLabel : MonoBehaviour
{
    public Image proficiencyBar;
    public Image champIcon;
    public Text winLoseLabel;
    public Text gameModeLabel;
    public Image spell1Icon;
    public Image spell2Icon;
    public Transform killsBar;
    public Transform assistsBar;
    public Transform killsAssistsSeparator;
    public Transform deathsBar;
    public Text gameDuration;
    public Text gameCreation;



    public void SetChampPortrait(Sprite champ, float proficiency)
    {
        champIcon.sprite = champ;
        proficiencyBar.fillAmount = proficiency;
    }
    public void WinLoseGamemode(bool win, string gameMode)
    {
        winLoseLabel.text = win ? "WYGRANA" : "PRZEGRANA";
        gameModeLabel.text = gameMode;
    }
    public void SetSummonerSpells(int spell1Id, int spell2Id)
    {
        spell1Icon.sprite = RiotApi.SummonerSpellSpriteFromID(spell1Id);
        spell2Icon.sprite = RiotApi.SummonerSpellSpriteFromID(spell2Id);
    }
    public void SetKDA(float killRatio, float deathRatio, float assistRatio)
    {
        float kaRatio = Mathf.Sqrt(killRatio + assistRatio);
        killRatio = Mathf.Sqrt(killRatio);
        deathRatio = Mathf.Sqrt(deathRatio);


        killsBar.localScale = new Vector3(killRatio, killRatio, 1);
        deathsBar.localScale = new Vector3(deathRatio, deathRatio, 1);
        assistsBar.localScale = new Vector3(kaRatio, kaRatio, 1);
        killsAssistsSeparator.localScale = new Vector3(killRatio + 0.02f, killRatio + 0.02f, 1);
    }
    public void SetGameTime(long creationTime, long duration)
    {
        int minutes = (int)(duration / 60);
        int seconds = (int)(duration - minutes * 60);

        gameDuration.text = minutes.ToString() + ":" + seconds.ToString("00");

        DateTime creation = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        creation = creation.AddMilliseconds(creationTime).ToLocalTime();
        gameCreation.text = creation.Day.ToString() + "." + creation.Month.ToString() + "." + (creation.Year % 1000).ToString() + " " + creation.Hour.ToString() + ":" + creation.Minute.ToString();
    }
}
