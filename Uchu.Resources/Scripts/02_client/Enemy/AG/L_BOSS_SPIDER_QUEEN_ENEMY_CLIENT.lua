--------------------------------------------------------------

-- L_BOSS_SPIDER_QUEEN_ENEMY_CLIENT.lua

-- Client side Spider Queen Boss fight behavior script
-- created abeechler ... 6/1/11

--------------------------------------------------------------

local SpiderQueenGUIDs = {["Spider_Scream"] = "{446b8ae0-4d72-478d-8927-f65e880a382b}"}

----------------------------------------------
-- Object set-up
----------------------------------------------
function onStartup(self)
    -- Register for the mini-map
    self:SetRegistrationForUIUpdate{eEventType = "POSITION_CHANGE", iMinimapObjType = 9, bRegister = true}
    
    -- Establish the proximity radius
    local dist = self:GetVar("healthBarRad") or 100
    self:SetProximityRadius{radius = dist, name="animProx", collisionGroup = 8, shapeType = "CYLINDER", height = 30, heightOffset = -10 }
    self:SetVar("PlayerInRange", false)
    
end

----------------------------------------------
-- Object zone script data collection
----------------------------------------------
function onRenderComponentReady(self, msg) 
	-- Query our containing property for the IDs of various spawner networks
	self:FireEventServerSide{args = "QueryZoneScript", senderID = self}
	
	-- We don't want the regular nametag to show
	self:SetNameBillboardState{bState = false, bOverrideDefaultSetting = true}
	
end

-------------------------------------------------------------
-- Handle proximity updates
-------------------------------------------------------------
function onProximityUpdate(self, msg)

	if msg.objId:IsCharacter().isChar and (msg.name == "animProx") then
	
		if msg.status == "ENTER" then
			if msg.objId:GetID() == GAMEOBJ:GetControlledID():GetID() then
				-- Turn on the health bar
				UI:SendMessage( "ToggleEnemyStatusBar", { {"visible", true}, {"healthVisible", true},
					{"armorVisible", false},
					{"nameVisible", true},
					{"health", math.floor((self:GetHealth().health / self:GetMaxHealth().health) * 100) },
					{"armor", math.floor((self:GetArmor().armor / self:GetMaxArmor().armor) * 100) },
					{"nameTxt", self:GetName().name},
					{"id", "|" .. self:GetID()} } )
					
					-- We now have the local player in range
					self:SetVar("PlayerInRange", true)
			end
			
		elseif msg.status == "LEAVE" then
			if self:GetVar("PlayerInRange") and msg.objId:GetID() == GAMEOBJ:GetControlledID():GetID() then
				-- Turn off the health bar, we don't have a player near us anymore :(
				UI:SendMessage( "ToggleEnemyStatusBar", { {"visible", false} } )
				self:SetVar("PlayerInRange", false)
			end
		end
	end
	
end

-------------------------------------------------------------
-- Update the Spider Boss health bar on damage done
-------------------------------------------------------------
function onOnHit(self, msg)

	if self:GetVar("PlayerInRange") then
		-- Ouch, update the health bar
		UI:SendMessage( "UpdateEnemyStatusBar", { {"healthVisible", true},
			{"armorVisible", false},
			{"nameVisible", true},
			{"health", math.floor((self:GetHealth().health / self:GetMaxHealth().health) * 100) },
			{"armor", math.floor((self:GetArmor().armor / self:GetMaxArmor().armor) * 100) },
			{"nameTxt", self:GetName().name} } )
	end
	
end

-------------------------------------------------------------
-- Toggle off the Boss health bar on Spider Boss death
-------------------------------------------------------------
function onDie(self, msg)

	if self:GetVar("PlayerInRange") then
		--Alack, I have died, no more health bar.
		UI:SendMessage( "ToggleEnemyStatusBar", { {"visible", false} } )
	end
	
end

-------------------------------------------------------------
-- Process server object process calls
-------------------------------------------------------------
function onNotifyClientObject(self, msg)

	if msg.name == "SetColGroup" then
		self:SetCollisionGroup{colGroup = msg.param1}
    
    elseif msg.name == "EmitScream" then
        -- Play the Spider Queen mountaintop scream
	    msg.paramObj:Play3DAmbientSound{m_NDAudioEventGUID = SpiderQueenGUIDs["Spider_Scream"]}
    
	end	
	
end
