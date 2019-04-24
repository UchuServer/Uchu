

--~ function onCollision(self, msg)
--~ 	local target = msg.objectID

--~ 	local faction = target:GetFaction()
--~ 	local target = GAMEOBJ:GetZoneControlID()
--~ 	-- If a player collided with me, then do our stuff
--~ 	if faction and faction.faction == 1 then
--~         --print "just entered the if statement"
--~             self:PlayFXEffect{effectType = "pickup"}
--~ 	  target:ArcadeScoreEvent{objectID = self}
--~ 	      print("onCollision")
--~ 		--print "i just cast a skill"
--~             self:KillObj{targetID = self}
--~ 		--print "i just killed myself"
--~ 	    
--~ 	end       	
--~ 	-- Ignore this on the server actually. More of a test than needed
--~ 	msg.ignoreCollision = true
--~ 	-- ONly do this once
--~   return msg
--~ end

--L_SPECIAL_NINJA-FLAG-SPAWNER.lua

-- local newcurrency = 0



function onCollision(self, msg)
	local target = msg.objectID
       	local faction = target:GetFaction()
	local score = GAMEOBJ:GetZoneControlID()
	
	
	-- If a player collided with me, then do our stuff
	if faction and faction.faction == 1 then
        --print "just entered the if statement"
            self:PlayFXEffect{effectType = "pickup"}
		

	self:KillObj{targetID = self}	-- Send the message to the zone object, who will relay it to C++
	score:ArcadeScoreEvent{objectID = self}
			
			
			--newcurrency = target:GetCurrency().currency
            --newcurrency = newcurrency + 100
           -- target:SetCurrency {currency = newcurrency}
		--print "i just cast a skill"
            --self:KillObj{targetID = self}
		--print "i just killed myself"
	    
	end       	
	-- Ignore this on the server actually. More of a test than needed
	msg.ignoreCollision = true
	-- ONly do this once
  return msg
end