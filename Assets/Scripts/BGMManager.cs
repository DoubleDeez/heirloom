﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BGMManager : MonoBehaviour
{
    public Player teenager;
    public Player grandpa;

    private string[] scene_list = {
        "KidHouseBGM",
        "WW2BGM",
        "WW2BarBGM",
        "NightStreetBGM",
        "GrandpaHouseBGM",
        "BackyardBGM",
        "SexyTimeClosetBGM",
        "BabyRoomGrampsBGM"
    };
    private Dictionary<string, AudioSource> scene_bgms =
        new Dictionary<string, AudioSource>();
    private Dictionary<string, string> player_scene_map =
        new Dictionary<string, string>();

    // Use this for initialization
    void Start()
    {
        foreach (string scene_name in scene_list)
        {
            AudioSource bgm = GameObject.Find(scene_name).GetComponent<AudioSource>();
            bgm.volume = 0;
            scene_bgms.Add(scene_name, bgm);
        }
        player_scene_map.Add("House", "KidHouseBGM");
        player_scene_map.Add("SexyTime", "SexyTimeClosetBGM");
        player_scene_map.Add("BarWhispers", "WW2BarBGM");
        player_scene_map.Add("BabyRoom", "BabyRoomGrampsBGM");
        player_scene_map.Add("CarStreet", "NightStreetBGM");
    }

    // Update is called once per frame
    void Update()
    {
        scene_bgms[player_scene_map[teenager.CurrentLevel]].volume = 1;
        scene_bgms[player_scene_map[grandpa.CurrentLevel]].volume = 1;
    }
}
