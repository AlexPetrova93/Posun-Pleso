using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject Tutorial;
    
    public void StartGame()
    {
        SceneManager.LoadScene("Level");
    }

    public void OpenTutorial()
    {
        
        Debug.Log("open tutorial");
        Tutorial.SetActive(true);
    }
    
    public void CloseTutorial()
    {
        Tutorial.SetActive(false);
    }
}
