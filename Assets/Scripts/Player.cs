﻿using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {
    
    public int PlayerNumber;
    public float HintDisplayTime = 2.0f;
    public GameObject PlayerHintBubble;
    
    public SpawnPoint UpSpawn;
    public SpawnPoint DownSpawn;
    public SpawnPoint LeftSpawn;
    public SpawnPoint RightSpawn;
    
    private GameStateManager GameManager;
    private List<GameStateManager.LevelInteraction> LevelInteractionsColliding;
    private List<string> Hints;
    private Vector3 ChatPositionDelta;
    private float TimeToHideHint = 0.0f;

	// Use this for initialization
	void Start () {
        LevelInteractionsColliding = new List<GameStateManager.LevelInteraction>();
        PlayerHintBubble.SetActive(false);
        ChatPositionDelta = PlayerHintBubble.transform.position - transform.position;
        GameManager = FindObjectOfType<GameStateManager>();
	}
	
	// Update is called once per frame
	void Update () {
	   PlayerHintBubble.transform.position = transform.position + ChatPositionDelta;
       if(PlayerHintBubble.activeSelf && Time.time > TimeToHideHint) {
           HideHint();
       }
	}
    
    public void ShowHint() {
        if(!PlayerHintBubble.activeSelf) {
            ChatBubbleController chatBubble = PlayerHintBubble.GetComponentInChildren<ChatBubbleController>();
            chatBubble.ChatText.Clear();
            chatBubble.ChatText.Add(Hints[Random.Range(0, Hints.Count)]);
            PlayerHintBubble.SetActive(true);
            TimeToHideHint = Time.time + HintDisplayTime;
        }
    }
    
    void HideHint() {
        PlayerHintBubble.SetActive(false);
    }
    
    public void AddHint(string hint) {
        Hints.Add(hint);
    }
    
    public void SetHints(List<string> hints) {
        Hints = new List<string>(hints);
    }
    
    public void ClearHints() {
        Hints = new List<string>();
    }
    
    public void AddCollidingInteraction(GameStateManager.LevelInteraction Interaction) {
        LevelInteractionsColliding.Add(Interaction);
    }
    
    public void RemoveCollidingInteraction(GameStateManager.LevelInteraction Interaction) {
        LevelInteractionsColliding.Remove(Interaction);
    }
    
    public List<GameStateManager.LevelInteraction> GetLevelInteractionsColliding() {
        return LevelInteractionsColliding;
    }
    
    //@TODO : Confirm the order in which the check should go
    public void OnDPadUp()
    {
        //Teleport to the Footbal Spawn
        if( GameManager.QueryFlag("FOOTBALL_TELEPORT") && UpSpawn!=null)
        {
            UpSpawn.TeleportPlayer(this);
        }
    }
    
    public void OnDPadDown()
    {
        //Teleport to Main Hub
        if(GameManager.QueryFlag("GRANDPA_HEIRLOOM") && DownSpawn!=null)
        {
            DownSpawn.TeleportPlayer(this);
        }
    }
    
    public void OnDPadLeft()
    {
        //Teleport to Diary
        if(GameManager.QueryFlag("") && LeftSpawn!=null)
        {
            LeftSpawn.TeleportPlayer(this);
        }
    }
    
    public void OnDPadRight()
    {
        //Teleport to Backyard
        if(GameManager.QueryFlag("") && RightSpawn!=null)
        {
            RightSpawn.TeleportPlayer(this);
        }
    }
}
