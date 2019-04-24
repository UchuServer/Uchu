--------------------------------------------------------------
-- Server-side death trigger that will kill the player with a
-- specific animation. Change the Custom Variables to fit your 
-- needs.
-- mrb... 5/21/09
-- djames: updated 9/28/09

--------------------------------------------------------------

local deathAnimation = "big-shark-death" 


function onFireEventServerSide(self, msg)

	if (msg.args == "achieve" ) then
	
	-- Note the reverse order of the updates; if you don't do it this way, it will update 445 and then immediately update 446 which we don't want in the same death.
		msg.senderID:UpdateMissionTask{taskType = "complete", value = 447, value2 = 1, target = self}
		msg.senderID:UpdateMissionTask{taskType = "complete", value = 446, value2 = 1, target = self}
		msg.senderID:UpdateMissionTask{taskType = "complete", value = 445, value2 = 1, target = self}

	end

end

function onCollisionPhantom(self, msg)
    
	if msg.objectID:BelongsToFaction{factionID = 1}.bIsInFaction then
	
		msg.objectID:RequestDie{killerID = self, deathType = deathAnimation}
		
	else
	
		msg.objectID:RequestDie{killerID = self, killType = VIOLENT}
		
	end
	
end