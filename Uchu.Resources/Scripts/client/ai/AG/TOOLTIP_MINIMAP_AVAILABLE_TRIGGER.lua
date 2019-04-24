function onCollisionPhantom(self, msg)                                                      

    -- Only the local player can collide with this
    if ( msg.objectID:GetID() == GAMEOBJ:GetLocalCharID() ) then
    
		local player = msg.objectID
    
		if not ( player:GetFlag{iFlagID = 2}.bFlag ) then
			
			-- 2 is the minimap tutorial
			player:Help{ iHelpID = 2 }
    
	    end
	    
	    GAMEOBJ:DeleteObject(self)
	    
    end
end


