---- Script that can be attached to any misison giver on the client in HF. Config data
---- variables must be set in HF. missionID, cinemaName, cinemaTime
---- Created: 6/16/09 mrb...

----//////////////////////////////////////////////////////////////////////////////////
---- local variables
local missionCamDist = 10 -- distance to teleport player away from npc
local npcName = '' -- name of the npc giving in the Config Data of HF
local player = ''
----//////////////////////////////////////////////////////////////////////////////////

function onStartup(self) 
    -- get the varables set in HF in the mission givers configData, if mission print error
    npcName = self:GetVar('npcName') -- name of npc in HF
    if npcName == '' then print('Missing npcName in ConfigData') end
    
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
    if not npcName then return end

    player:EndCinematic()
    player:SetVisible{visible = true, fadeTime = 0.5}
    self:SetVar('DialogueOK', true)
    --if msg.bIsComplete == false then   
    --    player:SetUserCtrlCompPause{bPaused = true}       
    --end
end

function onNotifyMission(self, msg)
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()) 
    if msg.missionState <= 1 then
        player:PlayCinematic { pathName = 'MissionDisplay_' .. self:GetVar('npcName') .. '_Cam' }   
        player:SetVisible{visible = false, fadeTime = 0.5} 
    end
end

function onTerminateInteraction(self, msg)    
    if self:GetVar('DialogueOK') then return end
    
    player:EndCinematic()
    player:SetVisible{visible = true, fadeTime = 0.5}
end

function onMissionDialogueCancelled(self, msg) 
    player:EndCinematic()
    player:SetVisible{visible = true, fadeTime = 0.5}
end