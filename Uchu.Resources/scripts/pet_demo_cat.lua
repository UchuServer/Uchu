require('o_mis')
require('State')


 --###########################################################
 --###              ON START UP                            ###
 --###########################################################

function onStartup(self)
  
end

function getPlayer(self, num)
    targetID = self:GetVar("PlayerTarget_"..num )
    return GAMEOBJ:GetObjectByID(targetID)
end

function storePlayer(self, target, num)
    idString = target:GetID()
    finalID = "|" .. idString
    self:SetVar("PlayerTarget_"..num , finalID)
end


 --###########################################################
 --#### EMOTE RECEIVED
 --###########################################################
 
onEmoteReceived = function(self,msg)

	local name = msg.emoteID
	local caster = msg.caster
      
end
 --###########################################################
 --#### ON CLICKED
 --###########################################################

onUse = function (self, msg)

   local targetID = msg.user 				-- Target OBJ ID 
   local tpos = targetID:GetPosition().pos  -- Target Position
   storeTarget(self, targetID) 				-- Store Target
    self:SetVar("GameState", 1 ) 
   
   self:FaceTarget{ target = targetID, degreesOff = 5, keepFacingTarget = true } -- Face Target
   GAMEOBJ:GetTimer():CancelTimer( "MeanderPause", self ) -- Cancel Timer
   setState( "PetGame", self )
   
end 
 






