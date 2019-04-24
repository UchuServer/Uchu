--L_ACT_CANNON.lua
-- Server Side

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')

--------------------------------------------------------------
-- Constants
--------------------------------------------------------------
CONSTANTS = {}

-- Default Skill ID for the projectile
CONSTANTS["IMPACT_SKILLID"] = 169

-- template for the projectile
CONSTANTS["PROJECTILE_TEMPLATEID"] = 4802

-- offset to place the player during the activity
CONSTANTS["CANNON_PLAYER_OFFSET"] = {x = 6.652, y = 0, z = -5.716}

-- velocity of the projectile
-- max distance = (v^2 / g) where v = velocity and g = the effect of gravity from the database.
-- original value = 100.  changed to 140
CONSTANTS["CANNON_VELOCITY"] = 90.0

-- Minimum distance that the cannon can hit
CONSTANTS["CANNON_MIN_DISTANCE"] = 20.0

-- cooldown time for firing
CONSTANTS["CANNON_REFIRE_RATE"] = 1000.0

-- muzzle offset
CONSTANTS["CANNON_BARREL_OFFSET"] = {x = 0, y = 4.3, z = 9}

-- cannon time out for activity
CONSTANTS["CANNON_TIMEOUT"] = 64.0


--------------------------------------------------------------
-- Startup
--------------------------------------------------------------
function onStartup(self)

    -- default the instance vars
	vec = {x = 0, y = 0, z = 0}
	self:SetVar("initVelVec",vec)
	
	-- set the parameters of the shooting gallery
	self:SetShootingGalleryParams{playerPosOffset		= CONSTANTS["CANNON_PLAYER_OFFSET"], 
	                              projectileVelocity	= CONSTANTS["CANNON_VELOCITY"], 
	                              cooldown				= CONSTANTS["CANNON_REFIRE_RATE"], 
	                              muzzlePosOffset		= CONSTANTS["CANNON_BARREL_OFFSET"], 
	                              minDistance			= CONSTANTS["CANNON_MIN_DISTANCE"], 
	                              timeLimit				= CONSTANTS["CANNON_TIMEOUT"]}

	-- set the impact skill (forwards to the projectile)
	--self:SetActiveProjectileSkill{ skillID = CONSTANTS["IMPACT_SKILLID"] }
	self:SetVar("ImpactSkillID",CONSTANTS["IMPACT_SKILLID"])
	
	-- send an object loaded message to the ZoneControl object
	GAMEOBJ:GetZoneControlID():ObjectLoaded{objectID = self, templateID = self:GetLOT().objtemplate}
	
end


--------------------------------------------------------------
-- Gets the current activity user or returns nil
--------------------------------------------------------------
function getActivityUser(self)

    local targetID = self:GetActivityUser().userID
    if (targetID == 0 or targetID == nil) then
		return nil
	else
		return targetID
	end
	
end


--------------------------------------------------------------
-- Called after loading a projectile
--------------------------------------------------------------
function onChildLoaded(self, msg)

	-- if we loaded a projectile, fire it
	if msg.templateID == CONSTANTS["PROJECTILE_TEMPLATEID"] then 

	    -- store who the parent is
	    storeParent(self, msg.childID)
		local ballObj = msg.childID

		-- get the skill for the projectile		
		skill = self:GetVar("ImpactSkillID")

		if (ballObj) and (getActivityUser(self)) and (skill) then
		
			-- store values in the projectile
			ballObj:SetVar("My_Faction", getActivityUser(self):GetFaction().faction)

			-- set the skill
			ballObj:SetActiveProjectileSkill{ skillID = skill }

			-- store the velocity
			local vec = self:GetVar("initVelVec")
			
			-- set projectile params
			if (vec.x ~= 0 and vec.y ~= 0 and vec.z ~= 0) then
				ballObj:SetProjectileParams{initVel = vec, iProjectileType = 1, fLifeTime = 10.0, owner = self}
			end
			
		end
		
	end
	
end


--------------------------------------------------------------
-- Called when the client wants to fire
--------------------------------------------------------------
function onShootingGalleryFire(self, msg)

	-- calculate firing parameters
	local params = self:CalculateFiringParameters{targetPos = msg.targetPos, bUseHighArc = false}
	
	local vec = params.outVelVector
	if (vec.x ~= 0 and vec.y ~= 0 and vec.z ~= 0) then	
	
		-- save off the velocity
		self:SetVar("initVelVec",vec)

		-- spawn the projectile
		local spawnPos = params.outSpawnPos
		RESMGR:LoadObject {
			objectTemplate = CONSTANTS["PROJECTILE_TEMPLATEID"],
			x = spawnPos.x,
			y = spawnPos.y,
			z = spawnPos.z,
			rw = 1,
			owner = self
		}
		
		self:PlayFXEffect{effectType = "onfire"}
		self:PlayFXEffect{effectType = "onfire2"}
		getActivityUser(self):PlayFXEffect{effectType = "SG-fire"}
		
		-- @TODO: rename this to a better message?
		-- Tell the zone control we just fired
		GAMEOBJ:GetZoneControlID():ShootingGalleryFire()
		
	end
	
end


--------------------------------------------------------------
-- Called when getting the cannon's faction
--------------------------------------------------------------
function onGetFaction(self, msg)

	-- return the user's faction
	if (getActivityUser(self)) then
		msg.faction = getActivityUser(self):GetFaction().faction
		return msg
	end
	
end


--------------------------------------------------------------
-- Calculate the activity rating
--------------------------------------------------------------
function onDoCalculateActivityRating(self, msg)
    
    -- Send the request to the zone control
    local newMsg = GAMEOBJ:GetZoneControlID():DoCalculateActivityRating
    {
		fValue1 = msg.fValue1, 
		fValue2 = msg.fValue2,
		fValue3 = msg.fValue3,
		fValue4 = msg.fValue4,
		fValue5 = msg.fValue5,
	}

	-- return whatever the zone control gave us
	msg.outActivityRating = newMsg.outActivityRating
	return msg
	
end


--------------------------------------------------------------
-- Start or stop the activity
-- @TODO: old func, replaced with onRequestActivityExit, change out later
--------------------------------------------------------------
function onRequestActivityStartStop(self, msg)

    -- forward this request on to the zone control object
	GAMEOBJ:GetZoneControlID():RequestActivityStartStop{bStart = msg.bStart, userID = msg.userID}
	
end

--------------------------------------------------------------
-- User Exits Activity
--------------------------------------------------------------
function onRequestActivityExit(self, msg)

    -- forward this request on to the zone control object
	GAMEOBJ:GetZoneControlID():RequestActivityExit{bUserCancel = msg.bUserCancel, userID = msg.userID}
	
end
