require('o_mis')
CONSTANTS = {}
CONSTANTS["NO_OBJECT"] = "0"
function onStartup(self)

	--self:SetCollisionGroup{colGroup = 7  }
 	self:SetProximityRadius { radius = 5 }
end
 
 
function onProximityUpdate(self, msg)
	
	local target = msg.objId
	local template = target:GetLOT{}.objtemplate

	
	if (msg.status == "ENTER") and (template == 8445) and (self:GetRebuildState{}.iState == 2) then

		--------------------------------------
		--   Store Activity Object if nil   --
		--------------------------------------
		if not self:GetVar("bossObj") then

			storeObjectByName(self, "bossObj", target)

		end	
		
		local eChangeType = "PUSH"
		target:SetStunned{ StateChangeType = eChangeType,
		bCantMove = true, bCantTurn = true, bCantAttack = true, bCantEquip = true, bCantInteract = true }
		
		DoObjectAction(self, "effect", "cast")
		DoObjectAction(target, "anim", "spider-electrocute")
		
		GAMEOBJ:GetTimer():AddTimerWithCancel( 5 , "unStun", self )
	end   
        
       
end

function onTimerDone(self,msg)


	if msg.name == "unStun" then
	
		local target = getObjectByName(self,"bossObj")	
		local eChangeType = "POP"
		target:SetStunned{ StateChangeType = eChangeType,
		bCantMove = true, bCantTurn = true, bCantAttack = true, bCantEquip = true, bCantInteract = true }	
		self:RebuildReset()
	end
	
	




end

