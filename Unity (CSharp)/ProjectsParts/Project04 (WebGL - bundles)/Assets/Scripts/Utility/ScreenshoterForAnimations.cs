using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScreenshoterForAnimations : MonoBehaviour 
{
	public Camera workingCamera;

	public GameObject[] modelsArr;

	public Texture2D[] spriteMap;

	// Use this for initialization
	void Start()
	{
		StartCoroutine(MakeSpriteMap());
	}

	public Texture2D LoadPNG(string filePath)
	{
		Texture2D tex = null;

		#if UNITY_WEBPLAYER
		#else
		byte[] fileData;
		
		if (System.IO.File.Exists(filePath))
		{
			fileData = System.IO.File.ReadAllBytes(filePath);
			tex = new Texture2D(2, 2, TextureFormat.ARGB32, true, true);
			tex.LoadImage(fileData);
		}

		#endif

		return tex;
	}

	public Texture2D CreateSprite(string greenPart, string redPart)
	{
		Texture2D texgreen = LoadPNG(greenPart);
		Color[] greenBuffer = texgreen.GetPixels();

		Texture2D texred = LoadPNG(redPart);
		Color[] redBuffer = texred.GetPixels();

		for (int pixelIndex = 0; pixelIndex < greenBuffer.Length; pixelIndex++) 
		{
			if (greenBuffer[pixelIndex] != Color.green || redBuffer[pixelIndex] != Color.red)
			{
				if (greenBuffer[pixelIndex] != redBuffer[pixelIndex])
				{
					greenBuffer[pixelIndex].a = 1.0f - (greenBuffer[pixelIndex].g - redBuffer[pixelIndex].g);
				}

				greenBuffer[pixelIndex].r = (greenBuffer[pixelIndex].r + redBuffer[pixelIndex].r) / 2.0f;
				greenBuffer[pixelIndex].g = redBuffer[pixelIndex].g;
				greenBuffer[pixelIndex].b = (greenBuffer[pixelIndex].b + redBuffer[pixelIndex].b) / 2.0f;
			}
			else
			{
				greenBuffer[pixelIndex] = new Color(0.25f, 0.25f, 0.25f, 0.0f);
			}
		}

		texgreen.SetPixels(greenBuffer);
		texgreen.Apply();

		return texgreen;
	}

	public void TakeShot(Color backColor, string picPath)
	{
		workingCamera.backgroundColor = backColor;
		ScreenCapture.CaptureScreenshot(picPath);
	}

	public IEnumerator MakeSpriteMap()
	{
		List<Texture2D> sprites = new List<Texture2D>();

		int[] framesCountArr = new int[3];

		for (int modelIndex = 0; modelIndex < modelsArr.Length; modelIndex++) 
		{
			modelsArr[modelIndex].SetActive(false);
		}

		for (int modelIndex = 0; modelIndex < modelsArr.Length; modelIndex++) 
		{
			GameObject model = modelsArr[modelIndex];

			model.SetActive(true);

			Animation modelAnimation = model.GetComponent<Animation>();

			if (modelIndex == 3)
				RenderSettings.ambientLight = new Color(0.5f, 0.5f, 0.5f);
			else
				RenderSettings.ambientLight = new Color(1.0f, 1.0f, 1.0f);

			if (modelAnimation == null)
			{
				TakeShot(Color.green, "tmp_green.png");
				yield return new WaitForSeconds(0.1f);

				TakeShot(Color.red, "tmp_red.png");
				yield return new WaitForSeconds(0.1f);
				
				sprites.Add(CreateSprite("tmp_green.png", "tmp_red.png"));

				if (modelIndex >= 2)
					framesCountArr[2]++;
			}
			else
			{
				float animationLength = modelAnimation.clip.length;

				string clipName = modelAnimation.clip.name;

				for (int clipPos = 0; clipPos < animationLength / 0.1; clipPos++)
				{
					modelAnimation[clipName].time = (float)clipPos * 0.1f;

					modelAnimation.Play(clipName);
					modelAnimation.Sample();
					modelAnimation.Stop();

					TakeShot(Color.green, "tmp_green.png");
					yield return new WaitForSeconds(0.1f);
					
					TakeShot(Color.red, "tmp_red.png");
					yield return new WaitForSeconds(0.1f);
					
					sprites.Add(CreateSprite("tmp_green.png", "tmp_red.png"));

					framesCountArr[modelIndex]++;
				}
			}

			model.SetActive(false);
		}

		CreateSpriteMap(sprites.ToArray(), framesCountArr);

		spriteMap = sprites.ToArray();
	}

	protected void CreateSpriteMap(Texture2D[] sprites, int[] framesCountArr)
	{
		int xSize = 0, ySize = framesCountArr.Length;

		for (int frameIndex = 0; frameIndex < framesCountArr.Length; frameIndex++)
		{
			if (framesCountArr[frameIndex] > xSize)
				xSize = framesCountArr[frameIndex];
		}

		Texture2D texAtlas = new Texture2D(xSize * Screen.width, ySize * Screen.height);

		int xPos = 0;
		int yPos = 0;

		for (int spriteIndex = 0; spriteIndex < sprites.Length; spriteIndex++)
		{
			texAtlas.SetPixels(xPos * Screen.width, (ySize - yPos - 1) * Screen.height, Screen.width, Screen.width, sprites[spriteIndex].GetPixels());

			xPos++;

			if (xPos >= framesCountArr[yPos])
			{
				xPos = 0;
				yPos++;
			}
		}

		texAtlas.Apply();

		GameObject rootObject = transform.parent.gameObject;

		File.WriteAllBytes(rootObject.name + ".png", texAtlas.EncodeToPNG());
	}
}
