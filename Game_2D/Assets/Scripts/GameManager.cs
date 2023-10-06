using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using static Cinemachine.DocumentationSortingAttribute;
using System.IO;

public class GameManager : MonoBehaviour
{
    private bool running;
    public static GameManager Instance;
    private GameData gameData;

    [Header ("UI Player")]
    public GameObject livesUI;
    public PlayerController player;
    public TMP_Text textCoin;
    public int coin;

    [Header ("Manager Level")]
    public TMP_Text saveGameText;
    public bool advancingLevel;
    public int currentLevel;
    public List<Transform> positionsAdvanced = new List<Transform>();
    public List<Transform> positionsBack = new List<Transform>();
    public GameObject transitionPanel;

    [Header ("Panels")]
    public GameObject panelPause;
    public GameObject panelGameOver;
    public GameObject panelLoad;
    public GameObject panelSelect;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);

        LoadGame();
    }

    public void ActiveTransitionPanel()
    {
        transitionPanel.GetComponent<Animator>().SetTrigger("Disguise");
    }

    public void ChangePlayerPosition()
    {
        if (advancingLevel)
        {
            if (currentLevel + 1 < positionsAdvanced.Count)
            {
                player.transform.position = positionsAdvanced[currentLevel + 1].transform.position;
                player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                player.GetComponent<Animator>().SetBool("IsWalk", false);
                player.finalMap = false;
            }
        }
        else
        {
            if (positionsBack.Count < currentLevel - 1)
            {
                player.transform.position = positionsBack[currentLevel - 1].transform.position;
                player.GetComponent<Rigidbody2D>().velocity += Vector2.zero;
                player.GetComponent<Animator>().SetBool("IsWalk", false);
                player.finalMap = false;
            }
        }
    }

    /*public void SaveGame()
    {
        float x, y;
        x = player.transform.position.x;
        y = player.transform.position.y;

        int live = player.lives;
        string nameScene = SceneManager.GetActiveScene().name;

        PlayerPrefs.SetInt("Coins", coin);
        PlayerPrefs.SetFloat("x", x);
        PlayerPrefs.SetFloat("y", y);
        PlayerPrefs.SetInt("Lives", live);
        PlayerPrefs.SetString("NameScene", nameScene);

        if (!running)
            StartCoroutine(ShowSavedText());
    }*/

    public void SaveGame()
    {
        gameData = new GameData();
        gameData.coins = coin;
        gameData.playerX = player.transform.position.x;
        gameData.playerY = player.transform.position.y;
        gameData.lives = player.lives;
        gameData.currentLevel = SceneManager.GetActiveScene().name;

        string jsonData = JsonUtility.ToJson(gameData);
        File.WriteAllText("save.json", jsonData);

        if (!running)
            StartCoroutine(ShowSavedText());
    }

    public void LoadGame()
    {
        if (File.Exists("save.json"))
        {
            string jsonData = File.ReadAllText("save.json");

            gameData = JsonUtility.FromJson<GameData>(jsonData);

            coin = gameData.coins;
            player.transform.position = new Vector2(gameData.playerX, gameData.playerY);
            player.lives = gameData.lives;
            textCoin.text = coin.ToString();

            SceneManager.LoadScene(gameData.currentLevel);

            int discountedLives = 3 - player.lives;
            player.UpdateLivesUI(discountedLives);
        }
    }

    private IEnumerator ShowSavedText()
    {
        running = true;
        saveGameText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        saveGameText.gameObject.SetActive(false);
        running = false;
    } 

    public void LoadLevel(string nameLevel)
    {
        SceneManager.LoadScene(nameLevel);
    }

    /*public void LoadingGame()
    {
        coin = PlayerPrefs.GetInt("Coins");
        player.transform.position = new Vector2(PlayerPrefs.GetFloat("x"), PlayerPrefs.GetFloat("y"));
        player.lives = PlayerPrefs.GetInt("Lives");
        textCoin.text = coin.ToString();
        //if(PlayerPrefs.GetString("NameScene") == string.Empty)
        //    SceneManager.LoadScene("Level_Select");
        //else
        //    SceneManager.LoadScene(PlayerPrefs.GetString("NameScene"));

        int discountedLives = 3 - player.lives;

        player.UpdateLivesUI(discountedLives);
    }*/

    //Contador de monedas
    public void UpdateCoinCounter()
    {
        coin++;
        textCoin.text = coin.ToString();
    }

    //Pausa el juego
    public void Pause_Game()
    {
        Time.timeScale = 0;
        panelPause.SetActive(true);
    }

    public void Continue_Game()
    {
        Time.timeScale = 1;
        panelPause.SetActive(false);
    }

    public void Back_to_Menu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Main_Menu");
    }
    
    /*public void Load_Selector()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Level_Select");
    }

    public void Load_Scene(string scene_To_Load)
    {
        SceneManager.LoadScene(scene_To_Load);
    }*/

    public void GameOver()
    {
        panelGameOver.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Load_Scene_Selector()
    {
        StartCoroutine(Load_Scene());
    }

    private IEnumerator Load_Scene()
    {
        panelLoad.SetActive(true);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Level_Select");

        //Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
