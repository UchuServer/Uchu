--------------------------------------------------------------

-- L_AG_MAELSTROM_SAMPLE.lua

-- Client side script for the Maelstrom sample objects in AG
-- Created abeechler - 6/9/11
-- updated mrb... 6/22/11 - removed vis checks
--------------------------------------------------------------
require('02_client/Map/General/L_VIS_TOGGLE_OBJ')

local maelSampleFX = 7680       -- Mesh effect for the Maelstrom sample

-- table to find the mission ID based on the spawner network name
local VisibilityObjectTable = {["MaelstromSamples"] = {1849, 1883},
                               ["MaelstromSamples2ndary1"] = {1883},
                               ["MaelstromSamples2ndary2"] = {1883}}
                               
----------------------------------------------------------------
-- Catch object instantiation
----------------------------------------------------------------                   
function onStartup(self)    
    setGameVariables(self, VisibilityObjectTable)    
end 