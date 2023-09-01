using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject panelLoss;
    [SerializeField] private GameObject panelWin;
    public float timeBoss;

    public NetworkVariable<int> score = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        panelLoss.SetActive(false);
        panelWin.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "" + score.Value;
        timeBoss -= Time.deltaTime;
        if (timeBoss<-1)
        {
            if (GameObject.FindWithTag("Enemy") == null)
            {
                GameWin();
            }
        }
        
    }

    public void AddScore(int _score)
    {
        score.Value += _score;
    }

    public void GameLoss()
    {
        panelLoss.SetActive(true);
        Time.timeScale = 0;
    }


    public void GameWin()
    {
        panelWin.SetActive(true);
        Time.timeScale = 0;
    }
}
