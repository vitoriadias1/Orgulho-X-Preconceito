using UnityEngine;
using UnityEngine.SceneManagement; // Biblioteca necessária para mudar de cena

public class MenuPrincipal : MonoBehaviour
{
    [Header("Configurações das Telas")]
    public GameObject telaInicio; // Arraste a pasta Tela_Inicio para cá no Inspector
    public GameObject telaMenu;   // Arraste a pasta Tela_Menu (ou Painel_Menu) para cá

    // 1. Método para o primeiro botão ("Clique aqui para começar")
    public void AbrirMenuDeOpcoes()
    {
        if (telaInicio != null && telaMenu != null)
        {
            telaInicio.SetActive(false); // Esconde o título e o botão inicial
            telaMenu.SetActive(true);    // Mostra o véu de fundo e os novos botões
        }
    }

    // 2. Método que será chamado quando o jogador clicar em "NOVO JOGO"
    public void Jogar()
    {
    
        SceneManager.LoadScene("SampleScene");
    }

    // 3. Método para fechar o jogo (pode usar no botão Sair, se criar um)
    /*public void SairDoJogo()
    {
        Application.Quit();
        Debug.Log("O jogo foi fechado!");
    } */
}