--------------------------------------------------------------
-- Client-side death trigger that will spawn an object while 
-- the player is dying. Change the Custom Variables to fit 
-- your needs.
-- mrb... 5/21/09
-- djames: updated 9/28/09
--------------------------------------------------------------
-- Custom Variables
--------------------------------------------------------------
local deathAnimation = "electro-shock-death"    -- Animation to play on the player when it dies
local deathObject = 6315                        -- Object to spawn when the player dies
local deathObjectOffset = {x=0,y=0,z=0}         -- Offset to apply when spawning the deathObject
local deathObjectAnimation = "idle"             -- name of the  animation playing on the deathObject

--------------------------------------------------------------
-- onCollisionPhantom handles the player colliding with the
-- attached object (i.e. water plane) by making them die
--------------------------------------------------------------
function onCollisionPhantom(self, msg)
    --print("******* onCollisionPhantom (client)")
    if not self:GetVar('bActive') then return end
    
    self:SetVar('bActive', false)
	local target = msg.objectID
	
	-- If a player collided with me, then do our stuff
	if target:BelongsToFaction{factionID = 1}.bIsInFaction then
		--print(self:GetLOT().objtemplate)
		--print(msg.objectID:GetLOT().objtemplate)
		
		-- stun the player so they can't wander around on the surface of the water
		target:SetStunned{StateChangeType = "PUSH", bCantMove = true, bIgnoreImmunity = true}
		
		--print("******* RequestDie (client)")
		target:RequestDie{killerID = self, deathType = deathAnimation}
    end

	return msg
end

--------------------------------------------------------------
-- onKilledPlayer handles the player dying after receiving the
-- message from the server by spawning in the deathObject.
--------------------------------------------------------------
function onKilledPlayer(self, msg)
	--print("******* onKilledPlayer (client)")
	
	local target = msg.playerID
	local pos = msg.deathPos
	local rot = msg.deathRot
	
	-- snap the player to the position where they really died
	target:SetPosition{ pos = pos }

	--print("******* Spawn Death Object *******")
	local config = { {"no_timed_spawn", true}, {"groupID", "DeathObject"} }
	RESMGR:LoadObject{ objectTemplate = deathObject, x = pos.x + deathObjectOffset.x, y = pos.y + deathObjectOffset.y, z = pos.z + deathObjectOffset.z, rw = rot.w, rx = rot.x, ry = rot.y, rz = rot.z, owner = self, configData = config}
	
    target:SetStunned{StateChangeType = "POP", bCantMove = true, bIgnoreImmunity = true}
    
	return msg
end

--------------------------------------------------------------
-- onChildLoaded starts a timer to remove the deathObject 
-- based on its deathObjectAnimation time.
--------------------------------------------------------------
function onChildLoaded( self, msg )
	--print("******* onChildLoaded (client)")
	local animTime = msg.childID:GetAnimationTime{animationID = deathObjectAnimation}  
	GAMEOBJ:GetTimer():AddTimerWithCancel(animTime.time, "RemoveDeathObject", self )
end

--------------------------------------------------------------
-- timers...
--------------------------------------------------------------
function onTimerDone(self, msg)
	if msg.name == "RemoveDeathObject" then
	    --print('******* RemoveDeathObject (client)')
	    local deathObj = self:GetObjectsInGroup{ group = "DeathObject" }.objects  
	    for k,v in ipairs(deathObj) do    
	        GAMEOBJ:DeleteObject(v)
	    end
        self:SetVar('bActive', true)
	end    
end
