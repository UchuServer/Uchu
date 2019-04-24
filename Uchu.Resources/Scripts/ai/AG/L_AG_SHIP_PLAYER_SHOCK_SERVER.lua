------------------------------------------------------------
--plays the shock animation on the player when they interact with the broken console on the space ship
------------------------------------------------------------


local ShockAnim = "knockback-recovery"
local FXTime = 2.0


function onUse(self, msg)

   --print("clicked!")
   msg.user:TerminateInteraction{type = 'fromInteraction', ObjIDTerminator = self}
   
   if self:GetVar("bActive") then
      
      return
      
   end
   
   local player = msg.user
   
   self:SetVar("bActive", true) 
   
   player:PlayAnimation{animationID = ShockAnim, bPlayImmediate = true}
   player:Knockback{vector={x=-20, y=10, z=-20}}
   
   self:PlayFXEffect{name = "console_sparks", effectType = "create", effectID = 1430}
   
   GAMEOBJ:GetTimer():AddTimerWithCancel(FXTime, "FXTime", self )
end

function onTimerDone(self, msg)

   self:StopFXEffect{name = "console_sparks"}
   self:SetVar("bActive", false)
end