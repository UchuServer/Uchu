namespace Uchu.World.Parsers
{
    public class SceneInfo
    {
        public byte SceneId { get; set; }
        public bool Audio { get; set; }
        public string Name { get; set; }
        public LevelObject[] Objects { get; set; }
    }
}