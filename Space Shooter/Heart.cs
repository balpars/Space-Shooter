using SDL2;

namespace Space_Shooter
{
    public class Heart : GameObject
    {
        protected string? assetPath;

        public Heart(int x, int y, int size) : base(x, y, size, size)
        {
            assetPath = "Assets/Heart/heart.png"; // Path to your heart image
        }

        public override void Update()
        {
            // Hearts don't move or update in this simple example
        }

        public string? GetAssetPath()
        {
            return assetPath;
        }

        public void Render(Renderer renderer)
        {
            renderer.Draw(this);
        }
    }
}
