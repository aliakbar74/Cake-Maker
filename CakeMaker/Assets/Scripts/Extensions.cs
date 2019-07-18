using UnityEngine;

public static class Extensions {
    public static Texture2D ToTexture2D(this Sprite source)
    {
        if(source.rect.width != source.texture.width){
            Texture2D newText = new Texture2D((int)source.rect.width,(int)source.rect.height);
            Color[] newColors = source.texture.GetPixels((int)source.textureRect.x, 
                (int)source.textureRect.y, 
                (int)source.textureRect.width, 
                (int)source.textureRect.height );
            newText.SetPixels(newColors);
            newText.Apply();
            return newText;
        } else
            return MakeTextureReadable(source.texture);
    }
    
    private static Texture2D MakeTextureReadable(this Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
            source.width,
            source.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }
}