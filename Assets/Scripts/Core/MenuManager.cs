using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 
using System.IO; 

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
            continuarBotao.interactable = File.Exists(Path.Combine(Application.persistentDataPath, "savegame.json"));
        }
    }

    public void NovoJogo()
    {
        // Deleta o arquivo físico para garantir um começo limpo
        string savePath = Path.Combine(Application.persistentDataPath, "savegame.json");
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }

        if (GameManager.Instance != null)
        {
            // Força os valores limpos no Singleton que está na memória
            GameManager.Instance.ResetarParaNovoJogo();
        }

        // Carrega a cena inicial do jogo
        SceneManager.LoadScene("SampleScene"); 
    }

    public void ContinuarJogo()
    {
        string savePath = Path.Combine(Application.persistentDataPath, "savegame.json");

        if (File.Exists(savePath))
        {
            // 1. 🔥 CORRIGIDO: O SaveSystem cuida de ler o arquivo e carregar os dados para a memória
            if (SaveSystem.LoadGame())
            {
                // 2. 🔥 Buscamos o nome exato da cena salva de forma limpa através do SaveSystem
                string cenaSalva = SaveSystem.ObterCenaSalva();

                // 3. Valida se o nome da cena não veio vazio por segurança
                if (!string.IsNullOrEmpty(cenaSalva))
                {
                    Debug.Log($"💾 Carregando save! Indo para a cena: {cenaSalva}");
                    SceneManager.LoadScene(cenaSalva);
                }
                else
                {
                    // Caso o arquivo exista mas não tenha o nome da cena, vai para a padrão
                    Debug.LogWarning("⚠️ Arquivo de save encontrado, mas sem nome de cena. Indo para a SampleScene.");
                    SceneManager.LoadScene("SampleScene");
                }
            }
        }
        else
        {
            Debug.LogWarning("❌ Nenhum arquivo de save encontrado para continuar!");
        }
    }

    public void IrParaCreditos() => SceneManager.LoadScene("Final");
    public void VoltarParaMenu() => SceneManager.LoadScene("MainMenu");
}