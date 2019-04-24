---------------------------------------------
-- script on the shield generator in CP, displays a popup when the player is close and closes it when the player leaves the area
--
-- updated by brandi... 1/11/11 - added ability to change the radius that the health bar shows up
-- updated by brnadi... 1/18/11 - changed the size of the proximity monitor and deleted the second one
---------------------------------------------

--***************************************************
-- TO CHANGE THE RADIUS OF THE HEALTH BAR
-- the radius of the health bar needs to be added to the config data in Happy Flower on the dragon
-- healthBarRad  1:####
--****************************************************

function onStartup(self)
    self:SetRegistrationForUIUpdate{ eEventType = "POSITION_CHANGE", iMinimapObjType = 9, bRegister = true }
    
    local dist = self:GetVar("healthBarRad") or 100 --self:GetTetherPoint().radius
    self:SetProximityRadius{radius = dist, name="animProx", collisionGroup = 8, shapeType = "CYLINDER", height = 50, heightOffset = -10 }
    self:SetVar("PlayerInRange", false)
end

function onRenderComponentReady(self, msg)
	--we don't want the regular nametag to show.
	self:SetNameBillboardState{bState = false, bOverrideDefaultSetting = true}
end

-------------------------------------------------------------
-- handle proximity updates
--------------------------------------------------------------
function onProximityUpdate(self, msg)
	if msg.objId:IsCharacter().isChar and (msg.name == "animProx") then
		if msg.status == "ENTER" then
			if self:IsInCombat().bInCombat == false then
				self:PlayAnimation{animationID = "prox"}
			end
			
			if msg.objId:GetID() == GAMEOBJ:GetControlledID():GetID() then
				--turn on the health bar
				UI:SendMessage( "ToggleEnemyStatusBar", { {"visible", true}, {"healthVisible", true},
					{"armorVisible", true},
					{"nameVisible", true},
					{"health", math.floor((self:GetHealth().health / self:GetMaxHealth().health) * 100) },
					{"armor", math.floor((self:GetArmor().armor / self:GetMaxArmor().armor) * 100) },
					{"nameTxt", self:GetName().name},
					{"id", "|" .. self:GetID()} } )
					
					--we now have the local player in range
					self:SetVar("PlayerInRange", true)
			end
		elseif msg.status == "LEAVE" then
			if self:GetVar("PlayerInRange") and msg.objId:GetID() == GAMEOBJ:GetControlledID():GetID() then
				--turn off the health bar, we don't have a player near us anymore :(
				UI:SendMessage( "ToggleEnemyStatusBar", { {"visible", false} } )
				self:SetVar("PlayerInRange", false)
			end
		end
	end
end

function onOnHit(self, msg)
	if self:GetVar("PlayerInRange") then
		--Ouch, update the health bar
		UI:SendMessage( "UpdateEnemyStatusBar", { {"healthVisible", true},
			{"armorVisible", true},
			{"nameVisible", true},
			{"health", math.floor((self:GetHealth().health / self:GetMaxHealth().health) * 100) },
			{"armor", math.floor((self:GetArmor().armor / self:GetMaxArmor().armor) * 100) },
			{"nameTxt", self:GetName().name} } )
	end
end

function onDie(self, msg)
	if self:GetVar("PlayerInRange") then
		--Alack, I have died, no more health bar.
		UI:SendMessage( "ToggleEnemyStatusBar", { {"visible", false} } )
	end
end

function onNotifyClientObject(self, msg)
    if msg.name == "DragonRevive" then
		if self:GetVar("PlayerInRange") then
			--I revived and there's a player nearby, tell them I'm back! Rarr!
			UI:SendMessage( "UpdateEnemyStatusBar", { {"healthVisible", true},
				{"armorVisible", true},
				{"nameVisible", true},
				{"health", math.floor((self:GetHealth().health / self:GetMaxHealth().health) * 100) },
				{"armor", math.floor((msg.param1 / self:GetMaxArmor().armor) * 100) },
				{"nameTxt", self:GetName().name} } )
		end
    end
end