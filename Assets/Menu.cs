using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public Transform firstMenu;
    public Transform secondMenu;
    public Transform thirdMenu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LevelSelect()
    {
        firstMenu.gameObject.SetActive(false);
        secondMenu.gameObject.SetActive(true);
    }

    public void Tutorial()
    {
        firstMenu.gameObject.SetActive(false);
        thirdMenu.gameObject.SetActive(true);
    }

    public void MainMenuFromLevel()
    {
        firstMenu.gameObject.SetActive(true);
        secondMenu.gameObject.SetActive(false);
    }

    public void LoadLevel(int LevelNumber)
    {
        SceneManager.LoadScene("Level "+LevelNumber);
    }

    public void MainMenuFromTutorial()
    {
        firstMenu.gameObject.SetActive(true);
        thirdMenu.gameObject.SetActive(false);
    }
}
