using UnityEngine;

public class WaveTexture : MonoBehaviour
{
    public bool isRain = true;
    [Range(1, 5)]
    public int RainTendency = 3;
    private Camera mainCame;
    private const int width = 256, height = 256;
    private readonly float[,] wavePixel = new float[width, height];
    private Texture2D texUV;
    private readonly Color[] pixelBuffer = new Color[width * height];
    private bool isSetTex = false;
    private float rainDeltaT = 0;
    private float rainWaitT = 0;

    void Start()
    {
        mainCame = Camera.main;
        texUV = new Texture2D(width, height);
        GetComponent<Renderer>().material.SetTexture("_WaveTex", texUV);
    }

    private void RainDrop()
    {
        if(isRain)
        {
            if(rainDeltaT > rainWaitT)
            {
                PutDrop(Random.Range(0, width), Random.Range(0, height));
                rainWaitT = Mathf.Pow(Random.Range(.1f, .5f), RainTendency);
                rainDeltaT = 0;
            }
            rainDeltaT += Time.deltaTime;
        }
    }

    private void PutDrop(int x, int y)
    {
        if (x > 0 && y > 0 && x < width - 1 && y < height - 1)
            wavePixel[x, y] = 1;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
            ShotRipple();
        SetTex();
        RainDrop();
        ComputeValue();
    }

    private void SetTex()
    {
        if (isSetTex)
        {
            texUV.SetPixels(pixelBuffer);
            texUV.Apply();
            isSetTex = false;
        }
    }

    private void ShotRipple()
    {
        if(Physics.Raycast(mainCame.ScreenPointToRay(Input.mousePosition), out var hit))
            if (hit.collider)
            {
                Vector3 p = transform.worldToLocalMatrix.MultiplyPoint(hit.point);
                PutDrop((int)((p.x + .5f) * width), (int)((p.y + .5f) * height));
            }
    }

    private void ComputeValue()
    {
        for (int w = 1; w < width - 1; w++)
            for (int h = 1; h < height - 1; h++)
            {
                wavePixel[w, h] = (
                    wavePixel[w, h - 1] +
                    wavePixel[w, h + 1] +
                    wavePixel[w - 1, h] +
                    wavePixel[w + 1, h] +
                    wavePixel[w + 1, h + 1] +
                    wavePixel[w + 1, h - 1] +
                    wavePixel[w - 1, h + 1] +
                    wavePixel[w - 1, h - 1]
                    ) / 8 - wavePixel[w, h];
                pixelBuffer[h * height + w] = new Color(
                    .25f + (wavePixel[w - 1, h] - wavePixel[w + 1, h]) * .25f,
                    .25f + (wavePixel[w, h - 1] - wavePixel[w, h + 1]) * .25f,
                    0);
                wavePixel[w, h] *= .99f;
            }
        isSetTex = true;
    }
}
