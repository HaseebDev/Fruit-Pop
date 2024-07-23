using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class LevelSelection : MonoBehaviour
{
    public GameObject LevelPrefab;
    public int LevelCount;
    public Transform Content;
    Vector3 currentPosition;
    List<int> count = new List<int>();
    // Start is called before the first frame update
    void Start()
    {
        RectTransform rect =  Content.GetComponent<RectTransform>();
        rect.rect.Set(rect.position.x, rect.position.y, rect.rect.width, LevelCount * 350);
        count.Add(3);
        count.Add(6);
        count.Add(10);
        count.Add(15);
        count.Add(19);
        count.Add(25);
        for(int i = 0; i < LevelCount; i++)
        {
            if(i == 0)
            {
                currentPosition = new Vector3(0, -(i + 1) * 50, 0);
            }
            else
            {
                currentPosition = new Vector3(0, -(i + 1) * 350, 0);
            }
           
            if (count.Contains(i))
            {
                GameObject item1 = Instantiate(LevelPrefab, Content);
                item1.GetComponent<Transform>().localPosition = currentPosition;
                item1.GetComponent<Transform>().GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(false);
            
                GameObject item2 = Instantiate(LevelPrefab, Content);
                item2.GetComponent<Transform>().localPosition = currentPosition + new Vector3(-350, 0, 0);
                item2.GetComponent<Transform>().GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(false);
                item2.GetComponent<Transform>().GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
                currentPosition = new Vector3(0, - (i + 1) * 350, 0);

            }
            else
            {
                GameObject item =  Instantiate(LevelPrefab, Content);
                item.GetComponent<Transform>().localPosition = currentPosition;
                if (i == 0)
                {
                    item.GetComponent<Transform>().GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(false);
                }
                item.GetComponent<Transform>().GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
                item.GetComponent<Transform>().GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(false);
            }
            
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
