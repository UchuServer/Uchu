--------------------------------------------------------------
-- (CLIENT SIDE) Obstacle Course Starter NPC
--
-- Handles client side dialogs/messages and input
--------------------------------------------------------------

----------------------------------------------------------------
-- Includes
----------------------------------------------------------------
require('c_AvantGardens')

--//////////////////////////////////////////////////////////////////////////////////
local misID = 335 -- missionID from DB
--//////////////////////////////////////////////////////////////////////////////////

----------------------------------------------------------------
-- Startup of the object
----------------------------------------------------------------
function onStartup(self)     
 
end


----------------------------------------------------------------
-- Happens on client interaction (must register for picking)
----------------------------------------------------------------
function onClientUse(self, msg)
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())       
    self:SetVar('DialogueOK', false)    
   
end


function onTerminateInteraction(self, msg)    
    if self:GetVar('DialogueOK') then return end
    
    --print('terminate interaction')
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())     

end

--------------------------------------------------------------
-- Make this object interactable (must register for picking)
--------------------------------------------------------------
function onGetOverridePickType(self, msg)
	msg.ePickType = 14	--Interactive type
	return msg

end

----------------------------------------------------------------
-- Server sends us a notification to do help, show dialogs
----------------------------------------------------------------
function onHelp(self, msg)
 
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
    if (player:GetMissionState{missionID = misID}.missionState == 2 or player:GetMissionState{missionID = misID}.missionState == 10 )then

        if (msg.iHelpID == 0) then

            player:DisplayMessageBox{bShow = true, 
                                     imageID = 1, 
                                     text = "Enter Battle to Collect Maelstrom?", 
                                     callbackClient = self, 
                                     identifier = "player_dialog_cancel_course"}
        
       
         end
    end
end
