namespace Space_Shooter
{
    class Enemy : GameObject
    {
        public Enemy(int x, int y, int w, int h) : base(x, y, w, h) { }

        public override void Update()
        {
            // Example enemy movement: move downwards
            Move(0, 2);
        }

        public override void HandleInput(byte[] keys)
        {
            // Enemies don't handle player input
        }
    }
}
