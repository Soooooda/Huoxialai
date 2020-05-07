using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    GameObject player;
    GameObject NPCs;
    public static int rank;
    // Start is called before the first frame update
    void Start()
    {
        
        player = GameObject.Find("Player");
        NPCs = GameObject.Find("NPCs");
    }

    // Update is called once per frame
    void Update()
    {
        rank = NPCs.transform.childCount + 1;
        NPCs = GameObject.Find("NPCs");
        if (player.GetComponent<PlayerMovement>().health > 0 && NPCs.transform.childCount == 0)
        {
            SceneManager.LoadScene("WinScene");
        }
        if (player.GetComponent<PlayerMovement>().health <= 0)
        {
            SceneManager.LoadScene("LoseScene");
        }
    }
}
