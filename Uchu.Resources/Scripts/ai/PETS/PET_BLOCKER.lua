function onCollisionPhantom(self, msg)
	local target = msg.objectID
    	
	local faction = target:GetFaction()
	--print("Hit Collision") 
	-- If a player collided with me, then do our stuff
	if faction and faction.faction == 88 then
		local player = msg.objectID:GetParentObj().objIDParent
	
		--print(player:GetName().name)
		
    end
          
      
end

