using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class MedievalSceneGenerator : EditorWindow
{
    [MenuItem("Tools/Generate Medieval Scene")]
    public static void GenerateMedievalScene()
    {
        // Pega a cena ativa
        var scene = EditorSceneManager.GetActiveScene();
        
        // Limpa objetos existentes (opcional - descomente para limpar a cena)
        // ClearScene();
        
        // Cria a rua principal
        CreateMainStreet();
        
        // Cria as casas
        CreateHouses();
        
        // Cria algumas decorações
        CreateDecorations();
        
        // Salva a cena
        EditorSceneManager.SaveScene(scene);
        
        Debug.Log("Cena Medieval criada com sucesso!");
    }
    
    static void CreateMainStreet()
    {
        // Cria o piso da rua principal
        GameObject street = new GameObject("MainStreet");
        Mesh roadMesh = CreateRoadMesh();
        
        MeshFilter meshFilter = street.AddComponent<MeshFilter>();
        meshFilter.mesh = roadMesh;
        
        MeshRenderer meshRenderer = street.AddComponent<MeshRenderer>();
        meshRenderer.material = GetOrCreateMaterial("RoadMaterial", new Color(0.6f, 0.5f, 0.4f));
        
        MeshCollider meshCollider = street.AddComponent<MeshCollider>();
        meshCollider.convex = false;
        
        street.transform.position = Vector3.zero;
    }
    
    static Mesh CreateRoadMesh()
    {
        Mesh mesh = new Mesh();
        
        // Cria um caminho principal longo
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-3, 0, -15),
            new Vector3(3, 0, -15),
            new Vector3(3, 0, 15),
            new Vector3(-3, 0, 15)
        };
        
        int[] triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        
        return mesh;
    }
    
    static void CreateHouses()
    {
        // Casa 1 - Esquerda da rua
        CreateHouse("House_01", new Vector3(-8, 0, -5), Color.HSVToRGB(0.05f, 0.3f, 0.6f));
        
        // Casa 2 - Direita da rua
        CreateHouse("House_02", new Vector3(8, 0, 0), Color.HSVToRGB(0.08f, 0.35f, 0.65f));
        
        // Casa 3 - Esquerda da rua (mais ao fundo)
        CreateHouse("House_03", new Vector3(-8, 0, 8), Color.HSVToRGB(0.07f, 0.32f, 0.62f));
        
        // Casa 4 - Direita da rua (mais ao fundo)
        CreateHouse("House_04", new Vector3(8, 0, 8), Color.HSVToRGB(0.06f, 0.3f, 0.58f));
    }
    
    static void CreateHouse(string name, Vector3 position, Color color)
    {
        GameObject house = new GameObject(name);
        house.transform.position = position;
        
        // Corpo principal da casa
        GameObject walls = GameObject.CreatePrimitive(PrimitiveType.Cube);
        walls.name = "Walls";
        walls.transform.SetParent(house.transform);
        walls.transform.localPosition = Vector3.zero;
        walls.transform.localScale = new Vector3(3, 3, 3);
        
        MeshRenderer renderer = walls.GetComponent<MeshRenderer>();
        renderer.material = GetOrCreateMaterial(name + "_WallsMat", color);
        
        DestroyImmediate(walls.GetComponent<BoxCollider>());
        
        // Telhado (pirâmide simples feita com cone)
        GameObject roof = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        roof.name = "Roof";
        roof.transform.SetParent(house.transform);
        roof.transform.localPosition = new Vector3(0, 2, 0);
        roof.transform.localScale = new Vector3(3.2f, 1.2f, 3.2f);
        roof.transform.localRotation = Quaternion.identity;
        
        MeshRenderer roofRenderer = roof.GetComponent<MeshRenderer>();
        roofRenderer.material = GetOrCreateMaterial(name + "_RoofMat", new Color(0.4f, 0.2f, 0.1f));
        
        DestroyImmediate(roof.GetComponent<CapsuleCollider>());
        
        // Porta (pequeno cubo)
        GameObject door = GameObject.CreatePrimitive(PrimitiveType.Cube);
        door.name = "Door";
        door.transform.SetParent(house.transform);
        door.transform.localPosition = new Vector3(0, 0, 1.51f);
        door.transform.localScale = new Vector3(0.8f, 1.2f, 0.2f);
        
        MeshRenderer doorRenderer = door.GetComponent<MeshRenderer>();
        doorRenderer.material = GetOrCreateMaterial(name + "_DoorMat", new Color(0.3f, 0.15f, 0.05f));
        
        DestroyImmediate(door.GetComponent<BoxCollider>());
        
        // Janelas (cubos pequenos)
        CreateWindow(house.transform, new Vector3(-0.8f, 0.5f, 1.51f), name);
        CreateWindow(house.transform, new Vector3(0.8f, 0.5f, 1.51f), name);
        CreateWindow(house.transform, new Vector3(-1.51f, 0.5f, 0), name);
    }
    
    static void CreateWindow(Transform parent, Vector3 localPosition, string houseName)
    {
        GameObject window = GameObject.CreatePrimitive(PrimitiveType.Cube);
        window.name = "Window";
        window.transform.SetParent(parent);
        window.transform.localPosition = localPosition;
        window.transform.localScale = new Vector3(0.4f, 0.4f, 0.1f);
        
        MeshRenderer windowRenderer = window.GetComponent<MeshRenderer>();
        windowRenderer.material = GetOrCreateMaterial(houseName + "_WindowMat", new Color(0.7f, 0.85f, 1f));
        
        DestroyImmediate(window.GetComponent<BoxCollider>());
    }
    
    static void CreateDecorations()
    {
        // Árvore 1
        CreateTree("Tree_01", new Vector3(-12, 0, -8));
        
        // Árvore 2
        CreateTree("Tree_02", new Vector3(12, 0, 5));
        
        // Poço no centro da rua
        CreateWell("Well", new Vector3(0, 0, 0));
    }
    
    static void CreateTree(string name, Vector3 position)
    {
        GameObject tree = new GameObject(name);
        tree.transform.position = position;
        
        // Tronco
        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.name = "Trunk";
        trunk.transform.SetParent(tree.transform);
        trunk.transform.localPosition = Vector3.zero;
        trunk.transform.localScale = new Vector3(0.5f, 2, 0.5f);
        
        MeshRenderer trunkRenderer = trunk.GetComponent<MeshRenderer>();
        trunkRenderer.material = GetOrCreateMaterial(name + "_TrunkMat", new Color(0.4f, 0.25f, 0.1f));
        
        DestroyImmediate(trunk.GetComponent<CapsuleCollider>());
        
        // Folhagem
        GameObject foliage = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        foliage.name = "Foliage";
        foliage.transform.SetParent(tree.transform);
        foliage.transform.localPosition = new Vector3(0, 2.5f, 0);
        foliage.transform.localScale = Vector3.one * 2;
        
        MeshRenderer foliageRenderer = foliage.GetComponent<MeshRenderer>();
        foliageRenderer.material = GetOrCreateMaterial(name + "_FoliageMat", new Color(0.2f, 0.5f, 0.2f));
        
        DestroyImmediate(foliage.GetComponent<SphereCollider>());
    }
    
    static void CreateWell(string name, Vector3 position)
    {
        GameObject well = new GameObject(name);
        well.transform.position = position;
        
        // Cilindro do poço
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.name = "WellCylinder";
        cylinder.transform.SetParent(well.transform);
        cylinder.transform.localPosition = Vector3.zero;
        cylinder.transform.localScale = new Vector3(1.5f, 0.5f, 1.5f);
        
        MeshRenderer cylinderRenderer = cylinder.GetComponent<MeshRenderer>();
        cylinderRenderer.material = GetOrCreateMaterial(name + "_StoneMat", new Color(0.5f, 0.5f, 0.5f));
        
        DestroyImmediate(cylinder.GetComponent<CapsuleCollider>());
    }
    
    static Material GetOrCreateMaterial(string materialName, Color color)
    {
        Material mat = new Material(Shader.Find("Standard"));
        mat.name = materialName;
        mat.color = color;
        return mat;
    }
    
    static void ClearScene()
    {
        GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.scene.name != null)
            {
                DestroyImmediate(obj);
            }
        }
    }
}
