function onCollisionPhantom(self, msg)
	local target = msg.objectID
    	
	local faction = target:GetFaction()
	
	-- If a player collided with me, then do our stuff
	if faction and faction.faction == 1 then
	
	target:SetSleddingState{bSleddingState = true}	

    	end
	
	return msg
end

