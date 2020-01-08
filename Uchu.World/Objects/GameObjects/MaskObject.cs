namespace Uchu.World
{
    public class MaskObject : GameObject
    {
        public GameObject Author { get; set; }
        
        private Mask OriginalLayer { get; set; }
        
        protected MaskObject()
        {
            Listen(OnStart, () =>
            {
                if (TryGetComponent<DestructibleComponent>(out var destructibleComponent))
                {
                    destructibleComponent.ResurrectTime = 15;
                }
                
                OriginalLayer = Author.Layer;
                
                Author.Layer = StandardLayer.Hidden;
            });
            
            Listen(OnDestroyed, () =>
            {
                Author.Layer = OriginalLayer;
            });
        }
    }
}