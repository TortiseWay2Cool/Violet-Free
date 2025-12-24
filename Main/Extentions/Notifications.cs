using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VioletTemplate.Main.Extentions
{
    public class Notifications : MonoBehaviour
    {
        private static Notifications instance;
        private Canvas canvas;
        private GameObject prefab;
        private List<GameObject> active = new List<GameObject>();

        public float displayTime = 1.5f;
        public float slideTime = 0.25f;
        public Vector2 offset = new Vector2(12, 30);
        public int maxNotifications = 6;

        private void Awake()
        {
            if (instance != null) { Destroy(gameObject); return; }
            instance = this;
            DontDestroyOnLoad(gameObject);
            CreateCanvas();
            CreatePrefab();
        }

        private void CreateCanvas()
        {
            var c = new GameObject("VioletNotifyCanvas");
            canvas = c.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;
            c.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            c.AddComponent<GraphicRaycaster>();
        }

        private void CreatePrefab()
        {
            prefab = new GameObject("VioletNotif");
            prefab.SetActive(false);
            prefab.AddComponent<CanvasGroup>();

            var bg = prefab.AddComponent<Image>();
            bg.color = new Color(0.05f, 0.05f, 0.1f, 0.94f);

            var outline = prefab.AddComponent<Outline>();
            outline.effectColor = new Color(0.7f, 0.5f, 1f, 1f);
            outline.effectDistance = new Vector2(0.7f, -0.7f);

            var barBg = new GameObject("Bar").AddComponent<Image>();
            barBg.transform.SetParent(prefab.transform, false);
            barBg.color = Color.black;
            var barRect = barBg.rectTransform;
            barRect.anchorMin = new Vector2(0, 1);
            barRect.anchorMax = new Vector2(1, 1);
            barRect.offsetMin = new Vector2(6, -12);
            barRect.offsetMax = new Vector2(-6, -2);

            var fill = new GameObject("Fill").AddComponent<Image>();
            fill.transform.SetParent(barBg.transform, false);
            fill.color = Color.black;
            var fillRect = fill.rectTransform;
            fillRect.anchorMin = new Vector2(0, 0);
            fillRect.anchorMax = new Vector2(1, 1);
            fillRect.sizeDelta = Vector2.zero;
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillAmount = 1f;

            var title = new GameObject("Title").AddComponent<TextMeshProUGUI>();
            title.transform.SetParent(barBg.transform, false);
            title.text = "Violet Free";
            title.fontStyle = FontStyles.Bold;
            title.fontSize = 8;
            title.color = Color.white;
            title.alignment = TextAlignmentOptions.Top;
            var titleRect = title.rectTransform;
            titleRect.anchorMin = titleRect.anchorMax = new Vector2(0, 1);
            titleRect.pivot = new Vector2(0, 1);
            titleRect.anchoredPosition = new Vector2(-25, 0);

            var text = new GameObject("Text").AddComponent<TextMeshProUGUI>();
            text.transform.SetParent(prefab.transform, false);
            text.fontSize = 11;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;
            var textRect = text.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(8, 8);
            textRect.offsetMax = new Vector2(-8, -14);

            var desc = new GameObject("Description").AddComponent<TextMeshProUGUI>();
            desc.transform.SetParent(prefab.transform, false);
            desc.fontSize = 7;
            desc.color = new Color(0.85f, 0.85f, 1f);
            desc.alignment = TextAlignmentOptions.Bottom;
            var descRect = desc.rectTransform;
            descRect.anchorMin = Vector2.zero;
            descRect.anchorMax = Vector2.one;
            descRect.offsetMin = new Vector2(8, 2);
            descRect.offsetMax = new Vector2(-0, 10);

            prefab.GetComponent<RectTransform>().sizeDelta = new Vector2(152, 38);

            DontDestroyOnLoad(prefab);
        }

        public static void Show(string msg, string description = "", Color? col = null)
        {
            if (instance) instance.StartCoroutine(instance.DoNotif(msg, description, col ?? Color.white));
        }

        private IEnumerator DoNotif(string msg, string description, Color textCol)
        {
            while (active.Count >= maxNotifications) yield return null;

            var n = Instantiate(prefab, canvas.transform);
            n.SetActive(true);

            var textComp = n.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            textComp.text = msg;
            textComp.fontStyle = FontStyles.Bold;
            textComp.color = textCol;

            var descComp = n.transform.Find("Description").GetComponent<TextMeshProUGUI>();
            descComp.text = description;

            var fill = n.transform.Find("Bar/Fill").GetComponent<Image>();
            var rt = n.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0, 0);
            rt.pivot = new Vector2(0, 0);

            float targetY = offset.y + active.Count * 42;
            rt.anchoredPosition = new Vector2(-200, targetY);
            active.Add(n);

            var cg = n.GetComponent<CanvasGroup>();
            cg.alpha = 0f;

            float t = 0;
            while (t < slideTime)
            {
                t += Time.unscaledDeltaTime;
                float p = t / slideTime;
                rt.anchoredPosition = new Vector2(Mathf.Lerp(-200, offset.x, p), targetY);
                cg.alpha = p;
                yield return null;
            }
            rt.anchoredPosition = new Vector2(offset.x, targetY);
            cg.alpha = 1f;

            fill.fillAmount = 1f;
            t = 0;
            while (t < displayTime)
            {
                t += Time.unscaledDeltaTime;
                fill.fillAmount = Mathf.Lerp(1f, 0f, t / displayTime);
                yield return null;
            }

            t = 0;
            while (t < slideTime)
            {
                t += Time.unscaledDeltaTime;
                float p = t / slideTime;
                rt.anchoredPosition = new Vector2(Mathf.Lerp(offset.x, -200, p), targetY);
                cg.alpha = 1f - p;
                yield return null;
            }

            active.Remove(n);
            for (int i = 0; i < active.Count; i++)
            {
                var r = active[i].GetComponent<RectTransform>();
                r.anchoredPosition = new Vector2(offset.x, offset.y + i * 42);
            }

            Destroy(n);
        }
    }
}