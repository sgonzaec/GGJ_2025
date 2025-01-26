using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CharacterSelector : NetworkBehaviour
{
    public List<GameObject> characters = new List<GameObject>();
    private NetworkVariable<int> currentCharacter = new NetworkVariable<int>(0);

    private GameObject currentCharacterInstance;
    private int auxInx;

    private void Start()
    {
        if (characters.Count == 0)
        {
            Debug.LogWarning("Character list is empty. Ensure characters are assigned in the inspector.");
            return;
        }

        /*if (IsOwner) // El propietario inicializa su selección
        {
            ActiveCharacter(currentCharacter.Value);
        }*/

        // Quitar cuando se active la condicional de owner
        ActiveCharacter(currentCharacter.Value);

        currentCharacter.OnValueChanged += OnCharacterChanged;
    }

    private void OnDestroyer()
    {
        currentCharacter.OnValueChanged -= OnCharacterChanged;
    }

    public void ActiveCharacter(int index)
    {
        // Verifica que la lista no esté vacía
        if (characters == null || characters.Count == 0)
        {
            Debug.LogError("Character list is null or empty. Please assign characters in the Inspector.");
            return;
        }

        // Valida el índice
        if (index < 0 || index >= characters.Count)
        {
            Debug.LogError($"Invalid character index: {index}. Must be between 0 and {characters.Count - 1}.");
            return;
        }

        // Desactiva todos los personajes
        for (int i = 0; i < characters.Count; i++)
        {
            characters[i].SetActive(false);
        }

        // Activa el personaje
        characters[index].SetActive(true);
        auxInx = index;

        // Actualiza el índice localmente
        currentCharacter.Value = index;

        // Sincroniza si somos propietario
        if (IsOwner)
        {
            UpdateCharacterServerRpc(index);
        }
    }

    public void SelectCharacter()
    {
        DontDestroyOnLoad(characters[auxInx]);

        StartCoroutine(StartCombatScene());
    }

    [ServerRpc(RequireOwnership = true)]
    private void UpdateCharacterServerRpc(int index)
    {
        if (index >= 0 && index < characters.Count)
        {
            currentCharacter.Value = index; // Actualiza la variable de red
        }
    }

    private void OnCharacterChanged(int oldIndex, int newIndex)
    {
        if (newIndex >= 0 && newIndex < characters.Count)
        {
            // Cambia el personaje activo en todos los clientes
            foreach (var character in characters)
            {
                character.SetActive(false);
            }

            characters[newIndex].SetActive(true);
        }
    }

    public void SwitchToPreviousCharacter()
    {
        // Quitar comentario cuando este en la red
        //if (!IsOwner) return; // Solo el propietario puede cambiar su personaje

        int previousIndex = currentCharacter.Value - 1;
        if (previousIndex < 0)
        {
            previousIndex = characters.Count - 1;
        }

        ActiveCharacter(previousIndex);
    }

    public void SwitchToNextCharacter()
    {
        // Quitar comentario cuando este en la red
        //if (!IsOwner) return; // Solo el propietario puede cambiar su personaje

        int nextIndex = (currentCharacter.Value + 1) % characters.Count;

        ActiveCharacter(nextIndex);
    }

    public IEnumerator StartCombatScene()
    {
        // Carga la escena de combate
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Additive);

        // Esperar a que la escena del combate cargue
        asyncLoad.completed += (AsyncOperation operation) =>
        {
            // Apagamos la escena de selección de personajes
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        };

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
