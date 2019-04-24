function onCollisionPhantom(self, msg)                                                      
   
    -- Only the local player can collide with this
    if ( msg.objectID:GetID() == GAMEOBJ:GetLocalCharID() ) then
    
		--print("collided")
        local player = msg.objectID
    
		local msgcheck = player:CheckPrecondition{PreconditionID = 63, iPreconditionType = 0}
        
        if not msgcheck.bPass then
            --print("failed")
            player:DisplayTooltip { bShow = true, strText = msgcheck.FailedReason, iTime = 3000 }
        
        end
        
	    --GAMEOBJ:DeleteObject(self)
	    
    end
end