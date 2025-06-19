using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUISpawwn : MonoBehaviour
{
    public List<GameObject> monsterPrefabs;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnGUI()
    {
        if(GUI.Button(new Rect(0, 0, 100, 50), "生成手怪"))
        {
            Vector2 pos = Random.insideUnitCircle * 30;
            Vector3 pos3d = new Vector3(pos.x, 1, pos.y);
            Instantiate(monsterPrefabs[0], pos3d, Quaternion.identity);
        }
        if (GUI.Button(new Rect(0, 50, 100, 50), "生成南瓜怪"))
        {
            Vector2 pos = Random.insideUnitCircle * 30;
            Vector3 pos3d = new Vector3(pos.x, 1, pos.y);
            Instantiate(monsterPrefabs[1], pos3d, Quaternion.identity);
        }
    }
}
