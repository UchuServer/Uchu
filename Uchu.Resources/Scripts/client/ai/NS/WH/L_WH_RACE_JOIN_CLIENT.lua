--------------------------------------------------------------
-- Description:
--
-- Client script for NS Race Instancer.
-- Lets client know the object can be interacted with
-- created pml... 11/8/10 copied from L_NS_RACE_JOIN_CLIENT.lua
--------------------------------------------------------------
require('client/ai/MINIGAME/BASE_INSTANCER')

local tVars = {
    releaseVersion = 183, -- which version release # the content should be made available for Beta 1
    misID = 624, -- run the foot race to unlock
    missionState = 2,
    itemType = 8092, -- allow vehicles to start the racing
    failItem = Localize("MINIGAME_LOBBY_RACE_DRAG_ITEM_FAIL_MESSAGE"),
    UI_Type = "NS_Race_02",
    failText = Localize("MINIGAME_LOBBY_NS_RACE_FAIL_MESSAGE"),
    bUseBothInteractions = true,} -- if this is set to true then the player will be able to drag or click      
     
function onStartup(self)
    baseSetVars(self, tVars)
end
