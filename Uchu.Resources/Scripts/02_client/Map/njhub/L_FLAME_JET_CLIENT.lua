function onRenderComponentReady(self,msg)
	self:PlayFXEffect{ name = "jetOn" , effectType = "jetOn" }
end

function onScriptNetworkVarUpdate(self,msg)
	-- parse through the table of network vars that were updated
	for k,v in pairs(msg.tableOfVars) do
		-- not not active, set notactive to false (its active)
		if k == "FlameOn" and not v then
			self:StopFXEffect{name = "jetOn"}
		elseif k == "FlameOn" and v then
			self:PlayFXEffect{ name = "jetOn" , effectType = "jetOn" }
		end
	end
end