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
        // Deleta o arquivo físico para garantir que não haja trapaça de carregamento
        string savePath = System.IO.Path.Combine(Application.persistentDataPath, "savegame.json");
        if (System.IO.File.Exists(savePath))
        {
            System.IO.File.Delete(savePath);
        }

        if (GameManager.Instance != null)
        {
            // Força os valores limpos no Singleton que está na memória
            GameManager.Instance.pride = 50f;
            GameManager.Instance.prejudice = 50f;
            GameManager.Instance.savedDialogueIndex = 0;
            GameManager.Instance.relationships.Clear();

            // precisamos avisar para ela que os pontos voltaram para 50!
            GameManager.Instance.OnStatsChanged?.Invoke(50f, 50f);
        }

        // Carrega a cena do jogo
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