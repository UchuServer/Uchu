function onCollisionPhantom(self, msg)                                                      
   
    -- Only the local player can collide with this
    if ( msg.objectID:GetID() == GAMEOBJ:GetLocalCharID() ) then
    
		local player = msg.objectID
    
		if not ( player:GetFlag{iFlagID = 51}.bFlag ) then
			
			player:DisplayTooltip { bShow = true, strText = Localize"AG_TOOLTIP_COLLECTIBLES", strImageName = "../textures/ui/tooltips/AG_flag.dds", iTime = 6000 } 			
			player:SetFlag{iFlagID = 51, bFlag = true}
    
	    end
	    
	    GAMEOBJ:DeleteObject(self)
	    
    end
end



