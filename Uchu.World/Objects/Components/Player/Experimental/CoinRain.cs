using System.Numerics;

namespace Uchu.World.Experimental
{
    public class CoinRain : Component
    {
        public CoinRain()
        {
            OnTick.AddListener(() =>
            {
                As<Player>().EntitledCurrency++;
                
                InstancingUtil.Currency(1, As<Player>(), GameObject, Transform.Position + Vector3.UnitY * 2);
            });
        }
    }
}