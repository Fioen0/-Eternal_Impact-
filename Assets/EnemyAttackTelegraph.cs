using System.Collections;
using UnityEngine;

/// <summary>Creates a brief sparkle warning before regular enemy attacks.</summary>
public static class EnemyAttackTelegraph
{
    private static Material sharedMaterial;

    public static GameObject Create(Vector3 position, Transform owner, int sortingLayerId, int sortingOrder)
    {
        Material material = GetMaterial();
        if (material == null) return null;

        GameObject effect = new GameObject("EnemyAttackTelegraph");
        effect.transform.position = position;
        if (owner != null) effect.transform.SetParent(owner, true);

        Vector2[] offsets = { Vector2.zero, new Vector2(-0.24f, 0.14f), new Vector2(0.23f, -0.12f) };
        float[] sizes = { 0.28f, 0.17f, 0.2f };
        Vector3[] starPoints =
        {
            new Vector3(0f, 0.5f, 0f), new Vector3(0.12f, 0.12f, 0f),
            new Vector3(0.5f, 0f, 0f), new Vector3(0.12f, -0.12f, 0f),
            new Vector3(0f, -0.5f, 0f), new Vector3(-0.12f, -0.12f, 0f),
            new Vector3(-0.5f, 0f, 0f), new Vector3(-0.12f, 0.12f, 0f)
        };

        for (int i = 0; i < offsets.Length; i++)
        {
            GameObject sparkle = new GameObject("Sparkle_" + (i + 1));
            sparkle.transform.SetParent(effect.transform, false);
            sparkle.transform.localPosition = offsets[i];
            sparkle.transform.localScale = Vector3.one * sizes[i];
            sparkle.transform.localRotation = Quaternion.Euler(0f, 0f, i * 27f);

            LineRenderer line = sparkle.AddComponent<LineRenderer>();
            line.useWorldSpace = false;
            line.loop = true;
            line.positionCount = starPoints.Length;
            line.startWidth = 0.055f;
            line.endWidth = 0.055f;
            line.numCapVertices = 2;
            line.sharedMaterial = material;
            line.sortingLayerID = sortingLayerId;
            line.sortingOrder = sortingOrder;
            line.startColor = Color.white;
            line.endColor = Color.white;
            line.SetPositions(starPoints);
        }

        return effect;
    }

    public static IEnumerator Pulse(GameObject effect, float duration)
    {
        duration = Mathf.Max(0.01f, duration);
        if (effect == null)
        {
            yield return new WaitForSeconds(duration);
            yield break;
        }

        LineRenderer[] lines = effect.GetComponentsInChildren<LineRenderer>();
        float elapsed = 0f;
        while (elapsed < duration && effect != null)
        {
            float progress = elapsed / duration;
            float pulse = 0.72f + 0.38f * Mathf.Abs(Mathf.Sin(elapsed * 22f));
            effect.transform.localScale = Vector3.one * pulse;
            Color color = Color.Lerp(new Color(1f, 0.72f, 0.18f, 1f), Color.white,
                Mathf.PingPong(elapsed * 8f, 1f));
            color.a = Mathf.Sin(progress * Mathf.PI);
            foreach (LineRenderer line in lines)
            {
                if (line == null) continue;
                line.startColor = color;
                line.endColor = color;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (effect != null) Object.Destroy(effect);
    }

    private static Material GetMaterial()
    {
        if (sharedMaterial != null) return sharedMaterial;
        Shader shader = Shader.Find("Sprites/Default");
        if (shader == null) shader = Shader.Find("Unlit/Color");
        if (shader == null) return null;

        sharedMaterial = new Material(shader)
        {
            name = "EnemyAttackTelegraphMaterial",
            hideFlags = HideFlags.HideAndDontSave
        };
        return sharedMaterial;
    }
}
