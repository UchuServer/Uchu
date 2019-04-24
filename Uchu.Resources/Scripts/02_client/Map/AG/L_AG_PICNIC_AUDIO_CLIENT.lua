--------------------------------------------------------------

-- L_AG_PICNIC_AUDIO_CLIENT.lua

-- Client side script for picnic area audio events
-- created by abeechler.. 6/27/11

--------------------------------------------------------------

local missionID = 768       -- Mission to check for picnic zone Maelstrom clearance

----------------------------------------------------------------
-- Store music var references
----------------------------------------------------------------
local MaelMusicCue = "Property_Maelstrom"
local ClearedMusicCue = "AG_Launch"

----------------------------------------------------------------
-- Detect volume collisions for sound event updates
----------------------------------------------------------------
function onCollisionPhantom(self, msg)
    -- Obtain the local client player ID
    local player = GAMEOBJ:GetControlledID()
    
    if msg.objectID:GetID() == player:GetID() then
		local soundCue = ClearedMusicCue
		
		if(player:GetMissionState{missionID = missionID}.missionState < 8) then
		    -- Play the Maelstrom music cue
		    soundCue = MaelMusicCue
		end
		
		SOUND:ActivateNDAudioMusicCue(soundCue)
	end
end

----------------------------------------------------------------
-- Detect off volume collisions for sound event updates
----------------------------------------------------------------
function onOffCollisionPhantom(self, msg)
    -- Obtain the local client player ID
    local player = GAMEOBJ:GetControlledID()
    
    if msg.objectID:GetID() == player:GetID() then
		local soundCue = ClearedMusicCue
		
		if(player:GetMissionState{missionID = missionID}.missionState < 8) then
		    -- Play the Maelstrom music cue
		    soundCue = MaelMusicCue
		end
		
		SOUND:DeactivateNDAudioMusicCue(soundCue)
	end
end
