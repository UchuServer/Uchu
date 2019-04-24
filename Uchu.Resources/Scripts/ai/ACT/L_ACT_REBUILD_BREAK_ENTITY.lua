require('o_mis')
--///////////////////////////////////////////////////////////////////////////////////////
--//            Generic Rebuild -- Script
--//   - The spawned entity that will be breaking a rebuild
--///////////////////////////////////////////////////////////////////////////////////////

local anim_attack_delay = 0.2
local attackDistance = -4.0
local delay_time = 1.5
    
function onStartup(self) 
    -- delay until the animation has a chance to play on the client
	local anim_time = self:GetAnimationTime{  animationID = "rebuild-enter" }.time
    GAMEOBJ:GetTimer():AddTimerWithCancel( anim_time + delay_time, "TimerMoveToTarget",self )  
end

function onArrived(self, msg)

   -- for some reason there needs to be a delay when an NPC arrives at a waypoint
   -- otherwise some actions and animations will get overriden or will not play
   GAMEOBJ:GetTimer():AddTimerWithCancel( delay_time, "ArrivedDelayTimer",self )  
   
end

onTimerDone = function(self, msg)
    
    -- This timer starts the NPC moving to the target
    if msg.name == "TimerMoveToTarget" then 
		-- set our destination
		local heading = getHeading(getParent(self))
		
		heading.x = heading.x * attackDistance
		heading.y = heading.y * attackDistance
		heading.z = heading.z * attackDistance
				
		local dest = getParent(self):GetPosition().pos 
		dest.x = dest.x + heading.x
		dest.y = dest.y + heading.y
		dest.z = dest.z + heading.z
		
		-- goto our destination
		self:GoTo{speed = 1.5, target = dest}
    end   
        
    -- This timer starts after the NPC has hit the waypoint
    if msg.name == "ArrivedDelayTimer" then 
       local anim_time = self:GetAnimationTime{  animationID = "rebuild-attack" }.time
       self:PlayFXEffect{effectType = "rebuild-attack"}
       getParent(self):PlayFXEffect{effectType = "rebuild-complete-2"}

	   -- start a timer for the rebuild reset
	   GAMEOBJ:GetTimer():AddTimerWithCancel( anim_time, "TimerRebuildReset",self )   
	   
	   -- start a timer for exit
	   GAMEOBJ:GetTimer():AddTimerWithCancel( anim_time + anim_attack_delay, "TimerEntityExit",self )   

    end    

    if msg.name == "TimerEntityExit" then 
	   local anim_time = self:GetAnimationTime{  animationID = "rebuild-exit" }.time
       self:PlayFXEffect{effectType = "rebuild-exit"}
	   GAMEOBJ:GetTimer():AddTimerWithCancel( anim_time + anim_attack_delay, "TimerDeleteObject",self )   
    end    

    if msg.name == "TimerDeleteObject" then 
        GAMEOBJ:DeleteObject(self)
    end    

    if msg.name == "TimerRebuildReset" then
		getParent(self):RebuildReset()
		--self:PlayAnimation{animationID = "rebuild-complete"}
    end    
end    