namespace Uchu.World
{
    public class MaskGameObject : GameObject
    {
        public GameObject Author { get; private set; }

        public override Transform Transform => Author.Transform;

        public static MaskGameObject Instantiate(Lot mask, GameObject author)
        {
            var instance = Instantiate<MaskGameObject>(author.Zone, mask);

            instance.Author = author;

            return instance;
        }

        public override void Start()
        {
            
        }
    }
}