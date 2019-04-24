--------------------------------------------------------------
-- Client-side death trigger that will spawn an object while 
-- the player is dying. Change the Custom Variables to fit 
-- your needs.
-- mrb... 5/21/09
-- djames: updated 9/28/09
--------------------------------------------------------------
-- Custom Variables
--------------------------------------------------------------
local deathAnimation = "big-shark-death"            -- Animation to play on the player when it dies
local deathObject = 6268                        -- Object to spawn when the player dies
local deathObject2 = 8570                        -- Object to spawn when the player dies
local deathObjectOffset = {x=0,y=0,z=0}         -- Offset to apply when spawning the deathObject
local deathObjectAnimation = "idle"             -- name of the  animation playing on the deathObject

--------------------------------------------------------------
-- onCollisionPhantom handles the player colliding with the
-- attached object (i.e. water plane) by making them die
--------------------------------------------------------------
function onCollisionPhantom(self, msg)
    
	local target = msg.objectID
	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	
	-- If a player collided with me, then do our stuff
	if player:GetID() == msg.objectID:GetID() then
	
		-- stun the player so they can't wander around on the surface of the water
		player:SetStunned{StateChangeType = "PUSH", bCantMove = true, bIgnoreImmunity = true}
		self:FireEventServerSide{ senderID = player, args = 'achieve' } --send a message to the server side script to update the shark death achievement
		--player:RequestDie{killerID = self, deathType = deathAnimation}
		
	end
	
end

function onOffCollisionPhantom(self, msg)    
	local target = msg.objectID
	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	
	-- If a player collided with me, then do our stuff
	if player:GetID() == msg.objectID:GetID() then	
		-- stun the player so they can't wander around on the surface of the water
		player:SetStunned{StateChangeType = "POP", bCantMove = true, bIgnoreImmunity = true}
	end	
end

function loadDeathObject(self, target, pos, rot)

	if not pos or not rot and target then	
	
		pos = target:GetPosition().pos
		rot = target:GetRotation()
		
	end
	
	-- snap the player to the position where they really died
	target:SetPosition{ pos = pos }

	--print("******* Spawn Death Object *******")
	local config = { {"no_timed_spawn", true}, {"groupID", "DeathObject2"} }
	--RESMGR:LoadObject{ objectTemplate = deathObject, x = pos.x + deathObjectOffset.x, y = pos.y + deathObjectOffset.y, z = pos.z + deathObjectOffset.z, rw = rot.w, rx = rot.x, ry = rot.y, rz = rot.z, owner = self, configData = config}
    
	--if deathObject2 then
	
		RESMGR:LoadObject{ objectTemplate = deathObject2, x = pos.x + deathObjectOffset.x, y = pos.y + deathObjectOffset.y, z = pos.z + deathObjectOffset.z, rw = rot.w, rx = rot.x, ry = rot.y, rz = rot.z, owner = self, configData = config}
	
	--end
	
	target:SetStunned{StateChangeType = "POP", bCantMove = true, bIgnoreImmunity = true}
    
end
--------------------------------------------------------------
-- onKilledPlayer handles the player dying after receiving the
-- message from the server by spawning in the deathObject.
--------------------------------------------------------------
function onKilledPlayer(self, msg)
	
	loadDeathObject(self,msg.playerID,msg.deathPos,msg.deathRot) 
	
end

--------------------------------------------------------------
-- onChildLoaded starts a timer to remove the deathObject 
-- based on its deathObjectAnimation time.
--------------------------------------------------------------
function onChildLoaded( self, msg )
	
	--print("******* onChildLoaded (client)")
	if msg.childID:GetLOT().objtemplate == deathObject2 then
	
		local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
		local animTime = msg.childID:GetAnimationTime{animationID = deathObjectAnimation}  
		
		--player:RequestDie{killerID = self, deathType = deathAnimation}
		GAMEOBJ:GetTimer():AddTimerWithCancel(animTime.time, "RemoveDeathObject", self )
		
	end
	
end

--------------------------------------------------------------
-- timers...
--------------------------------------------------------------
function onTimerDone(self, msg)

	if msg.name == "RemoveDeathObject" then
	
	    --print('******* RemoveDeathObject (client)')
	    local deathObj = self:GetObjectsInGroup{ group = "DeathObject2" }.objects  
		
	    for k,v in ipairs(deathObj) do    
	        GAMEOBJ:DeleteObject(v)
	    end
	    
        self:SetVar('bActive', true)
		
	end  
	
end
