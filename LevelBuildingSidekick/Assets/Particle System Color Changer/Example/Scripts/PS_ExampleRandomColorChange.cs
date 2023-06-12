namespace TheDeveloper.ColorChanger
{
    using System.Collections;
    using UnityEngine;

    public class PS_ExampleRandomColorChange : MonoBehaviour
    {
        public PS_ColorChanger colorChanger;
        public ParticleSystem ps;
        public float changeTime = 2.5f;

        private void Start()
        {
            if(this.colorChanger == null)
            {
                Debug.LogError("Please assign color changer!");
                return;
            }
            StartCoroutine(ChangeColorToRandom());
        }

        private IEnumerator ChangeColorToRandom()
        {
            while (Application.isPlaying)
            {
                yield return new WaitForSeconds(changeTime);

                this.colorChanger.newColor = Color.HSVToRGB(Random.Range(0.0f, 1.0f), Random.Range(0.8f, 1.0f), 1.0f);
                this.colorChanger.ChangeColor();
                this.colorChanger.SwapCurrentWithNewColors();

                if (this.ps != null)
                    this.ps.Play();
            }
        }
    }
}