require('o_mis')
CONSTANTS = {}
CONSTANTS["PLAYER_RESPAWN_TIME"] = 5
function onStartup(self)

	
end

function onCollisionPhantom (self,msg)

	 local target = msg.objectID
 	 if  target:GetFaction().faction == 1  or target:GetFaction().faction == 7  then 
           target:SetUserCtrlCompPause{bPaused = true}
	        local theTime = GAMEOBJ:GetSystemTime()
		
            
			target:DisplayChatBubble{wsText = CONSTANTS["PLAYER_RESPAWN_TIME"] }
			GAMEOBJ:GetTimer():AddTimerWithCancel( 1 , "count", self )
	 end 
   storeObjectByName(self, "player", target)
end

onTimerDone = function(self, msg)

    if msg.name == "count" and CONSTANTS["PLAYER_RESPAWN_TIME"] > 0 then
        player = getObjectByName(self, "player")
        s = CONSTANTS["PLAYER_RESPAWN_TIME"] - 1
        CONSTANTS["PLAYER_RESPAWN_TIME"] =  CONSTANTS["PLAYER_RESPAWN_TIME"] - 1 
        player:DisplayChatBubble{wsText = tostring(s) }
        timeplus(self) 
    else
        player = getObjectByName(self, "player")
       
        CONSTANTS["PLAYER_RESPAWN_TIME"] = 5
 

            player:SetUserCtrlCompPause{bPaused = false}
            local spawnPoint = self:GetObjectsInGroup{ group = "vol2" }.objects
        
             
            local spawnpos = spawnPoint[1]:GetPosition().pos
            player:Teleport{pos = spawnpos }
                   
               
                
     end
end

function timeplus(self) 

    GAMEOBJ:GetTimer():AddTimerWithCancel( 1 , "count", self )

end

