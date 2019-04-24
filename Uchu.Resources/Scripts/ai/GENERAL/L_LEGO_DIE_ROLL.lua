--require('o_mis')

function onStartup(self)
    GAMEOBJ:GetTimer():AddTimerWithCancel( 10, "DoneRolling", self )
    local player = self:GetParentObj().objIDParent   
    local animationMsg = player:GetAnimationTime{ animationID = "dice-roll" } 
    GAMEOBJ:GetTimer():AddTimerWithCancel( animationMsg.time, "ThrowDice", self )
end

function onTimerDone(self, msg)
	if msg.name == "DoneRolling" then
         self:RequestDie{killerID = self, killType = "SILENT"}
    elseif msg.name == "ThrowDice" then
    
         -- Choose a random number from 1 - 6.
        local dieRoll = math.random(1,6)
        local Anim= { "Die-Roll-1", "Die-Roll-2", "Die-Roll-3", "Die-Roll-4", "Die-Roll-5", "Die-Roll-6" }
        -- put any world messages concerning the die roll here

        self:PlayFXEffect {effectType = Anim[dieRoll], priority = 1.3, name = "diceroll" }
        
        local player = self:GetParentObj().objIDParent
        if dieRoll == 6 then
            player:UpdateMissionTask{taskType = "complete", value = 756, value2 = 1, target = self}
        end
    end
end

 