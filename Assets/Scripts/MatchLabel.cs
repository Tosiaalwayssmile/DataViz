using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MatchLabel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image proficiencyBar;
    public Image champIcon;
    public Text winLoseLabel;
    public Text gameModeLabel;
    public Image spell1Icon;
    public Image spell2Icon;
    public Transform killsBar;
    public Transform assistsBar;
    public Transform deathsBar;
    public Text gameDuration;
    public Text gameCreation;
    public Text killsCount;
    public Text deathsCount;
    public Text assistsCount;
    public Text kpHover;
    public Text deathsHover;
    public Text champHover;
    public GameObject glow;

    Map.mapMethod onEnter;
    Map.mapMethod onExit;


    public void SetChampPortrait(Sprite champ, int maestryLevel, string champName)
    {
        champIcon.sprite = champ;
        proficiencyBar.fillAmount = maestryLevel / 7f;

        champHover.text = champName + "\nMaestry Lvl: " + maestryLevel;
    }
    public void WinLoseGamemode(bool win, string gameMode)
    {
        winLoseLabel.text = win ? "VICTORY" : "DEFEAT";
        winLoseLabel.color = win ? new Color(0.1568628f, 0.5843138f, 0.7372549f) : new Color(0.9716981f, 0.04531838f, 0.03208439f);
        gameModeLabel.text = gameMode;
    }
    public void SetSummonerSpells(int spell1Id, int spell2Id)
    {
        spell1Icon.sprite = RiotApi.SummonerSpellSpriteFromID(spell1Id, out string name1);
        spell2Icon.sprite = RiotApi.SummonerSpellSpriteFromID(spell2Id, out string name2);

        spell1Icon.GetComponentInChildren<RoleHoverer>().image.GetComponentInChildren<Text>().text = name1;
        spell2Icon.GetComponentInChildren<RoleHoverer>().image.GetComponentInChildren<Text>().text = name2;
    }
    public void SetKDA(float killRatio, float deathRatio, float assistRatio, int kill, int deaths, int assists)
    {
        killsCount.text = kill.ToString();
        deathsCount.text = deaths.ToString();
        assistsCount.text = assists.ToString();

        float kaRatio = Mathf.Sqrt(killRatio + assistRatio);
        killRatio = Mathf.Sqrt(killRatio);
        deathRatio = Mathf.Sqrt(deathRatio);

        kpHover.text = "Kill participation: " + ((int)(kaRatio * 100)).ToString() + "%";
        deathsHover.text = ((int)(deathRatio * 100)).ToString() + "% of team's deaths";

        killRatio *= 0.7f;
        killRatio += 0.3f;

        deathRatio *= 0.7f;
        deathRatio += 0.3f;

        kaRatio *= 0.7f;
        kaRatio += 0.3f;

        killRatio *= 0.78f;
        deathRatio *= 0.78f;
        kaRatio *= 0.78f;


        killsBar.localScale = new Vector3(killRatio, killRatio, 1);
        deathsBar.localScale = new Vector3(deathRatio, deathRatio, 1);
        assistsBar.localScale = new Vector3(kaRatio, kaRatio, 1);
    }
    public void SetGameTime(long creationTime, long duration)
    {
        int minutes = (int)(duration / 60);
        int seconds = (int)(duration - minutes * 60);

        gameDuration.text = minutes.ToString() + ":" + seconds.ToString("00");

        DateTime creation = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        creation = creation.AddMilliseconds(creationTime).ToLocalTime();
        gameCreation.text = creation.Day.ToString() + "." + creation.Month.ToString("00") + "." + (creation.Year % 1000).ToString() + " " + creation.Hour.ToString() + ":" + creation.Minute.ToString();
    }
    public void SetPosition(string pos)
    {
        if (pos.Equals("TOP"))
            onEnter += Map.SetTop;
        else if (pos.Equals("JUNGLE"))
            onEnter += Map.SetJgl;
        else if (pos.Equals("MIDDLE"))
            onEnter += Map.SetMid;
        else if (pos.Equals("BOTTOM") || pos.Equals("UTILITY"))
            onEnter += Map.SetBot;
        onExit += Map.SetDef;
    }

    //public void OnMouseEnter()
    //{
        
    //}

    //public void OnMouseExit()
    //{
        
    //}

    public void OnPointerExit(PointerEventData eventData)
    {
        onExit?.Invoke();
        glow.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onEnter?.Invoke();
        glow.SetActive(true);
    }
}
