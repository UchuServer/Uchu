

function onStartup(self)
	-- print("STARTING UP")
	--self:SetProximityRadius { radius = 10, name = "blowRadius" }
end





function onCollisionPhantom(self, msg)
   
	--print("ON COLLISSIOOOONNNN")
	
    --local target = msg.objectID
    --local faction = target:GetFaction()
    --local isfaction = msg.senderID:GetFaction().faction
   
	--if isfaction == 113 then

		--print("CASTING SKILLZZZ")
      
        --self:CastSkill{skillID = 411, optionalTargetID = target}
		--self:Die{killerID = self, killType = "VIOLENT"}
      
   -- end   
end


    
--------------------------------------------------------------
-- When enemies enter the proximity radius
--------------------------------------------------------------
--function onProximityUpdate(self, msg)

	
	--print("in prox")
	-- print("msg.name")
	-- print(msg.objId:GetFaction().faction)
	
	-- if this was a proximity update for seeking an opponent and the object is of the 'car faction'...	
	--if ( ( msg.name == "blowRadius" ) and ( msg.objId:GetFaction().faction == 113 ) ) then 
		
	--	if ( msg.status == "ENTER" ) then
	--		print("CASTING SKILL AND DIEING")
	--		 local target = msg.objectID
			-- self:CastSkill{skillID = 411, optionalTargetID = target}
			-- self:Die{killerID = self, killType = "VIOLENT"}
		--end
		
	--end
	
--end