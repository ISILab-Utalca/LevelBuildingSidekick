namespace TheDeveloper.ColorChanger
{
    using UnityEngine;

    /**
     * Changes only Start Color in all child Particle Systems.
     * It doesn't change "Color over Lifetime" or "Color by Speed"
     * 
     * Made by: The Developer
     * YouTube Channel: https://www.youtube.com/channel/UCwO0k5dccZrTW6-GmJsiFrg
     * Website: https://thedevelopers.tech
     */
    [DisallowMultipleComponent]
    public class PS_ColorChanger : MonoBehaviour
    {
        #region Fields
        [Tooltip("Current \"Main\" color of all particle systems")]
        public Color currentColor;
        [Tooltip("New \"Main\" color of all particle systems")]
        public Color newColor;

        private Color currentHSV; // r -> H; g -> S; b -> V (not a really correct way to do it :D)
        private Color newHSV; // r -> H; g -> S; b -> V (not a really correct way to do it :D)
        #endregion

        #region Methods
        /**
            Update colors of all child systems
        */
        public void ChangeColor()
        {
            var systems = this.GetComponentsInChildren<ParticleSystem>();

            Color.RGBToHSV(this.currentColor, out this.currentHSV.r, out this.currentHSV.g, out this.currentHSV.b);
            Color.RGBToHSV(this.newColor, out this.newHSV.r, out this.newHSV.g, out this.newHSV.b);

            foreach (var system in systems)
            {
                var main = system.main;
                switch (main.startColor.mode)
                {
                    case ParticleSystemGradientMode.Color:
                        main.startColor = new ParticleSystem.MinMaxGradient(
                            this.ConvertCurrentToNew(main.startColor.color)
                            );
                        break;
                    case ParticleSystemGradientMode.TwoColors:
                        main.startColor = new ParticleSystem.MinMaxGradient(
                                this.ConvertCurrentToNew(main.startColor.colorMin),
                                this.ConvertCurrentToNew(main.startColor.colorMax)
                            );
                        break;
                    case ParticleSystemGradientMode.Gradient:
                        main.startColor = new ParticleSystem.MinMaxGradient(
                                this.ConvertCurrentToNew(main.startColor.gradient)
                            );
                        break;
                    case ParticleSystemGradientMode.TwoGradients:
                        main.startColor = new ParticleSystem.MinMaxGradient(
                                this.ConvertCurrentToNew(main.startColor.gradientMin),
                                this.ConvertCurrentToNew(main.startColor.gradientMax)
                            );
                        break;
                }

            }
        }

        /**
            Swap currentColor & newColor
        */
        public void SwapCurrentWithNewColors()
        {
            Color temp = this.currentColor;
            this.currentColor = this.newColor;
            this.newColor = temp;
        }

        /**
            Convert a gradient from current color system to the new one
        */
        public Gradient ConvertCurrentToNew(Gradient gradient)
        {
            Gradient g = new Gradient();
            g.mode = gradient.mode;

            var alphaKeys = new GradientAlphaKey[gradient.alphaKeys.Length];
            var colorKeys = new GradientColorKey[gradient.colorKeys.Length];

            for (int i = 0; i < g.colorKeys.Length; ++i)
                colorKeys[i] = new GradientColorKey(
                        this.ConvertCurrentToNew(gradient.colorKeys[i].color),
                        gradient.colorKeys[i].time
                    );

            System.Array.Copy(gradient.alphaKeys, alphaKeys, alphaKeys.Length);

            g.SetKeys(colorKeys, alphaKeys);
            return g;
        }

        /**
            Convert color from current color system to the new one
        */
        public Color ConvertCurrentToNew(Color color)
        {
            Color hsv;
            Color.RGBToHSV(color, out hsv.r, out hsv.g, out hsv.b);
            Color endRes = Color.HSVToRGB(
                    Mathf.Clamp01(Mathf.Abs(this.newHSV.r + (this.currentHSV.r - hsv.r))),
                    hsv.g,
                    hsv.b
                );
            endRes.a = color.a;
            return endRes;
        }
        #endregion
    }
}