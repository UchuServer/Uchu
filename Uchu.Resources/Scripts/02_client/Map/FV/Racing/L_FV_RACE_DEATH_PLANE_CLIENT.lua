-----------------------------------------------------------
-- client-side script for the FV death planes
-- modified from the GF racing death plane script 
-- the camera is grabbed from the config data set in the object in happy flower EG: DeathCam || 0:DeathCam1
-- Steve Y .. 8-31-10
-----------------------------------------------------------

local playedDeathFX = false

function onCollisionPhantom(self, msg)
   

	--print("hit death phantom")
   
	if ( msg.objectID:GetID() == GAMEOBJ:GetControlledID():GetID() ) and not playedDeathFX then

		--print("calling cinematic")
		local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
		player:PlayCinematic { pathName = self:GetVar("DeathCam") }
		self:SendLuaNotificationRequest{requestTarget=GAMEOBJ:GetControlledID(), messageName="Resurrect"}
		playedDeathFX = true
		
	end

end

function notifyResurrect(self, object, msg)
    --print "HAI!"
    playedDeathFX = false
	self:SendLuaNotificationCancel{requestTarget=object, messageName="Resurrect"}
end