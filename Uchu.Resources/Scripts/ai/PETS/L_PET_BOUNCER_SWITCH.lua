require('o_mis')


function onStartup(self)
	self:SetVar("activator", 0)
	self:SetVar("switch", "up") 
end

function onCollisionPhantom(self, msg)

	local target = msg.objectID
	
	-- If pet
	if target:BelongsToFaction{factionID = 99}.bIsInFaction then
	
		if (self:GetVar("switch") == "up") then

			local petAbilityMsg = target:GetPetAbilityObject()

			if petAbilityMsg.bHasAbilityObj == true and petAbilityMsg.ObjIDAbilityObj:GetID() == self:GetID() then

				self:PlayAnimation{animationID = "down"}
				self:SetVar("switch", "down") 
				local petbouncer = getObjectByName(self, "pet_bouncer")
		    		petbouncer:NotifyObject{ name = "FXON" }

			end
		    
		end
	
		-- Notify pet they're at a jump a ctivated object
		target:NotifyPet{ ObjIDSource = target, ObjToNotifyPetAbout = self, iPetNotificationType = 4 }	      		
     	end
end

function onOffCollisionPhantom(self, msg)

	local target = msg.senderID

	-- Make sure this is a pet 
	if target:BelongsToFaction{factionID = 99}.bIsInFaction then
	
		if self:GetVar("switch") == "down" then
				
			self:PlayAnimation{animationID = "up"}
			self:SetVar("switch", "up") 		
		end
	
		
		local bouncer = getObjectByName(self, "pet_bouncer")
        bouncer:NotifyObject{ name = "FXOFF" }
		-- Notify pet they've left the pet switch
		target:NotifyPet{ ObjIDSource = target, ObjToNotifyPetAbout = self, iPetNotificationType = 5 }

		-- If the pet that activated the switch has left then turn off the bouncer in 3 seconds
		if ( target:GetID() == getObjectByName(self, "activator"):GetID() ) then
            self:SetVar("activator", 0)
			local petbouncer = getObjectByName(self, "pet_bouncer")
			GAMEOBJ:GetTimer():AddTimerWithCancel( 3 , "TurnOffBouncer", petbouncer )		
        
		end			
	end
end

function onNotifyObject(self, msg)
	if ( msg.name == "petjumpedon" ) then
		local petbouncer = getObjectByName(self, "pet_bouncer")

	    	petbouncer:NotifyObject{ name="SwitchPressed" }

		-- Store the ID of the pet that jumped on the switch
		storeObjectByName(self, "activator", msg.ObjIDSender)
	end
end




