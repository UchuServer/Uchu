require('o_mis')
--//////////////////////////////////////////////////////////////////////////////////
local npcName = 'Rusty'
local missionCamDist = 10 -- distance to teleport player away from npc
local misID = 318 -- missionID from DB
local camTime = 16 -- time to release player
--//////////////////////////////////////////////////////////////////////////////////
 
----------------------------------------------------------------
-- Happens on client interaction (must register for picking)
----------------------------------------------------------------
function onClientUse(self, msg)
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())   
    local oPos = self:GetPosition()    
    local oDir = self:GetObjectDirectionVectors()
    local camPos = {x = oPos.pos.x + (oDir.forward.x * missionCamDist), y = oPos.pos.y, z = oPos.pos.z + (oDir.forward.z * missionCamDist)} --+ (oDir.forward.y * posOffset)
    
    self:SetVar('DialogueOK', false)    
    player:Teleport{pos = camPos}
    player:PlayCinematic { pathName = 'MissionDisplay_' .. npcName .. '_Cam' }    
end

function onMissionDialogueOK(self, msg)
    --print('Dialogue OK')
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()) 
    player:EndCinematic()
    self:SetVar('DialogueOK', true)
    
    if msg.bIsComplete == false and msg.missionID == misID then 
        -- freeze player and play cinematic
        player:SetUserCtrlCompPause{bPaused = true}
        player:PlayCinematic { pathName = 'Mission_' .. npcName .. '_Cam' }    
        
        -- start timer to release player movement
        GAMEOBJ:GetTimer():AddTimerWithCancel( camTime, npcName .. 'Cam',self )            
    end    
end

function onTerminateInteraction(self, msg)    
    if self:GetVar('DialogueOK') then return end
    
    --print('terminate interaction')
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())     
    player:EndCinematic()
end

function onTimerDone (self,msg)
    if (msg.name == npcName .. 'Cam') then          
        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()) 
        player:SetUserCtrlCompPause{bPaused = false}
    end
end
