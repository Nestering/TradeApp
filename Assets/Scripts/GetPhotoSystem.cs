using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GearPhotoSystem : MonoBehaviour
{
    [SerializeField] private Button[] photoButtons; 

    void Start()
    {
        foreach (Button button in photoButtons)
        {
            Image targetImage = button.GetComponentInChildren<Image>();
            button.onClick.AddListener(() => StartCoroutine(LoadImageFromGallery(targetImage)));
        }
    }

    IEnumerator LoadImageFromGallery(Image targetImage)
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                Texture2D texture = new Texture2D(2, 2);
                byte[] fileData = System.IO.File.ReadAllBytes(path);
                texture.LoadImage(fileData);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                targetImage.sprite = sprite;
            }
        }, "Select an image", "image/*");

        yield return new WaitForEndOfFrame();
    }
}