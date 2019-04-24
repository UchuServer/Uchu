require('o_mis')

-- OnEnter in HF Trigger system
function onCollisionPhantom(self, msg)
  	 local objID = msg.objectID
  	 local player = msg.senderID

    if GAMEOBJ:GetTimer():GetTime(player:GetName().name,self ) == 0 and msg.senderID:BelongsToFaction{factionID = 113}.bIsInFaction then
       -- --print("Vol Hit "..self:GetVar("num") )

        GAMEOBJ:GetTimer():AddTimerWithCancel( 1 , player:GetName().name , self )
        GAMEOBJ:GetZoneControlID():NotifyObject{ name="DriverPosition", ObjIDSender = player,  param1 = tonumber( self:GetVar("num"))  }
    end 	    
end