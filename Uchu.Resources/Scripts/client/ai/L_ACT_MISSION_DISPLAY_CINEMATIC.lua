-- Script that can be attached to any misison giver on the client in HF. Config data
-- variables must be set in HF. missionID, cinemaName, cinemaTime
-- Created: 6/16/09 mrb...

--//////////////////////////////////////////////////////////////////////////////////
-- local variables
local missionCamDist = 10 -- distance to teleport player away from npc
local missionID = '' -- missionID from DB
local cinemaName = '' -- name of cinematic in HF
local cinemaTime = '' -- time of cinematic and to release player
local player = ''
--//////////////////////////////////////////////////////////////////////////////////

function onStartup(self)
    -- get the varables set in HF in the mission givers configData, if mission print error
    missionID = self:GetVar('missionID') -- missionID from DB
    if missionID == '' then print('Missing missionID in ConfigData') end
    
    cinemaName = self:GetVar('cinemaName') -- name of cinematic in HF
    if cinemaName == '' then print('Missing cinemaName in ConfigData') end
    
    cinemaTime = self:GetVar('cinemaTime') -- time of cinematic and to release player
    if cinemaTime == '' then print('Missing cinemaTime in ConfigData') end
        
    player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()) -- the player
    
    local oRot = self:GetRotation()
    
    self:SetVar('rotX', oRot.x)
    self:SetVar('rotY', oRot.y)
    self:SetVar('rotZ', oRot.z)
    self:SetVar('rotW', oRot.w)
end

function onClientUse(self, msg)

    self:SetRotation{x = self:GetVar('rotX'), y = self:GetVar('rotY'), z = self:GetVar('rotZ'), w = self:GetVar('rotW')}
    --local oPos = self:GetPosition()    
    --local teleOffset = missionCamDist + 5
    --local oDir = self:GetObjectDirectionVectors()
    --local telePos = {x = oPos.pos.x + (oDir.forward.x * teleOffset), y = oPos.pos.y, z = oPos.pos.z + (oDir.forward.z * teleOffset)}
        
    self:SetVar('DialogueOK', false)    
    --player:Teleport{pos = camPos}
    player:SetVisible{visible = false, fadeTime = 0.5}
    player:PlayCinematic { pathName = 'MissionDisplay_' .. self:GetVar('npcName') .. '_Cam' }    
end

function onMissionDialogueOK(self, msg)
    if not missionID or not cinemaName or not cinemaTime then return end

    player:EndCinematic()
    player:SetVisible{visible = true, fadeTime = 0.5}
    
    local bComplete = self:GetVar('PlayOnComplete')
    if not bComplete then bComplete = false end
    
    if msg.bIsComplete == bComplete and msg.missionID == self:GetVar('missionID') then    
        self:SetVar('DialogueOK', true)
        -- freeze player and play cinematic
        player:SetUserCtrlCompPause{bPaused = true}
        player:PlayCinematic { pathName = self:GetVar('cinemaName') }    
        
        -- start timer to release player movement
        GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar('cinemaTime'), 'missionCamera',self )            
    end    
end

function onNotifyMission(self, msg)
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()) 
    if msg.missionState <= 1 then
        player:PlayCinematic { pathName = 'MissionDisplay_' .. self:GetVar('npcName') .. '_Cam' }    
        player:SetVisible{visible = false, fadeTime = 0.5}
    end
end

function onMissionDialogueCancelled(self, msg)
    player:EndCinematic()
    player:SetVisible{visible = true, fadeTime = 0.5}
end

function onTimerDone (self,msg)
    if (msg.name == 'missionCamera') then   
        -- release player movement       
        player:SetUserCtrlCompPause{bPaused = false}
    end
end

function onTerminateInteraction(self, msg)    
    if self:GetVar('DialogueOK') then return end
    player:EndCinematic()
    player:SetVisible{visible = true, fadeTime = 0.5}
end