--------------------------------------------------------------

-- L_AG_PLUNGER_TARGET.lua

-- Client side script for the Plunger Target objects in AG
-- Created abeechler - 6/10/11
--------------------------------------------------------------
require('L_VIS_TOGGLE_OBJ')

-- Table to find the mission ID based on the spawner network name
local VisibilityObjectTable = {["PlungerGunTargets"] = {1880}}
                               
----------------------------------------------------------------
-- Catch object instantiation
----------------------------------------------------------------                   
function onStartup(self)
    
    setGameVariables(self, VisibilityObjectTable)
    
end 
