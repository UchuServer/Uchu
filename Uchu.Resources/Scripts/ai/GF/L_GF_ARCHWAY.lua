--------------------------------------------------------------
-- Server Script on the archway in Gnarled Forest
-- this script casts skill on rebuild compete
-- trent... 06/09/10
--------------------------------------------------------------


function onRebuildComplete(self,msg)

	if msg.userID then
	
		 msg.userID:CastSkill{skillID = 863  , optionalTargetID = msg.userID } 
	end

end
