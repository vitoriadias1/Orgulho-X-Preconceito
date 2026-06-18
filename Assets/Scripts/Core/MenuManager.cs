using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Importante para gerenciar botões

public class MenuManager : MonoBehaviour
{
    [Header("UI References")]
    public Button continuarBotao; // Arraste o botão "Continuar" aqui no Inspector

    private void Start()
    {
        // Se o botão de continuar foi associado no Inspector
        if (continuarBotao != null)
        {
            // O botão CONTINUAR só fica clicável se o arquivo de save já existir no PC do jogador!
            continuarBotao.interactable = System.IO.File.Exists(System.IO.Path.Combine(Application.persistentDataPath, "savegame.json"));
        }
    }

    public void NovoJogo()
    {
        // 1. IMPORTANTE: Reseta os valores para o padrão exato de 50 de Orgulho e Preconceito
        if (GameManager.Instance != null)
        {
            GameManager.Instance.pride = 50f;
            GameManager.Instance.prejudice = 50f;
            GameManager.Instance.savedDialogueIndex = 0; // Reseta a linha do diálogo para o começo
            GameManager.Instance.relationships.Clear();  // Limpa o dicionário de afeição com NPCs
        }

        // 2. OPCIONAL (Altamente recomendado): Deleta o arquivo físico do computador 
        // para que o botão "Continuar" volte a ficar cinza até o jogador salvar de novo.
        string savePath = System.IO.Path.Combine(Application.persistentDataPath, "savegame.json");
        if (System.IO.File.Exists(savePath))
        {
            System.IO.File.Delete(savePath);
        }

        // 3. Carrega a cena inicial da história do zero
        SceneManager.LoadScene("SampleScene"); 
    }

    public void ContinuarJogo()
    {
        // Tenta carregar os dados salvos no JSON
        bool sucesso = SaveSystem.LoadGame();

        if (sucesso)
        {
            Debug.Log("Save carregado com sucesso! Iniciando a cena...");
            // Carrega a cena do jogo com os pontos antigos já restaurados no GameManager
            SceneManager.LoadScene("SampleScene"); 
        }
        else
        {
            Debug.LogError("Erro crítico: Arquivo de save não encontrado.");
        }
    }

    // Função que o botão de Salvar vai chamar na SampleScene
    public void BotaoSalvarJogo(int currentLine)
    {
        // Chama a linha mágica que converte os pontos atuais do GameManager em JSON
        SaveSystem.SaveGame(currentLine); // Você pode passar o índice atual do diálogo se quiser salvar isso também
        
        Debug.Log("Jogo salvo manualmente pelo jogador!");
        
        // (Opcional) Se você tiver algum textinho de "Jogo Salvo!" piscando na tela, 
       
    }

    public void IrParaCreditos() => SceneManager.LoadScene("Final");
    public void VoltarParaMenu() => SceneManager.LoadScene("MainMenu");
} 