using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MenuUI : NetworkBehaviour
{
    [SerializeField] private GameObject panelMenu;
    [SerializeField] private GameObject spawnEnemy;
    [SerializeField] private GameObject gameManager;
    [SerializeField] private GameObject panelGame;

    private void Awake()
    {
        spawnEnemy.SetActive(false);
        gameManager.SetActive(false);
        panelGame.SetActive(false);
    }

    public void HostClick()
    {
        NetworkManager.Singleton.StartHost();
        spawnEnemy.SetActive(true);
        HideMenu();
    }

    public void ServerClick()
    {
        NetworkManager.Singleton.StartServer();
        HideMenu();
    }

    public void ClientClick()
    {
        NetworkManager.Singleton.StartClient();
        HideMenu();
    }

    private void HideMenu()
    {
        panelMenu.SetActive(false);
        gameManager.SetActive(true);
        panelGame.SetActive(true);
    }
}
