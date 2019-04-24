-----------------------------------------------------------
--smashables for racing
-----------------------------------------------------------

function onStartup(self)
    self:SetVar("bIsDead", false)
end


function onDie(self, msg)

    if ( self:GetVar("bIsDead") == true ) then
        return
    end
    
	--print("onDie Crate")
		
	local target = msg.killerID
    
    -- update the killer's smashable achievement count
	target:RacingPlayerEvent{ eventType="SMASHED_SOMETHING", playerID=target, objectID=self }
     
	self:SetVar("bIsDead", true)

end