--------------------------------------------------------------
-- Description:
--
-- Client script for AG Survival Instancer.
-- Lets client know the object can be interacted with
-- updated mrb... 1/26/10
--------------------------------------------------------------
require('client/ai/MINIGAME/BASE_INSTANCER')

local tVars = {
    -- misID = 629, -- run the foot race
    -- missionState = 8,
     itemType = 8092, -- allow vehicles to start the racing
     failItem = "Please drag a vehicle to the activator to start!",
     UI_Type = "GF_Race_01",
     failText = "Talk to Gangway Kelley to unlock this race.",}
     
     
function onStartup(self)
    baseSetVars(self, tVars)
end
