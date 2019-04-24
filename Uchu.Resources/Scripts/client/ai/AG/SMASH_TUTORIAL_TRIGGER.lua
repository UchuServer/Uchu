function onCollisionPhantom(self, msg)              
        
    -- Only the local player can collide with this
    if ( msg.objectID:GetID() == GAMEOBJ:GetLocalCharID() ) then
    
		local player = msg.objectID
    
		if not ( player:GetFlag{iFlagID = 30}.bFlag ) then
				           
			-- 30 is the smashing controls tutorial
			player:Help{ iHelpID = 30 }
    
	    end
	    
	    GAMEOBJ:DeleteObject(self)
	    
    end
end



