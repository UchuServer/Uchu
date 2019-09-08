
--------------------------------------------------------------
-- server side script
--------------------------------------------------------------


--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('L_BOUNCER_BASIC')
require('c_Zorillo')




--------------------------------------------------------------
-- Constants
--------------------------------------------------------------

CONSTANTS["BABY_SKUNK_LOT"] = 3476

CONSTANTS["STINKY_SKUNK_SKILL"] = 109

--CONSTANTS["BOUNCER_EFFECT_NUM"] = 337					-- don't need to specify this because we're using PlayFXEffect
CONSTANTS["BOUNCER_EFFECT_NAME"] = "skunkBouncer"
--CONSTANTS["TARGET_EFFECT_NUM"] = 338					-- don't need to specify this because we're using PlayFXEffect
CONSTANTS["BABY_EFFECT_NAME"] = "skunkBouncerBaby"
CONSTANTS["ADULT_EFFECT_NAME"] = "skunkBouncerAdult"






--------------------------------------------------------------
-- check whatever just collided with the bouncer for stink
--------------------------------------------------------------
function CheckForStink( self, target )

	--print( "SKUNK BOUNCER: CheckForStink" )

	-- if this is a a baby skunk, or a stinky adult skunk, then add a stink trail effect
	local bAddStinkTrail = false

	-- check if it's a baby skunk
	if ( IsTargetABabySkunk( target ) ) then
		--print( "SKUNK BOUNCER: CheckForStink found a baby skunk" )
		bAddStinkTrail = true
		
	-- check if it's a stinky adult skunk
	elseif ( IsTargetAnAdultSkunk( target ) and target:HasSkill{ iSkillID = CONSTANTS["STINKY_SKUNK_SKILL"] }.bHas ) then
		--print( "SKUNK BOUNCER: CheckForStink found a stinky adult skunk" )
		bAddStinkTrail = true

	end


	-- if we should add the stink trail effect to the bouncer and whoever collided with it, do so
	-- Note: we add the effects server-side.
		-- They time out so we don't have to worry about how to get rid of them from the server.
	if ( bAddStinkTrail == true ) then
		
		AddStinkEffectToBouncer( self )
		AddStinkEffectToTarget( target )
	end
	
end





-----------------------------------------------------------------
-- adds a stink effect to the bouncer
-----------------------------------------------------------------
function AddStinkEffectToBouncer( self )
	
	--print( "SKUNK BOUNCER: AddStinkEffectToBouncer" )
	
	self:PlayFXEffect{ effectType = CONSTANTS["BOUNCER_EFFECT_NAME"], priority = 1.1 }

	-- we don't have to save off the effect number because it times out and we never need to cancel it
end




-----------------------------------------------------------------
-- adds a stink effect to whoever got bounced
-----------------------------------------------------------------
function AddStinkEffectToTarget( target )
	
	--print( "SKUNK BOUNCER: AddStinkEffectToTarget" )
	
	if ( IsTargetABabySkunk( target ) == true ) then
		--print( "SKUNK BOUNCER: add stink effect to baby" )
		target:PlayFXEffect{ effectType = CONSTANTS["BABY_EFFECT_NAME"], priority = 1.1 }
		
	elseif ( IsTargetAnAdultSkunk( target ) == true ) then
		--print( "SKUNK BOUNCER: add stink effect to adult" )
		target:PlayFXEffect{ effectType = CONSTANTS["ADULT_EFFECT_NAME"], priority = 1.1 }
		
	end

	-- we don't have to save off the effect number because it times out and we never need to cancel it
end




--------------------------------------------------------------
-- on Collision functions
--------------------------------------------------------------
function onCollisionPhantom(self, msg)

	--print( "SKUNK BOUNCER: onCollisionPhantom" )

	BounceNow( self, msg.objectID )
	
	return msg
end




--------------------------------------------------------------
-- if it was a skunk that collided with the bouncer, bounce them and show the stink effects
--------------------------------------------------------------
function BounceNow( self, target )

	
	if ( IsTargetASkunk( target ) == true ) then
	
		bounceObj( self, target )
	
		CheckForStink( self, target )
	end
	
end




--------------------------------------------------------------
-- returns whether the given object is a skunk
--------------------------------------------------------------
function IsTargetASkunk( target )

	if ( IsTargetABabySkunk( target ) == true or IsTargetAnAdultSkunk( target ) ) then
		return true
	end
	
	return false
	
end




--------------------------------------------------------------
-- returns whether the given object is a baby skunk
--------------------------------------------------------------
function IsTargetABabySkunk( target )

	if ( target:GetLOT().objtemplate == CONSTANTS["BABY_SKUNK_LOT"] ) then
		return true
	end
	
	return false
	
end





--------------------------------------------------------------
-- returns whether the given object is an adult skunk
--------------------------------------------------------------
function IsTargetAnAdultSkunk( target )

    -- list of npcs does not exist
    if (CONSTANTS["INVASION_SKUNK_LOT"] == nil) then
        return false
    end
	
    -- look for a valid actor
    local templateID = target:GetLOT().objtemplate
	for actors = 1, #CONSTANTS["INVASION_SKUNK_LOT"] do
		if (templateID == CONSTANTS["INVASION_SKUNK_LOT"][actors]) then
			return true
		end
	end

	return false
	
end







