using System;
using System.Collections.Generic;
using System.Text;

namespace Uchu.World
{
    public enum DeleteReason : int
    {
        PickingUpModel = 0,
        ReturningModelToInventory,
        BreakingModelApart
    }
}
