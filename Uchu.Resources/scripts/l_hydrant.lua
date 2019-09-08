
require('o_mis')


CONSTANTS = {}
CONSTANTS["STINKY_SKILL"] = 33
CONSTANTS["DESTINK_SKILL_ID"] = 116




function onStartup( self )
	
	self:AddSkill{ skillID = CONSTANTS["DESTINK_SKILL_ID"] }
	
	self:SetProximityRadius { radius = 10 }
	
	self:SetVar( "broken", false )
		
end



function CleanNearbyPlayers( self )

	--print( "HYDRANT CleanNearbyPlayers" )

	-- look for stinky players near the hyrdrant and destink them
	local objs = self:GetProximityObjects().objects
	
	local index = 1
	while index <= table.getn(objs)  do
		local target = objs[index]
		local faction = target:GetFaction()
		
		if ( faction and faction.faction == 1 ) then
		
			--print( "hydrant found player" )
			
			if ( target:IsSkillActive{ iSkillID = CONSTANTS["STINKY_SKILL"] }.bOn ) then
				
				--print( "hydrant destinks player" )
				self:CastSkill{ optionalTargetID = target, skillID = CONSTANTS["DESTINK_SKILL_ID"] }
				
			end
		end
		
		index = index + 1
	end
end



function onFireEventServerSide( self, msg )

	if ( msg.args == "cleanPlayers" ) then
		--print( "-------------HYDRANT: onFireEventServerSide cleanPlayers" )
		CleanNearbyPlayers( self )
		
	elseif ( msg.args == "breakHydrant" ) then
		--print( "-------------HYDRANT: onFireEventServerSide breakHydrant" )
		self:SetVar( "broken", true )
	
	elseif ( msg.args == "repairHydrant" ) then
		--print( "-------------HYDRANT: onFireEventServerSide repairHydrant" )
		self:SetVar( "broken", false )
		
	end
end



function onProximityUpdate( self, msg )

    if ( msg.status == "ENTER" and self:GetVar("broken") == true ) then
    
		--print( "-------------HYDRANT: onProximityUpdate" )

		if ( msg.objId:IsSkillActive{ iSkillID = CONSTANTS["STINKY_SKILL"] }.bOn ) then
			
			--print( "hydrant destinks player" )
			self:CastSkill{ optionalTargetID = msg.objId, skillID = CONSTANTS["DESTINK_SKILL_ID"] }
			
		end
    end

end
