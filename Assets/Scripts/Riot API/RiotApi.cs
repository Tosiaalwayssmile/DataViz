using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using UnityEngine;

public static class RiotApi
{
    static string apiKey;
    static string region;
    static string playerId;
    static string puuid;

    static HttpResponseMessage GET(string URL)
    {
        using (HttpClient client = new HttpClient())
        {
            var result = client.GetAsync(URL);
            result.Wait();
            return result.Result;
        }
    }
    static string GetURI(string path)
    {
        string regionCode;
        switch (region)
        {
            case "Brazil":
                regionCode = "br1";
                break;
            case "Europe North East":
                regionCode = "eun1";
                break;
            case "Europe West":
                regionCode = "euw1";
                break;
            case "Japan":
                regionCode = "jp1";
                break;
            case "Korea":
                regionCode = "k1";
                break;
            case "Latin America 1":
                regionCode = "la1";
                break;
            case "Latin America 2":
                regionCode = "la2";
                break;
            case "North America":
                regionCode = "na1";
                break;
            case "Oceania":
                regionCode = "oc1";
                break;
            case "Turkey":
                regionCode = "tr1";
                break;
            case "Russia":
                regionCode = "ru";
                break;
            default:
                throw new NotImplementedException("Region " + region + " not recognised");
        }
        return "https://" + regionCode + ".api.riotgames.com" + path + "?api_key=" + apiKey;
    }
    static string GetRegionURI(string uri)
    {
        uri = uri.Replace("br1", "america");
        uri = uri.Replace("eun1", "europe");
        uri = uri.Replace("euw1", "europe");
        uri = uri.Replace("jp1", "asia");
        uri = uri.Replace("k1", "asia");
        uri = uri.Replace("la1", "america");
        uri = uri.Replace("la2", "america");
        uri = uri.Replace("na1", "america");
        uri = uri.Replace("oc1", "america");
        uri = uri.Replace("tr1", "europe");
        uri = uri.Replace("ru", "europe");

        return uri;
    }
    static string GetJson(string uri)
    {
        var response = GET(uri);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            // TODO
            return null;
        }

        string content = response.Content.ReadAsStringAsync().Result;

        if (response.StatusCode != HttpStatusCode.OK)
        {
            // TODO
            return null;
        }

        return content;
    }

    static T GetDTOFromJson<T>(string path) where T : DTO
    {
        string content = GetJson(path);
        if (content == null)
            return null;

        return JsonConvert.DeserializeObject<T>(content);
    }
    static List<T> GetDTOsFromJson<T>(string path) where T : DTO
    {
        string content = GetJson(path);
        if (content == null)
            return null;

        return JsonConvert.DeserializeObject<List<T>>(content);
    }

    static T GetDTOFromApi<T>(string path) where T : DTO
    {
        string uri = GetURI(path);
        return GetDTOFromJson<T>(uri);
    }
    static List<T> GetDTOsFromApi<T>(string path) where T : DTO
    {
        string uri = GetURI(path);
        return GetDTOsFromJson<T>(uri);
    }

    static Sprite SpriteFromImg(string filePath, float pixelsPerUnit)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(fileData);

        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
    }



    static public void SetApiKey(string apiKey)
    {
        RiotApi.apiKey = apiKey;
    }
    static public void SetRegion(string region)
    {
        RiotApi.region = region;
    }
    static public void SetPlayer(string playerId)
    {
        RiotApi.playerId = playerId;
    }

    static public ChampionDTO[] GetChampsInfo()
    {
        const string champsJsonPath = "https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/v1/champion-summary.json";
        var result = GetDTOsFromJson<ChampionDTO>(champsJsonPath).Cast<ChampionDTO>().ToList();
        result.RemoveAt(0);
        return result.OrderBy(o => o.name).ToArray();
    }
    static public SummonerDTO GetSummonerInfo(string playerName)
    {
        string summonerDataPath = "/lol/summoner/v4/summoners/by-name/" + playerName;
        var playerData = GetDTOFromApi<SummonerDTO>(summonerDataPath);

        SetPlayer(playerData.id);
        puuid = playerData.puuid;

        return playerData;
    }
    static public ChampionMasteryDTO[] GetChampsMasteryInfo()
    {
        string champDetailsPath = "/lol/champion-mastery/v4/champion-masteries/by-summoner/" + playerId;
        return GetDTOsFromApi<ChampionMasteryDTO>(champDetailsPath).ToArray();
    }
    static public Dictionary<long, Sprite> DownloadChampionSprites(ChampionDTO[] dtos)
    {
        var dict = new Dictionary<long, Sprite>();
        string directory = "ChampImages\\";

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        WebClient client = new WebClient();

        foreach (var champ in dtos)
        {
            string fileName = directory + champ.id + ".jpg";
            string url = "https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/v1/champion-tiles/" + champ.id + "/" + champ.id + "000.jpg";
            client.DownloadFile(new Uri(url), fileName);
            dict.Add(champ.id, SpriteFromImg(fileName, 400f));
        }

        return dict;
    }
    static public Sprite DownloadIconSprite(int iconId)
    {
        string url = "https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/v1/profile-icons/" + iconId + ".jpg";

        string directory = "SummonerIcon\\";
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        string fileName = directory + "icon.jpg";
        WebClient client = new WebClient();
        client.DownloadFile(new Uri(url), fileName);
        return SpriteFromImg(fileName, 100f);
    }
    static public LeagueEntryDTO GetEntries(string encryptedSummonerId)
    {
        string entryPath = "/lol/league/v4/entries/by-summoner/" + encryptedSummonerId;
        var data = GetDTOsFromApi<LeagueEntryDTO>(entryPath);
        return data.FirstOrDefault();
    }
    static public MatchInfo[] GetMatches(int amount)
    {
        string url = "/lol/match/v5/matches/by-puuid/" + puuid + "/ids?start=0&count=" + amount;
        string uri = GetURI(url);
        uri = GetRegionURI(uri);
        int index = uri.LastIndexOf("?");
        StringBuilder sb = new StringBuilder(uri);
        sb[index] = '&';
        uri = sb.ToString();
        var result = GetJson(uri);
        result = result.Replace("[", "");
        result = result.Replace("]", "");
        result = result.Replace("\\", "");
        result = result.Replace("\"", "");
        string[] ids = result.Split(',');

        MatchInfo[] matches = new MatchInfo[amount];

        for (int i = 0; i < amount; i++)
        {
            url = "/lol/match/v5/matches/" + ids[i];
            uri = GetURI(url);
            uri = GetRegionURI(uri);
            MatchDTO match = GetDTOFromJson<MatchDTO>(uri);
            MatchInfo matchInfo = new MatchInfo();
            ParticipantDTO player = null;
            foreach (var p in from p in match.info.participants
                              where p.puuid.Equals(puuid)
                              select p)
            {
                player = p;
                break;
            }

            matchInfo.assists = player.assists;
            matchInfo.championId = player.championId;
            matchInfo.deaths = player.deaths;
            matchInfo.gameCreation = match.info.gameCreation;
            matchInfo.gameDuration = match.info.gameDuration;
            matchInfo.goldEarned = player.goldEarned;
            matchInfo.kills = player.kills;
            matchInfo.totalMinionsKilled = player.totalMinionsKilled;
            matchInfo.win = player.win;
            matchInfo.individualPosition = player.individualPosition;
            matchInfo.role = player.role;

            matches[i] = matchInfo;
        }

        return matches;
    }
    public static string GetMaestryScore(string summonerId)
    {
        string url = GetURI("/lol/champion-mastery/v4/scores/by-summoner/" + summonerId);
        return GetJson(url);
    }
}
