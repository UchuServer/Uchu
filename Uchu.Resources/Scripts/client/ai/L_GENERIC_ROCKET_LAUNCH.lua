require('o_mis')

function onStartup(self)
    if not self:GetVar('ZoneToMap') then
        print('** Missing Config Data (newZone) for Rocket Transition **')
        self:SetVar('ZoneToMap', 30)
    end
end

function onRebuildNotifyState(self, msg)
	if msg.iState == 2 then
		--if self:ActivityUserExists{userID = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())}.bExists then
		storeObjectByName(self, "Rocketbuilder", msg.player)
			GAMEOBJ:GetTimer():AddTimerWithCancel( 0.01, "PlayerRebuild",self )
			GAMEOBJ:GetTimer():AddTimerWithCancel( 2.9, "PlayerSoarHack",self )
			GAMEOBJ:GetTimer():AddTimerWithCancel( 3.0, "PlayerAnimate",self )
			GAMEOBJ:GetTimer():AddTimerWithCancel( 5.1, "RocketLaunch",self )
			GAMEOBJ:GetTimer():AddTimerWithCancel( 7.06, "RocketFiring",self )
			-- trying to prevent aother animations breaking the rocket
		if GAMEOBJ:GetLocalCharID() == msg.player then
			local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
			player:SetUserCtrlCompPause{bPaused = true}
		end
        
    end
end

function onTimerDone (self,msg)

    local player = getObjectByName(self, "Rocketbuilder")
    local rocketPosition = self:GetPosition{}.pos
    local rocketRotation = self:GetRotation{}

	if (msg.name == "PlayerRebuild") then
        if(player:GetID() == GAMEOBJ:GetLocalCharID() ) then
            player:PlayAnimation{animationID = "rebuild-rocket"}
            player:SetUserCtrlCompPause{bPaused = true}
        end
    end

	if (msg.name == "PlayerSoarHack") then
        if(player:GetID() == GAMEOBJ:GetLocalCharID() ) then
            player:PlayAnimation{animationID = "rocket-soar"}
        end
    end

	if (msg.name == "PlayerAnimate") then
		self:PlayAnimation{animationID = "launch-AG"}

        if(player:GetID() == GAMEOBJ:GetLocalCharID() ) then
            player:SetPosition {pos = rocketPosition}
            player:SetRotation {x = rocketRotation.x, y = rocketRotation.y, z = rocketRotation.z, w = rocketRotation.w}
            player:PlayCinematic { pathName = self:GetVar("cameraPath") }
        end

        player:PlayAnimation{animationID = "rocket-launch-AG"} -- if we don't do this, people don't see other clients animate
    end

    if (msg.name == "RocketLaunch") then
		getObjectByName(self, "sectionTop"):PlayFXEffect{effectType = "launch"}
		getObjectByName(self, "sectionBot"):PlayFXEffect{effectType = "launch"}
    end

	if (msg.name == "RocketFiring") then
		getObjectByName(self, "sectionTop"):PlayFXEffect{effectType = "firing"}
		getObjectByName(self, "sectionBot"):PlayFXEffect{effectType = "firing"}
		
        -- Set zone player timer
		local zTime = getObjectByName(self, "sectionTop"):GetAnimationTime{animationID = "firing"}.time
		GAMEOBJ:GetTimer():AddTimerWithCancel( zTime, "ZoneTimer",self )
    end
    if (msg.name == "ZoneTimer") then
        self:FireEventServerSide{senderID = player, args = "ZonePlayer", param1 = self:GetVar('ZoneToMap')}
        -- set flag to false so we know the player has left the zone
        player:SetFlag{iFlagID = 32, bFlag = false}  
        
        --player:DisplayZoneSummary{sender = self, show = true}
        
        --local objs = self:GetObjectsInGroup{ group = "ZoneTransferObj", ignoreSpawners = true }.objects

        --for i = 1, table.maxn ( objs ) do
        --    player:DisplayZoneSummary{sender = objs[i], show = true}
        --    player:PlayCinematic { pathName = "AG_Summary" }
             
        --end

    end
end

function onCustomRebuildSelected(self, msg)
	self:PlayFXEffect{effectType = "rebuild-complete"}
	
	if self:ActivityUserExists{userID = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())}.bExists then
        GAMEOBJ:GetZoneControlID():NotifyObject{name = "customRocketSelected1", param1 = msg.choice1LOT}
        GAMEOBJ:GetZoneControlID():NotifyObject{name = "customRocketSelected2", param1 = msg.choice2LOT}
        GAMEOBJ:GetZoneControlID():NotifyObject{name = "customRocketSelected3", param1 = msg.choice3LOT}
	end
end

function onChildLoaded(self, msg)
	--TODO: Make this better
--print "child loaded"
	if msg.templateID == 4713 or msg.templateID == 4716 or msg.templateID == 4719 then
		storeObjectByName(self, "sectionTop", msg.childID)
	elseif msg.templateID == 4714 or msg.templateID == 4717 or msg.templateID == 4720 then
		storeObjectByName(self, "sectionMid", msg.childID)
	elseif msg.templateID == 4715 or msg.templateID == 4718 or msg.templateID == 4721 then
		storeObjectByName(self, "sectionBot", msg.childID)
	end
end

--function rotateVector(quaternion, vector)
--    local matrix = {}  
--    matrix[1][1] = 1 - 2*math.pow(quaternion.y, 2) - 2*math.pow(quaternion.z, 2)
--    matrix[1][2] = 2*quaternion.x*quaternion.y - 2*quaternion.z*quaternion.w
--end


--1 - 2*qy2 - 2*qz2  	2*qx*qy - 2*qz*qw  	2*qx*qz + 2*qy*qw
--2*qx*qy + 2*qz*qw 	1 - 2*qx2 - 2*qz2 	2*qy*qz - 2*qx*qw
--2*qx*qz - 2*qy*qw 	2*qy*qz + 2*qx*qw 	1 - 2*qx2 - 2*qy2