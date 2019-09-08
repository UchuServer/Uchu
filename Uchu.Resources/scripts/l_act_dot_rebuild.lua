--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')

function onStartup(self) 
	
	GAMEOBJ:GetTimer():AddTimerWithCancel( 5 , "startup", self )
end


function onRebuildComplete(self, msg)

  
         local Player1 = getObjectByName(GAMEOBJ:GetZoneControlID(), "PLAYER1")
         local Player2 = getObjectByName(GAMEOBJ:GetZoneControlID(), "PLAYER2")
    
         if  Player1:GetName().name == msg.userID:GetName().name then
             QBP = 1
         elseif   Player2:GetName().name  == msg.userID:GetName().name then
            QBP = 2
         end
    
         self:SetVar("PlayerName", msg.userID:GetName().name)
         GAMEOBJ:GetZoneControlID():NotifyObject{ name="QBuildID", param1 = QBP , ObjIDSender = self }
 
end  


onTimerDone = function (self, msg)


	if msg.name == "startup" then
	
		self:RebuildReset{ bFail = false }
	end



end

function fffRebuildComplete(self, msg)
    print("test")
    msg = temp

end

