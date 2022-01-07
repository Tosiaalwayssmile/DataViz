using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MainManager : MonoBehaviour
{
    [Header("Player Info")]
    public Image playerIcon;
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

    [Header("Match History")]
    public Transform viewport;
    public GameObject matchLabelPrefab;


    SummonerDTO player;
    LeagueEntryDTO playerEntry;
    ChampionDTO[] champions;
    MatchInfo[] matchInfos;
    Dictionary<long, Sprite> champSprites;

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
        playerRank.text = playerEntry == null ? "UNRANKED" : playerEntry.tier + " " + playerEntry.rank;
        playerMaestry.text = RiotApi.GetMaestryScore(player.id).ToString();
    }
    void LoadChampions()
    {
        champions = RiotApi.GetChampsInfo();
        champSprites = RiotApi.DownloadChampionSprites(champions);
    }
    void LoadMatchHistory()
    {
        int matchAmount = 5;
        matchInfos = RiotApi.GetMatches(matchAmount);

        int topAmount = 0;
        int jglAmount = 0;
        int midAmount = 0;
        int botAmount = 0;
        int utilAmount = 0;

        int won = 0;

        var champGamesAmount = new Dictionary<int, int>();

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


            if (champGamesAmount.ContainsKey(info.championId))
                champGamesAmount[info.championId]++;
            else
                champGamesAmount.Add(info.championId, 1);
        }

        foreach (var info in matchInfos)
        {
            var label = Instantiate(matchLabelPrefab).GetComponent<MatchLabel>();

            label.transform.SetParent(viewport);
            label.transform.localScale = Vector3.one;
            label.transform.localPosition = Vector3.zero;

            label.SetChampPortrait(champSprites[info.championId], champGamesAmount[info.championId] / 10f);
            label.WinLoseGamemode(info.win, info.gameMode);
            label.SetSummonerSpells(info.summoner1Id, info.summoner2Id);
            label.SetKDA((float)info.kills / info.teamKills, (float)info.deaths / info.teamDeaths, (float)info.assists / info.teamKills, info.kills, info.deaths, info.assists);
            label.SetGameTime(info.gameCreation, info.gameDuration);
            label.SetPosition(info.individualPosition);
        }

        int top1 = -1;
        int top2 = -1;

        if (topAmount > top1)
        {
            top2 = top1;
            top1 = topAmount;
        }
        else if (topAmount > top2)
        {
            top2 = topAmount;
        }

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

        topImg.transform.localScale = Vector3.one;
        jglImg.transform.localScale = Vector3.one;
        midImg.transform.localScale = Vector3.one;
        botImg.transform.localScale = Vector3.one;
        supImg.transform.localScale = Vector3.one;

        Vector3 bigImgScale = Vector3.one * 1.5f;

        bool topBig = false;
        bool jglBig = false;
        bool midBig = false;
        bool botBig = false;
        bool suppBig = false;


        if (top1 == topAmount)
        {
            topImg.transform.localScale = bigImgScale;
            topBig = true;
        }
        else if (top1 == jglAmount)
        {
            jglImg.transform.localScale = bigImgScale;
            jglBig = true;
        }
        else if (top1 == midAmount)
        {
            midImg.transform.localScale = bigImgScale;
            midBig = true;
        }
        else if (top1 == botAmount)
        {
            botImg.transform.localScale = bigImgScale;
            botBig = true;
        }
        else
        {
            supImg.transform.localScale = bigImgScale;
            suppBig = true;
        }

        if (top2 == topAmount && topAmount != 0 && !topBig)
            topImg.transform.localScale = bigImgScale;
        else if (top2 == jglAmount && jglAmount != 0 && !jglBig)
            jglImg.transform.localScale = bigImgScale;
        else if (top2 == midAmount && midAmount != 0 && !midBig)
            midImg.transform.localScale = bigImgScale;
        else if (top2 == botAmount && botAmount != 0 && !botBig)
            botImg.transform.localScale = bigImgScale;
        else if (top2 == utilAmount && utilAmount != 0 && !suppBig)
            supImg.transform.localScale = bigImgScale;

        float winrate = (float)won / matchAmount;
        winrate += 0.1f;
        if (winrate < 0.5f)
            winrate = 1 - winrate;
        wrImg.fillAmount = winrate;
    }
}
