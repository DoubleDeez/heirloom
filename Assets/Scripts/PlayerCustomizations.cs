﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCustomizations : MonoBehaviour {
    
    public bool AllowRandomization;
    
    public List<Sprite> Eyes; //Go at Y=1
    public int EyeSelection = 0;
    
    public List<Sprite> HairStyles;
    public int HairSelection = 0;
    
    public List<Sprite> Hats;
    public int HatSelection = 0;
    
    public List<Sprite> Tattoos;
    public int TattooSelection = 0;
    
    private bool IsInit = false;
    public GameObject PlayerHair;
    public GameObject PlayerEyes;
    public GameObject PlayerHat; //Sprite can be null
    public GameObject PlayerTattoo; //Sprite can be null
    
    private SpriteRenderer HairSprite;
    private SpriteRenderer EyesSprite;
    private SpriteRenderer HatSprite;
    private SpriteRenderer TattooSprite;

	// Use this for initialization
	void Start () 
    {
	   
	}
    
    void OnEnable()
    {
        HairSprite = PlayerHair.GetComponent<SpriteRenderer>();
        EyesSprite = PlayerEyes.GetComponent<SpriteRenderer>();
        HatSprite = PlayerHat.GetComponent<SpriteRenderer>();
        TattooSprite = PlayerTattoo.GetComponent<SpriteRenderer>();
        
        if(!IsInit)
        {
            if(AllowRandomization)
            {
                HairSelection = (int) (Random.value*int.MaxValue) % HairStyles.Count;
                EyeSelection = (int) (Random.value*int.MaxValue) % Eyes.Count;
                // HatSelection = (int) (Random.value*1000) % Hats.Count;
                TattooSelection = (int) (Random.value*int.MaxValue) % Tattoos.Count;
            }
            HairSprite.sprite = HairStyles[HairSelection];
            EyesSprite.sprite = Eyes[EyeSelection];
            HatSprite.sprite = Hats[HatSelection];
            TattooSprite.sprite = Tattoos[TattooSelection];
        }
    }
	
	// Update is called once per frame
	void Update () 
    {
       
	}
    
    public void ChangeHair(int selection)
    {
        HairSelection = selection;
        HairSprite.sprite = HairStyles[HairSelection];
    }
    
    public void ChangeEyes(int selection)
    {
        EyeSelection=selection;
        EyesSprite.sprite = Eyes[EyeSelection];
    }
    
    public void ChangeHat(int selection)
    {
        HatSelection = selection;
        HatSprite.sprite = Hats[HatSelection];
    }
    
    public void ChangeTattoo(int selection)
    {
        TattooSelection = selection;
        TattooSprite.sprite = Tattoos[TattooSelection];
    }
    
    public void RandomizeHair()
    {
        HairSelection = (int) (Random.value*int.MaxValue) % HairStyles.Count;
        HairSprite.sprite = HairStyles[HairSelection];
    }
    
    public void RandomizeTattoo()
    {
        TattooSelection = (int) (Random.value*int.MaxValue) % Tattoos.Count;
        TattooSprite.sprite = Tattoos[TattooSelection];
    }
    
    public void RandomizeEyes()
    {
        EyeSelection = (int) (Random.value*int.MaxValue) % Eyes.Count;
        EyesSprite.sprite = Eyes[EyeSelection];
    }
    
}
