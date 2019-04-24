--------------------------------------------------------------
-- script on the wandering vendor in Aura mar
-- used to handle animation calls specific to this vendor

-- created by MEdwards 1/5/11
--------------------------------------------------------------

function onStartup(self)
	
    self:SetProximityRadius{radius = 10, collisionGroup = 1}
end 

function onProximityUpdate(self,msg)
	-- someone entered the vendors bubble
    if msg.status == "ENTER" then
		-- get the intruder
        local player = msg.objId
		-- does the intruder really exist?
        if player:Exists() then 
			-- its a player, pause a second and welcome them
			 GAMEOBJ:GetTimer():AddTimerWithCancel( 1 , "welcomeShopper", self )   
        end
	-- someone left the vendors bubble
    elseif msg.status == "LEAVE" then
		-- get all objects still in the vendors bubble
		local proxObjs = self:GetProximityObjects().objects	
		-- check to see if there were objects in the bubble
		if #proxObjs > 0 then   
			--if it finds anything return out of the function
			return
		end
		-- if no player was found in the vendors bubble, tell the vendor to continue on his way
        self:PlayAnimation{animationID = "vendor-leave"}
   end
end

function onTimerDone(self, msg)
    if msg.name == "welcomeShopper" then
        self:PlayAnimation{animationID = "vendor-welcome"}
    end  
end