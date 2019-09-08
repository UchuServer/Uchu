require('o_mis')

function onStartup(self)
	
	self:GetVar("trigger_build", 0 )
	
end




function onGetOverridePickType(self, msg)
    msg.ePickType = 14
	return msg
end




function onScriptNetworkVarUpdate(self,msg)

	if msg.tableOfVars.name == "lockPlayer" then
		
		local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
		
	
		local eChangeType = "PUSH"
		player:SetStunned{ StateChangeType = eChangeType,
		bCantMove = true, bCantTurn = true, bCantAttack = true, bCantEquip = true, bCantInteract = true }	

	elseif msg.tableOfVars.name == "Rebuild" then
	
		
		if not self:GetVar("rebuildP") then

				local rebuildP = self:GetObjectsInGroup{ group = "rebuildP" ,ignoreSpawners = true }.objects
				storeObjectByName(self, "rebuildP", rebuildP[1])

		end
	
		local rebuildP = getObjectByName(self, "rebuildP")
		
		
		
		rebuildP:SpawnModelBricks{amount=0.5 ,pos= rebuildP:GetPosition().pos}
	
	
	

					
	end
	

end
