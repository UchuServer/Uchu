-- used to hide the life bars over the bomb crate smashable
function onRenderComponentReady(self, msg)
    self:SetNameBillboardState{bState = false }
end

function onOnHit(self, msg)

	-- Make it look like we died on the client.
	self:PerformClientSideDeath{i64AttackerID = msg.attacker, 
								directionRelative_AngleY = msg.directionRelative_AngleY, 
								directionRelative_AngleXZ = msg.directionRelative_AngleXZ, 
								directionRelative_Force = msg.directionRelative_Force, 
								bDontFlagAsDead = true,
								deathType = msg.deathType }
	
end
