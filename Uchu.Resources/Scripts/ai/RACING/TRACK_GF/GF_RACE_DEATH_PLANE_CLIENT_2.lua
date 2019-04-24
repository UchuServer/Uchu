-----------------------------------------------------------
-- client-side script for the Gnarled Forest death plane
-----------------------------------------------------------

function onStartup(self)
    self:SetVar("bIsDead", false)
end

function onCollisionPhantom(self, msg)
   
	--print("hit death phantom")
   
    local target = msg.objectID
   
	if ( target:GetID() == GAMEOBJ:GetControlledID():GetID() ) then

		--print("calling cinematic")
		local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
		player:PlayCinematic { pathName = "DeathVol2" }
		
	end

end