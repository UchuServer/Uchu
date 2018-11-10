namespace Uchu.Core
{
    public class Scene
    {
        public byte SceneId { get; set; }
        public bool Audio { get; set; }
        public string Name { get; set; }
        public LevelObject[] Objects { get; set; }
    }
}