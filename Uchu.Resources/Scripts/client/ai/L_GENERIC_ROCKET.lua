require('o_mis')

function onCollisionPhantom(self, msg)
	if GAMEOBJ:GetLocalCharID() == msg.senderID:GetID() then
        print "CollisionPhantom"
	    storeObjectByName(self, "Rocketbuilder", msg.senderID)
		GAMEOBJ:GetTimer():AddTimerWithCancel( 2.9, "PlayerSoarHack",self )
		GAMEOBJ:GetTimer():AddTimerWithCancel( 3.0, "PlayerAnimate",self )
		GAMEOBJ:GetTimer():AddTimerWithCancel( 5.1, "RocketLaunch",self )
		GAMEOBJ:GetTimer():AddTimerWithCancel( 7.06, "RocketFiring",self )

	    print "is local char"
		local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
		player:SetUserCtrlCompPause{bPaused = true}

        for i =0, player:GetInventorySize{inventoryType = 1 }.size  do
            if player:GetInventoryItemInSlot{slot = i }.itemID:Exists() then
                if player:GetInventoryItemInSlot{slot = i }.itemID:GetLOT{}.objtemplate >= 4684 
                and player:GetInventoryItemInSlot{slot = i }.itemID:GetLOT{}.objtemplate <=4721 then
                   local item = player:GetInventoryItemInSlot{slot = i }.itemID:GetLOT{}.objtemplate
                   self:SetVar("rocketLOT", item)
                   print ("Found rocket " .. item .. "in slot " .. i)

                   RESMGR:LoadObject { objectTemplate =  item , owner = self }

                   break
                end
            end
        end
	end

end

function onStartup (self)
    print "Generic Rocket Starting"
end

function onChildLoaded(self, msg)
    print "ChildLoaded"	
	    if msg.templateID == self:GetVar("rocketLOT") then
	        storeObjectByName(self,"rocketID",msg.childID)
	    end
end



function onTimerDone (self,msg)
    print ("TimerDone " .. msg.name)
    local player = getObjectByName(self, "Rocketbuilder")
    local rocketPosition = self:GetPosition{}.pos
    local rocketRotation = self:GetRotation{}

	if (msg.name == "PlayerSoarHack") then
        if(player:GetID() == GAMEOBJ:GetLocalCharID() ) then
            player:PlayAnimation{animationID = "rocket-soar"}
        end
    end

	if (msg.name == "PlayerAnimate") then
		self:PlayAnimation{animationID = "launch"}

        if(player:GetID() == GAMEOBJ:GetLocalCharID() ) then
            player:SetPosition {pos = rocketPosition}
            player:SetRotation {x = rocketRotation.x, y = rocketRotation.y, z = rocketRotation.z, w = rocketRotation.w}
            player:PlayCinematic { pathName = self:GetVar("cameraPath") }
        end

        player:PlayAnimation{animationID = "rocket-launch"} -- if we don't do this, people don't see other clients animate
    end

    if (msg.name == "RocketLaunch") then
		--getObjectByName(self, "sectionTop"):PlayFXEffect{effectType = "launch"}
		--getObjectByName(self, "sectionBot"):PlayFXEffect{effectType = "launch"}
        --getObjectByName(self, "rocketID"):PlayFXEffect{effectType="launch"}
    end

	if (msg.name == "RocketFiring") then
		--getObjectByName(self, "sectionTop"):PlayFXEffect{effectType = "firing"}
		--getObjectByName(self, "sectionBot"):PlayFXEffect{effectType = "firing"}
        --getObjectByName(self, "rocketID"):PlayFXEffect{effectType="firing"}
		
        -- Set zone player timer
		local zTime = 2 --getObjectByName(self, "rocketID"):GetAnimationTime{animationID = "firing"}.time
		GAMEOBJ:GetTimer():AddTimerWithCancel( zTime, "ZoneTimer", self )
    end
    if (msg.name == "ZoneTimer") then
        print ("ZoneTimer " .. self:GetVar("zoneID"))
        self:FireEventServerSide{senderID = player, args = "ZonePlayer", param1 = self:GetVar("zoneID")}
    end
end
