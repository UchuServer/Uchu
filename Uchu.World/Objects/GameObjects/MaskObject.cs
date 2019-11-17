namespace Uchu.World
{
    public class MaskObject : GameObject
    {
        public GameObject Author { get; set; }
        
        private Mask OriginalLayer { get; set; }
        
        protected MaskObject()
        {
            OnStart.AddListener(() =>
            {
                if (TryGetComponent<DestructibleComponent>(out var destructibleComponent))
                {
                    destructibleComponent.ResurrectTime = 15;
                }
                
                OriginalLayer = Author.Layer;
                
                Author.Layer = World.Layer.Hidden;
            });
            
            OnDestroyed.AddListener(() =>
            {
                Author.Layer = OriginalLayer;
            });
        }
    }
}