require('o_mis')

function onStartup(self)
    self:SetProximityRadius { radius = 4 , name = "petBounce" } 
	self:SetVar("active", false)
	self:SetVar("storedOnce", false)
	registerWithZoneControlObject(self)

	-- We delay storing of information for this bouncer in case the other objects in the
        -- group haven't fully loaded up yet.
	GAMEOBJ:GetTimer():AddTimerWithCancel( 3 , "DelayStore", self )
end


function StoreOnce(self)

	self:SetVar("storedOnce", true)

	local switch = self:GetObjectsInGroup{ group = self:GetVar("grp_name") }.objects
	for i = 1, table.maxn (switch) do 
            if ( switch[i]:GetLOT().objtemplate == 3463 ) then
               
		storeObjectByName(self, "pet_switch", switch[i])
               	storeObjectByName(switch[i], "pet_bouncer", self)

            end 
	end         
      	
	self:SetVar("SaveObjests", "stored") 
end 


onNotifyObject = function(self, msg)

    if msg.name == "FXON" then
        
         GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{ name="on" }
    end
	if ( msg.name == "SwitchPressed" ) then
   
		 self:SetVar("active", true)
		 bounceAllInProximity(self)
	end
end


function onCollisionPhantom(self, msg)
	
 	if ( self:GetVar("storedOnce") == false ) then

		StoreOnce(self)

	end


 	local target = msg.objectID
  	local faction = target:BelongsToFaction{factionID = 1}.bIsInFaction
  	local pet = target:GetPetID().objID
   
   	-- If object colliding is a player, and they have a pet
    	if ( faction ) then

		if ( self:GetVar("active") == false ) then

			if ( pet:Exists() ) then

				local petswitch = getObjectByName(self, "pet_switch")
				--print("BOUNCER STEPPED ON, SENDING " .. petswitch:GetID() .. " to pet")

	    			-- Have parent notify pet that it has arrived at a pet bouncer
    				pet:NotifyPet{ ObjIDSource = target, ObjToNotifyPetAbout = petswitch, iPetNotificationType = 2}

			end
		else	-- Bouncer is already active
			
			self:BouncerTriggered{triggerObj = target}

		end
    
    -- Pets can bounce as well!
    elseif ( (faction == 99) and self:GetVar("active") ) then
		
		if ( self:GetVar("active") == true ) then

			target:SetLinearVelocity{ x = 0, y = 0, z = 0 }

			-- We call this on the server. The only time we want to call a bounce message on the client
			-- is when we're bouncing client/player's. If we need to bounce anything else it should be server-side.
			bounceObj(self, target)
	
			-- Now that they've bounced, they're done with the ability state
			target:RemovePetState{ iStateType = 9 }
			-- Set them back to idling
			target:SetPetMovementState{ iStateType = 1 }

		end
    else
	
	if ( self:GetVar("active") == true ) then

		bounceObj(self, target)	

	end

    end

end

function bounceAllInProximity(self)

	local objs = self:GetProximityObjects{ name = "petBounce"}.objects
	local index = 1
	
	while index <= table.getn(objs)  do
		local target = objs[index]
		local pet = target:GetPetID().objID
			
		-- Bounce players client-side
		if target:BelongsToFaction{factionID = 1}.bIsInFaction then
			self:BouncerTriggered{triggerObj = target}
		else	-- Bounce NPCs server-side
			bounceObj(self, target)
		end

		index = index + 1

		-- If player has a pet, let them know about the bounce
		if ( pet:Exists() ) then
			-- Let pet know player has used a bouncer
			pet:NotifyPet{ ObjIDSource = target, ObjToNotifyPetAbout = self, iPetNotificationType = 3} 
		end
	end

end

function bounceObj(self, target)

	local vecString = self:GetVar("bouncer_destination")

	-- Parse the NiPoint3 from the level file into three floats
	local posX, posY, posZ = string.match(vecString, "(%p?%w+%p?%w+)\031(%p?%w+%p?%w+)\031(%p?%w+%p?%w+)")

	local speed = self:GetVar("bouncer_speed")

	--Hackish fix for the 'bounce collision'
	local objPos = target:GetPosition().pos
	objPos.y = objPos.y + 1
	target:SetPosition{pos = objPos}

	-- Create a vector in Lua to pass in message
	local vec = {x = posX, y = posY, z = posZ}

	target:BouncePlayer{niDestPt = vec, fSpeed = speed, ObjIDBouncer = self}
end

function onTimerDone(self, msg)

    if ( msg.name == "TurnOffBouncer" ) then

        self:SetVar("active", false)
--------------------------------------------------------------------------------------------
		-- remove effect for the bouncer --
--------------------------------------------------------------------------------------------
	     GAMEOBJ:GetZoneControlID():NotifyObject{ name = "off" }

    end

    if ( msg.name == "DelayStore" ) then
	StoreOnce(self)
    end
    
end

