--------------------------------------------------------------
-- Description:
--
-- Client script for Shooting Gallery NPC in GF area.
-- Lets client know the object can be interacted with
-- updated mrb... 3/23/10
--------------------------------------------------------------
require('client/ai/MINIGAME/BASE_INSTANCER')

local tVars = {
     releaseVersion = 182, -- which version release # the content should be made available for Beta 1
     misID = 370,
     missionState = 8,
     transferZoneID = 1202,
     UI_Type = "NS_SG_01",
     failText = Localize("MINIGAME_LOBBY_NS_SG_FAIL_MESSGE"),}
     
function onStartup(self)
    baseSetVars(self, tVars)
end
