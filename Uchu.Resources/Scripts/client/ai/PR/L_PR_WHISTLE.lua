--------------------------------------------------------------
-- Server side script on Coalessa in pet rock
-- this script unlocks the pet taming emote for the player

-- created by Brandi... 2/17/10
--------------------------------------------------------------

function onFireEventServerSide(self, msg)
    --print('onMissionDialogueOK ' .. msg.iMissionState .. ' - ' .. msg.missionID)
    
    if msg.args == 'unlockEmote' then
      
		msg.senderID:SetEmoteLockState{ emoteID=115, bLock = false }
		
	end
end