local skillid = 43
local ProxRadius = 4
local FIRE_COOLDOWN = 2

function onStartup(self)
self:SetVar("counter", 0)
self:SetProximityRadius{radius = ProxRadius}
isBurning = true
self:PlayFXEffect{ name  = "Burn", effectID = 295, effectType = "running"}
end
function onProximityUpdate(self, msg)
    if msg.status == "ENTER" then
        local target = msg.objId
        if target:BelongsToFaction{factionID = 1}.bIsInFaction then        
            local counter = self:GetVar("counter")
            counter = counter + 1
            self:SetVar("counter", counter)
			
				
			
            if counter == 1 then
            	self:CastSkill{skillID = skillid }
				msg.objId:UpdateMissionTask{taskType = "complete", value = 440, value2 = 1, target = self}
            	GAMEOBJ:GetTimer():AddTimerWithCancel(FIRE_COOLDOWN, "TimeBetweenCast", self )
            	print "Set the timer" 
            end  -- end if counter == 1           
        end -- end if faction = 1
    else 
        local counter = self:GetVar("counter")
        if counter > 0 then
            counter = counter - 1
            self:SetVar("counter", counter)
            if counter == 0 then
            	-- cancelling the timer
            	GAMEOBJ:GetTimer():CancelAllTimers( self )
            end
        end 
    end -- end if msg.status == "ENTER"
end 

function onSquirtWithWatergun( self, msg )
    if isBurning then 
        self:StopFXEffect{ name = "Burn" }
        --obj:PlayFXEffect{ name  = "Off", effectID = 295, effectType = "end"} -- could be a transitional effect
        self:PlayFXEffect{ name  = "Off", effectID = 295, effectType = "idle"} 
        GAMEOBJ:GetTimer():AddTimerWithCancel( 28, "FireRestart", self )
        isBurning = false 
        player = msg.shooterID
    end
end
-- turns fire on/off based on burn variable, needs: obj = LWOOBJID, burn = bool


function onTimerDone(self, msg)
	if (msg.name == "TimeBetweenCast") then
		GAMEOBJ:GetTimer():AddTimerWithCancel(FIRE_COOLDOWN, "TimeBetweenCast", self )
		self:CastSkill{skillID = skillid}
	end
	
	if msg.name == "FireRestart" then
        if not isBurning then 
            isBurning = true
			self:StopFXEffect{ name = "Off" }
			self:PlayFXEffect{ name  = "Burn", effectID = 295, effectType = "running"}
        end
    end
end            