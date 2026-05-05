using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    private GameObject tutorialPanel;
    private bool dismissed = false;

    private void Start()
    {
        BuildTutorialUI();
        Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    private void Update()
    {
        if (!dismissed && Input.anyKeyDown)
            Dismiss();
    }

    private void Dismiss()
    {
        dismissed = true;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Destroy(tutorialPanel);
    }

    private void BuildTutorialUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("TutorialCanvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;
            canvasGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        tutorialPanel = new GameObject("TutorialPanel");
        tutorialPanel.transform.SetParent(canvas.transform, false);
        Image overlay = tutorialPanel.AddComponent<Image>();
        overlay.color = new Color(0f, 0f, 0f, 0.85f);
        RectTransform panelRect = tutorialPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        GameObject card = CreateCard(tutorialPanel.transform);
        PopulateCard(card.transform);
    }

    private GameObject CreateCard(Transform parent)
    {
        GameObject card = new GameObject("Card");
        card.transform.SetParent(parent, false);
        Image img = card.AddComponent<Image>();
        img.color = new Color(0.067f, 0.067f, 0.067f, 1f);
        RectTransform rt = card.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(420f, 520f);

        Outline outline = card.AddComponent<Outline>();
        outline.effectColor = new Color(0.25f, 0.25f, 0.25f, 1f);
        outline.effectDistance = new Vector2(1f, -1f);

        VerticalLayoutGroup vlg = card.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(44, 44, 40, 40);
        vlg.spacing = 0f;
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        ContentSizeFitter csf = card.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        return card;
    }

    private void PopulateCard(Transform card)
    {
        CreateLabel(card, "CONTROLS", 11f, new Color(0.4f, 0.4f, 0.4f), 0f, 8f, 120f);
        CreateLabel(card, "HOW TO PLAY", 24f, Color.white, 0f, 28f, 0f);
        CreateDivider(card, new Color(0.18f, 0.18f, 0.18f), 4f, 24f);
        CreateRow(card, "Move Left / Right", new string[] { "A", "D" }, 4f);
        CreateRow(card, "Jump", new string[] { "SPACE" }, 16f);
        CreateRow(card, "Slam Down (airborne)", new string[] { "S" }, 16f);
        CreateRow(card, "Pause", new string[] { "ESC" }, 16f);
        CreateDivider(card, new Color(0.18f, 0.18f, 0.18f), 24f, 20f);
        CreateLabel(card, "press any key to continue", 12f, new Color(0.35f, 0.35f, 0.35f), 0f, 0f, 0f);
    }

    private void CreateLabel(Transform parent, string text, float size, Color color, float topPad, float botPad, float spacing)
    {
        GameObject go = new GameObject("Label_" + text);
        go.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.characterSpacing = spacing;
        tmp.margin = new Vector4(0f, topPad, 0f, botPad);
        tmp.enableWordWrapping = false;
        LayoutElement le = go.AddComponent<LayoutElement>();
        le.preferredHeight = size + topPad + botPad + 8f;
    }

    private void CreateDivider(Transform parent, Color color, float topPad, float botPad)
    {
        GameObject spacerTop = new GameObject("SpacerTop");
        spacerTop.transform.SetParent(parent, false);
        LayoutElement leTop = spacerTop.AddComponent<LayoutElement>();
        leTop.preferredHeight = topPad;
        leTop.minHeight = topPad;

        GameObject divider = new GameObject("Divider");
        divider.transform.SetParent(parent, false);
        Image img = divider.AddComponent<Image>();
        img.color = color;
        LayoutElement le = divider.AddComponent<LayoutElement>();
        le.preferredHeight = 1f;
        le.minHeight = 1f;

        GameObject spacerBot = new GameObject("SpacerBot");
        spacerBot.transform.SetParent(parent, false);
        LayoutElement leBot = spacerBot.AddComponent<LayoutElement>();
        leBot.preferredHeight = botPad;
        leBot.minHeight = botPad;
    }

    private void CreateRow(Transform parent, string label, string[] keys, float topPad)
    {
        GameObject spacer = new GameObject("Spacer");
        spacer.transform.SetParent(parent, false);
        LayoutElement leSpacer = spacer.AddComponent<LayoutElement>();
        leSpacer.preferredHeight = topPad;
        leSpacer.minHeight = topPad;

        GameObject row = new GameObject("Row_" + label);
        row.transform.SetParent(parent, false);
        LayoutElement rowLe = row.AddComponent<LayoutElement>();
        rowLe.preferredHeight = 40f;
        rowLe.minHeight = 40f;

        HorizontalLayoutGroup hlg = row.AddComponent<HorizontalLayoutGroup>();
        hlg.childAlignment = TextAnchor.MiddleLeft;
        hlg.childControlWidth = false;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = true;
        hlg.spacing = 8f;

        GameObject labelGO = new GameObject("RowLabel");
        labelGO.transform.SetParent(row.transform, false);
        TextMeshProUGUI tmp = labelGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 14f;
        tmp.color = new Color(0.6f, 0.6f, 0.6f);
        tmp.alignment = TextAlignmentOptions.MidlineLeft;
        tmp.enableWordWrapping = false;
        LayoutElement labelLe = labelGO.AddComponent<LayoutElement>();
        labelLe.preferredWidth = 230f;
        labelLe.minWidth = 230f;
        labelGO.GetComponent<RectTransform>().sizeDelta = new Vector2(230f, 40f);

        GameObject keysContainer = new GameObject("Keys");
        keysContainer.transform.SetParent(row.transform, false);
        LayoutElement keysLe = keysContainer.AddComponent<LayoutElement>();
        keysLe.preferredWidth = 100f;
        keysLe.minWidth = 100f;
        HorizontalLayoutGroup keysHlg = keysContainer.AddComponent<HorizontalLayoutGroup>();
        keysHlg.childAlignment = TextAnchor.MiddleRight;
        keysHlg.spacing = 6f;
        keysHlg.childControlWidth = false;
        keysHlg.childControlHeight = false;
        keysHlg.childForceExpandWidth = false;
        keysHlg.childForceExpandHeight = false;
        keysContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 40f);

        foreach (string key in keys)
        {
            float keyWidth = key.Length > 3 ? 80f : key.Length > 1 ? 50f : 34f;
            CreateKeyBadge(keysContainer.transform, key, keyWidth);
        }
    }

    private void CreateKeyBadge(Transform parent, string keyText, float width)
    {
        GameObject badge = new GameObject("Key_" + keyText);
        badge.transform.SetParent(parent, false);
        Image img = badge.AddComponent<Image>();
        img.color = new Color(0.13f, 0.13f, 0.13f);
        RectTransform rt = badge.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(width, 30f);
        LayoutElement le = badge.AddComponent<LayoutElement>();
        le.preferredWidth = width;
        le.minWidth = width;
        le.preferredHeight = 30f;
        le.minHeight = 30f;
        Outline outline = badge.AddComponent<Outline>();
        outline.effectColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        outline.effectDistance = new Vector2(1f, -1f);

        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(badge.transform, false);
        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = keyText;
        tmp.fontSize = 12f;
        tmp.color = new Color(0.87f, 0.87f, 0.87f);
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        tmp.enableWordWrapping = false;
        RectTransform textRt = textGO.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;
    }
}