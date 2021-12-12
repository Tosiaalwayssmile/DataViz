using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MainManager : MonoBehaviour
{
    [Header("Player Info")]
    public SpriteRenderer playerIcon;
    public Text playerLevel;
    public Text playerName;
    public Text playerRank;
    public Text playerMaestry;
    [Space]

    [Header("Minimap Info")]
    public Image topImg;
    public Image jglImg;
    public Image midImg;
    public Image botImg;
    public Image supImg;
    [Space]

    [Header("Roles")]
    public Slider tankSlider;
    public Slider fighterSlider;
    public Slider assassinSlider;
    public Slider mageSlider;
    public Slider marksmanSlider;
    public Slider supportSlider;
    [Space]

    [Header("WinRate Pie Chart")]
    public Image wrImg;


    SummonerDTO player;
    LeagueEntryDTO playerEntry;
    ChampionDTO[] champions;
    MatchInfo[] matchInfos;

    // Start is called before the first frame update
    void Start()
    {
        RiotApi.SetApiKey("RGAPI-0467abfa-db51-40e9-a467-65c443481d9a");
        RiotApi.SetRegion("Europe North East");

        LoadPlayer();
        LoadChampions();
        LoadMatchHistory();
    }

    void LoadPlayer()
    {
        string ign = "TosiaAlwaysSmile";
        //string ign = "Dragonatis";

        player = RiotApi.GetSummonerInfo(ign);
        playerEntry = RiotApi.GetEntries(player.id);

        playerIcon.sprite = RiotApi.DownloadIconSprite(player.profileIconId);
        playerName.text = ign;
        playerLevel.text = player.summonerLevel.ToString();
        playerRank.text = playerEntry.tier + " " + playerEntry.rank;
        playerMaestry.text = RiotApi.GetMaestryScore(player.id).ToString();
    }
    void LoadChampions()
    {
        champions = RiotApi.GetChampsInfo();
    }
    void LoadMatchHistory()
    {
        int matchAmount = 10;
        matchInfos = RiotApi.GetMatches(matchAmount);

        int topAmount = 0;
        int jglAmount = 0;
        int midAmount = 0;
        int botAmount = 0;
        int utilAmount = 0;

        int won = 0;

        foreach (var info in matchInfos)
        {
            if (info.win)
                won++;

            if (info.individualPosition.Equals("TOP"))
                topAmount++;
            else if (info.individualPosition.Equals("JUNGLE"))
                jglAmount++;
            else if (info.individualPosition.Equals("MIDDLE"))
                midAmount++;
            else if (info.individualPosition.Equals("BOTTOM"))
                botAmount++;
            else if (info.individualPosition.Equals("UTILITY"))
                utilAmount++;
            else
                print("Unknown position: " + info.individualPosition);


            string role = string.Empty;
            foreach (var c in from c in champions
                              where info.championId == c.id
                              select c)
            {
                role = c.roles[0];
                break;
            }

            if (role.Equals("tank"))
                tankSlider.value++;
            else if (role.Equals("fighter"))
                fighterSlider.value++;
            else if (role.Equals("assassin"))
                assassinSlider.value++;
            else if (role.Equals("mage"))
                mageSlider.value++;
            else if (role.Equals("marksman"))
                marksmanSlider.value++;
            else if (role.Equals("support"))
                supportSlider.value++;
            else
                print("Unknown role: " + role);
        }

        int top1 = topAmount;
        int top2 = topAmount;

        if (jglAmount > top1)
        {
            top2 = top1;
            top1 = jglAmount;
        }
        else if (jglAmount > top2)
        {
            top2 = jglAmount;
        }

        if (midAmount > top1)
        {
            top2 = top1;
            top1 = midAmount;
        }
        else if (midAmount > top2)
        {
            top2 = midAmount;
        }

        if (botAmount > top1)
        {
            top2 = top1;
            top1 = botAmount;
        }
        else if (botAmount > top2)
        {
            top2 = botAmount;
        }

        if (utilAmount > top1)
        {
            top2 = top1;
            top1 = utilAmount;
        }
        else if (utilAmount > top2)
        {
            top2 = utilAmount;
        }

        topImg.enabled = false;
        jglImg.enabled = false;
        midImg.enabled = false;
        botImg.enabled = false;
        supImg.enabled = false;

        if (top1 == topAmount)
            topImg.enabled = true;
        else if (top1 == jglAmount)
            jglImg.enabled = true;
        else if (top1 == midAmount)
            midImg.enabled = true;
        else if (top1 == botAmount)
            botImg.enabled = true;
        else
            supImg.enabled = true;

        if (top2 == topAmount && top1 != topAmount)
            topImg.enabled = true;
        else if (top2 == jglAmount && top1 != jglAmount)
            jglImg.enabled = true;
        else if (top2 == midAmount && top1 != midAmount)
            midImg.enabled = true;
        else if (top2 == botAmount && top1 != botAmount)
            botImg.enabled = true;
        else if (top2 == utilAmount && top1 != utilAmount)
            supImg.enabled = true;

        wrImg.fillAmount = (float)won / matchAmount;
    }
}
