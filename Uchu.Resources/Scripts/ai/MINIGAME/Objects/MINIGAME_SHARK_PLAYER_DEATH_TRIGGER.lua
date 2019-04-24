--------------------------------------------------------------
-- Death trigger that will spawn and object while the player is
-- dying. Change the Custom Variables to fit your needs.
-- mrb... 5/21/09
--------------------------------------------------------------
-- Custom Variables
--------------------------------------------------------------
local deathAnimation = "shark-death"            -- Animation to play on the player when it dies
local deathObject = 6268                        -- Object to spawn when the player dies
local deathObjectOffset = {x=0,y=0,z=0}         -- Offset to apply when spawning the deathObject
local deathObjectAnimation = "idle"             -- name of the  animation playing on the deathObject

--------------------------------------------------------------
-- onCollision
--------------------------------------------------------------
function onCollisionPhantom(self, msg)
    --print("collision!")
	local target = msg.objectID
	
	-- If a player collided with me, then do our stuff
	if target:IsDead().bDead == false and target:IsCharacter().isChar then		
		SpawnDeathObject(self, target)
    end

	return msg
end

--------------------------------------------------------------
-- SpawnDeathObject kills the player using the deathAnimation,
-- and spawns in the deathObject.
--------------------------------------------------------------
function SpawnDeathObject(self, target)	
    --print("******* Kill Player *******")
    target:Die{killerID = self, deathType = deathAnimation}
    
    -- get the position and rotation of the player
    local oPos = {pos = target:GetPosition().pos}
    oPos.rot = target:GetRotation()
    
    --print("******* Spawn Death Object *******")
    local config = { {"no_timed_spawn", true}, {"groupID", "DeathObject"} }
    RESMGR:LoadObject{ objectTemplate = deathObject, x= oPos.pos.x + deathObjectOffset.x, y= oPos.pos.y + deathObjectOffset.y, z= oPos.pos.z + deathObjectOffset.z, rw = oPos.rot.w, rx = oPos.rot.x, ry = oPos.rot.y, rz = oPos.rot.z, owner = self, configData = config}
end

--------------------------------------------------------------
-- onChildLoaded start a timer to remove the deathObject based
-- on it's deathObjectAnimation time.
--------------------------------------------------------------
function onChildLoaded( self, msg )    
    local animTime = msg.childID:GetAnimationTime{animationID = deathObjectAnimation}  
    GAMEOBJ:GetTimer():AddTimerWithCancel(animTime.time, "RemoveDeathObject", self )
end

--------------------------------------------------------------
-- timers...
--------------------------------------------------------------
function onTimerDone(self, msg)
    if msg.name == "RemoveDeathObject" then
        --print('******* Remove the Death Object *******')        
        local deathObj = self:GetObjectsInGroup{ group = "DeathObject" }.objects  
        for k,v in ipairs(deathObj) do    
            GAMEOBJ:DeleteObject(v)
        end
    end    
end














