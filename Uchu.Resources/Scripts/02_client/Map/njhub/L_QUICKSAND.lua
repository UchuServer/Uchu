--------------------------------------------------------------
-- client side Script for the quicksand in the earth transition of the monastery
-- 
-- created by brandi... 6/6/11 
--------------------------------------------------------------

function onCollisionPhantom(self, msg)

	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	--Checking to see if the player still exists
	if (not( player:GetID() == msg.objectID:GetID()) ) or ( not player:Exists() ) then return end
	-- change the players speed, slow them down
	player:ChangeIdleFlags{on = 2}
    player:AddRunSpeedModifier{ uiModifier = 100, i64Caster = self }
	-- stop the players velocity
	local vel = {x = 0, y = 0, z = 0}
	player:SetLinearVelocity{ linVelocity = vel }
	-- turn the gravity down
	player:SetGravityScale{ scale = 0.03 }
	--player can jump without having support
	player:SetAllowJumpWithoutSupport{ bAllow = true }
	-- lower the players jump height so they cant jumpu out of the quicksand as easily
	player:SetJumpHeightScale{ fScale = 0.7 }
	-- turn the players resistance up
	player:SetVelocityResistance{ resistance = 0.9 }
	-- player the sand particles fx
	player:PlayFXEffect{name = "sand_particles", effectType = "create", effectID = 7542}
	
end


function onOffCollisionPhantom(self, msg)
	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	--Checking to see if the player still exists
	if (not( player:GetID() == msg.objectID:GetID()) ) or ( not player:Exists() ) then return end
	-- turn off the players run modifier
    player:ChangeIdleFlags{off = 2}
    player:RemoveRunSpeedModifier{ uiModifier = 100, i64Caster = self }
	-- set the gravity back to normal
	player:SetGravityScale{ scale = 1.0 }
	-- turn off the players jump support
	player:SetAllowJumpWithoutSupport{ bAllow = false }
	-- set the players jump height back to normal
	player:SetJumpHeightScale{ fScale = 1.0 }
	-- set the players resistance back to normal
	player:SetVelocityResistance{ resistance = 0.0 }
	-- give the player a little velocity to help them get out of the quicksand
	local vel = {x = 0, y = 6, z = 0}
	player:ModifyLinearVelocity{ linVelocity = vel }
	-- stop playing the sand particle fx
	player:StopFXEffect{ name = "sand_particles" }	

end