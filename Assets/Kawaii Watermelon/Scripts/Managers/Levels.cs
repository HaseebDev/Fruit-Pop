using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new level",menuName ="Levels/Create Level")]
public class Levels : ScriptableObject
{
    public List<FruitDictionaryItem> fruit = new List<FruitDictionaryItem>();
   
}
[Serializable]
public class FruitDictionaryItem
{
    public int collect;
    public Sprite value;
    public FruitType fruitType;
}