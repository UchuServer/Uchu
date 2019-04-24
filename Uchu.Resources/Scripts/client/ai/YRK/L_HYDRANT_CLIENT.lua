require('o_mis')
require('client/ai/L_BOUNCER_BASIC')


CONSTANTS = {}
CONSTANTS["NO_OBJECT"] = "0"


CONSTANTS["BOUNCER_LOT"] = 3736
CONSTANTS["HF_NODE_BOUNCER"] = 7
CONSTANTS["HF_SUB_ITEM_SEP_STRING"] = "\x1F"

CONSTANTS["SPOUT_BOUNCER_SPEED"] = 700.0
CONSTANTS["SPOUT_BOUNCER_DEST"] = {x = 150.2, y = 316.47, z = -253.5}



function onStartup( self )

	--print( "HYDRANT: onStartup" )
	
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

	-- get the hydrant's position
	local hydrantPos = self:GetPosition{}.pos
	
	-- we'll spawn the bouncer at the same position
	local bouncerPos = self:GetPosition{}.pos
	bouncerPos.x = hydrantPos.x
	bouncerPos.y = hydrantPos.y
	bouncerPos.z = hydrantPos.z
	
	
    -- default bouncer information
    local landingPos = CONSTANTS["SPOUT_BOUNCER_DEST"]
    local bounceSpeed = CONSTANTS["SPOUT_BOUNCER_SPEED"]
    
	-- set up bouncer config data
	local landingString = landingPos.x .. CONSTANTS["HF_SUB_ITEM_SEP_STRING"] ..landingPos.y .. CONSTANTS["HF_SUB_ITEM_SEP_STRING"] .. landingPos.z
	local config = { {"bouncer_speed", bounceSpeed} , {"objtype", CONSTANTS["HF_NODE_BOUNCER"]}, {"bouncer_destination", landingString } }
	
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

