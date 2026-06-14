# 📚 Orgulho & Preconceito - Unity Project Modeling Guide

Este projeto é um simulador narrativo desenvolvido no Unity, focado em diálogos, escolhas e impacto em estatísticas de personagem (Orgulho e Preconceito) e relacionamentos.

## 🛠️ Arquitetura do Projeto

O projeto utiliza um sistema modular baseado em **ScriptableObjects** para a gestão de conteúdo, permitindo que roteiros sejam criados sem a necessidade de alterar código.

### 📁 Estrutura de Pastas
- `Assets/Scripts/Core`: Lógica central do jogo (`GameManager`, `SaveSystem`).
- `Assets/Scripts/Data`: Definições de dados (`DialogueLine`, `ChoiceData`).
- `Assets/Scripts/Dialogue`: Sistema de processamento de texto e escolhas (`DialogueManager`).
- `Assets/Scripts/Player`: Controle do jogador (`PlayerController`).
- `Assets/Scripts/UI`: Gestão da interface do usuário (`UIManager`).

---

## 🚀 Guia Passo a Passo para Modelagem no Unity

### 1. Configuração da Cena Base
Para colocar o sistema para funcionar, siga estes passos:
1. **Hierarquia**: Crie um objeto vazio chamado `GameManager` e anexe o script `GameManager.cs`.
2. **UI de Diálogo**: 
   - Crie um `Canvas` com um painel para o diálogo.
   - Adicione dois campos de texto (`Text`): um para o **Nome do Personagem** e outro para o **Texto do Diálogo**.
   - Adicione uma `Image` para o **Retrato (Portrait)** do personagem.
   - Crie um painel de escolhas (`ChoicePanel`) que comece desativado.
3. **DialogueManager**:
   - Crie um objeto `DialogueManager` na cena.
   - Arraste as referências de UI (textos, imagem, painel de escolhas) para os campos correspondentes no Inspetor do `DialogueManager`.
   - Crie um Prefab de botão simples e atribua-o ao `Choice Button Prefab`.

### 2. Criando Conteúdo de Diálogo (Roteiro)
A modelagem do jogo acontece principalmente via **ScriptableObjects**:
1. No Project Window, clique com o botão direito $\rightarrow$ `Create` $\rightarrow$ `Dialogue` $\rightarrow$ `DialogueLine`.
2. **Configure a linha**:
   - `Speaker`: Nome de quem está falando.
   - `Text`: O conteúdo da fala.
   - `Portrait`: A imagem do personagem.
   - `Voice`: (Opcional) Áudio da fala.
3. **Adicionando Escolhas**:
   - No campo `Choices`, aumente o tamanho da lista.
   - Defina o texto da escolha e quanto ela altera os níveis de **Orgulho** e **Preconceito**.
   - Adicione mudanças de relacionamento com NPCs específicos se necessário.

### 3. Implementando o Fluxo de Jogo
Para iniciar um diálogo:
1. Chame a função `StartDialogue(DialogueLine[] lines)` do `DialogueManager`.
2. Você pode criar um array de `DialogueLine` no Inspetor de um script de controle ou carregar de uma pasta de Resources.

---

## 📈 Sistema de Estatísticas e Impacto

O jogo modela a personalidade do jogador através de dois eixos:
- **Orgulho ($\text{Pride}$)**: Influencia a percepção de status e auto-estima.
- **Preconceito ($\text{Prejudice}$)**: Influencia o julgamento precipitado sobre os outros.

Cada `ChoiceData` permite definir:
- `prideChange`: Valor entre -1 e 1.
- `prejudiceChange`: Valor entre -1 e 1.
- `relationshipChanges`: Lista de NPCs e quanto a relação melhorou ou piorou.

## 🛠️ Como Expandir o Projeto

- **Novas Condições**: Modificar o `DialogueManager` para que certas linhas de diálogo só apareçam se o jogador tiver X de Orgulho.
- **Sistema de Inventário**: Adicionar um `ItemData` (ScriptableObject) e integrá-lo ao `GameManager`.
- **Navegação**: Integrar o `PlayerController` para que o diálogo seja disparado ao colidir com um NPC.

---

## 📝 Notas Técnicas
- **Typewriter Effect**: O texto é exibido letra por letra para simular leitura natural.
- **Persistência**: Utilize o `SaveSystem` para salvar as estatísticas de Orgulho/Preconceito entre sessões.
