require('o_mis')
function onRenderComponentReady(self) 

		
		self:SetNameBillboardState{bState = false }
		


end


function onScriptNetworkVarUpdate(self,msg)
	

	if msg.tableOfVars.name == "SetHealth" then
		local  health = self:GetHealth().health 
		local  armor  = self:GetArmor().armor
		
	 	UI:SendMessage("SetBossVars", {{"health", tostring(health)} ,{"healthmax", "100"}} )
	 	UI:SendMessage("SetBossVars", {{"armor", tostring(armor)}  ,{"armormax", "100"}} )
	
	end
	
	
	if msg.tableOfVars.name == "intGUI" then

		---- Temp button
		GAMEOBJ:GetTimer():AddTimerWithCancel( 4 , "int", self )
		
	end


	if msg.tableOfVars.name == "removeArmor" then
	
		UI:SendMessage("SetBossVars", {{"armor", "none"}  ,{"armormax", "100"}} )
		self:ResetSecondaryAnimation()
		self:ResetPrimaryAnimation()
	end
	
	if msg.tableOfVars.name == "resetArmore" then
	
		UI:SendMessage("SetBossVars", {{"armor", "40"}  ,{"armormax", "100"}} )
		self:ResetSecondaryAnimation()
		self:ResetPrimaryAnimation()
	end
	
	
	if msg.tableOfVars.name == "resetAnim" then
	
		
	end
	
end
               
               
               
function onTimerDone(self,msg)


	if msg.name == "int" then
	
			local  health = self:GetHealth().health 
			local  armor  = self:GetArmor().armor
			
			UI:SendMessage("SetBossVars", {{"health", tostring(health)} ,{"healthmax", "100"}} )
			UI:SendMessage("SetBossVars", {{"armor", tostring(armor)}  ,{"armormax", "100"}} )
		    UI:SendMessage("SetBossVars", {{"bossname", self:GetName().name}} )
		
	end

end