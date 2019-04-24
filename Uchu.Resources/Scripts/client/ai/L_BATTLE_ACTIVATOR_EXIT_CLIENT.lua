--------------------------------------------------------------
-- Battle instance Activators single and multiple player
-- created by Trent
-- updated mrb... 5/4/10
--------------------------------------------------------------

--------------------------------------------------------------
-- Sent when a player enter/leave a Proximity Radius
--------------------------------------------------------------
function onProximityUpdate(self, msg)
    local playerID = GAMEOBJ:GetControlledID():GetID() 
    -- check to see if we are the correct player
    if playerID ~= msg.objId:GetID() then return end
    
    -- send the correct UI message 
    if (msg.status == "ENTER") then
 
    end
      
end 
--------------------------------------------------------------
-- Spawn Temp FX Rings
--------------------------------------------------------------
function onRenderComponentReady(self, msg)

		local Markpos = self:GetPosition().pos 
		local Markrot = self:GetRotation()
		
		RESMGR:LoadObject { objectTemplate = 10068 , x = Markpos.x ,
		y = Markpos.y - 4 , z = Markpos.z ,rw = Markrot.w , rx = Markrot.x, ry = Markrot.y  , 
		rz = Markrot.z, owner = self }; 

		RESMGR:LoadObject { objectTemplate = 10068 , x = Markpos.x ,
		y = Markpos.y - 8 , z = Markpos.z ,rw = Markrot.w , rx = Markrot.x, ry = Markrot.y  , 
		rz = Markrot.z, owner = self }; 


end

--------------------------------------------------------------
-- ON CLIENT USE check Missions
--------------------------------------------------------------

function onClientUse(self,msg)                      

    local player = msg.user
	 player:DisplayMessageBox{bShow = true, imageID = 1, callbackClient = self, text = "Exit?" , identifier = "Exit"}
	 
	
end
function onMessageBoxRespond(self,msg)
   local player = msg.sender
   if msg.iButton == 1 and msg.identifier == "Exit" then
   
         if player ~= nil then 
            player:TransferToLastNonInstance{ playerID = player, bUseLastPosition = true } 
         end
   end

end

