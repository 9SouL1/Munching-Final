using UnityEngine;
using UnityEngine.SceneManagement;
public class Button : MonoBehaviour
{
    internal object onClick;

    public void QuitGame()
    {
        Application.Quit();
    }
}

