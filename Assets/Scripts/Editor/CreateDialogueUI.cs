using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class CreateDialogueUI
{
    [MenuItem("Tools/Create VN Dialogue UI")]
    public static void CreateUI()
    {
        // Create Canvas
        GameObject canvasGO = new GameObject("DialogueCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(UnityEngine.UI.GraphicRaycaster));
        Canvas canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // Ensure EventSystem exists
        if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
            EditorUtility.SetDirty(es);
        }

        // Create Panel at bottom
        GameObject panel = new GameObject("DialoguePanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(canvasGO.transform, false);
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 0f);
        rt.anchorMax = new Vector2(1f, 0.25f); // bottom 25%
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        Image img = panel.GetComponent<Image>();
        img.color = new Color(0f, 0f, 0f, 0.6f);

        // Speaker Text (top-left of panel)
        GameObject speaker = new GameObject("SpeakerText", typeof(RectTransform), typeof(Text));
        speaker.transform.SetParent(panel.transform, false);
        RectTransform srt = speaker.GetComponent<RectTransform>();
        srt.anchorMin = new Vector2(0f, 0.75f);
        srt.anchorMax = new Vector2(0.3f, 1f);
        srt.offsetMin = new Vector2(10, -10);
        srt.offsetMax = new Vector2(-10, 0);
        Text sText = speaker.GetComponent<Text>();
        sText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        sText.fontSize = 24;
        sText.alignment = TextAnchor.MiddleLeft;
        sText.color = Color.white;
        sText.text = "Speaker";

        // Dialogue Text (main body)
        GameObject dialogue = new GameObject("DialogueText", typeof(RectTransform), typeof(Text));
        dialogue.transform.SetParent(panel.transform, false);
        RectTransform drt = dialogue.GetComponent<RectTransform>();
        drt.anchorMin = new Vector2(0f, 0f);
        drt.anchorMax = new Vector2(1f, 0.75f);
        drt.offsetMin = new Vector2(10, 10);
        drt.offsetMax = new Vector2(-10, -10);
        Text dText = dialogue.GetComponent<Text>();
        dText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        dText.fontSize = 20;
        dText.alignment = TextAnchor.UpperLeft;
        dText.color = Color.white;
        dText.horizontalOverflow = HorizontalWrapMode.Wrap;
        dText.verticalOverflow = VerticalWrapMode.Truncate;
        dText.text = "Dialogue goes here...";

        // Portrait Image (left side of panel)
        GameObject portrait = new GameObject("Portrait", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        portrait.transform.SetParent(panel.transform, false);
        RectTransform prt = portrait.GetComponent<RectTransform>();
        prt.anchorMin = new Vector2(0f, 0f);
        prt.anchorMax = new Vector2(0f, 0.75f);
        prt.pivot = new Vector2(0f, 0f);
        prt.sizeDelta = new Vector2(150, 150);
        prt.anchoredPosition = new Vector2(10, 10);
        Image pImg = portrait.GetComponent<Image>();
        pImg.color = Color.white;

        // Choice Panel (right side)
        GameObject choicePanel = new GameObject("ChoicePanel", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        choicePanel.transform.SetParent(panel.transform, false);
        RectTransform cRt = choicePanel.GetComponent<RectTransform>();
        cRt.anchorMin = new Vector2(0.7f, 0f);
        cRt.anchorMax = new Vector2(1f, 0.75f);
        cRt.offsetMin = new Vector2(-10, 10);
        cRt.offsetMax = new Vector2(-10, -10);
        var vlg = choicePanel.GetComponent<VerticalLayoutGroup>();
        vlg.childForceExpandHeight = false;
        vlg.childForceExpandWidth = true;
        vlg.spacing = 8f;
        var csf = choicePanel.GetComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // Ensure Prefabs folder
        string prefabFolder = "Assets/Prefabs";
        if (!AssetDatabase.IsValidFolder(prefabFolder))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        // Create sample choice button and save as prefab
        GameObject button = new GameObject("ChoiceButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        button.transform.SetParent(choicePanel.transform, false);
        RectTransform bRt = button.GetComponent<RectTransform>();
        bRt.sizeDelta = new Vector2(0, 40);
        Image bImg = button.GetComponent<Image>();
        bImg.color = new Color(1f, 1f, 1f, 0.1f);

        GameObject btnText = new GameObject("Text", typeof(RectTransform), typeof(Text));
        btnText.transform.SetParent(button.transform, false);
        RectTransform btRt = btnText.GetComponent<RectTransform>();
        btRt.anchorMin = Vector2.zero;
        btRt.anchorMax = Vector2.one;
        btRt.offsetMin = Vector2.zero;
        btRt.offsetMax = Vector2.zero;
        Text bt = btnText.GetComponent<Text>();
        bt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        bt.alignment = TextAnchor.MiddleCenter;
        bt.color = Color.white;
        bt.text = "Choice";

        // Save prefab
        string prefabPath = prefabFolder + "/ChoiceButton.prefab";
        PrefabUtility.SaveAsPrefabAsset(button, prefabPath);

        // Remove the runtime instance of the button from the scene
        Object.DestroyImmediate(button);

        // Assign references to DialogueManager if present
        var dm = Object.FindObjectOfType<DialogueManager>();
        if (dm != null)
        {
            Undo.RecordObject(dm, "Assign Dialogue UI References");
            dm.speakerText = sText;
            dm.dialogueText = dText;
            dm.portraitImage = pImg;
            dm.choicePanel = choicePanel;
            dm.choiceButtonPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            dm.choiceButtonContainer = choicePanel.transform;
            EditorUtility.SetDirty(dm);
            Debug.Log("Assigned UI references to DialogueManager.");
        }

        EditorUtility.DisplayDialog("VN Dialogue UI", "Dialogue UI created and ChoiceButton prefab saved at " + prefabPath, "OK");
    }
}