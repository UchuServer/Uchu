require('o_mis')
--//////////////////////////////////////////////////////////////////////////////////
local npcName = 'Wisp'
local misID = 311 -- missionID from DB
local camTime = 3.5 -- time to release player

local misID_2 = 335 -- missionID from DB
local camTime_2 = 4.5 -- time to release player

local missionCamDist = 10 -- distance to teleport player away from npc
--//////////////////////////////////////////////////////////////////////////////////

----------------------------------------------------------------
-- Happens on client interaction (must register for picking)
----------------------------------------------------------------
function onClientUse(self, msg)
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())   
    --local oPos = self:GetPosition()    
    --local oDir = self:GetObjectDirectionVectors()
    --local camPos = {x = oPos.pos.x + (oDir.forward.x * missionCamDist), y = oPos.pos.y, z = oPos.pos.z + (oDir.forward.z * missionCamDist)} --+ (oDir.forward.y * posOffset)
    
    self:SetVar('DialogueOK', false)    
    --player:Teleport{pos = camPos}
    player:SetVisible{visible = false, fadeTime = 0.5}
    player:PlayCinematic { pathName = 'MissionDisplay_' .. npcName .. '_Cam' }    
end

function missionCinematic(self, player, mission, time)
    self:SetVar('DialogueOK', true)
    -- freeze player and play cinematic
    player:SetUserCtrlCompPause{bPaused = true}
    player:PlayCinematic { pathName = 'Mission_' .. npcName .. '_Cam_' .. tostring(mission)}    
    
    -- start timer to release player movement
    GAMEOBJ:GetTimer():AddTimerWithCancel( camTime, npcName .. 'Cam',self )   
end

function onMissionDialogueOK(self, msg)
    --print('Dialogue OK')
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()) 

    player:EndCinematic()    
    player:SetVisible{visible = true, fadeTime = 0.5}
    
    if msg.bIsComplete == false and msg.missionID == misID then 
        missionCinematic(self, player, misID, camTime)
    elseif msg.bIsComplete == false and msg.missionID == misID_2 then
        missionCinematic(self, player, misID_2, camTime_2) 
    end    
end

function onNotifyMission(self, msg)
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()) 
    if msg.missionState <= 1 then
        player:PlayCinematic { pathName = 'MissionDisplay_' .. npcName .. '_Cam' }    
        player:SetVisible{visible = false, fadeTime = 0.5}
    end
end

function onTerminateInteraction(self, msg)    
    if self:GetVar('DialogueOK') then return end
    
    --print('terminate interaction')
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())     
    player:EndCinematic()
    player:SetVisible{visible = true, fadeTime = 0.2}
end

function onTimerDone (self,msg)
    if (msg.name == npcName .. 'Cam') then          
        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()) 
        player:SetUserCtrlCompPause{bPaused = false}
    end
end
