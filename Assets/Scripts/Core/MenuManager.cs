using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void IrParaCreditos() => SceneManager.LoadScene("Final");
    public void VoltarParaMenu() => SceneManager.LoadScene("MainMenu");
}