require('o_mis')

CONSTANTS = {}
CONSTANTS["radius"] = 4
CONSTANTS["switchFrequency"] = 1	-- once the switch is activated, how often to check and see if it's still activated


function onStartup(self)
	self:SetProximityRadius { radius = CONSTANTS["radius"] }

end



onTimerDone = function(self, msg)

    if (msg.name == "checkSwitchTimer") then
    
		--print ( "------------------------------------" )
		--print ( " hazmat bouncer onTimerDone: checkSwitchTimer" )
		--print ( "------------------------------------" ) 

		if ( IsSwitchActivated( self ) ) then
			BouncePlayers( self )
			GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["switchFrequency"], "checkSwitchTimer", self )		
		end
	end
end


	
function onFireEvent(self, msg)

	--print ( "------------------------------------" )
	--print ( "hazmat bouncer onFireEvent" )
	--print ( "------------------------------------" )
	
	if ( msg.args == "switchPressed" ) then
	
		--print ( "------------------------------------" )
		--print ( "hazmat bouncer switchPressed" )
		--print ( "------------------------------------" )
		
		-- remember the objID of the trigger (switch) so we can keep checking whether there's a player in its proximity
		-- that way you don't have to back off and touch it again in order to bounce somebody else
		storeObjectByName( self, "switchID", msg.senderID )
		
		BouncePlayers( self )
		
		GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["switchFrequency"], "checkSwitchTimer", self )

	end		-- switchPressed
end



function BouncePlayers( self )

	local objs = self:GetProximityObjects().objects
	local index = 1

	while index <= table.getn(objs)  do
		local target = objs[index]
		local faction = target:GetFaction()
		
		--verify that we are only bouncing players
		if faction and faction.faction == 1 then
		
			--print ( "------------------------------------------------------------------" )
			--print ( "hazmat bouncer BouncePlayers: found a player on the bouncepad" )
			--print ( "------------------------------------------------------------------" )
			
			self:BouncerTriggered{triggerObj = target}
		end
		index = index + 1
	end

end



function IsSwitchActivated( self )

	triggerID = getObjectByName( self, "switchID" )
	
	if ( triggerID == nil ) then
		return false
	end
	

	-- check if there are any players near the switch
	local objs = triggerID:GetProximityObjects().objects
	
	local index = 1
	while index <= table.getn(objs)  do
		local target = objs[index]
		local faction = target:GetFaction()
		
		if ( faction and faction.faction == 1 ) then
		
			--print ( "------------------------------------------------------------------" )
			--print ( "hazmat bouncer IsSwitchActivated: found a player on the switch" )
			--print ( "------------------------------------------------------------------" )
		
			return true
		end
		
		index = index + 1
	end

	--print ( "------------------------------------------------------------------" )
	--print ( "hazmat bouncer IsSwitchActivated: nobody on the switch" )
	--print ( "------------------------------------------------------------------" )
				
	return false
	
end

