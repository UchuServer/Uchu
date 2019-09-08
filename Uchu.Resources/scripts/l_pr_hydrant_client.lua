require('o_mis')
require('L_BOUNCER_BASIC')


CONSTANTS = {}
CONSTANTS["NO_OBJECT"] = "0"


CONSTANTS["BOUNCER_LOT"] = 3736
CONSTANTS["HF_NODE_BOUNCER"] = 7
CONSTANTS["HF_SUB_ITEM_SEP_STRING"] = "\x1F"



function onStartup( self )

	--print( "PET HYDRANT: onStartup" )
	
	--self:AddSkill{ skillID = CONSTANTS["DESTINK_SKILL_ID"] }
    
    self:SetVar( "waterEffect", "" )
    
    self:SetProximityRadius { radius = 10 }
    
    self:SetVar( "broken", false )
    
end



function onGetOverridePickType(self, msg)

	msg.ePickType = 14	--interactive
	return msg

end




function BreakHydrant( self )

	--print( "HYDRANT: break_hydrant" )
	
	self:FireEventServerSide{ senderID = self, args = "breakHydrant" }
		
	AddWaterEffect( self )
	SpawnBouncer( self )
end





function RepairHydrant( self )

	--print( "HYDRANT: repair_hydrant" )
	
	self:FireEventServerSide{ senderID = self, args = "repairHydrant" }
		
	RemoveBouncer( self )
	CancelWaterEffect( self )
end



function AddWaterEffect( self )

	-- if this hydrant already has the water effect, don't add another one
	if ( self:GetVar( "waterEffect" ) == true ) then
		return
	end

	self:PlayFXEffect{ name = "water", effectID = 384, effectType = "water" }
	self:SetVar( "waterEffect", true )

end




function CancelWaterEffect( self )

	local myEffect = self:GetVar("waterEffect")
		
	if ( myEffect ) then
		self:StopFXEffect{ name = "water" }
	end
	
	self:SetVar( "waterEffect", false )

end



function SpawnBouncer( self )
    --print ("start of spawnBouncer")
	-- get the hydrant's position
	local hydrantPos = self:GetPosition{}.pos
	
	-- determine where to spawn the bouncer
	local bouncerPos = self:GetPosition{}.pos
	bouncerPos.x = hydrantPos.x
	bouncerPos.y = hydrantPos.y
	bouncerPos.z = hydrantPos.z
	
	-- determine where to make the player land
	local landingPos = self:GetPosition{}.pos
	landingPos.x = bouncerPos.x + 200
	landingPos.y = bouncerPos.y
	landingPos.z = bouncerPos.z + 500
    --print ("PetBouncer")    
    --landingPos.x = 28.55
	--landingPos.y = 197.66
	--landingPos.z = -142.74
	
	-- set up bouncer config data
	local landingString = landingPos.x .. CONSTANTS["HF_SUB_ITEM_SEP_STRING"] .. landingPos.y .. CONSTANTS["HF_SUB_ITEM_SEP_STRING"] .. landingPos.z
	local config = { {"bouncer_speed", 500.0} , {"objtype", CONSTANTS["HF_NODE_BOUNCER"]}, {"bouncer_destination", landingString } } --bouncer speed was 100. Changing this somehow caused it to go straight up.
	
	RESMGR:LoadObject { objectTemplate = CONSTANTS["BOUNCER_LOT"],
						x = bouncerPos.x, y = bouncerPos.y, z = bouncerPos.z,
						owner = self,
						objType = CONSTANTS["HF_NODE_BOUNCER"],
						configData = config }

end




function onChildLoaded( self,msg )

	--print( "HYRDRANT: onChildLoaded" )
    
    if ( msg.childID:GetLOT().objtemplate == CONSTANTS["BOUNCER_LOT"] ) then
		--print( "HYDRANT: bouncer loaded" )
		storeObjectByName( self, "bouncer", msg.childID )
	end

end




function RemoveBouncer( self )

	--print( "HYDRANT: RemoveBouncer" )

	local bouncerObj = getObjectByName( self, "bouncer" )
	if( bouncerObj ~= nil ) then
		--print( "HYRDRANT: bouncer Die" )
		GAMEOBJ:DeleteObject( bouncerObj )
	end
	
end





function onBouncerTriggered( self, msg )

	--CleanNearbyPlayers( self )

	--Hackish fix for the 'bounce collision'
	
	local player = msg.triggerObj
	local objPos = player:GetPosition().pos
	objPos.y = objPos.y + 1
	player:SetPosition{pos = objPos}
	
	bounceObj(self, msg.triggerObj)
end




function onClientUse( self, msg )

	--print( "HYDRANT: onClientUse" )
	
	if ( self:GetVar( "broken" ) == false ) then	-- break the hydrant
		self:SetVar( "broken", true )
	
		BreakHydrant( self ) 
	
		CleanNearbyPlayers( self )
	
	else											-- it was already broken, so repair it
		self:SetVar( "broken", false )
	
		RepairHydrant( self ) 
	end
	
end





function CleanNearbyPlayers( self )
	
	--print( "client-sde CleanNearbyPlayers" )
	
	self:FireEventServerSide{ senderID = self, args = "cleanPlayers" }
end








