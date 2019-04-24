-- Script that can be attached to any misison giver on the client in HF. Config data
-- variables must be set in HF. missionID, cinemaName, cinemaTime
-- Created: 4/09/09 mrb...

--//////////////////////////////////////////////////////////////////////////////////
-- local variables
local missionID = '' -- missionID from DB
local cinemaName = '' -- name of cinematic in HF
local cinemaTime = '' -- time of cinematic and to release player
local player = ''
--//////////////////////////////////////////////////////////////////////////////////

function onStartup(self)
    -- get the varables set in HF in the mission givers configData, if mission print error
    missionID = self:GetVar("missionID") -- missionID from DB
    if missionID == '' then print('Missing missionID in ConfigData') end
    cinemaName = self:GetVar("cinemaName") -- name of cinematic in HF
    if cinemaName == '' then print('Missing cinemaName in ConfigData') end
    cinemaTime = self:GetVar("cinemaTime") -- time of cinematic and to release player
    if cinemaTime == '' then print('Missing cinemaTime in ConfigData') end
    player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()) -- the player
end

function onMissionDialogueOK(self, msg)
    if not missionID or not cinemaName or not cinemaTime then return end
    
    if msg.bIsComplete == false and msg.missionID == missionID then    
        -- freeze player and play cinematic
        player:SetUserCtrlCompPause{bPaused = true}
        player:PlayCinematic { pathName = cinemaName }    
        
        -- start timer to release player movement
        GAMEOBJ:GetTimer():AddTimerWithCancel( cinemaTime, "missionCamera",self )            
    end    
end

function onTimerDone (self,msg)
    if (msg.name == "missionCamera") then   
        -- release player movement       
        player:SetUserCtrlCompPause{bPaused = false}
    end
end
