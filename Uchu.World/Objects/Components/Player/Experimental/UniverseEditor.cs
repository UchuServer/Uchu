using System.Numerics;

namespace Uchu.World.Experimental
{
    public class UniverseEditor : Component
    {
        private const int SelectBrickLot = 32;
        
        private GameObject SelectBrick { get; set; }

        public GameObject Target { get; set; }

        public UniverseEditor()
        {
            OnTick.AddListener(FocusBrick);
            
            OnDestroyed.AddListener(() =>
            {
                if (SelectBrick != null) Destroy(SelectBrick);
            });
        }

        private void FocusBrick()
        {
            if (SelectBrick == null) return;

            if (Target == null)
            {
                Destroy(SelectBrick);

                SelectBrick = null;
                
                return;
            }
            
            SelectBrick.Transform.Position = Target.Transform.Position + Vector3.UnitY * 5;
            
            GameObject.Serialize(SelectBrick);
        }

        public void SetTarget(GameObject gameObject)
        {
            Target = gameObject;

            if (gameObject == null)
            {
                if (SelectBrick != null) Destroy(SelectBrick);
                
                return;
            }
            
            if (SelectBrick != null) Destroy(SelectBrick);
            
            SelectBrick = GameObject.Instantiate(
                Zone,
                SelectBrickLot,
                Target.Transform.Position,
                Quaternion.Identity
            );

            Start(SelectBrick);

            As<Player>().Perspective.Hallucinate(SelectBrick);
        }
    }
}