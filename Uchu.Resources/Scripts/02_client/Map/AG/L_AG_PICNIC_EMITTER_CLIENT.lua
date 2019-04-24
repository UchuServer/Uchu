--------------------------------------------------------------

-- L_AG_PICNIC_EMITTER_CLIENT.lua

-- Client side script for picnic area audio emitter events
-- created by abeechler.. 7/5/11

--------------------------------------------------------------

local missionID = 768       -- Mission to check for picnic zone Maelstrom clearance

local PrePushbackTable = {["Ambient_Audio_Birds"] = "{c7d1870c-85f5-4def-907c-4d766200f472}",
                           ["Ambient_Audio_Edge"] = "{445b845d-fa7d-427d-a5cc-0e548356aed1}"}
                           
local PostPushbackTable = {["Ambient_Audio_Birds"] = "{6fe9f62e-cce1-4628-85f8-c909f203651f}"}

----------------------------------------------
-- Process the emitter for appropriate sound output
----------------------------------------------
function playAmbientCues(self, player)
    -- Obtain our group name for appropriate sound cue processing
    local emitterGroup = string.sub(self:GetStoredConfigData{optionalKey = "groupID"}.configData["groupID"], 1, -2)
    
    -- Obtain the emitter cue table to use based on Maelstrom state
    local emitterTable = PostPushbackTable
    if(player:GetMissionState{missionID = missionID}.missionState < 8) then
		-- Play the Maelstrom amb cues
		emitterTable = PrePushbackTable
    end
    
    -- Do we have a cue to play?
    if(emitterTable[emitterGroup]) then
        self:Play3DAmbientSound{m_NDAudioEventGUID = emitterTable[emitterGroup]}
    end
end

----------------------------------------------
-- Catch when the local player comes within ghosting distance
----------------------------------------------
function onScopeChanged(self, msg)
    -- Has the player entered ghosting range?
    if(msg.bEnteredScope) then
        -- Obtain the local player object
        local player = GAMEOBJ:GetControlledID()
        if((not player) or (not player:Exists()) or (not player:GetPlayerReady().bIsReady)) then
            -- Subscribe to a zone control object notification alerting the script
            -- when the local player object is ready
            self:SendLuaNotificationRequest{requestTarget = GAMEOBJ:GetZoneControlID(), messageName="PlayerReady"}
            return
        end
        
        -- Custom function to play the appropriate ambient sound cues
        playAmbientCues(self, player)
    end
end

----------------------------------------------
-- The zone control object says when the player is loaded
----------------------------------------------
function notifyPlayerReady(self, zoneObj, msg)
    -- Get the player
    local player = GAMEOBJ:GetControlledID()
    -- Custom function to play the appropriate ambient sound cues
    playAmbientCues(self, player)
    -- Cancel the notification request
    self:SendLuaNotificationCancel{requestTarget= GAMEOBJ:GetZoneControlID(), messageName="PlayerReady"}
end
