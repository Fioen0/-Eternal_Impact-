using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string gameplaySceneName = "SampleScene";
    private Font uiFont;

    private void Start()
    {
        EnsureMenuCamera();
        uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        CreateEventSystemIfNeeded();
        BuildMenu();
    }

    private void EnsureMenuCamera()
    {
        if (Camera.main != null) return;

        GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0f, 0f, -10f);

        Camera menuCamera = cameraObject.GetComponent<Camera>();
        menuCamera.orthographic = true;
        menuCamera.clearFlags = CameraClearFlags.SolidColor;
        menuCamera.backgroundColor = new Color(0.035f, 0.055f, 0.09f, 1f);
    }

    private void CreateEventSystemIfNeeded()
    {
        if (EventSystem.current != null) return;

        GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
    }

    private void BuildMenu()
    {
        GameObject canvasObject = new GameObject("MainMenuCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        Image backdrop = CreateImage("Backdrop", canvasObject.transform, new Color(0.035f, 0.055f, 0.09f, 1f));
        Stretch(backdrop.rectTransform);

        Image accent = CreateImage("AccentPanel", canvasObject.transform, new Color(0.08f, 0.13f, 0.19f, 0.92f));
        RectTransform panel = accent.rectTransform;
        panel.anchorMin = new Vector2(0.5f, 0.5f);
        panel.anchorMax = new Vector2(0.5f, 0.5f);
        panel.pivot = new Vector2(0.5f, 0.5f);
        panel.sizeDelta = new Vector2(620, 570);
        panel.anchoredPosition = Vector2.zero;

        Text title = CreateText("Title", canvasObject.transform, "ETERNAL IMPACT", 58, FontStyle.Bold, new Color(0.92f, 0.78f, 0.43f));
        Place(title.rectTransform, new Vector2(0.5f, 0.63f), new Vector2(0.5f, 0.63f), new Vector2(900, 100));
        title.alignment = TextAnchor.MiddleCenter;

        Text subtitle = CreateText("Subtitle", canvasObject.transform, "DESCEND INTO THE UNKNOWN", 20, FontStyle.Normal, new Color(0.68f, 0.75f, 0.82f));
        Place(subtitle.rectTransform, new Vector2(0.5f, 0.56f), new Vector2(0.5f, 0.56f), new Vector2(700, 50));
        subtitle.alignment = TextAnchor.MiddleCenter;

        CreateButton(canvasObject.transform, "START GAME", new Vector2(0.5f, 0.43f), StartGame);
        CreateButton(canvasObject.transform, "QUIT GAME", new Vector2(0.5f, 0.33f), Application.Quit);

        Text footer = CreateText("Footer", canvasObject.transform, "A DUNGEON RUN BEGINS", 14, FontStyle.Normal, new Color(0.48f, 0.57f, 0.67f));
        Place(footer.rectTransform, new Vector2(0.5f, 0.20f), new Vector2(0.5f, 0.20f), new Vector2(600, 36));
        footer.alignment = TextAnchor.MiddleCenter;
    }

    private void CreateButton(Transform parent, string label, Vector2 anchor, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonObject = new GameObject(label + "Button", typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        Place(rect, anchor, anchor, new Vector2(360, 68));

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.67f, 0.49f, 0.22f, 1f);
        Button button = buttonObject.GetComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.67f, 0.49f, 0.22f, 1f);
        colors.highlightedColor = new Color(0.84f, 0.66f, 0.34f, 1f);
        colors.pressedColor = new Color(0.48f, 0.34f, 0.16f, 1f);
        button.colors = colors;
        button.onClick.AddListener(onClick);

        Text text = CreateText(label + "Label", buttonObject.transform, label, 26, FontStyle.Bold, Color.white);
        Stretch(text.rectTransform);
        text.alignment = TextAnchor.MiddleCenter;
        text.raycastTarget = false;
    }

    private Image CreateImage(string objectName, Transform parent, Color color)
    {
        GameObject item = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        item.transform.SetParent(parent, false);
        Image image = item.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private Text CreateText(string objectName, Transform parent, string value, int size, FontStyle style, Color color)
    {
        GameObject item = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        item.transform.SetParent(parent, false);
        Text text = item.GetComponent<Text>();
        text.font = uiFont;
        text.text = value;
        text.fontSize = size;
        text.fontStyle = style;
        text.color = color;
        text.raycastTarget = false;
        return text;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static void Place(RectTransform rect, Vector2 min, Vector2 max, Vector2 size)
    {
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = Vector2.zero;
    }

    private void StartGame()
    {
        if (Application.CanStreamedLevelBeLoaded(gameplaySceneName))
            SceneManager.LoadScene(gameplaySceneName);
        else
            Debug.LogError($"빌드 설정에서 '{gameplaySceneName}' 씬을 찾을 수 없습니다.", this);
    }
}
