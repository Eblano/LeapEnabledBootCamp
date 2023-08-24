using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class MultiPostEffectCamera : MonoBehaviour
{
    private RenderTexture[] _tex = new RenderTexture[2];

    private void OnEnable()
    {
        if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
            enabled = false;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        _tex[0] = source;
        _tex[1] = RenderTexture.GetTemporary(source.width, source.height);
        RenderTexture releaseMe = _tex[1];
        int index = 0;

        System.Collections.Generic.List<PostEffectsBase> sorted = new System.Collections.Generic.List<PostEffectsBase>();

        int i = 0;
        foreach (PostEffectsBase fx in GetComponents<PostEffectsBase>())
        {
            if (fx && fx.enabled)
            {
                sorted.Add(fx);
                i++;
            }
        }

        while (sorted.Count > 0)
        {
            int indexToUse = 0;
            int orderValue = -1;
            for (i = 0; i < sorted.Count; i++)
            {
                if (sorted[i].order > orderValue)
                {
                    orderValue = sorted[i].order;
                    indexToUse = i;
                }
            }

            PostEffectsBase effect = sorted[indexToUse];
            if (effect.PreferRenderImage3())
            {
                effect.OnRenderImage3(_tex[index], _tex[1 - index]);
            }
            else
            {
                effect.OnRenderImage2(_tex[index], _tex[1 - index]);
                index = 1 - index;
            }

            sorted.RemoveAt(indexToUse);
        }

        Graphics.Blit(_tex[index], destination);

        RenderTexture.ReleaseTemporary(releaseMe);
    }
}
