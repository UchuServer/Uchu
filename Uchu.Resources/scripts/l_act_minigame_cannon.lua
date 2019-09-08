--L_ACT_CANNON.lua
-- Server Side

require('o_mis')

-- template for the projectile (water effect cannon ball)
local ballTemplateID = 2357

-- offset to place the player during the activity
local playerOffset = {x = 4, y = 0, z = -4}

-- velocity of the projectile
-- max distance = (v^2 / g) where v = velocity and g = the effect of gravity from the database.
-- original value = 100.  changed to 140
local velocity = 129.0

-- Minimum distance that the cannon can hit
local minDist = 20.0

-- cooldown time for firing
local refireRate = 800.0

-- muzzle offset
local barrelOffset = {x = 0, y = 4.3, z = 9}

-- cannon time out for activity
local cannonTimeOut = -1.0

function onStartup(self)
    -- default the instance vars
	vec = {x = 0, y = 0, z = 0}
	self:SetVar("initVelVec",vec)
	
	-- set the parameters of the shooting gallery
	self:SetShootingGalleryParams{playerPosOffset = playerOffset, projectileVelocity = velocity, cooldown = refireRate, muzzlePosOffset = barrelOffset, minDistance = minDist, timeLimit = cannonTimeOut}
end

function onChildLoaded(self, msg)
	-- if we loaded a projectile, fire it
	if msg.templateID == ballTemplateID then 
	    -- store who the parent is
	    storeParent(self, msg.childID)
		local ballObj = msg.childID
		if (ballObj) then
			ballObj:SetFaction{ faction = self:GetFaction().faction }

			local vec = self:GetVar("initVelVec")
			if (vec.x ~= 0 and vec.y ~= 0 and vec.z ~= 0) then
				ballObj:SetProjectileParams{initVel = vec, iProjectileType = 1, fLifeTime = 10.0, owner = self}
			end
		end
	end
end


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
			objectTemplate = ballTemplateID,
			x = spawnPos.x,
			y = spawnPos.y,
			z = spawnPos.z,
			rw = 1,
			owner = self
		}
		
		self:PlayFXEffect{effectType = "onfire"}
	end
end
